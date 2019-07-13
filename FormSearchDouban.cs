using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            tbOrigTitle.Text = "原标题：" +  _torrentFile.PurifiedName;

            tbSearchText.Text = _torrentFile.ChineseName;

#if DEBUG
            tbSearchText.Text = "https://movie.douban.com/subject/26811825/";
#endif
        }

        private void DoSearcch(bool searchId=false)
        {
            listView1.Items.Clear();
            var subjects = searchId ? DoubanSubject.SearchById(tbSearchText.Text.Trim(), out var msg)
                : DoubanSubject.SearchSuggest(tbSearchText.Text.Trim(), out msg);
            //if(subjects.Count==0)
            //    subjects = DoubanSubject.SearchSubject(tbSearchText.Text.Trim());
            tbInfo.Text = msg;

            foreach (var subject in subjects)
            {
                string[] row = {subject.title,
                                subject.sub_title,
                                subject.year,
                                subject.type
                            };

                listView1.Items.Add(new ListViewItem(row) { Tag = subject });
            }

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
                using (var stream = new FileStream(subject.img_local, FileMode.Open, FileAccess.Read))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }


        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var subject = (DoubanSubject)listView1.SelectedItems[0].Tag;
            if (!subject.TryQueryDetail(out var msg))
            {
                MessageBox.Show($"查找豆瓣详细信息失败：{msg}", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_torrentFile.UpdateDoubanInfo(FormMain.DbConnectionString, subject,out msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            DoubanSubject = subject;
            DialogResult = DialogResult.OK;

        }

        private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) DoSearcch();
        }

        
    }
}
