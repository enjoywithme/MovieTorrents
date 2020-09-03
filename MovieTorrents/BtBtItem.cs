using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MovieTorrents.Common;
using mySharedLib;

namespace MovieTorrents
{
    public class BtBtItem
    {
        public static string BtBtHomeUrl;
        public static string DownLoadRootPath;
        public static string WebProxy { get; set; }


        public string Title { get; set; }

        public string DouBanRating { get; set; }

        //public string AttachmentUrl { get; set; }
        public List<string> AttachmentUrls { get; set; } = new List<string>();
        public string PublishTime { get; set; }
        public string Keyword { get; set; }
        public string Gene { get; set; }
        public string Tag { get; set; }
        public int? Year { get; set; }
        public bool Checked { get; set; } //是否勾选

        public decimal Rating { get; private set; }


        static BtBtItem()
        {
            BtBtHomeUrl = Utility.GetSetting("BtBtHomeUrl", "https://www.btbtt.me/");
            DownLoadRootPath = Utility.GetSetting("DownLoadPath", "f:\\temp");
            WebProxy = Utility.GetSetting("WebProxy", "");
#if DEBUG
            DownLoadRootPath = "x:\\temp";
#endif
        }

        public void Parse()
        {
            if (!string.IsNullOrEmpty(DouBanRating) && decimal.TryParse(DouBanRating, out var result))
            {
                Rating = result;
            }

            //年份
            var title = Title.Purify();
            if (string.IsNullOrEmpty(title)) return;
            var chinese = title.ExtractChinese();
            if (!string.IsNullOrEmpty(chinese)) title = chinese;
            var i = title.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            if (i == -1) i = title.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
            if (i == -1) i = title.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
            if (i == -1) i = title.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);
            Keyword = i > 0 ? title.Substring(0, i) : title;

            //判断是否勾选此项
            Checked = (Rating >= 7.0M || Tag.Contains("情色")) &&
                      !TorrentFile.ExistInDb(FormMain.DbConnectionString, Keyword, Year);

        }

        //下一页url
        public static string NextPageUrl(string pageUrl)
        {
            var match = Regex.Match(pageUrl, $"{BtBtHomeUrl}index-index-page-([0-9]*).htm");
            var p = match.Success ? int.Parse(match.Groups[1].Value) + 1 : 1;

            return $"{BtBtHomeUrl}index-index-page-{p}.htm";
        }

        //上一页url
        public static string PrevPageUrl(string pageUrl)
        {
            var match = Regex.Match(pageUrl, $"{BtBtHomeUrl}index-index-page-([0-9]*).htm");
            if (!match.Success) return BtBtHomeUrl;
            var p = int.Parse(match.Groups[1].Value);
            if (p <= 2) return BtBtHomeUrl;
            p--;
            return $"{BtBtHomeUrl}index-index-page-{p}.htm";

        }

        //搜索页url
        public static string SearPageUrl(string searchText)
        {
            return $"https://www.btbtt.me/search-index-keyword-{searchText}.htm";
        }

        //构造webclient
        private static WebClient CreateWebClient()
        {
            var client = new WebClient();
            if (string.IsNullOrEmpty(WebProxy)) return client;
            var wp = new WebProxy(WebProxy);
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

            if (!pageUrl.StartsWith(BtBtHomeUrl))
            {
                msg = "非BtBt网页";
                return null;
            }

            var items = new List<BtBtItem>();
            var parser = new HtmlParser();

            try
            {
                using (var client = CreateWebClient()) // WebClient class inherits IDisposable
                {


                    var htmlData = client.DownloadData(pageUrl);
                    var htmlCode = Encoding.UTF8.GetString(htmlData);
                    //File.WriteAllText("d:\\temp\\1.txt", htmlCode, Encoding.UTF8);
                    //var htmlCode = File.ReadAllText("d:\\temp\\1.txt");
                    var document = parser.ParseDocument(htmlCode);

                    var links = document.All.Where(a => a.LocalName == "a" && a.ClassList.Contains("subject_link"));
                    foreach (var link in links)
                    {
                        var title = link.TextContent.Trim();
                        if (string.IsNullOrEmpty(title)) continue;

                        var detailLink = link.Attributes["href"].Value;
                        if (string.IsNullOrEmpty(detailLink)) continue;

                        var btItem = new BtBtItem {Title = title};

                        //下载具体页面
                        var pageData = client.DownloadData($"{BtBtHomeUrl}{detailLink}");
                        var pageCode = Encoding.UTF8.GetString(pageData);
                        //File.WriteAllText("d:\\temp\\2.txt", pageCode, Encoding.UTF8);

                        //查找可能的完整标题
                        var match = Regex.Match(pageCode, "<blockquote>帖子完整标题：(.+?)</blockquote>");
                        if (match.Success)
                            btItem.Title = match.Groups[1].Value;

                        //查找豆瓣评分
                        match = Regex.Match(pageCode, "豆瓣评分(.*)/10 from");
                        btItem.DouBanRating = match.Success ? match.Groups[1].Value.Trim() : "";

                        //<span class="grey">发帖时间：</span><b>.*</b>
                        match = Regex.Match(pageCode, "<span class=\"grey\">发帖时间：</span><b>(.*)</b>");
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
                            Debug.Print($"{attachmentName} {attachmentUrl}");

                            btItem.AttachmentUrls.Add(attachmentUrl.Replace("dialog", "download"));

                        }

                        btItem.Parse();
                        items.Add(btItem);



                        Debug.WriteLine($"{btItem.Title} {btItem.Keyword} {btItem.DouBanRating} {btItem.Rating}");


                    }



                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                return null;
            }


            return items;
        }

