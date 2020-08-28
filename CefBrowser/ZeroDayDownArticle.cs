using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WizKMCoreLib;
using mySharedLib;

namespace CefBrowser
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

        private string _html;

        public string Html
        {
            get => _html;
            set

            {
                _html = value;
                Parse();
            }
        }

        public string Url { get; private set; }
        public string Content { get;private set; }
        public string Title { get; private set; }
        public string Version { get; private set; }
        public string Name { get; set; }
        public string DocFileName { get; private set; }
        public string WizFolderLocation { get; private set; }
        public void Parse()
        {
            //查找url
            var match = Regex.Match(_html, "SourceURL:(.*)[^\\n]");
            Url = match.Success ? match.Groups[1].Value:"";
            //File.WriteAllText("d:\\temp\\1.txt", _html, Encoding.UTF8);

            //抽取剪贴板正文
            Content = ExtractHtmlCopyHeader(Html);

            //抽取0DayDown文章的标题
            match = Regex.Match(Html, "<a(.*)>(.*?)</a></h1>");//非贪婪模式
            Title = match.Success&&match.Groups.Count==3? match.Groups[2].Value:"";

            //抽取版本号
            match = Regex.Match(Title, "\\s(\\d+.[^\\s]*)\\b");
            Version = match.Success ? match.Groups[1].Value:"";
            Debug.WriteLine($"Version={Version}");

            //名称
            if (string.IsNullOrEmpty(Version))
                Name = Title;
            else
            {
                var n = Title.IndexOf(Version,StringComparison.OrdinalIgnoreCase);
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
        public static void SaveFromClipboard()
        {
            if (!Clipboard.ContainsText(TextDataFormat.Html))
            {
                MessageBox.Show("剪贴板没有html");
                return;
            }
            var article = new ZeroDayDownArticle { Html = Clipboard.GetText(TextDataFormat.Html) };
            if (string.IsNullOrEmpty(article.Title))
            {
                MessageBox.Show("没有找到文章标题");
                return;
            }

            try
            {
                article.SaveToWiz();
                MessageBox.Show($"保存成功!\r\n{article.Title}\r\n{article.WizFolderLocation}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        //保存为为知笔记文档
        public void SaveToWiz()
        {
            //打开数据库 https://www.wiz.cn/m/api/descriptions/IWizDatabase.html
            var dbPath = System.Configuration.ConfigurationManager.AppSettings["IndexDbPath"];
#if DEBUG
            dbPath = "C:\\Users\\duanxin\\Documents\\My Knowledge\\Data\\xxinduan@hotmail.com\\index.db";
#endif
            var wizDb = new WizDatabaseClass();
            wizDb.Open(dbPath);

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
            WizFolderLocation = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];
            if (document == null)
            {

                //查找相似文档的目录
                var splits = Name.Split();
                var firstWord = true;
                var sb = new StringBuilder();
                foreach (var split in splits)
                {
                    if(split.Length<=2) continue;
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
            else
            {
                WizFolderLocation = document.Location;
            }


            var flag = wizUpdateDocumentIncludeScript;
            document.UpdateDocument3(Content, flag);
            document.Title = Title;

            wizDb.Close();
        }
    }
}
