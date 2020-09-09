using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using mySharedLib;
using Timer = System.Threading.Timer;

namespace MovieTorrents
{
    public class BtBtItem
    {
        public static string BtBtHomeUrl;
        public static string DownLoadRootPath;
        public static string WebProxy { get; set; }

        //自动下载
        public static int AutoDownloadInterval;
        public static int AutoDownloadSearchPages;
        public static int AutoDownloadSearchHours;
        public static int AutoDownloadRunning;
        public static int AutoDownloadLastTid;
        public static DateTime AutoDownloadLastPostDateTime;
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


        static BtBtItem()
        {
            BtBtHomeUrl = Utility.GetSetting(nameof(BtBtHomeUrl), "https://www.btbtt.me/");
            DownLoadRootPath = Utility.GetSetting(nameof(DownLoadRootPath), "f:\\temp");
            WebProxy = Utility.GetSetting(nameof(WebProxy), "");

            AutoDownloadInterval = Utility.GetSetting(nameof(AutoDownloadInterval), 20);
            AutoDownloadSearchPages = Utility.GetSetting(nameof(AutoDownloadSearchPages), 10);
            AutoDownloadSearchHours = Utility.GetSetting(nameof(AutoDownloadSearchHours), 24);

            AutoDownloadLastTid = Utility.GetSetting(nameof(AutoDownloadLastTid), 0);
            AutoDownloadLastPostDateTime = Utility.GetSetting<DateTime>(nameof(AutoDownloadLastPostDateTime));
#if DEBUG
            DownLoadRootPath = "x:\\temp";
#endif

        }

        public void Parse(bool checkInDb=true)
        {
            if (!string.IsNullOrEmpty(DouBanRating) && decimal.TryParse(DouBanRating, out var result))
            {
                Rating = result;
            }

            //判断是否勾选此项
            Checked = (Rating >= 7.0M || Tag.Contains("情色")) && 
                      (!checkInDb || !TorrentFile.ExistInDb(FormMain.DbConnectionString, Keyword, Year));

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
        public static List<BtBtItem> QueryPage(string pageUrl, out string msg,bool checkInDb = true)
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
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsKeepingSourceReferences = true,
            });//保留元素在文章中的位置 https://anglesharp.github.io/docs/Questions.html#can-i-retrieve-the-positions-of-elements-in-the-source-code

            try
            {
                using (var client = CreateWebClient()) // WebClient class inherits IDisposable
                {


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

                        btItem.Parse(checkInDb);
                        items.Add(btItem);



                        Debug.WriteLine($"{btItem.Title} {btItem.Keyword} {btItem.DouBanRating} {btItem.Rating} {btItem.tid} {btItem.PostDateTime}");


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
                    //Debug.WriteLine(disposition);

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
                MyLog.Log($"有{zipFiles.Count()}个zip 文件：{DownLoadRootPath}");
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

                    try
                    {
                        if (File.Exists(destFileName))
                        {
                            msg += $"\r\n文件 {destFileName} 已经存在！";
                            File.Delete(file);
                            continue;
                        }

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
                var pageUrl = BtBtHomeUrl;
                while (true)
                {
                    var searched = QueryPage(pageUrl,out var msg,false);
                    if (searched != null)
                    {
                        items.AddRange(searched);

                        //如果上次已经有查询，退出
                        if (AutoDownloadLastTid!=0 
                            && AutoDownloadLastPostDateTime != default(DateTime)
                            && searched.Any(x =>
                                x.tid == AutoDownloadLastTid && x.PostDateTime == AutoDownloadLastPostDateTime))
                        {

                            MyLog.Log("=====已抵达上次搜索文章，退出======");
                            break;
                        }

                        //如果超过24小时的贴，退出
                        var now = DateTime.Now;
                        if (searched.Any(x =>
                            x.PostDateTime != null && (now - x.PostDateTime.Value).TotalHours > AutoDownloadSearchHours))
                        {
                            MyLog.Log($"====已搜索{AutoDownloadSearchHours}小时的文章，退出======");
                            break;
                        }

                    }

                    pages++;
                    if(pages>AutoDownloadSearchPages) break;
                    pageUrl = NextPageUrl(pageUrl);
                    MyLog.Log($"====Page {pages}====={pageUrl}");
                }

                //记录最新查询的
                var maxTid = items.Max(x => x.tid);
                var latestItem = items.FirstOrDefault(x => x.tid != 0 && x.tid == maxTid);
                if (latestItem?.PostDateTime != null)
                {
                    AutoDownloadLastPostDateTime = latestItem.PostDateTime.Value;
                    AutoDownloadLastTid = latestItem.tid;
                    Utility.SaveSetting(nameof(AutoDownloadLastPostDateTime),AutoDownloadLastPostDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    Utility.SaveSetting(nameof(AutoDownloadLastTid),AutoDownloadLastTid.ToString());
                    MyLog.Log($"===Last item===={latestItem.Title}=={latestItem.tid}=={latestItem.PostDateTime}");
                }

                //下载附件
                var checkedItems = items.Where(x => x.Checked).ToList();
                var i = 0;
                foreach (var btItem in checkedItems)
                {
                    i+=btItem.DownLoadAttachments(out var msg);
                }
                MyLog.Log($"下载了 {i} 个文件");
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

                _autoDownloadTimer.Change(10 * 1000, AutoDownloadInterval * 60 * 1000);
            }
            else
            {
                if(_autoDownloadTimer==null) return;
                _autoDownloadTimer.Change(Timeout.Infinite, Timeout.Infinite);

            }
        }
    }
}