        //下载附件
        public int DownLoadAttachments(out string msg)
        {
            msg = string.Empty;
            if (AttachmentUrls.Count == 0) return 0;
            if (string.IsNullOrEmpty(DownLoadRootPath) || !Directory.Exists(DownLoadRootPath))
            {
                msg = $"下载路径\"{DownLoadRootPath}\"不存在";
                return 0;
            }

            var i = 0;
            try
            {
                using (var client = CreateWebClient())
                {
                    if (AttachmentUrls.Count == 1)
                    {
                        //如果只有一个附件，使用电影标题作为文件名
                        DownloadAttachmentFile(client, $"{BtBtHomeUrl}{AttachmentUrls[0]}", DownLoadRootPath,
                            Title.SanitizeFileName());
                        i++;
                    }
                    else if (AttachmentUrls.Count > 1)
                    {
                        //下载到标题下的子目录
                        var subPath = Path.Combine(DownLoadRootPath, Title.SanitizeFileName());
                        Directory.CreateDirectory(subPath);
                        foreach (var attachmentUrl in AttachmentUrls)
                        {
                            DownloadAttachmentFile(client, $"{BtBtHomeUrl}{attachmentUrl}", subPath);
                            i++;
                        }
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
            using (var rawStream = client.OpenRead(fileUrl))
            {
                if (rawStream == null) return;

                var originalFileName = string.Empty;
                var contentDisposition = client.ResponseHeaders["content-disposition"];
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    //参照 https://ithelp.ithome.com.tw/articles/10198852 的提示反编码
                    var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(contentDisposition);
                    var disposition = Encoding.GetEncoding("UTF-8").GetString(bytes);
                    Debug.WriteLine(disposition);

                    var match = Regex.Match(disposition, "filename=\"(.*)\"", RegexOptions.IgnoreCase);
                    if (match.Success)
                        originalFileName = match.Groups[1].Value;
                }

                if (originalFileName.Length > 0)
                {
                    if (string.IsNullOrEmpty(fileName))
                        fileName = originalFileName;
                    else
                    {
                        fileName += Path.GetExtension(originalFileName);
                        fileName = fileName.Replace("[BT下载]", "");
                    }

                    var path = Path.Combine(downloadPath, fileName);

                    using (var outputFileStream = new FileStream(path, FileMode.Create))
                    {
                        rawStream.CopyTo(outputFileStream);
                    }
                }

                rawStream.Close();
            }


        }

        //解压zip文件
        public static int ExtractZipFiles(out string msg)
        {
            var i = 0;
            msg = "";
            try
            {
                var zipFiles = Directory.GetFiles(DownLoadRootPath)
                    .Where(s => Path.GetExtension(s).ToLowerInvariant() == ".zip");
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

                            entry.ExtractToFile(Path.Combine(DownLoadRootPath,
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

            return i;
        }

        //将目录下的种子文件转移到收藏目录
        public static int ArchiveTorrentFiles(out string msg)
        {
            msg = "";
            var i = 0;
            try
            {
                var files = Directory.GetFiles(DownLoadRootPath, "*.torrent");

                foreach (var file in files)
                {
                    var destFileName = Path.GetFileName(file).NormalizeTorrentFileName();
                    var year = destFileName.ExtractYear();
                    if (year == 0)
                    {
                        msg += $"\r\n文件 {destFileName} 没有年份！";
                        continue;
                    }

                    var destPath = Path.Combine(FormMain.DefaultInstance.TorrentFilePath,
                        TorrentFile.ArchiveYearSubPath(year));
                    destFileName = Path.Combine(destPath, destFileName);
                    Debug.WriteLine(destFileName);

                    if (File.Exists(destFileName))
                    {
                        msg += $"\r\n文件 {destFileName} 已经存在！";
                        continue;
                    }

                    try
                    {
                        File.Move(file, destFileName);
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

            return i;
        }
    }
}
