using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using _0dayrar.Properties;
using System.Collections.Generic;

namespace _0dayrar
{
    public partial class FormMain : Form
    {
        private const string WinRarFilter = "WinRar command|rar.exe";
        private string _currentProcessFolder;
        private const long Giga = 1073741824L;
        //private long _singleFileLimit;
        readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
        readonly OpenFileDialog _openFileDialog = new OpenFileDialog();
        private string _processedListFilePath;// = AppDomain.CurrentDomain.BaseDirectory + Process.GetCurrentProcess().ProcessName + ".lst";
        private string _curDestSubPath;
        private bool _isRunning;
        private bool _cancelling;
        private readonly List<string> _processedList = new List<string>();
        public FormMain()
        {
            InitializeComponent();
        }

        private void BtnSelectSourceFolderClick(object sender, EventArgs e)
        {
            if (_folderBrowser.ShowDialog() == DialogResult.OK)
                tbSourceFolder.Text = _folderBrowser.SelectedPath;
        }

        private void BtSelectDestFolderClick(object sender, EventArgs e)
        {
            if (_folderBrowser.ShowDialog() == DialogResult.OK)
                tbDestFolder.Text = _folderBrowser.SelectedPath;
        }

        private void BtSelectWinrarPathClick(object sender, EventArgs e)
        {
            _openFileDialog.Filter = WinRarFilter;
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
                tbWinrarPath.Text = _openFileDialog.FileName;
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            tbSourceFolder.Text = AppDomain.CurrentDomain.BaseDirectory;
        }

        private void LoadProcessedList()
        {
            _processedListFilePath = tbSourceFolder.Text + "\\" + Process.GetCurrentProcess().ProcessName + ".lst";
            listProcessed.Items.Clear();
            _processedList.Clear();
            if (!File.Exists(_processedListFilePath)) return;
            var lines = File.ReadAllLines(_processedListFilePath);
            foreach (var s in lines.Select(line => line.Trim()).Where(s => !string.IsNullOrEmpty(s)))
            {
                listProcessed.Items.Add(s);
                _processedList.Add(s);
            }
        }

        private void EnableControles(bool enable)
        {
            btSelectSourceFolder.Enabled = btSelectDestFolder.Enabled = btSelectWinrarPath.Enabled = enable;
            tbSourceFolder.ReadOnly = tbDestFolder.ReadOnly = tbWinrarPath.ReadOnly = !enable;
            nuSingleFileSize.Enabled = nuFreespace.Enabled = enable;
        }

