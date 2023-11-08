using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mySharedLib;
using WizKMCoreLib;
using System.Net;
using MyPageLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZeroDownLib
{
    public class ZeroDayDownArticle
    {
        private const int wizUpdateDocumentSaveSel = 0x0001;    //保存选中部分，仅仅针对UpdateDocument2有效
        private const int wizUpdateDocumentIncludeScript = 0x0002;    //包含html里面的脚本
        private const int wizUpdateDocumentShowProgress = 0x0004;    //显示进度
        private const int wizUpdateDocumentSaveContentOnly = 0x0008;   //只保存正文 
        private const int wizUpdateDocumentSaveTextOnly = 0x0010;    //只保存文字内容，并且为纯文本
        private const int wizUpdateDocumentDonotDownloadFile = 0x0020;   //不从网络下载html里面的资源
        private const int wizUpdateDocumentAllowAutoGetContent = 0x0040;   //如果只保存正文，允许使用自动获得正文方式

        private static readonly List<string> SimilarSkipWords = new List<string>{"pro","ultimate"};
        public static string WizDbPath;
        public static string WizDefaultFolder;
        public static string MyPageTempPath;
        public static string SaveFormat;//WIZ保存到为知笔记，PIZ保存为PIZ文件

        public string Html { get; set; }
        public string Url { get; set; }
        public string Content { get; private set; }//净化后的html内容

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                ParseTitle();
            }
        } //完整标题，包含版本号

        public string Version { get; private set; }
        public string Name { get; set; }//名称，不含版本号
        public string DocFileName { get; private set; }//Wiz保存文件名
        public string WizFolderLocation { get; private set; }

        private string _pageDocTempPath;

        public string PizFileName { get; private set; }

        static ZeroDayDownArticle()
        {
            var skipWordsFile = Path.Combine(mySharedLib.Utility.ExecutingAssemblyPath(), "VersionSkipWords.txt");
            if (File.Exists(skipWordsFile))
                SimilarSkipWords.AddRange(File.ReadAllLines(skipWordsFile));
        }

        public ZeroDayDownArticle()
        {

        }

        public ZeroDayDownArticle(string html, string url)
        {
            Content = html;
            Url = url;

            //抽取0DayDown文章的标题
            var match = Regex.Match(html, "<a(.*)>(.*?)</a></h1>");//非贪婪模式
            Title = match.Success && match.Groups.Count == 3 ? match.Groups[2].Value : "";
        }

        //从剪贴的html解析
        public void ParsePasteHtml(string html)
        {
            Html = html;

            //查找url
            var match = Regex.Match(html, "SourceURL:(.*)[^\\n]");
            Url = match.Success ? match.Groups[1].Value : "";
            //File.WriteAllText("d:\\temp\\1.txt", _html, Encoding.UTF8);

            //抽取剪贴板正文
            Content = ExtractHtmlCopyHeader(html);

            //抽取0DayDown文章的标题
            match = Regex.Match(html, "<a(.*)>(.*?)</a></h1>");//非贪婪模式
            Title = match.Success && match.Groups.Count == 3 ? match.Groups[2].Value : "";
        }

        //解析标题
        public void ParseTitle()
        {

            //抽取版本号
            var match = Regex.Match(Title, "\\sv?(\\d+.[^\\s]*)\\b");
            Version = match.Success ? match.Groups[1].Value : "";
            Debug.WriteLine($"Version={Version}");

            //名称
            if (string.IsNullOrEmpty(Version))
                Name = Title;
            else
            {
                var n = Title.IndexOf(Version, StringComparison.OrdinalIgnoreCase);
                Name = Title.Substring(0, n).Trim();
            }

            //统一化保存的文件名
            DocFileName = Title.Replace(" ", "_").MakeValidFileName();
        }

        //剥离剪贴板的HTML头
        public static string ExtractHtmlCopyHeader(string html)
        {
            var n = html.IndexOf("<html>", StringComparison.InvariantCultureIgnoreCase);
            if (n >= 0)
                html = html.Substring(n, html.Length - n);


            return html;
        }

        //从剪贴板保存
        public bool SaveFromClipboard(out string msg)
        {
            if (!Clipboard.ContainsText(TextDataFormat.Html))
            {
                msg = "剪贴板没有html";
                return false;
            }
            ParsePasteHtml(Clipboard.GetText(TextDataFormat.Html));
            if (string.IsNullOrEmpty(Title))
            {
                msg = "没有找到文章标题";
                return false;
            }

            try
            {
                Save();
                msg = $"保存成功!";

            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }

            return true;
        }

        public void Save()
        {
            if(SaveFormat == "PIZ")
                SaveAsPiz();
            else
                SaveAsWiz();
        }

        //保存为为知笔记文档
        //https://www.wiz.cn/zh-cn/m/api-summary.html
        public void SaveAsWiz()
        {
            //打开数据库 https://www.wiz.cn/m/api/descriptions/IWizDatabase.html
            //var dbPath = System.Configuration.ConfigurationManager.AppSettings["IndexDbPath"];
#if DEBUG
            WizDbPath = "C:\\Users\\duanxin\\Documents\\My Knowledge\\Data\\xxinduan@hotmail.com\\index.db";
#endif
            if (string.IsNullOrEmpty(WizDbPath))
                throw new Exception("为知笔记数据库路径为空！");
            if (string.IsNullOrEmpty(WizDefaultFolder))
                throw new Exception("必须指定为知笔记默认保存文件夹！");

            var wizDb = new WizDatabaseClass();
            wizDb.Open(WizDbPath);

            //列举文件夹
            //var folders = (WizFolderCollection)wizDb.Folders;
            //foreach (WizFolder fd in folders)
            //{
            //    Debug.WriteLine(fd.Name);
            //}

            WizDocument document = null;
            //查找相同URL的文档
            if (!string.IsNullOrEmpty(Url))
            {
                var documents = (WizDocumentCollection)wizDb.DocumentsFromURL(Url);
                if (documents.count > 0)
                    document = (WizDocument)documents[0];
            }

            //创建新文档
            //WizFolderLocation = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];
            if (document == null)
            {

                //查找相似文档的目录
                var splits = Name.Split();
                var firstWord = true;
                var sb = new StringBuilder();
                foreach (var split in splits)
                {
                    if (split.Length <= 2) continue;
                    if (firstWord)
                    {
                        sb.Append($"DOCUMENT_TITLE like '%{split}%'");
                        firstWord = false;
                        continue;
                    }
                    sb.Append($"AND DOCUMENT_TITLE like '%{split}%'");
                }

                var similarDocuments = (WizDocumentCollection)wizDb.DocumentsFromSQL(sb.ToString());
                if (similarDocuments.count > 0)
                {
                    var similarDocument = (WizDocument)similarDocuments[0];
                    WizFolderLocation = similarDocument.Location;
                }

                //Folder 对象 https://www.wiz.cn/m/api/descriptions/IWizFolder.html
                var folder = (WizFolder)wizDb.GetFolderByLocation(WizFolderLocation, false);
                //在目录下创建文档
                document = (WizDocument)folder.CreateDocument(Title, DocFileName, Url);
            }




            var flag = wizUpdateDocumentIncludeScript;
            //更新文档内容、标题 https://www.wiz.cn/m/api/descriptions/IWizDocument.html
            document.UpdateDocument3(Content, flag);
            document.Title = Title;
            WizFolderLocation = document.Location;

            wizDb.Close();
        }

        public void SaveAsPiz()
        {
            FindWizLocation();
            var targetPath = FindWizLocation();
            if (string.IsNullOrEmpty(targetPath))
                throw new Exception("找不到文章保存路径");

            if(string.IsNullOrEmpty(MyPageTempPath)||!Directory.Exists(MyPageTempPath))
                throw new Exception("找不到My pages临时目录");

            var guId = Guid.NewGuid().ToString();
            _pageDocTempPath = Path.Combine(MyPageTempPath, guId);
            if (Directory.Exists(_pageDocTempPath))
                Directory.Delete(_pageDocTempPath, true);
            Directory.CreateDirectory(_pageDocTempPath);


            //下载图片
            var doc = ProcessDocument();
            var indexHtmlFile = Path.Combine(_pageDocTempPath, "index.html");
            doc.Save(indexHtmlFile);


            //创建manifest
            var fileName = Path.Combine(_pageDocTempPath, "manifest.json");
            var jo = File.Exists(fileName) ? JObject.Parse(File.ReadAllText(fileName)) : new JObject();

            jo["title"] = _title;
            jo["originalUrl"] = Url;
            jo["archiveTime"] = DateTime.UtcNow.ToString("s") + "Z";
            jo["rate"] = 0;

            File.WriteAllText(fileName, JsonConvert.SerializeObject(jo, JsonSerializerSettings), Encoding.UTF8);

            //zip打包
            var tempZip = Path.Combine(MyPageTempPath, $"{guId}.zip");
            if (File.Exists(tempZip))
                File.Delete(tempZip);
            System.IO.Compression.ZipFile.CreateFromDirectory(_pageDocTempPath, tempZip);

            //移动到目标路径
            PizFileName = $"{Title} - 0DayDown - {DateTime.Now:yyyy_MM_dd_HH_mm_ss}.piz".MakeValidFileName();
            PizFileName = Path.Combine(targetPath, PizFileName);
            File.Move(tempZip, PizFileName);


            //删除临时目录
            Directory.Delete(_pageDocTempPath,true);

            //提交my page 索引
            MyPageLib.MyPageIndexer.Instance.Enqueue(PizFileName);

        }

        private static readonly JsonSerializerSettings JsonSerializerSettings
            = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };


        private HtmlAgilityPack.HtmlDocument ProcessDocument()
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(Content);

            //处理图片
            var imageNodes = doc.DocumentNode.SelectNodes("//img")?.ToList();
            foreach (var imageNode in imageNodes)
            {
                var imgsrc = imageNode.Attributes["src"].Value;
                System.Diagnostics.Debug.WriteLine(imgsrc);
                if (Utility.IsValidateImageUrl(imgsrc))
                {
                    if (!DownloadImage(imgsrc, out var imgPath, out var message))
                        throw new Exception(message);

                    imageNode.Attributes["src"].Value = imgPath;
                    continue;

                    

                }

                imgsrc = imageNode.Attributes["data-src"].Value;
                if (Utility.IsValidateImageUrl(imgsrc))
                {
                    if (!DownloadImage(imgsrc, out var imgPath, out var message))
                        throw new Exception(message);

                    imageNode.Attributes["src"].Value = imgPath;
                    imageNode.Attributes.Remove("data-src");
                }

                

            }

            //删除 <span>?<span>
            var nodes = doc.DocumentNode.SelectNodes("//span")?.Where(t => t.InnerLength <=2)
                .ToList();
            if (nodes != null)
            {

                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }


            //删除 <p>?</p>
            nodes = doc.DocumentNode.SelectNodes("//p")?.Where(t => t.InnerLength <= 2)
                .ToList();
            if (nodes != null)
            {

                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }

            return doc;
        }


        private bool DownloadImage(string url,out string imgPath,out string message)
        {
            imgPath = null;
            message = null;
            try
            {
                var uri = new Uri(url);
                var fileName = Path.GetFileName(uri.LocalPath);
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception($"图片URL {url} 没有图片名称。");

                var imgsPath = Path.Combine(_pageDocTempPath, "images");
                if (!Directory.Exists(imgsPath))
                    Directory.CreateDirectory(imgsPath);

                var localFileName = Path.Combine(imgsPath, fileName);
                using var client = new WebClient();
                    client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                    client.DownloadFile(new Uri(url), localFileName);

                imgPath = $"images/{fileName}";
            }
            catch (Exception e)
            {
                imgPath = null;
                message = e.Message;
                return false;
            }
            
            return true;
        }


        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string FindWizLocation()
        {
            var wizDocPath = string.Empty;
//#if DEBUG

//            WizDbPath = "C:\\Users\\duanxin\\Documents\\My Knowledge\\Data\\xxinduan@hotmail.com\\index.db";
//#endif
            if (string.IsNullOrEmpty(WizDbPath))
                throw new Exception("为知笔记数据库路径为空！");
            if (string.IsNullOrEmpty(WizDefaultFolder))
                throw new Exception("必须指定为知笔记默认保存文件夹！");

            var wizDb = new WizDatabaseClass();
            wizDb.Open(WizDbPath);


            WizDocument document = null;
            //查找相同URL的文档
            if (!string.IsNullOrEmpty(Url))
            {
                var documents = (WizDocumentCollection)wizDb.DocumentsFromURL(Url);
                if (documents.count > 0)
                {
                    document = (WizDocument)documents[0];
                    wizDocPath = document.Location;
                }
            }

            //创建新文档
            //WizFolderLocation = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];
            if (document == null)
            {

                //查找相似文档的目录
                var splits = Name.Split();
                var firstWord = true;
                var sb = new StringBuilder();
                foreach (var split in splits)
                {
                    if (split.Length <= 2) continue;
                    if (firstWord)
                    {
                        sb.Append($"DOCUMENT_TITLE like '%{split}%'");
                        firstWord = false;
                        continue;
                    }
                    sb.Append($"AND DOCUMENT_TITLE like '%{split}%'");
                }

                var similarDocuments = (WizDocumentCollection)wizDb.DocumentsFromSQL(sb.ToString());
                if (similarDocuments.count > 0)
                {
                    var similarDocument = (WizDocument)similarDocuments[0];
                    wizDocPath = similarDocument.Location;
                }

                //Folder 对象 https://www.wiz.cn/m/api/descriptions/IWizFolder.html
                var folder = (WizFolder)wizDb.GetFolderByLocation(WizFolderLocation, false);

            }
            wizDb.Close();
            if (string.IsNullOrEmpty(wizDocPath)) return string.Empty;
            if(wizDocPath.StartsWith("/") || wizDocPath.StartsWith("\\"))
                wizDocPath = wizDocPath.Substring(1);

            wizDocPath = wizDocPath.Replace("/", "\\");
            var rootPath = Path.GetDirectoryName(WizDbPath);

            return Path.Combine(rootPath,wizDocPath);
        }


        //检查wiz笔记数据库是否有同名同url的文章
        public bool ExistsInWizDb()
        {
            var wizDb = new WizDatabaseClass();
            wizDb.Open(WizDbPath);
            var where = $"DOCUMENT_TITLE='{Title}' AND DOCUMENT_URL='{Url}'";
            var documents = (WizDocumentCollection)wizDb.DocumentsFromSQL(where);
            var yes = documents.count > 0;
            wizDb.Close();
            return yes;
        }

        public bool ExistsInMyPageDb()
        {
            var pageDb = new MyPageDb();
            var doc = pageDb.FindOriginUrl(Url);
            
            return doc!=null;
        }


        //检查wiz笔记数据库是否有类似标题的文章
        public bool SimilarInWizDb()
        {
            var splits = Name.Split();
            var firstWord = true;
            var sb = new StringBuilder();
            foreach (var split in splits)
            {
                if (split.Length <= 2 || SimilarSkipWords.Any(x=>split.ToLower()==x)) continue;
                if (firstWord)
                {
                    sb.Append($"DOCUMENT_TITLE like '%{split}%'");
                    firstWord = false;
                    continue;
                }
                sb.Append($"AND DOCUMENT_TITLE like '%{split}%'");
            }

            var wizDb = new WizDatabaseClass();
            wizDb.Open(WizDbPath);

            var similarDocuments = (WizDocumentCollection)wizDb.DocumentsFromSQL(sb.ToString());
            var yes = similarDocuments.count > 0;
            wizDb.Close();
            return yes;
        }
    }
}
