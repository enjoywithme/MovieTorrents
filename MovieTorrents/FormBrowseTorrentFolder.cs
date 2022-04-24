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
        public bool CreateFolder { get; private set; }
        public string SelectedPath { get; private set; }
        private readonly bool _forFolderMove;

        public FormBrowseTorrentFolder(bool forFolderMove=false)
        {
            InitializeComponent();
            _forFolderMove= forFolderMove;
            cbRenameMoveFolder.Visible = forFolderMove;
            cbCreateFolder.Visible = !forFolderMove;
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

            _folderBrowserDialog1=new FolderBrowserDialog {ShowNewFolderButton = true};
            btSelectPath.Click += BtSelectPath_Click;
            btOk.Click += BtOk_Click;

            cbRenameMoveFolder.CheckedChanged+=(_, _) => FolderRename=cbRenameMoveFolder.Checked;
            cbCreateFolder.CheckedChanged += (_, _) => CreateFolder = cbCreateFolder.Checked;

        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            SelectedPath = tbSelectedPath.Text.Trim();
            try
            {
                CheckFolder(SelectedPath);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbSelectedPath.SelectAll();
                tbSelectedPath.Focus();
                return;
            }
     

            //SelectedPath = SelectedPath.TrimEnd('\\');

            DialogResult = DialogResult.OK;
        }

        private void CheckFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception(Resources.TextSelectCorrectFolder);

            if (_forFolderMove)
            {
                if (!FolderRename)
                {
                    if (!Directory.Exists(path))
                        throw new Exception(string.Format(Resources.TextFolderNotExists, path));

                }
                else
                {
                    var d = Directory.GetParent(path);
                    if (d is not { Exists: true })
                        throw new Exception(string.Format(Resources.TextFolderNotExists, d));
                }
            }
            else
            {
                if(!CreateFolder && !Directory.Exists(path))
                    throw new Exception(string.Format(Resources.TextFolderNotExists, path));

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }


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