        private void TbnStartClick(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                if (MessageBox.Show(Resources.TextConfirmStop, Resources.Warning,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                    return;
                _cancelling = true;
                btStart.Enabled = false;
                return;
            }

            if (string.IsNullOrEmpty(tbSourceFolder.Text))
            {
                MessageBox.Show(Resources.TextSelectSourceFolder, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(tbSourceFolder.Text))
            {
                MessageBox.Show(string.Format("Source folder '{0}' does not exists!", tbSourceFolder.Text),
                    Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tbDestFolder.Text))
            {
                MessageBox.Show(Resources.TextSelectDestFolder, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(tbDestFolder.Text))
            {
                MessageBox.Show(string.Format("Destination folder '{0}' does not exists!", tbDestFolder.Text),
                    Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.Compare(tbDestFolder.Text, tbSourceFolder.Text, true, CultureInfo.InvariantCulture) == 0)
            {
                MessageBox.Show(Resources.TextDestSourceSame, Resources.Warning,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //_singleFileLimit = (long)nuSingleFileSize.Value * 1024 * 1024;

            LoadProcessedList();
            GetCurDestSubFolder();
            EnableControles(false);
            btStart.Text = Resources.TextStop;
            _isRunning = true;
            _cancelling = false;
            tbLog.Text = "";
            backgroundWorker.RunWorkerAsync();
        }

        private void GetCurDestSubFolder()
        {
            var max = 0;
            var dest = new DirectoryInfo(tbDestFolder.Text);
            foreach (var directory in dest.GetDirectories())
            {
                int cur;
                if (!int.TryParse(directory.Name, out cur)) continue;
                if (cur > max) max = cur;
            }

            max++;
            tbCurDestSubPath.Text = _curDestSubPath = tbDestFolder.Text + "\\" + max.ToString(CultureInfo.InvariantCulture);
        }//获取压缩目的子路径


        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var sourceFolder = new DirectoryInfo(tbSourceFolder.Text);

            var driverletter = tbDestFolder.Text.Substring(0, 1);
            var driverInfo = new DriveInfo(driverletter);
            var dirs = sourceFolder.GetDirectories();
            try
            {
                foreach (var dir in dirs)
                {
                    //检查是否已经处理

                    var processed = _processedList.Any(s => s.ToLower() == dir.Name.ToLower());
                    if (processed)
                    {
                        //backgroundWorker.ReportProgress(0, string.Format("Skiped -- {0}", dir.Name));
                        continue;
                    }

                    var nodiskSpaceMessaged = false;
                    //检查磁盘剩余空间
                    while (!_cancelling)
                    {
                        if (driverInfo.AvailableFreeSpace >= nuFreespace.Value * Giga) break;
                        if (!nodiskSpaceMessaged)
                        {
                            backgroundWorker.ReportProgress(0, "!!!Disk no enough free space.Wait free space...!!!");
                            nodiskSpaceMessaged = true;
                        }

                        Thread.Sleep(6000);

                    }

                    //检查是否要求退出
                    if (_cancelling)
                    {
                        backgroundWorker.ReportProgress(0, "User cancelled...");
                        return;
                    }


                    //检查文件夹首是否日期
                    var rarFileName = dir.Name;
                    if (dir.Name.Length > 8)
                    {
                        var s = rarFileName.Substring(0, 8).Split(new[] { '.' });
                        if (s.Length == 3)
                        {
                            var stripe = true;
                            foreach (var s1 in s)
                            {
                                int i;
                                if (int.TryParse(s1, out i)) continue;
                                stripe = false;
                                break;
                            }
                            if (stripe) rarFileName = rarFileName.Substring(9);
                        }

                    }

                    backgroundWorker.ReportProgress(1, dir.Name);
                    if (!Directory.Exists(_curDestSubPath)) Directory.CreateDirectory(_curDestSubPath);
                    var finalRarPath = _curDestSubPath+ "\\" + rarFileName;
                    if (!Directory.Exists(finalRarPath)) Directory.CreateDirectory(finalRarPath);

                    //var folderSize = dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);

                    var command = string.Format("a -r -v{3}m -ierr -inul -ep1 {0}\\{1}.rar {2}", finalRarPath, rarFileName, dir.FullName, (int)nuSingleFileSize.Value);



                    var ok = true;
                    try
                    {
                        var procStartInfo = new ProcessStartInfo(tbWinrarPath.Text, command)
                        {
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WorkingDirectory = tbSourceFolder.Text
                        };
                        var proc = new Process { StartInfo = procStartInfo };
                        proc.Start();
                        var result = proc.StandardError.ReadToEnd();
                        if (proc.ExitCode != 0)
                        {
                            ok = false;
                            backgroundWorker.ReportProgress(0, result);

                        }
                        else _processedList.Add(dir.Name);
                    }
                    catch (Exception exception)
                    {
                        backgroundWorker.ReportProgress(0, exception.Message);
                        ok = false;
                    }

                    if (ok) backgroundWorker.ReportProgress(100);
                    else backgroundWorker.ReportProgress(-1);
                }
            }
            catch (Exception exception)
            {
                backgroundWorker.ReportProgress(0, string.Format("!!!Exception:{0}", exception.Message));
            }

            backgroundWorker.ReportProgress(0,"--------------ALL FINSIHED----------");

        }



        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    tbLog.AppendText((string)e.UserState + "\r\n");
                    break;
                case 1:
                    _currentProcessFolder = (string)e.UserState;
                    tbLog.AppendText(string.Format("Started -- {0}", _currentProcessFolder));
                    break;
                case 100:
                    listProcessed.Items.Add(_currentProcessFolder);
                    if (!File.Exists(_processedListFilePath))
                    {
                        using (var sw = File.CreateText(_processedListFilePath))
                        {
                            sw.WriteLine(_currentProcessFolder);
                        }
                    }
                    else
                    {
                        using (var sw = File.AppendText(_processedListFilePath))
                        {
                            sw.WriteLine(_currentProcessFolder);
                        }
                    }
                    tbLog.AppendText(" -- Finished\r\n");
                    break;
                case -1:
                    tbLog.AppendText(" -- Failed\r\n");
                    break;
            }
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
