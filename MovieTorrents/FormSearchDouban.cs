using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using MovieTorrents.Common;
using MovieTorrents.WebPWrapper;
using Nito.AsyncEx.Synchronous;

namespace MovieTorrents
{
    public partial class FormSearchDouban : Form
    {
        private readonly TorrentFile _torrentFile;
        public DouBanSubject DouBanSubject { get; private set; }

        public FormSearchDouban(TorrentFile torrentFile)
        {
            InitializeComponent();
            _torrentFile = torrentFile;
        }

        private async void FormSearchDouban_Load(object sender, EventArgs e)
        {
            tbOrigTitle.Text = "原标题：" + _torrentFile.PurifiedName;
            tbSearchText.Text = _torrentFile.FirstName;
            if (!string.IsNullOrWhiteSpace(tbSearchText.Text))
                await DoSearcch();

#if DEBUG
            //tbSearchText.Text = "https://movie.douban.com/subject/26811825/";
#endif
        }



        private async Task DoSearcch(bool searchId = false)
        {
            listView1.Items.Clear();
#if true


            var sr = searchId ? await DouBanSubject.SearchById(tbSearchText.Text.Trim()) 
                : await DouBanSubject.SearchSuggest(tbSearchText.Text.Trim());

            //if(subjects.Count==0)
            //    subjects = DoubanSubject.SearchSubject(tbSearchText.Text.Trim());
            tbInfo.Text = sr.Message;

#else
            var subjects = new List<DoubanSubject>()
            {
                DoubanSubject.InitFromPageHtml(@"https://movie.douban.com/subject/1866471/",
                    File.ReadAllText(@"d:\temp\2.txt"))
            };

#endif

            foreach (var subject in sr.Subjects)
            {
                string[] row = {subject.title,
                    subject.sub_title,
                    subject.year,
                    subject.type
                };

                listView1.Items.Add(new ListViewItem(row) { Tag = subject });
            }

            if (listView1.Items.Count > 0)
                listView1.Items[0].Selected = true;

        }


        private async void btSearch_Click(object sender, EventArgs e)
        {
            await DoSearcch();
        }

        private async void btSearchId_Click(object sender, EventArgs e)
        {
            await DoSearcch(true);
        }


        private async void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            if (listView1.SelectedItems.Count == 0) return;

            var subject = (DouBanSubject)listView1.SelectedItems[0].Tag;

            if(string.IsNullOrEmpty(subject.ImgLocal))
              await subject.TryToDownloadSubjectImg();

            if (string.IsNullOrEmpty(subject.ImgLocal) || !File.Exists(subject.ImgLocal)) return;

            if((DouBanSubject)listView1.SelectedItems[0].Tag!=subject)
                return;

            try
            {
                var ext = Path.GetExtension(subject.ImgLocal);
                if (ext.Equals(".webp", StringComparison.InvariantCultureIgnoreCase))
                {
                    using var webp = new WebP();
                    pictureBox1.Image = webp.Load(subject.ImgLocal);
                }
                else
                {
                    await using var stream = new FileStream(subject.ImgLocal, FileMode.Open, FileAccess.Read);
                    pictureBox1.Image = Image.FromStream(stream);
                }

            }
            catch (Exception exception)
            {
                tbInfo.AppendText(exception.Message);
            }



        }

        private async void btSave_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var subject = (DouBanSubject)listView1.SelectedItems[0].Tag;
            var (ret, msg) = await subject.TryQueryDetail();
            if (!ret)
            {
                MessageBox.Show($"查找豆瓣详细信息失败：{msg}", Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_torrentFile.UpdateDoubanInfo(subject, out msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            DouBanSubject = subject;
            DialogResult = DialogResult.OK;

        }

        private async void tbSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) await DoSearcch();
        }

        private void btnSearchBrowser_Click(object sender, EventArgs e)
        {
            var searchText = Uri.EscapeUriString(tbSearchText.Text.Trim());
            var url = $"https://movie.douban.com/subject_search?search_text={searchText}";
            var formWebBrowser = new FormWebBrowser(url);
            if (formWebBrowser.ShowDialog() != DialogResult.OK || formWebBrowser.DouBanSubject == null)
            {
                Close();
                return;
            }

            if (!_torrentFile.UpdateDoubanInfo(formWebBrowser.DouBanSubject, out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            DouBanSubject = formWebBrowser.DouBanSubject;
            DialogResult = DialogResult.OK;
        }

        private void FormSearchDouban_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}
