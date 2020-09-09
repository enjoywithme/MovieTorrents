using System;
using System.Threading;
using System.Windows.Forms;
using mySharedLib;

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
            Resize += FormBtBtt_Resize;
            tbSearch.KeyDown += TbSearch_KeyDown;
            tbSearch.Pasted += TbSearch_Pasted;
            tbUrl.KeyDown += TbUrl_KeyDown;
            btLog.Click += BtLog_Click;
        }

        private void TbSearch_Pasted(object sender, ClipboardEventArgs e)
        {
            tbSearch.Text = e.ClipboardText;
            DoSearch();
        }

        private void FormBtBtt_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void TbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                DoQuery();
        }

        private void BtLog_Click(object sender, EventArgs e)
        {
            if (!MyLog.OpenLog(out var msg))
                MessageBox.Show(this, msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void TbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return) return;
            DoSearch();
        }

        private void DoSearch()
        {
            if (!CheckAutoDownloading()) return;
            if (string.IsNullOrEmpty(tbSearch.Text.Trim())) return;
            tbUrl.Text = BtBtItem.SearPageUrl(tbSearch.Text.Trim());
            DoQuery();
        }
        private void DoQuery()
        {
            var c = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
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
                        string[] row =
                        {
                            btItem.Title,
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = c;
                Interlocked.Exchange(ref BtBtItem.AutoDownloadRunning, 0);
            }
            
        }


        //下一页
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!CheckAutoDownloading()) return;

            tbUrl.Text = BtBtItem.NextPageUrl(tbUrl.Text);
            DoQuery();
        }

        //上一页
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (!CheckAutoDownloading()) return;

            tbUrl.Text = BtBtItem.PrevPageUrl(tbUrl.Text);
            DoQuery();

        }

        //下载勾选的种子文件
        private void btDownload_Click(object sender, EventArgs e)
        {
            if (!CheckAutoDownloading()) return;

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
            Interlocked.Exchange(ref BtBtItem.AutoDownloadRunning, 0);

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

       
        //检查是否正在自动下载
        private bool CheckAutoDownloading()
        {
            if (0 == Interlocked.Exchange(ref BtBtItem.AutoDownloadRunning, 1)) return true;
            MessageBox.Show("自动下载正在运行，稍后重试。","提示",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
            return false;

        }

        //将目录下的种子文件转移到收藏目录
        private void BtArchiveTorrent_Click(object sender, EventArgs e)
        {
            var i = BtBtItem.ArchiveTorrentFiles(out var msg);
            MessageBox.Show($"成功转移 {i} 个文件。{msg}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbAutoDownload_CheckedChanged(object sender, EventArgs e)
        {
            BtBtItem.EnableAutoDownload(cbAutoDownload.Checked);
        }
    }
}
