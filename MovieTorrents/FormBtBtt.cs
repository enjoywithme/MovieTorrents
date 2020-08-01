using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Html.Parser;

namespace MovieTorrents
{
    public partial class FormBtBtt : Form
    {

        public class BtItem
        {
            public string Title { get; set; }
            public string DouBanRating { get; set; }
            public string AttachmentUrl { get; set; }
            public string PublishTime { get; set; }
            public string Keyword { get; set; }
            public string Gene { get; set; }
            public string Tag { get; set; }

            public decimal Rating { get; private set; }
                

            public void Parse()
            {
                var b = decimal.TryParse(DouBanRating, out var result);
                Rating = b?result:0;

                var i = Title.IndexOf(".");
                if(i==-1) return;
                Keyword = Title.Substring(0, i);
            }
        }

        private const string BtHomeUrl = "https://www.btbtt.me/";
        public FormBtBtt()
        {
            InitializeComponent();
        }

        private void FormBtBtt_Load(object sender, EventArgs e)
        {
            tbUrl.Text = BtHomeUrl;
#if DEBUG
            tbDownloadPath.Text = "x:\\temp";
#endif
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var c = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                QueryPage();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor = c;

        }

        private void QueryPage()
        {
            var pageUrl = tbUrl.Text.Trim();
            if (string.IsNullOrEmpty(pageUrl)) return;
            var parser = new HtmlParser();

            using (var client = new WebClient()) // WebClient class inherits IDisposable
            {
                if (!string.IsNullOrEmpty(tbProxy.Text.Trim()))
                {
                    var wp = new WebProxy(tbProxy.Text);
                    client.Proxy = wp;
                }
                
                var htmlData = client.DownloadData(pageUrl);
                var htmlCode = Encoding.UTF8.GetString(htmlData);
                //File.WriteAllText("d:\\temp\\1.txt", htmlCode, Encoding.UTF8);
                //var htmlCode = File.ReadAllText("d:\\temp\\1.txt");
                var document = parser.ParseDocument(htmlCode);

                var links = document.All.Where(a => a.LocalName == "a" && a.ClassList.Contains("subject_link"));
                lvResults.Items.Clear();
                foreach (var link in links)
                {
                    var title = link.TextContent.Trim();
                    if (string.IsNullOrEmpty(title)) continue;
                    
                    var detailLink = link.Attributes["href"].Value;
                    if(string.IsNullOrEmpty(detailLink)) continue;

                    var btItem = new BtItem {Title = title};

                    //下载具体页面
                    var pageData = client.DownloadData($"{BtHomeUrl}{detailLink}");
                    var pageCode = Encoding.UTF8.GetString(pageData);

                    //查找豆瓣评分
                    var match = Regex.Match(pageCode, "豆瓣评分(.*)/10 from");
                    btItem.DouBanRating = match.Success ? match.Groups[1].Value.Trim() : "";

                    //<span class="grey">发帖时间：</span><b>.*</b>
                    match = Regex.Match(pageCode, "<span class=\"grey\">发帖时间：</span><b>(.*)</b>");
                    btItem.PublishTime = match.Success ? match.Groups[1].Value : "";

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
                    if (attachments.Length > 0)
                    {
                        Debug.Print(attachments[0].Attributes["href"].Value);
                        var attachUrl = attachments[0].Attributes["href"].Value.Replace("dialog", "download");
                        btItem.AttachmentUrl = attachUrl;
                    }

                    btItem.Parse();
                    string[] row = { btItem.Title,
                        btItem.PublishTime,
                        btItem.DouBanRating,
                        btItem.Gene,
                        btItem.Tag
                    };
                    
                    Debug.WriteLine($"{btItem.Title} {btItem.Keyword} {btItem.DouBanRating} {btItem.Rating}");
                    lvResults.Items.Add(new ListViewItem(row)
                    {
                        Tag = btItem,
                        Checked = (btItem.Rating >= 7.0M)
                    });

                }



            }

        }

        

        private string SanitizeFileName(string fileName)
        {
            fileName = fileName.Replace("/", " ");
            fileName = fileName.Replace(":", "：");

            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }

            return fileName;
        }

        private void btnHomePage_Click(object sender, EventArgs e)
        {
            tbUrl.Text = BtHomeUrl;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var match = Regex.Match(tbUrl.Text, $"{BtHomeUrl}index-index-page-([0-9]*).htm");
            var p = match.Success? int.Parse(match.Groups[1].Value) + 1:2;

            tbUrl.Text = $"{BtHomeUrl}index-index-page-{p}.htm";
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            var match = Regex.Match(tbUrl.Text, $"{BtHomeUrl}index-index-page-([0-9]*).htm");
            if (match.Success)
            {
                var p =int.Parse(match.Groups[1].Value);
                if (p > 2) 
                {
                    p--;
                    tbUrl.Text = $"{BtHomeUrl}index-index-page-{p}.htm";
                    return;
                }

            }
            tbUrl.Text = BtHomeUrl;

        }

        private void btDownload_Click(object sender, EventArgs e)
        {
            if (lvResults.CheckedItems.Count == 0) return;
            var c = Cursor;
            Cursor = Cursors.WaitCursor;
            var msg = "";
            var i = 0;
            using (var client = new WebClient()) // WebClient class inherits IDisposable
            {
                if (!string.IsNullOrEmpty(tbProxy.Text.Trim()))
                {
                    var wp = new WebProxy(tbProxy.Text);
                    client.Proxy = wp;
                }

                foreach (ListViewItem checkedItem in lvResults.CheckedItems)
                {
                    var btItem = (BtItem)checkedItem.Tag;
                    try
                    {
                        DownloadAttachmentFile(client,$"{BtHomeUrl}{btItem.AttachmentUrl}", SanitizeFileName(btItem.Title));
                        i++;
                    }
                    catch (Exception exception)
                    {
                        msg +=  $"\r\n下载 {btItem.Title} 错误:{exception.Message}";
                    }
                }
            }
            Cursor = c;

            msg = $"下载了{i}个文件。{msg}";
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DownloadAttachmentFile(WebClient client, string fileUrl, string fileName)
        {
            using (var rawStream = client.OpenRead(fileUrl))
            {
                if (rawStream == null) return;

                var originalFileName = string.Empty;
                var contentDisposition = client.ResponseHeaders["content-disposition"];
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    var match = Regex.Match(contentDisposition, "filename=\"(.*)\"", RegexOptions.IgnoreCase);
                    if (match.Success)
                        originalFileName = match.Groups[1].Value;
                }
                if (originalFileName.Length > 0)
                {
                    fileName += Path.GetExtension(originalFileName);
                    var path = Path.Combine(tbDownloadPath.Text, fileName);

                    using (var outputFileStream = new FileStream(path, FileMode.Create))
                    {
                        rawStream.CopyTo(outputFileStream);
                    }
                }
                rawStream.Close();
            }

            /*
            var request = WebRequest.Create(fileUrl);
            var response = request.GetResponse();
            var originalFileName = response.Headers["Content-Disposition"];
            fileName = fileName + Path.GetFileName(originalFileName);
            var streamWithFileBody = response.GetResponseStream();
            if(streamWithFileBody==null) return;
            var path = Path.Combine("d:\\temp", fileName);
            using (var outputFileStream = new FileStream(path, FileMode.Create))
            {
                streamWithFileBody.CopyTo(outputFileStream);
            }*/
        }
    }
}
