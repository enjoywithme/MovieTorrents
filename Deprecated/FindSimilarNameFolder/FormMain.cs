using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using FindSimilarNameFolder.Properties;

namespace FindSimilarNameFolder
{

    public partial class FormMain : Form
    {

        private string _rootFolderPath;
        private readonly List<FolderItem> _folderItems=new List<FolderItem>();
        private Single[,] _similarityMatrix;

        private readonly List<SimilarGroup> _listSimilarGroups = new List<SimilarGroup>();
        private float _minimumSimilarity;
        private readonly string _logFilePath = AppDomain.CurrentDomain.BaseDirectory + Process.GetCurrentProcess().ProcessName + ".log";

        public FormMain()
        {
            InitializeComponent();
        }

        private void TsbFolderClick(object sender, EventArgs e)
        {
            if(backgroundWorker1.IsBusy)
            {
                if (MessageBox.Show(Resources.MsgConfirmCancel, 
                    Resources.TextConfirm, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                {
                    return;
                }
                backgroundWorker1.CancelAsync();
                return;
            }

            var formStart = new FormStart();
            if (formStart.ShowDialog() == DialogResult.Cancel) return;
            _rootFolderPath = formStart.StartPath;
            _minimumSimilarity = formStart.MinimumSimilarity;
            LogMsg(string.Format("Search \"{0}\" with minimum similairty \"{1}\" ...",_rootFolderPath,_minimumSimilarity));

            _folderItems.Clear();
            _listSimilarGroups.Clear();
           
            LoadFoldersList(_rootFolderPath);
            if(_folderItems.Count==0)
            {
                LogMsg("No 0day book folders found!");
                return;
            }

            LogMsg(string.Format("{0} 0day book folders found!",_folderItems.Count));
            toolStripProgressBar1.Maximum = _folderItems.Count;
            toolStripProgressBar1.Visible = true;
            tsbFolder.Text=Resources.TextCancel;
            backgroundWorker1.RunWorkerAsync();
        }

        private void LogMsg(string msg)
        {
            if(tbLog.InvokeRequired)
            {
                tbLog.Invoke(new Action<string>(LogMsg), msg);
                return;

            }
            tbLog.AppendText(string.Format("{0}\t{1}\r\n",DateTime.Now.ToString("hh:MM:ss"),msg));
        }

        //获取所有目录
        private void LoadFoldersList(string path)
        {
            var df = new DirectoryInfo(path);
            var subdf = df.GetDirectories();
            foreach (var directoryInfo in subdf)
            {
                switch (directoryInfo.Name.Length)
                {
                    case 4:
                        {
                            int dummyInt;
                            if(Int32.TryParse(directoryInfo.Name,out dummyInt)) //确认是年
                                LoadFoldersList(directoryInfo.FullName);
                        }
                        break;
                    case 9:
                        if(directoryInfo.Name.Substring(4,1)=="-") //确认是月
                            LoadFoldersList(directoryInfo.FullName);
                        break;
                    default:
                        _folderItems.Add(new FolderItem(directoryInfo.Name,directoryInfo.FullName));
                        break;
                }

            }
        }

        //计算相似度
        private bool CalculateSimilarity()
        {
            if(_folderItems.Count<=1) return true;

            _similarityMatrix=new float[_folderItems.Count,_folderItems.Count];

            LogMsg("Begin to calculate the similarity.");
            //计算相似度到矩阵
            for(var i=0;i<_folderItems.Count;i++)
            {
                for (var j = i+1; j < _folderItems.Count; j++)
                {
                    if (i == j) _similarityMatrix[i, j] = -1;
                    else _similarityMatrix[i, j] = FolderItem.Similarity(_folderItems[i], _folderItems[j]);
                    if (backgroundWorker1.CancellationPending)
                        return false;
                }
                //Thread.Sleep(2000);
                backgroundWorker1.ReportProgress(i);
            }
            LogMsg("Finish calculation of the similarity.");

            //对相似度超过设定值的项分组
            LogMsg("Group the items...");
            for(var i=0;i<_folderItems.Count;i++)
            {
                if (backgroundWorker1.CancellationPending) return false;
                if(_folderItems[i].Checked) continue;
                SimilarGroup currentGroup=null;
                for(var j=i+1;j<_folderItems.Count;j++)
                {
                    if (_similarityMatrix[i, j] < _minimumSimilarity) continue;
                    if(currentGroup==null) currentGroup=new SimilarGroup();
                    currentGroup.Add(_folderItems[j],_similarityMatrix[i,j]);
                    _folderItems[j].Checked = true;
                }
                if (currentGroup == null) continue;
                currentGroup.Add(_folderItems[i]);
                _folderItems[i].Checked = true;
                _listSimilarGroups.Add(currentGroup);
            }

            LogMsg("____END____");
            return !backgroundWorker1.CancellationPending;
        }

        private void FillResults()
        {
            LogMsg(string.Format("{0} group found.",_listSimilarGroups.Count));
            
            var fs = new FileStream(_logFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            var sw = new StreamWriter(fs);


            for(var i=0;i<_listSimilarGroups.Count;i++)
            {
                var currentGroup = _listSimilarGroups[i];
                for(var j=0;j<currentGroup.Items.Count;j++)
                {
                    var folderItem = currentGroup.Items[j];
                    sw.WriteLine("{0}\t{1}\t{2}",i,(int)currentGroup.Similarity,folderItem.FolderPath);
                }
                sw.WriteLine("");
            }

            sw.Close();
            fs.Close();
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            LayoutStatusBar();

        }

        private void BackgroundWorker1DoWork(object sender, DoWorkEventArgs e)
        {
            e.Cancel=!CalculateSimilarity();
        }

        private void UpdateStatusText(int i)
        {
            if(statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new Action<int>(UpdateStatusText), i);
                return;
            }

            toolStripProgressBar1.Value = i+1;
            toolStripStatusLabel1.Text = string.Format("{0}/{1}", i+1,toolStripProgressBar1.Maximum);
            statusStrip1.Refresh();
        }

        private void BackgroundWorker1ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateStatusText(e.ProgressPercentage);
            
        }

        private void BackgroundWorker1RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            tsbFolder.Text = Resources.TextStart;

            if(e.Cancelled)
            {
                LogMsg("Operation cancelled.");
            }

            FillResults();
            
        }

        private void StatusStrip1SizeChanged(object sender, EventArgs e)
        {
            LayoutStatusBar();
        }

        private void LayoutStatusBar()
        {
            if(toolStripProgressBar1.Visible)
            {
                toolStripStatusLabel1.Width = 100;
                toolStripProgressBar1.Width = statusStrip1.ClientSize.Width - toolStripStatusLabel1.Width - 18;

            }
            else
            {

                toolStripStatusLabel1.Width = statusStrip1.ClientSize.Width - 18;
            }
            
        }

        private void ToolStripProgressBar1VisibleChanged(object sender, EventArgs e)
        {
            LayoutStatusBar();
        }

        private void FormMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!backgroundWorker1.IsBusy) return;
            LogMsg("Calculating...Cannot exit program!");
            e.Cancel = true;
        }

        private void tsbOpenResult_Click(object sender, EventArgs e)
        {
            var info = new ProcessStartInfo { FileName = "notepad.exe", Arguments = _logFilePath };
            try
            {
                Process.Start(info);
            }
            catch (Win32Exception ex)
            {
                LogMsg(ex.Message);
            }
        }

       
    }
}
