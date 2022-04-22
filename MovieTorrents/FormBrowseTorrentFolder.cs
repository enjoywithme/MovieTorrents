using System;
using System.IO;
using System.Windows.Forms;
using MovieTorrents.Properties;

namespace MovieTorrents
{
    public partial class FormBrowseTorrentFolder : Form
    {
        private FolderBrowserDialog _folderBrowserDialog1;
        public bool FolderRename { get; private set; }
        public string SelectedPath { get; private set; }

        public FormBrowseTorrentFolder(bool showFolderRename=false)
        {
            InitializeComponent();
            checkBox1.Visible = showFolderRename;
        }

        

        private void FormBrowseTorrentFolder_Load(object sender, EventArgs e)
        {
            var tvPath = Path.Combine(TorrentFile.TorrentRootPath, "[剧集]");
            if (Directory.Exists(tvPath))
            {
                foreach (var directory in Directory.GetDirectories(tvPath))
                {
                    var dirName=new DirectoryInfo(directory).Name;
                    if (dirName.StartsWith("[") && dirName.EndsWith("]"))
                        tbSelectedPath.Items.Add(directory+"\\");
                }
            }

            tbSelectedPath.Text = Path.Combine(TorrentFile.TorrentRootPath, "[剧集]\\[美剧]\\");

            _folderBrowserDialog1=new FolderBrowserDialog(){ShowNewFolderButton = true};
            btSelectPath.Click += BtSelectPath_Click;
            btOk.Click += BtOk_Click;

            checkBox1.CheckedChanged+=(_, _) => FolderRename=checkBox1.Checked;
        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            SelectedPath = tbSelectedPath.Text.Trim();
            if (!CheckFolder(SelectedPath))
            {
                MessageBox.Show(Resources.TextSelectCorrectFolder, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbSelectedPath.SelectAll();
                tbSelectedPath.Focus();
                return;
            }

            //SelectedPath = SelectedPath.TrimEnd('\\');

            DialogResult = DialogResult.OK;
        }

        private bool CheckFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (!FolderRename) return Directory.Exists(path);

            var d = Directory.GetParent(path);
            return d is { Exists: true };

        }

        private void BtSelectPath_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TorrentFile.TorrentRootPath))
                _folderBrowserDialog1.SelectedPath = TorrentFile.TorrentRootPath;
            if (_folderBrowserDialog1.ShowDialog(this) == DialogResult.Cancel) return;
            tbSelectedPath.Text = _folderBrowserDialog1.SelectedPath;
        }
    }
}
