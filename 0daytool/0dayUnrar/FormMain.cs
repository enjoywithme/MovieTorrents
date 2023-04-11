using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using _0dayUnrar.Properties;
using Exception = System.Exception;

namespace _0dayUnrar
{
    public partial class FormMain : Form
    {
        private const string WinRarFilter = "WinRar command|winrar.exe";
        //private long _singleFileLimit;
        readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
        readonly OpenFileDialog _openFileDialog = new OpenFileDialog();
       
        private bool _isRunning;
        private string _baseSourcePath;
        private string _destPath;
        private string _rarFilePath;
        private bool _deleteFileAfterExtracting;
        private IList<string> _paths;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
#if DEBUG
            tbDestFolder.Text = "X:\\temp\\x\\";
#endif
            btClear.Click += (o, args) =>
            {
                listView1.Items.Clear();
            };
        }


        private void BtnSelectSourceFolderClick(object sender, EventArgs e)
        {
#if DEBUG
            _folderBrowser.SelectedPath = "X:\\temp\\EBOOKS.PACK2.OCTOBER.2021-TL\\";
#endif
            if (_folderBrowser.ShowDialog() != DialogResult.OK) return;
            _baseSourcePath = _folderBrowser.SelectedPath;
            listView1.Items.Clear();
            SearchAddSourceFolder(_baseSourcePath);
        }

        //搜索包含zip/rar的目录并添加-先简单搜索一层
        private void SearchAddSourceFolder(string folderPath)
        {
            var directory = new DirectoryInfo(folderPath);
            var subDirs = directory.GetDirectories();
            foreach (var subDir in subDirs)
            {
                if(!DirHasZipRarFile(subDir)) continue;
                
                var item = listView1.Items.Add(subDir.Name,subDir.Name,0);
                item.SubItems.Add("-");
                item.SubItems.Add("");

            }

        }

        private bool DirHasZipRarFile(DirectoryInfo directoryInfo)
        {
            return directoryInfo.GetFiles("*.zip").Any() || directoryInfo.GetFiles("*.rar").Any();
        }

        public bool IsSubfolder(string parentPath, string childPath)
        {
            var parentUri = new Uri(parentPath);
            var childUri = new DirectoryInfo(childPath).Parent;
            while (childUri != null)
            {
                if (new Uri(childUri.FullName) == parentUri)
                {
                    return true;
                }
                childUri = childUri.Parent;
            }
            return false;
        }

        private void BtSelectDestFolderClick(object sender, EventArgs e)
        {
            if (_folderBrowser.ShowDialog() == DialogResult.OK)
                _destPath = tbDestFolder.Text = _folderBrowser.SelectedPath;
        }

        private void BtSelectWinrarPathClick(object sender, EventArgs e)
        {
            _openFileDialog.Filter = WinRarFilter;
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
                tbWinrarPath.Text = _openFileDialog.FileName;
        }

  


        private void EnableControles(bool enable)
        {
            btSelectSourceFolder.Enabled = btSelectDestFolder.Enabled = btSelectWinrarPath.Enabled = enable;
            cbDeleteFileAfterExtracting.Enabled = enable;
            btClear.Enabled = enable;
            tbDestFolder.ReadOnly = !enable;
            tbWinrarPath.ReadOnly = !enable;
        }

