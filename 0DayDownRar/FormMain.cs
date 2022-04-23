using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Exception = System.Exception;
using _0DayDownRar.Properties;

namespace _0DayDownRar
{
    public partial class FormMain : Form
    {
        private const string WinRarFilter = "WinRar command|winrar.exe";
        //private long _singleFileLimit;
        readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
        readonly OpenFileDialog _openFileDialog = new OpenFileDialog();
        private WinRar _winRar;

        private bool _isRunning;
        private string _baseSourcePath;
        private string _rarFilePath;
        private IList<string> _files;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            btClear.Click += (o, args) =>
            {
                listView1.Items.Clear();
            };

            listView1.ShowItemToolTips = true;
        }


        private void BtnSelectSourceFolderClick(object sender, EventArgs e)
        {
#if DEBUG
            _folderBrowser.SelectedPath = @"X:\temp\";
#else
            _folderBrowser.SelectedPath = @"F:\Downloads\";

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
                foreach (var file in DirHasZipRarFile(subDir))
                {
                    var item = listView1.Items.Add(file, file, 0);
                    item.SubItems.Add("-");
                    item.SubItems.Add("");

                }

            }
            
        }

        private List<string> DirHasZipRarFile(DirectoryInfo directoryInfo)
        {
            var list = new List<string>();
            list.AddRange(directoryInfo.GetFiles("*.zip").Select(x=>x.FullName));

            var rarFiles = directoryInfo.GetFiles("*.rar");
            foreach (var fileInfo in rarFiles)
            {
                if (fileInfo.Name.IndexOf("part", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    list.Add(fileInfo.FullName);
                    continue;
                }
                
                if (!Regex.Match(fileInfo.Name, @"\.part[0]*1.rar", RegexOptions.IgnoreCase).Success) continue;
                list.Add(fileInfo.FullName);
            }

            return list;
        }



  

        private void BtSelectWinrarPathClick(object sender, EventArgs e)
        {
            _openFileDialog.Filter = WinRarFilter;
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
                tbWinrarPath.Text = _openFileDialog.FileName;
        }

        
        private void EnableControles(bool enable)
        {
            btSelectSourceFolder.Enabled  = btSelectWinrarPath.Enabled = enable;
            btClear.Enabled = enable;
            tbWinrarPath.ReadOnly = !enable;
        }

        private void ResetItemProcessInfo()
        {
            for (var i = 0; i < listView1.Items.Count; i++)
            {
                var item = listView1.Items[i];
                item.SubItems[1].Text = "-";
                item.SubItems[2].Text = "";
            }
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


            _rarFilePath = tbWinrarPath.Text;
            if (string.IsNullOrEmpty(_rarFilePath)||!Directory.Exists(_rarFilePath))
            {
                MessageBox.Show(Resources.TxtWinRarNotFound, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //_singleFileLimit = (long)nuSingleFileSize.Value * 1024 * 1024;

            _files = (from ListViewItem item in listView1.Items select item.Text).ToList();
            EnableControles(false);
            ResetItemProcessInfo();

            btStart.Text = Resources.TextStop;
            progressBar1.Maximum = _files.Count;
            progressBar1.Value = 0;
            _isRunning = true;
            Cursor = Cursors.WaitCursor;
            backgroundWorker.RunWorkerAsync();
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
             _winRar = new WinRar(_rarFilePath, _baseSourcePath);

            foreach (var path in _files)
            {
                if(backgroundWorker.CancellationPending) break;

                var ok = true;
                var msg = "成功完成。";
                try
                {
                    ProcessFile(path);
                }
                catch (Exception exception)
                {
                    msg = exception.Message;
                    ok = false;
                }

                i++;
                backgroundWorker.ReportProgress(i, new PathItem {Path = path,Result = ok?"o":"x",Message = msg});
            }

        }

        //解压主函数
        private void ProcessFile(string fileName)
        {
            backgroundWorker.ReportProgress(0, new PathItem { Path = fileName, Result = "*", Message = "测试文件..." });

            var (exitCode, message) = _winRar.TestFile(fileName);
            if (exitCode == 0)
            {
                CheckCreateSfv(fileName);
                return;
            }

            if (exitCode != 11)//密码错误
                throw new Exception(message);

            var outputs = _winRar.ListContent(fileName);
            //查找最短行作为顶级路径
            var n = outputs.Min(x => x.Length);
            var topFolder = outputs.First(x => x.Length == n);

            var currentDir = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(currentDir))
                throw new Exception(Resources.TextIlegalPath);

            var extractDir = currentDir;
            if (!outputs.All(x => x.StartsWith(topFolder)))
            {
                topFolder = WinRar.RarBaseName(fileName);
                extractDir = Path.Combine(currentDir, topFolder);
            }

            //解压文件
            backgroundWorker.ReportProgress(0, new PathItem { Path = fileName, Result = "*", Message = "解压文件..." });
            (exitCode, message) = _winRar.ExtractFile(fileName, extractDir);
            if (exitCode != 0) throw new Exception(message);

            //删除源文件
            _winRar.DeleteRarFiles(fileName);

            //替换文件夹中的空格为下划线
            var topFolderNew = topFolder.Replace(" ", "_");
            if(!topFolderNew.Equals(topFolder,StringComparison.InvariantCultureIgnoreCase))
                Directory.Move(
                    Path.Combine(currentDir,topFolder),
                    Path.Combine(currentDir,topFolderNew));

            //压缩文件夹
            backgroundWorker.ReportProgress(0, new PathItem { Path = fileName, Result = "*", Message = "压缩文件..." });
            (exitCode, message) = _winRar.CompressFolder(Path.Combine(currentDir,$"{topFolderNew}.rar"),
                Path.Combine(currentDir, topFolderNew));
            if (exitCode != 0) throw new Exception(message);

            //检查创建sfv
            CheckCreateSfv(fileName,topFolderNew);
        }

        private void CheckCreateSfv(string fileName,string rarBaseName=null)
        {
            if (string.IsNullOrEmpty(rarBaseName))
                rarBaseName = WinRar.RarBaseName(fileName);
            
            var dir = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(dir))
                throw new Exception(Resources.TextIlegalPath);
            var sfvFile = Path.Combine(dir, $"{rarBaseName}.sfv");

            var volumeFiles = WinRar.ListRarVolumeFiles(dir, rarBaseName);

            if (File.Exists(sfvFile))
            {
                var lines = File.ReadAllLines(sfvFile);
                var checkedFiles = 0;
                foreach (var line in lines)
                {
                    if(string.IsNullOrEmpty(line)||line.StartsWith(";")) continue;
                    var splits = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    if(splits.Length<2) continue;

                    var volumeFile = volumeFiles.FirstOrDefault(x => Path.GetFileName(x).Equals(splits[0],StringComparison.InvariantCultureIgnoreCase));
                    if(string.IsNullOrEmpty(volumeFile)) continue;

                    var crc = WinRar.FileCrc32(volumeFile);
                    if (!crc.Equals(splits[1], StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception($"{splits[0]} CRC 错误");
                    }
                    checkedFiles++;
                }

                if(checkedFiles!=volumeFiles.Count)
                    throw new Exception($"SFV中文件缺失");

            }
            else
            {
                backgroundWorker.ReportProgress(0, new PathItem { Path = fileName, Result = "*", Message = "创建CRC..." });

                var sb = new StringBuilder();
                sb.AppendLine("; Generated by my 0DayDownRar");
                foreach (var volumeFile in volumeFiles)
                {
                    sb.AppendLine($"{Path.GetFileName(volumeFile)}\t{WinRar.FileCrc32(volumeFile)}");
                }

                File.WriteAllText(sfvFile,sb.ToString());
            }


        }



        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage !=0) progressBar1.Value = e.ProgressPercentage;
            var state = (PathItem)e.UserState;
            listView1.Items[state.Path].SubItems[1].Text = state.Result;
            listView1.Items[state.Path].SubItems[2].Text = state.Message;

        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btStart.Text = Resources.TextStart;
            _isRunning = false;
            EnableControles(true);
            btStart.Enabled = true;
            Cursor = Cursors.Default;
        }

        private void FormMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isRunning) return;
            MessageBox.Show(Resources.TextWaitFinish, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }

       
    }
}
