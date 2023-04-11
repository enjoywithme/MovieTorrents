using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using _0dsfvcheck.Properties;

namespace _0dsfvcheck
{
    public partial class FormMain : Form
    {
        readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
        private string _processedListFilePath;
        private readonly List<string> _processedList = new List<string>();
        private bool _isRunning;
        private bool _cancelling;
        private string _currentProcessFolder;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            tbSourceFolder.Text = AppDomain.CurrentDomain.BaseDirectory;
        }

        private void BtSelectSourceFolderClick(object sender, EventArgs e)
        {
            if (_folderBrowser.ShowDialog() == DialogResult.OK)
                tbSourceFolder.Text = _folderBrowser.SelectedPath;
        }

        private void LoadProcessedList()
        {
            //_processedListFilePath = tbSourceFolder.Text + "\\" + Process.GetCurrentProcess().ProcessName + ".lst";
            _processedListFilePath = tbSourceFolder.Text + "\\0dsfvcheck.lst";
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

        private void BtStartClick(object sender, EventArgs e)
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


            //_singleFileLimit = (long)nuSingleFileSize.Value * 1024 * 1024;

            LoadProcessedList();
            EnableControles(false);
            btStart.Text = Resources.TextStop;
            _isRunning = true;
            _cancelling = false;
            tbLog.Text = "";
            backgroundWorker.RunWorkerAsync();

        }

        private void EnableControles(bool enable)
        {
            btSelectSourceFolder.Enabled = enable;
            tbSourceFolder.ReadOnly =  !enable;
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var sourceFolder = new DirectoryInfo(tbSourceFolder.Text);

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

                    //检查是否要求退出
                    if (_cancelling)
                    {
                        backgroundWorker.ReportProgress(0, "User cancelled...");
                        return;
                    }

                    backgroundWorker.ReportProgress(1, dir.Name);

                    string msg;
                    var sfvFound = 0;
                    if(!ValidateSfvInFolder(dir,ref sfvFound,out msg))
                    {
                        backgroundWorker.ReportProgress(-2);
                        continue;
                    }
                    if(sfvFound==0)
                    {
                        backgroundWorker.ReportProgress(-1);
                        continue;
                    }
                    backgroundWorker.ReportProgress(100);
                }
            }
            catch (Exception exception)
            {
                backgroundWorker.ReportProgress(0, string.Format("!!!Exception:{0}", exception.Message));
            }

            backgroundWorker.ReportProgress(0, "--------------ALL FINSIHED----------");
        }

        private Boolean ValidateSfvInFolder(DirectoryInfo dir,ref int sfvFileCount,out string msg)
        {
            msg = "";
            var sfvfiles = dir.GetFiles("*.sfv");
            
            if (sfvfiles.Length == 0)
            {
                var subdirs = dir.GetDirectories();
                foreach (var subdir in subdirs)
                {
                    if (!ValidateSfvInFolder(subdir, ref sfvFileCount, out msg)) return false;
                }
            }
            else
            {
                sfvFileCount++;
                return VerifySfvFile(sfvfiles[0], out msg);
            }

            return true;
        }

        private Boolean VerifySfvFile(FileInfo sfvFile,out string msg)
        {
            msg = "";
            var ok = true;
            try
            {
                var stream = new StreamReader(sfvFile.FullName);
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith(";")) continue;
                    var splits = line.Split(new[] { ' ' });
                    if(splits.Length!=2)
                    {
                        ok = false;
                        break;
                    }

                    var fileToVerify = sfvFile.DirectoryName +  "\\" + splits[0].Trim();
                    if(!File.Exists(fileToVerify))
                    {
                        ok = false;
                        break;
                    }

                    var crc = GetFileCrc(fileToVerify);
                    if(string.Compare(crc,splits[1].Trim(),StringComparison.InvariantCultureIgnoreCase)!=0)
                    {
                        ok = false;
                        break;
                    }
                }
                stream.Close();
            }
            catch (Exception exception)
            {
                msg = exception.Message;
                return false;
            }
            
            return ok;
        }

        private string GetFileCrc(string filePath)
        {
            var crc32 = new DamienG.Security.Cryptography.Crc32();
            var hash = String.Empty;

            using (var fs = File.Open(filePath, FileMode.Open))
                hash = crc32.ComputeHash(fs).Aggregate(hash, (current, b) => current + b.ToString("x2").ToLower());
            return hash;
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
                    tbLog.AppendText(" -- Failed:NO SFV found!\r\n");
                    break;
                case -2:
                    tbLog.AppendText(" -- Failed:SFV check error!\r\n");
                    break;
            }
        }
    }
}
