using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AngleSharp.Html.Parser;
using MovieTorrents.Common;

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
            public int? Year { get; set; }

            public decimal Rating { get; private set; }


            public void Parse()
            {
                if (!string.IsNullOrEmpty(DouBanRating) && decimal.TryParse(DouBanRating, out var result))
                {
                    Rating = result;
                }

                //年份

                var title = TorrentFile.PurifyName(Title);
                if (string.IsNullOrEmpty(title)) return;
                var chinese = TorrentFile.ExtractChinese(title);
                if (!string.IsNullOrEmpty(chinese)) title = chinese;
                var i = title.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                if (i == -1) i = title.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                if (i == -1) i = title.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
                if (i == -1) i = title.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);
                Keyword = i > 0 ? title.Substring(0, i) : title;

                //查找
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
            btArchiveTorrent.Click += BtArchiveTorrent_Click;
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
                    if (string.IsNullOrEmpty(detailLink)) continue;

                    var btItem = new BtItem { Title = title };

                    //下载具体页面
                    var pageData = client.DownloadData($"{BtHomeUrl}{detailLink}");
                    var pageCode = Encoding.UTF8.GetString(pageData);
                    //File.WriteAllText("d:\\temp\\2.txt", pageCode, Encoding.UTF8);

                    //查找豆瓣评分
                    var match = Regex.Match(pageCode, "豆瓣评分(.*)/10 from");
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
                        Checked = (btItem.Rating >= 7.0M && !TorrentFile.ExistInDb(FormMain.DbConnectionString, btItem.Keyword, btItem.Year) )
                                  || btItem.Tag.Contains("情色")
                                  
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
            var p = match.Success ? int.Parse(match.Groups[1].Value) + 1 : 2;

            tbUrl.Text = $"{BtHomeUrl}index-index-page-{p}.htm";

            btnQuery.PerformClick();
        }

        //上一页
        private void btnPrev_Click(object sender, EventArgs e)
        {
            var match = Regex.Match(tbUrl.Text, $"{BtHomeUrl}index-index-page-([0-9]*).htm");
            if (match.Success)
            {
                var p = int.Parse(match.Groups[1].Value);
                if (p > 2)
                {
                    p--;
                    tbUrl.Text = $"{BtHomeUrl}index-index-page-{p}.htm";
                    btnQuery.PerformClick();

                    return;
                }

            }
            tbUrl.Text = BtHomeUrl;
            btnQuery.PerformClick();

        }

        //下载勾选的种子文件
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
                        DownloadAttachmentFile(client, $"{BtHomeUrl}{btItem.AttachmentUrl}", SanitizeFileName(btItem.Title));
                        i++;
                    }
                    catch (Exception exception)
                    {
                        msg += $"\r\n下载 {btItem.Title} 错误:{exception.Message}";
                    }
                }
            }
            Cursor = c;

            msg = $"下载了{i}个文件。{msg}";
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //下载种子附件
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
                    fileName = fileName.Replace("[BT下载]","");
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


        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var btItem = (BtItem)lvResults.SelectedItems[0].Tag;
            tbTitle.Text = btItem.Keyword;


        }

        //解压zip文件
        private void btUnzip_Click(object sender, EventArgs e)
        {
            var zipFiles = Directory.GetFiles(tbDownloadPath.Text).Where(s => Path.GetExtension(s).ToLowerInvariant() == ".zip");
            var i = 0;
            var msg = "";
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
                        entry.ExtractToFile(Path.Combine(tbDownloadPath.Text, Path.GetFileNameWithoutExtension(file) + ".torrent"));
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

            MessageBox.Show($"成功解压 {i} 个文件。{msg}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            lvTorrents.Items.Clear();
            var text = tbTitle.Text.Trim();
            if (string.IsNullOrEmpty(text) || text.Length < 2) return;

            var torrents = TorrentFile.Search(FormMain.DbConnectionString, tbTitle.Text, out var msg);
            if (torrents == null)
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            foreach (var torrentFile in torrents)
            {
                string[] row = { torrentFile.name,
                    torrentFile.rating.ToString(),
                    torrentFile.year,
                    torrentFile.seelater.ToString(),
                    torrentFile.seenowant.ToString(),
                    torrentFile.seeflag.ToString(),
                    torrentFile.seedate,
                    torrentFile.seecomment
                };
                lvTorrents.Items.Add(new ListViewItem(row));
            }
        }

        private void FormBtBtt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = true;
            Hide();
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbSearch.Text)) return;
            tbUrl.Text = $"https://www.btbtt.me/search-index-keyword-{tbSearch.Text}.htm";
            QueryPage();
        }

        //将目录下的种子文件转移到收藏目录
        private void BtArchiveTorrent_Click(object sender, EventArgs e)
        {
            var msg = "";
            var i = 0;
            try
            {
                var files = Directory.GetFiles(tbDownloadPath.Text, "*.torrent");

                foreach (var file in files)
                {
                    var destFileName = Path.GetFileName(file).NormalizeTorrentFileName();
                    var year = TorrentFile.ExtractYear(destFileName);
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

            MessageBox.Show($"成功转移 {i} 个文件。{msg}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
