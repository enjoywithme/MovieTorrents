using System;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormBtBtt : Form
    {

        public FormBtBtt()
        {
            InitializeComponent();
        }

        private void FormBtBtt_Load(object sender, EventArgs e)
        {
            tbUrl.Text = BtBtItem.BtBtHomeUrl;
            btArchiveTorrent.Click += BtArchiveTorrent_Click;
#if DEBUG
            tbSearch.Text = "模范刑警";
#endif

        }


        private void DoQuery()
        {
            var c = Cursor;
            Cursor = Cursors.WaitCursor;

            lvResults.Items.Clear();
            var btItems = BtBtItem.QueryPage(tbUrl.Text.Trim(), out var msg);
            if (btItems == null)
            {
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreach (var btItem in btItems)
                {
                    string[] row = { btItem.Title,
                        btItem.PublishTime,
                        btItem.DouBanRating,
                        btItem.Gene,
                        btItem.Tag
                    };
                    lvResults.Items.Add(new ListViewItem(row)
                    {
                        Tag = btItem,
                        Checked = btItem.Checked

                    });
                }
            }


            Cursor = c;

        }


        //下一页
        private void btnNext_Click(object sender, EventArgs e)
        {
            tbUrl.Text = BtBtItem.NextPageUrl(tbUrl.Text);
            DoQuery();
        }

        //上一页
        private void btnPrev_Click(object sender, EventArgs e)
        {
            tbUrl.Text = BtBtItem.PrevPageUrl(tbUrl.Text);
            DoQuery();

        }

        //下载勾选的种子文件
        private void btDownload_Click(object sender, EventArgs e)
        {
            if (lvResults.CheckedItems.Count == 0) return;
            var c = Cursor;
            Cursor = Cursors.WaitCursor;
            var message = "";
            var i = 0;
            foreach (ListViewItem checkedItem in lvResults.CheckedItems)
            {
                var btItem = (BtBtItem)checkedItem.Tag;
                i += btItem.DownLoadAttachments(out var msg);
                if (!string.IsNullOrEmpty(msg))
                    message += $"{msg}\r\n";

            }
            Cursor = c;

            message = $"下载了{i}个文件。{message}";
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var btItem = (BtBtItem)lvResults.SelectedItems[0].Tag;
            tbTitle.Text = btItem.Keyword;


        }

        //解压zip文件
        private void btUnzip_Click(object sender, EventArgs e)
        {
            var i = BtBtItem.ExtractZipFiles(out var msg);
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
            if (string.IsNullOrEmpty(tbSearch.Text.Trim())) return;
            tbUrl.Text = BtBtItem.SearPageUrl(tbSearch.Text.Trim());
            DoQuery();
        }

        //将目录下的种子文件转移到收藏目录
        private void BtArchiveTorrent_Click(object sender, EventArgs e)
        {
            var i = BtBtItem.ArchiveTorrentFiles(out var msg);
            MessageBox.Show($"成功转移 {i} 个文件。{msg}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
