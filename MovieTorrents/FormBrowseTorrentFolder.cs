using System;
using System.IO;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormBrowseTorrentFolder : Form
    {
        private FolderBrowserDialog _folderBrowserDialog1;

        public string SelectedPath { get; private set; }

        public FormBrowseTorrentFolder()
        {
            InitializeComponent();
        }

        

        private void FormBrowseTorrentFolder_Load(object sender, EventArgs e)
        {
            _folderBrowserDialog1=new FolderBrowserDialog(){ShowNewFolderButton = true};
            btSelectPath.Click += BtSelectPath_Click;
            btOk.Click += BtOk_Click;
        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            SelectedPath = tbSelectedPath.Text.Trim();
            if (string.IsNullOrEmpty(SelectedPath) || !Directory.Exists(SelectedPath))
            {
                MessageBox.Show("请选择一个正确的目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbSelectedPath.SelectAll();
                tbSelectedPath.Focus();
                return;
            }

            SelectedPath = SelectedPath.TrimEnd('\\');

            DialogResult = DialogResult.OK;
        }

        private void BtSelectPath_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TorrentFile.TorrentFilePath))
                _folderBrowserDialog1.SelectedPath = TorrentFile.TorrentFilePath;
            if (_folderBrowserDialog1.ShowDialog(this) == DialogResult.Cancel) return;
            tbSelectedPath.Text = _folderBrowserDialog1.SelectedPath;
        }
    }
}