        private void TbnStartClick(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                if (MessageBox.Show(Resources.TextConfirmStop, Resources.Warning,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                    return;
                backgroundWorker.CancelAsync();
                btStart.Enabled = false;
                return;
            }

            if (listView1.Items.Count==0)
            {
                MessageBox.Show(Resources.TextSelectSourceFolder, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (string.IsNullOrEmpty(tbDestFolder.Text))
            {
                MessageBox.Show(Resources.TextSelectDestFolder, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _destPath = tbDestFolder.Text;
            if (!Directory.Exists(tbDestFolder.Text))
            {
                Directory.CreateDirectory(tbDestFolder.Text);
                //MessageBox.Show($"Destination folder '{tbDestFolder.Text}' does not exists!",
                //    Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //return;
            }

            if (IsSubfolder(_baseSourcePath,_destPath))
            {
                MessageBox.Show(Resources.TextDestSourceSame, Resources.Warning,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            _rarFilePath = tbWinrarPath.Text;
            if (string.IsNullOrEmpty(_rarFilePath)||!File.Exists(_rarFilePath))
            {
                MessageBox.Show("Rar.exe 路径不存在。", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //_singleFileLimit = (long)nuSingleFileSize.Value * 1024 * 1024;

            _paths = (from ListViewItem item in listView1.Items select item.Text).ToList();
            EnableControles(false);
            _deleteFileAfterExtracting = cbDeleteFileAfterExtracting.Checked;
            btStart.Text = Resources.TextStop;
            progressBar1.Maximum = _paths.Count;
            progressBar1.Value = 0;

            ResetItemStatus();
            _isRunning = true;

            backgroundWorker.RunWorkerAsync();
        }

        private void ResetItemStatus()
        {
            for (var i = 0; i < listView1.Items.Count; i++)
            {
                var item = listView1.Items[i];
                item.SubItems[1].Text = "-";
                item.SubItems[2].Text = "";
            }
        }

        struct PathItem
        {
            public string Path;
            public string Result;
            public string Message;
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var i = 0;
            foreach (var path in _paths)
            {
                if(backgroundWorker.CancellationPending) break;

                var ok = true;
                var message = "成功完成。";
                backgroundWorker.ReportProgress(0,new PathItem { Path = path, Result = "*" ,Message = "解压..."});
                try
                {
                    ExtractFolder(path);
                }
                catch (Exception exception)
                {
                    ok = false;
                    message = exception.Message;
                }

                i++;
                backgroundWorker.ReportProgress(i, new PathItem(){Path = path,Result = ok?"o":"x",Message =message});
            }

        }

        //解压主函数
        private void ExtractFolder(string folder)
        {
            var curPath = Path.Combine(_baseSourcePath, folder);
            var curDest = Path.Combine(_destPath, folder);
            if (!Directory.Exists(curDest))
                Directory.CreateDirectory(curDest);

            //解压zip
            var zipFiles = Directory.GetFiles(curPath,"*.zip");
            foreach (var zipFile in zipFiles)
            {
                var cmd = $"x -IBCK -o+ -y \"{zipFile} \"  \"{curDest} \"";
                RunWinRarCmd(cmd);
            }


            //解压rar
            var rarFiles = Directory.GetFiles(curDest,"*.rar");
            if (rarFiles.Length == 0)
                throw new Exception("没有找到.rar文件。");

            foreach (var rarFile in rarFiles)
            {
                var cmd = $"x -IBCK -o+ -y  \"{rarFile} \"  \"{curDest} \"";//背景解压，覆盖文件不确认
                RunWinRarCmd(cmd);
            }

            //if(!_deleteFileAfterExtracting) return;
            //foreach (var zipFile in zipFiles)
            //{
            //    File.Delete(zipFile);
            //}

            rarFiles = Directory.GetFiles(curDest, "*.r??");
            foreach (var rarFile in rarFiles)
            {
                File.Delete(rarFile);
            }

        }

        private readonly Dictionary<int, string> _winRarError = new Dictionary<int, string>()
        {
            { 1, " 警告。发生非致命错误" },
            { 2, " 发生致命错误。" },
            { 3, "无效校验和。数据损坏。 " },
            { 4, " 尝试修改一个 锁定的压缩文件。 " },
            { 5, " 写错误。 " },
            { 6, "文件打开错误。 " },
            { 7, "错误命令行选项。 " },
            { 8, "内存不足。 " },
            { 9, "文件创建错误。 " },
            { 10, " 没有找到与指定的掩码和选项匹配的文件。 " },
            { 11, " 密码错误。 " },
            { 255, " 用户中断。 " }
        };

        private void RunWinRarCmd(string command)
        {
            var procStartInfo = new ProcessStartInfo(_rarFilePath, command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _destPath
            };
            var proc = new Process { StartInfo = procStartInfo };
            proc.Start();
            proc.WaitForExit();
            //var result = proc.StandardError.ReadToEnd();
            var errCode = _winRarError.ContainsKey(proc.ExitCode) ? _winRarError[proc.ExitCode] : "未知错误";
            if (proc.ExitCode != 0)
            {

                throw new Exception($"{errCode}");

            }
        }


        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var state = (PathItem)e.UserState;
            listView1.Items[state.Path].SubItems[1].Text = state.Result;
            listView1.Items[state.Path].SubItems[2].Text = state.Message;
            if(e.ProgressPercentage>0)
                progressBar1.Value = e.ProgressPercentage;

        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btStart.Text = Resources.TextStart;
            _isRunning = false;
            EnableControles(true);
            btStart.Enabled = true;
        }

        private void FormMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isRunning) return;
            MessageBox.Show(Resources.TextWaitFinish, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }

       
    }
}
