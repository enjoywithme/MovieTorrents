﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Microsoft.VisualBasic.FileIO;
using mySharedLib;
using static System.Windows.Forms.LinkLabel;
using Timer = System.Threading.Timer;

namespace MovieTorrents.Common
{
    public class BtBtItem
    {

        //自动下载
        public static int AutoDownloadRunning;
        private static Timer _autoDownloadTimer;
        
        //成员
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Keyword = _title.Purify().ExtractFirstToken();
            }
        }

        public string DouBanRating { get; set; }
        public List<string> AttachmentUrls { get; set; } = new List<string>();
        public string PublishTime { get; set; }
        public string Keyword { get; set; }
        public string Gene { get; set; }
        public string Tag { get; set; }
        public int? Year { get; set; }
        public int tid { get; set; }
        public DateTime? PostDateTime { get; set; }
        public bool Checked { get; set; } //是否勾选

        public decimal Rating { get; private set; }



        public void Parse()
        {
            if (!string.IsNullOrEmpty(DouBanRating) && decimal.TryParse(DouBanRating, out var result))
            {
                Rating = result;
            }

            //判断是否勾选此项
            Checked = Rating >= 7.0M;

            Debug.WriteLine($"{Title} -- {DouBanRating} --{Rating} -- {Checked}");


        }


        //下一页url
        public static string NextPageUrl(string pageUrl)
        {
            //搜索下一页
            var match = Regex.Match(pageUrl, $"{MyMtSettings.Instance.BtBtHomeUrl}search-(.*)-1-([0-9]*).htm");
            if (match.Success)
            {
                if (!int.TryParse(match.Groups[2].Value, out var p)) return pageUrl;
                p += 1;
                return
                    $"{MyMtSettings.Instance.BtBtHomeUrl}search-{match.Groups[1].Value}-1-{p}.htm";
            }

            //搜索第二页
            match = Regex.Match(pageUrl, $"{MyMtSettings.Instance.BtBtHomeUrl}search-(.*).htm");
            if(match.Success)
                return $"{MyMtSettings.Instance.BtBtHomeUrl}search-{match.Groups[1].Value}-1-2.htm";

            //主页下一页
            match = Regex.Match(pageUrl, $"{MyMtSettings.Instance.BtBtHomeUrl}index-([0-9]*).htm");
            if (match.Success)
            {
                var p = int.Parse(match.Groups[1].Value) + 1;
                return $"{MyMtSettings.Instance.BtBtHomeUrl}index-{p}.htm";
            }

            return $"{MyMtSettings.Instance.BtBtHomeUrl}index-2.htm";
        }

        //上一页url
        public static string PrevPageUrl(string pageUrl)
        {
            //搜索前一页
            var match = Regex.Match(pageUrl, $"{MyMtSettings.Instance.BtBtHomeUrl}search-(.*)-1-([0-9]*).htm");
            if (match.Success)
            {
                if (!int.TryParse(match.Groups[2].Value, out var p)) return pageUrl;
                if (p == 2) return $"{MyMtSettings.Instance.BtBtHomeUrl}search-{match.Groups[1].Value}-1-1.htm";
                p -= 1;
                return $"{MyMtSettings.Instance.BtBtHomeUrl}search-{match.Groups[1].Value}-1-{p}.htm";
            }

            //搜索当前页
            match = Regex.Match(pageUrl, $"{MyMtSettings.Instance.BtBtHomeUrl}search-(.*).htm");
            if (match.Success) return pageUrl;

            //主页下一页
            match = Regex.Match(pageUrl, $"{MyMtSettings.Instance.BtBtHomeUrl}index-([0-9]*).htm");
            if (match.Success)
            {

                var p = int.Parse(match.Groups[1].Value);
                if (p <= 2) return MyMtSettings.Instance.BtBtHomeUrl;
                p--;
                return $"{MyMtSettings.Instance.BtBtHomeUrl}index-{p}.htm";
            }

            return MyMtSettings.Instance.BtBtHomeUrl;
        }

        //搜索页url
        public static string SearPageUrl(string searchText)
        {
            
            return $"{MyMtSettings.Instance.BtBtHomeUrl}search-{searchText}-1-1.htm";
        }

        //构造webclient
        private static WebClient CreateWebClient()
        {
            var client = new WebClient();
            if (string.IsNullOrEmpty(MyMtSettings.Instance.WebProxy)) return client;
            var wp = new WebProxy(MyMtSettings.Instance.WebProxy);
            client.Proxy = wp;

            return client;
        }

        //查询页面上的文章
        public static List<BtBtItem> QueryPage(string pageUrl, out string msg)
        {
            msg = string.Empty;
            if (string.IsNullOrEmpty(pageUrl))
            {
                msg = "网页内容为空";
                return null;
            }

            if (!pageUrl.StartsWith(MyMtSettings.Instance.BtBtHomeUrl))
            {
                msg = "非BtBt网页";
                return null;
            }

            var items = new List<BtBtItem>();
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsKeepingSourceReferences = true,
            });//保留元素在文章中的位置 https://anglesharp.github.io/docs/Questions.html#can-i-retrieve-the-positions-of-elements-in-the-source-code

            try
            {
                using var client = CreateWebClient();
                var htmlData = client.DownloadData(pageUrl);
                var htmlCode = Encoding.UTF8.GetString(htmlData);
                //File.WriteAllText("d:\\temp\\1.txt", htmlCode, Encoding.UTF8);
                //var htmlCode = File.ReadAllText("d:\\temp\\1.txt");
                var document = parser.ParseDocument(htmlCode);
                var topDiv = document.All.Where(d => d.LocalName == "div" && d.ClassList.Contains("bg2")).ToArray();//置顶帖分割线

                var links = document.All.Where(a => a.LocalName == "a" && a.ClassList.Contains("subject_link"));
                foreach (var link in links)
                {
                    //略过置顶帖
                    if(topDiv.Length>0 && link.SourceReference.Position.Position<topDiv[0].SourceReference.Position.Position)
                        continue;
                        
                    var title = link.TextContent.Trim();
                    if (string.IsNullOrEmpty(title)) continue;
                     
                    var detailLink = link.Attributes["href"].Value;
                    if (string.IsNullOrEmpty(detailLink)) continue;

                    var btItem = new BtBtItem {Title = title};

                    //试图查找tid,lastpost
                    var threadTable = link.Ancestors().OfType<IElement>()
                        .Where(a => a.LocalName == "table" && a.Attributes.Any(b => b.Name == "tid")).ToArray();
                    if (threadTable.Length > 0 && int.TryParse(threadTable[0].Attributes["tid"].Value,out var tid))
                        btItem.tid = tid;

                    //下载具体页面
                    var pageData = client.DownloadData($"{MyMtSettings.Instance.BtBtHomeUrl}{detailLink}");
                    var pageCode = Encoding.UTF8.GetString(pageData);
                    //File.WriteAllText("d:\\temp\\2.txt", pageCode, Encoding.UTF8);

                    //查找可能的完整标题
                    var match = Regex.Match(pageCode, "<blockquote>帖子完整标题：(.+?)</blockquote>");
                    if (match.Success)
                        btItem.Title = match.Groups[1].Value;

                    //查找豆瓣评分
                    match = Regex.Match(pageCode, "豆瓣评分(.*?)/10");//非贪婪
                    btItem.DouBanRating = match.Success ? match.Groups[1].Value.Trim() : "";

                    //<span class="grey">发帖时间：</span><b>.*</b>
                    match = Regex.Match(pageCode, "<span class=\"grey\">发帖时间：</span><b>(.*?)</b>");
                    btItem.PublishTime = match.Success ? match.Groups[1].Value : "";

                    //◎年　　代　1954<br />
                    match = Regex.Match(pageCode, "年　　代(.*?)<br />");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out var d)) btItem.Year = d;

                    //◎类　　别　剧情 / 喜剧 / 爱情 / 悬疑 / 音乐<br />
                    match = Regex.Match(pageCode, "类　　别(.*?)<br />");
                    btItem.Gene = match.Success ? match.Groups[1].Value.Trim() : "";

                    //标　　签　西班牙 | 西班牙电影 | JulioMedem | 爱情 | 1993 | 红松鼠杀人事件 | Julio_Medem | 密谭<br />
                    match = Regex.Match(pageCode, "标　　签(.*?)<br />");
                    btItem.Tag = match.Success ? match.Groups[1].Value.Trim() : "";

                    //发帖时间
                    match = Regex.Match(pageCode, "<span class=\"grey\">发帖时间：</span><b>(.*?)</b>");
                    if (match.Success && DateTime.TryParse(match.Groups[1].Value, out var dt))
                        btItem.PostDateTime = dt;

                    //附件 <a href="attach-dialog-fid-1183-aid-5138072.htm" target="_blank" rel="nofollow"><img src="/view/image/filetype/zip.gif" width="16" height="16" />The.Red.Squirrel.1993.1080p.BluRay.x264-USURY.zip</a>
                    var documentItem = parser.ParseDocument(pageCode);
                    var attachments = documentItem.All.Where(a =>
                        a.LocalName == "a"
                        && a.HasAttribute("href")
                        && a.Attributes["href"].Value.Contains("attach-dialog-fid-")).ToArray();
                    foreach (var attachment in attachments)
                    {
                        var attachmentName = attachment.Text().Trim();
                        var attachmentUrl = attachment.Attributes["href"].Value;

                        if (!attachmentName.ToLower().EndsWith(".zip") &&
                            !attachmentName.ToLower().EndsWith(".torrent")) continue;

                        btItem.AttachmentUrls.Add(attachmentUrl.Replace("dialog", "download"));

                    }

                    btItem.Parse();
                    items.Add(btItem);



                    Debug.WriteLine($"{btItem.Title} {btItem.Keyword} {btItem.DouBanRating} {btItem.Rating} {btItem.tid} {btItem.PostDateTime}");


                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                return null;
            }


            return items;
        }

        /// <summary>
        /// Parse the list index page
        /// </summary>
        /// <param name="html"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<string> ParseIndexPage(string html, out string msg)
        { 
            msg = string.Empty;
            var items = new List<string>();
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsKeepingSourceReferences = true,
            });//保留元素在文章中的位置 https://anglesharp.github.io/docs/Questions.html#can-i-retrieve-the-positions-of-elements-in-the-source-code

            try
            {
                var document = parser.ParseDocument(html);
                var topDiv = document.All.Where(d => d.LocalName == "div" && d.ClassList.Contains("bg2")).ToArray();//置顶帖分割线

                var links = document.All.Where(a => a.LocalName == "a" && a.ClassList.Contains("text-title"));
                foreach (var link in links)
                {
                    //略过置顶帖
                    if (topDiv.Length > 0 && link.SourceReference.Position.Position < topDiv[0].SourceReference.Position.Position)
                        continue;

                    var title = link.TextContent.Trim();
                    if (string.IsNullOrEmpty(title)) continue;

                    var detailLink = link.Attributes["href"].Value;
                    if (string.IsNullOrEmpty(detailLink)) continue;

                    items.Add(detailLink);

                    


                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return items;
        }

        public static List<string> ParseSearchPage(string html, out string msg)
        {
            msg = string.Empty;
            var items = new List<string>();
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsKeepingSourceReferences = true,
            });//保留元素在文章中的位置 https://anglesharp.github.io/docs/Questions.html#can-i-retrieve-the-positions-of-elements-in-the-source-code

            try
            {
                var document = parser.ParseDocument(html);
                var divs = document.All.Where(d => d.LocalName == "div" && d.ClassName == "subject break-all").ToArray();//置顶帖分割线

                foreach (var div in divs)
                {
                    var links = div.Children.Where(a => a.LocalName == "a"
                                            && a.HasAttribute("href")
                                            && a.GetAttribute("href").StartsWith("thread-")).ToList();

                    if(!links.Any()) continue;

                    var title = div.TextContent;
                    

                    var detailLink = links[0].Attributes["href"].Value;
                    if (string.IsNullOrEmpty(detailLink)) continue;

                    items.Add(detailLink);




                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return items;
        }


        /// <summary>
        /// Parse the thread page to BT item
        /// </summary>
        /// <param name="pageCode"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static BtBtItem ParseThreadPage(string pageCode, string url,out string msg)
        {
            msg = string.Empty;
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsKeepingSourceReferences = true,
            });//保留元素在文章中的位置 https://anglesharp.github.io/docs/Questions.html#can-i-retrieve-the-positions-of-elements-in-the-source-code

            try
            {
                var document = parser.ParseDocument(pageCode);
                var titles = document.All.Where(a => a.LocalName == "h4" && a.ClassList.Contains("break-all")).ToList();
                if (!titles.Any())
                {
                    return null;
                }

                var btItem = new BtBtItem { Title = Regex.Replace(titles[0].Text(), @"\t|\n|\r", "")  };

                /*附件
                 *
                    <ul class="attachlist">
                    <li aid="1369169">
		                    <a href="attach-download-1369169.htm" target="_blank">
			                    <i class="icon filetype torrent"></i>
			                    无法抗拒的谎言[全24集][国语配音+中文字幕].Liars.in.Love.S01.2024.1080p.WEB-DL.H264.AAC-ZeroTV 13.71GB[1lou.me].torrent
		                    </a>
                    </li>
                    </ul>
                 * */
                var attachments = document.All.Where(a =>
                    a.LocalName == "a"
                    && a.HasAttribute("href")
                    && a.Attributes["href"].Value.Contains("attach-download-")).ToArray();
                foreach (var attachment in attachments)
                {
                    var attachmentName = attachment.Text().Trim();
                    var attachmentUrl = attachment.Attributes["href"].Value;

                    if (!attachmentName.ToLower().EndsWith(".zip") &&
                        !attachmentName.ToLower().EndsWith(".torrent")) continue;

                    btItem.AttachmentUrls.Add(attachmentUrl.Replace("dialog", "download"));

                }

                if(btItem.AttachmentUrls.Count ==0)
                    return null;

                //Thread id thread-487019.htm
                var match = Regex.Match(url, "thread-([0-9]*).");
                if (match.Success && int.TryParse(match.Groups[1].Value, out var tid))
                    btItem.tid = tid;

                //查找可能的完整标题 完整标题：无法抗拒的谎言[全24集][国语配音/中文字幕].Liars.in.Love.S01.2024.1080p.WEB-DL.H264.AAC-ZeroTV 13.71GB
                match = Regex.Match(pageCode, "<br>完整标题：(.+?)<br>");
                if (match.Success)
                    btItem.Title = match.Groups[1].Value;

                //查找豆瓣评分
                match = Regex.Match(pageCode, "豆瓣评分(.*?)/10");//非贪婪
                btItem.DouBanRating = match.Success ? match.Groups[1].Value.Trim() : "";

                
                match = Regex.Match(pageCode, "<span class=\"grey\">发帖时间：</span><b>(.*?)</b>");
                btItem.PublishTime = match.Success ? match.Groups[1].Value : "";

                //◎年　　代　1954<br />
                match = Regex.Match(pageCode, "年　　代(.*?)<br />");
                if (match.Success && int.TryParse(match.Groups[1].Value, out var d)) btItem.Year = d;

                //◎类　　别　剧情 / 喜剧 / 爱情 / 悬疑 / 音乐<br />
                match = Regex.Match(pageCode, "类　　别(.*?)<br />");
                btItem.Gene = match.Success ? match.Groups[1].Value.Trim() : "";

                //标　　签　西班牙 | 西班牙电影 | JulioMedem | 爱情 | 1993 | 红松鼠杀人事件 | Julio_Medem | 密谭<br />
                match = Regex.Match(pageCode, "标　　签(.*?)<br />");
                btItem.Tag = match.Success ? match.Groups[1].Value.Trim() : "";

                //发帖时间 <span class="date text-grey ml-2">2024-07-21 10:31</span>
                var elements = document.All.Where(e => e.LocalName == "span" && e.ClassName == "date text-grey ml-2").ToList();
                if (elements.Count > 0 && DateTime.TryParse(elements[0].TextContent,out var dt))
                    btItem.PostDateTime = dt;

 

                btItem.Parse();

                return btItem;

            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return null;
        }

        //下载附件
        public int DownLoadAttachments(out string msg)
        {
            msg = string.Empty;
            if (AttachmentUrls.Count == 0) return 0;
            if (string.IsNullOrEmpty(MyMtSettings.Instance.DownLoadRootPath) || !Directory.Exists(MyMtSettings.Instance.DownLoadRootPath))
            {
                msg = $"下载路径\"{MyMtSettings.Instance.DownLoadRootPath}\"不存在";
                return 0;
            }

            var i = 0;
            try
            {
                using var client = CreateWebClient();
                switch (AttachmentUrls.Count)
                {
                    case 1:
                        //如果只有一个附件，使用电影标题作为文件名
                        DownloadAttachmentFile(client, $"{MyMtSettings.Instance.BtBtHomeUrl}{AttachmentUrls[0]}", MyMtSettings.Instance.DownLoadRootPath,
                            Title.SanitizeFileName());
                        i++;
                        break;
                    case > 1:
                    {
                        //下载到标题下的子目录
                        var subPath = Path.Combine(MyMtSettings.Instance.DownLoadRootPath, Title.SanitizeFileName());
                        Directory.CreateDirectory(subPath);
                        foreach (var attachmentUrl in AttachmentUrls)
                        {
                            DownloadAttachmentFile(client, $"{MyMtSettings.Instance.BtBtHomeUrl}{attachmentUrl}", subPath);
                            i++;
                        }

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return i;
        }

        //下载种子附件
        private void DownloadAttachmentFile(WebClient client, string fileUrl, string downloadPath,
            string fileName = null)
        {
            using var rawStream = client.OpenRead(fileUrl);
            if (rawStream == null) return;

            var originalFileName = string.Empty;
            var contentDisposition = client.ResponseHeaders["content-disposition"];
            if (!string.IsNullOrEmpty(contentDisposition))
            {
                //参照 https://ithelp.ithome.com.tw/articles/10198852 的提示反编码
                var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(contentDisposition);
                var disposition = Encoding.GetEncoding("UTF-8").GetString(bytes);
                //Debug.WriteLine(disposition);

                var match = Regex.Match(disposition, "filename=\"(.*)\"", RegexOptions.IgnoreCase);
                if (match.Success)
                    originalFileName = match.Groups[1].Value;
            }

            if (originalFileName.Length > 0)
            {
                fileName = originalFileName;

                //if (string.IsNullOrEmpty(fileName))
                //    fileName = originalFileName;
                //else
                //{
                //    fileName += Path.GetExtension(originalFileName);
                //    fileName = fileName.Replace("[BT下载]", "");
                //}

                var path = Path.Combine(downloadPath, fileName);

                using var outputFileStream = new FileStream(path, FileMode.Create);
                rawStream.CopyTo(outputFileStream);
            }

            rawStream.Close();
        }

        //解压zip文件
        public static string ExtractZipFiles()
        {
            var i = 0;
            var msg = "";
            try
            {
                var zipFiles = Directory.GetFiles(MyMtSettings.Instance.DownLoadRootPath)
                    .Where(s => Path.GetExtension(s).ToLowerInvariant() == ".zip");
                MyLog.Log($"有{zipFiles.Count()}个zip 文件：{MyMtSettings.Instance.DownLoadRootPath}");
                foreach (var file in zipFiles)
                {
                    try
                    {
                        using (var zip = ZipFile.OpenRead(file))
                        {
                            var entry = zip.Entries.FirstOrDefault(et =>
                                et.Name.EndsWith(".torrent", StringComparison.InvariantCultureIgnoreCase));
                            if (entry == null)
                            {
                                msg += $"文件 {file} 不包含 torrent 文件。";
                                continue;
                            }

                            entry.ExtractToFile(Path.Combine(MyMtSettings.Instance.DownLoadRootPath,
                                Path.GetFileNameWithoutExtension(file) + ".torrent"));
                        }

                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception exception)
                        {
                            msg += $"\r\n删除 {file} 出错:{exception.Message}";
                        }

                        i++;
                    }
                    catch (Exception exception)
                    {
                        msg += $"\r\n解压 {file} 出错:{exception.Message}";
                    }

                }
            }
            catch (Exception e)
            {
                msg += e.Message;
            }

            msg += $"成功解压 {i} 个文件。\r\n";
            return msg;
        }
        //尝试重命名BBQDDQ的文件
        public static string RenameSpecialFiles()
        {
            var files = Directory.GetFiles(MyMtSettings.Instance.DownLoadRootPath)
                .Where(s => Path.GetExtension(s).ToLowerInvariant() == ".torrent");
            var sb = new StringBuilder();
            try
            {
                var parser = new BencodeParser(); // Default encoding is Encoding.UTF8, but you can specify another if you need to

                foreach (var file in files)
                {
                    try
                    {
                        var torrent = parser.Parse<Torrent>(file);

                        var name = StripBtPrefix(torrent.DisplayName,Path.GetFileNameWithoutExtension(file));
                        name = name.Replace("[中文字幕]", "");
                        if(name==Path.GetFileNameWithoutExtension(file)) 
                            continue;

                        var destFile = file.Replace(Path.GetFileNameWithoutExtension(file), name);
                        if (File.Exists(destFile))
                        {
                            File.Delete(file);
                            continue;
                        }
                        File.Move(file, destFile);
                    }
                    catch (Exception e)
                    {
                        sb.AppendLine(e.Message);
                    }

                }
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            return sb.ToString();
        }

        public static string StripBtPrefix(string name,string fileName)
        {
            if (Utility._btItemPrefix == null || Utility._btItemPrefix.Length == 0) return fileName;

            foreach (var s in Utility._btItemPrefix)
            {
                if(string.IsNullOrWhiteSpace(s)) continue;
                var match = Regex.Match(name, s, RegexOptions.IgnoreCase);
                if (match.Success) 
                    fileName = name.Replace(match.Groups[0].Value, "");
                //name = Regex.Replace(name, s, "", RegexOptions.IgnoreCase);

            }

            return fileName;
        }

        //将目录下的种子文件转移到收藏目录
        public static string ArchiveTorrentFiles()
        {
            var msg = "";
            var i = 0;
            try
            {
                var files = Directory.GetFiles(MyMtSettings.Instance.DownLoadRootPath, "*.torrent");

                foreach (var file in files)
                {
                    //不考虑第XX集的文件
                    if (Regex.IsMatch(file, "第[\\s]*[0-9]*[\\s]*集"))
                    {
                        msg += $"\r\n{Path.GetFileName(file)} 非全季！";
                        try
                        {
                            FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        catch (Exception e)
                        {
                            msg += $"\r\n删除失败：{e.Message}";
                        }
                        continue;
                    }


                    var destFileName = Path.GetFileName(file).NormalizeTorrentFileName();
                    var year = destFileName.ExtractYear();
                    if (year != 0)
                    {
                        var destPath = Path.Combine(TorrentFile.TorrentRootPath, TorrentFile.ArchiveYearSubPath(year));
                        destFileName = Path.Combine(destPath, destFileName);
                    }
                    else
                    {
                        var torrentFile = TorrentFile.FindByName(Path.GetFileNameWithoutExtension(file)) ?? TorrentFile.FindByName(Path.GetFileNameWithoutExtension(destFileName));
                        if (torrentFile == null)
                        {
                            msg += $"\r\n文件 {Path.GetFileName(file)} 没有年份！";
                            continue;
                        }
                        var destPath = Path.GetDirectoryName(torrentFile.FullName);

                        destFileName = Path.Combine(destPath, destFileName);

                    }

                    Debug.WriteLine(destFileName);

                    try
                    {
                        if (File.Exists(destFileName))
                        {
                            msg += $"\r\n文件 {Path.GetFileName(file)} 已经存在！";
                            File.Copy(file,destFileName,true);
                            File.Delete(file);
                        }
                        else
                        {
                            File.Move(file, destFileName);
                        }

                        i++;
                    }
                    catch (Exception exception)
                    {
                        msg += $"\r\n{destFileName} {exception.Message}";
                    }
                }

            }
            catch (Exception exception)
            {
                msg += $"\r\n{exception.Message}";
            }

            msg+=$"\r\n\r\n成功转移{i}个文件。";
            return msg;
        }

        //自动下载
        public static void AutoDownloadCallback(object o)
        {
            if (0 != Interlocked.Exchange(ref AutoDownloadRunning, 1))
                return;
            MyLog.Log("=====自动下载开始运行======");
            try
            {
                var items = new List<BtBtItem>();

                var pages = 0;
                var pageUrl = MyMtSettings.Instance.BtBtHomeUrl;
                while (true)
                {
                    var searched = QueryPage(pageUrl,out var msg);
                    if (searched != null)
                    {
                        items.AddRange(searched);

                        //如果上次已经有查询，退出
                        if (MyMtSettings.Instance.AutoDownloadLastTid !=0 
                            && MyMtSettings.Instance.AutoDownloadLastPostDateTime != default(DateTime)
                            && searched.Any(x =>
                                x.tid == MyMtSettings.Instance.AutoDownloadLastTid && x.PostDateTime == MyMtSettings.Instance.AutoDownloadLastPostDateTime))
                        {

                            MyLog.Log("=====已抵达上次搜索文章，退出======");
                            break;
                        }

                        //如果超过24小时的贴，退出
                        var now = DateTime.Now;
                        if (searched.Any(x =>
                            x.PostDateTime != null && (now - x.PostDateTime.Value).TotalHours > MyMtSettings.Instance.AutoDownloadSearchHours))
                        {
                            MyLog.Log($"====已搜索{MyMtSettings.Instance.AutoDownloadSearchHours}小时的文章，退出======");
                            break;
                        }

                    }

                    pages++;
                    if(pages> MyMtSettings.Instance.AutoDownloadSearchPages) break;
                    pageUrl = NextPageUrl(pageUrl);
                    MyLog.Log($"====Page {pages}====={pageUrl}");
                }

                //下载附件
                var checkedItems = items.Where(x => x.Checked 
                                                    &&(x.tid==0 ||MyMtSettings.Instance.AutoDownloadLastTid ==0 ||x.tid>MyMtSettings.Instance.AutoDownloadLastTid)
                                                    ).ToList();
                var i = 0;
                foreach (var btItem in checkedItems)
                {
                    i+=btItem.DownLoadAttachments(out var msg);
                }
                MyLog.Log($"下载了 {i} 个文件");

                //记录最新查询的
                var maxTid = items.Max(x => x.tid);
                var latestItem = items.FirstOrDefault(x => x.tid != 0 && x.tid == maxTid);
                if (latestItem?.PostDateTime != null)
                {
                    MyMtSettings.Instance.AutoDownloadLastPostDateTime = latestItem.PostDateTime.Value;
                    MyMtSettings.Instance.AutoDownloadLastTid = latestItem.tid;
                    //Utility.SaveSetting(nameof(AutoDownloadLastPostDateTime), AutoDownloadLastPostDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    //Utility.SaveSetting(nameof(AutoDownloadLastTid), AutoDownloadLastTid.ToString());
                    MyMtSettings.Instance.Save();
                    MyLog.Log($"===Last item===={latestItem.Title}=={latestItem.tid}=={latestItem.PostDateTime}");
                }
            }
            catch (Exception e)
            {
                MyLog.Log($"====Error===={e.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref AutoDownloadRunning, 0);
                MyLog.Log($"====AUTO DOWNLOAD===ENDING====");

            }

        }

        

        //启动自动下载
        public static void EnableAutoDownload(bool bEnable)
        {
            if (bEnable)
            {
                if (_autoDownloadTimer == null)
                {
                    _autoDownloadTimer = new Timer(AutoDownloadCallback,null,Timeout.Infinite,Timeout.Infinite);
                }

                _autoDownloadTimer.Change(10 * 1000, MyMtSettings.Instance.AutoDownloadInterval * 60 * 1000);
            }
            else
            {
                if(_autoDownloadTimer==null) return;
                _autoDownloadTimer.Change(Timeout.Infinite, Timeout.Infinite);

            }
        }
    }
}
