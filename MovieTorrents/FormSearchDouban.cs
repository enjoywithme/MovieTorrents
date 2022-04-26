using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using WebPWrapper;

namespace MovieTorrents
{
    public partial class FormSearchDouban : Form
    {
        private readonly TorrentFile _torrentFile;
        public DoubanSubject DoubanSubject { get; private set; }

        public FormSearchDouban(TorrentFile torrentFile)
        {
            InitializeComponent();
            _torrentFile = torrentFile;
        }

        private void FormSearchDouban_Load(object sender, EventArgs e)
        {
            tbOrigTitle.Text = "原标题：" + _torrentFile.PurifiedName;
            tbSearchText.Text = _torrentFile.FirstName;
            if (!string.IsNullOrWhiteSpace(tbSearchText.Text))
                DoSearcch();

#if DEBUG
            //tbSearchText.Text = "https://movie.douban.com/subject/26811825/";
#endif
        }

        private void DoSearcch(bool searchId = false)
        {
            listView1.Items.Clear();
#if true
            var subjects = searchId ? 
                DoubanSubject.SearchById(tbSearchText.Text.Trim(), out var msg)
                : DoubanSubject.SearchSuggest(tbSearchText.Text.Trim(), out msg);
            //if(subjects.Count==0)
            //    subjects = DoubanSubject.SearchSubject(tbSearchText.Text.Trim());
            tbInfo.Text = msg;

#else
            var subjects = new List<DoubanSubject>()
            {
                DoubanSubject.InitFromPageHtml(@"https://movie.douban.com/subject/1866471/",
                    File.ReadAllText(@"d:\temp\2.txt"))
            };

#endif

            foreach (var subject in subjects)
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


        private void btSearch_Click(object sender, EventArgs e)
        {
            DoSearcch();
        }

        private void btSearchId_Click(object sender, EventArgs e)
        {
            DoSearcch(true);
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            if (listView1.SelectedItems.Count == 0) return;

            var subject = (DoubanSubject)listView1.SelectedItems[0].Tag;
            if (!string.IsNullOrEmpty(subject.img_local) && File.Exists(subject.img_local))
            {
                try
                {
                    var ext = Path.GetExtension(subject.img_local);
                    if (ext.Equals(".webp", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using var webp = new WebP();
                        pictureBox1.Image = webp.Load(subject.img_local);
                    }
                    else
                    {
                        using var stream = new FileStream(subject.img_local, FileMode.Open, FileAccess.Read);
                        pictureBox1.Image = Image.FromStream(stream);
                    }

                }
                catch (Exception exception)
                {
                    tbInfo.AppendText(exception.Message);
                }

            }


        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var subject = (DoubanSubject)listView1.SelectedItems[0].Tag;
            if (!subject.TryQueryDetail(out var msg))
            {
                MessageBox.Show($"查找豆瓣详细信息失败：{msg}", Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_torrentFile.UpdateDoubanInfo(subject, out msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            DoubanSubject = subject;
            DialogResult = DialogResult.OK;

        }

        private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) DoSearcch();
        }

        private void btnSearchBrowser_Click(object sender, EventArgs e)
        {
            var searchText = Uri.EscapeUriString(tbSearchText.Text.Trim());
            var url = $"https://movie.douban.com/subject_search?search_text={searchText}";
            var formWebBrowser = new FormWebBrowser(url);
            if(formWebBrowser.ShowDialog()!=DialogResult.OK || formWebBrowser.DoubanSubject==null) return;

            if (!_torrentFile.UpdateDoubanInfo(formWebBrowser.DoubanSubject, out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            DoubanSubject = formWebBrowser.DoubanSubject;
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
