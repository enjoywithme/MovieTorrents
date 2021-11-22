using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mySharedLib;
using Exception = System.Exception;
using Timer = System.Threading.Timer;

namespace MovieTorrents
{
    public partial class FormMain : Form
    {
        public static FormMain DefaultInstance { get; set; }
        private FormBtBtt _formBtBtt;

        
        private CancellationTokenSource _queryTokenSource;
        private CancellationTokenSource _operTokenSource;

        private ToolStripMenuItem[] _limitMenuItems;

        private const int OperationNone = 0;
        private const int OperationQueryFile = 1;
        private const int OperationProcessingAddedFile = 2;
        private const int OperationClearFile = 3;

        private enum OperationType
        {
            None,
            QueryFile,
            ProcessingAddedFile,
            ClearFile
        };

        private int _currentOperation = 0;
        private string _lastSearchText = string.Empty;



        //private System.Timers.Timer _tt;


        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //_tt = new System.Timers.Timer(5000);
            //_tt.Elapsed += (o, j) => DisplayInfo("test");
            //_tt.Enabled = true;

            DefaultInstance = this;

            if (!TorrentFile.CheckTorrentPath(out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            if (!string.IsNullOrEmpty(TorrentFile.TorrentFilePath) && !Directory.Exists(TorrentFile.TorrentFilePath))
            {
                MessageBox.Show($"种子目录“{TorrentFile.TorrentFilePath}”不存在!", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tsCurrentDir.Text = $"种子目录[{TorrentFile.TorrentFilePath}]";

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = Icon;


            lbGenres.Text = lbKeyName.Text = lbOtherName.Text = lbZone.Text = lbRating.Text = string.Empty;
            tsSummary.Text = $"加载了0 个文件，0 已知项目。";

            StartFileWatch();

            //查询limit菜单项
            _limitMenuItems = new[] { tsmiLimit100, tsmiLimit200, tsmiLimit500, tsmiLimit1000, tsmiLimit2000 };
            foreach (var menuItem in _limitMenuItems)
            {
                menuItem.Click += tsmiLimit_Click;
            }



            tsbDelete.Click += tsmiDelete_Click;//删除
            tsbCopyDouban.Click += tsmiCopyDouban_Click;//拷贝豆瓣信息
            tsbMove.Click += tsmiMove_Click;//移动到文件夹

            tsbRating0.Click += (o, a) => { tbSearchText.Text = "Rating:0";};
            tsbRating8.Click += (o, a) => { tbSearchText.Text = "Rating:>8"; };
            tsbRating9.Click += (o, a) => { tbSearchText.Text = "Rating:>9"; };

            //var d1 = DateTime.MinValue;
            //var d2 = DateTime.Parse("2018-09-10 14:05:04");
            //Debug.WriteLine((d2 - d1).TotalMilliseconds / 1000);
#if DEBUG
            tbSearchText.Text = "雷神";
#endif
        }



        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentOperation == OperationNone || 0 != Interlocked.Exchange(ref BtBtItem.AutoDownloadRunning, 1))
            {
                _formBtBtt?.Close();
                return;
            }
            MessageBox.Show("尚有操作在进行中，不能退出！", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }

        //目录监视
        #region 目录监视

        private bool _isWatching;
        private bool _ignoreFileWatch;

        private BlockingCollection<TorrentFile> _monitoredFilesToProcess = new BlockingCollection<TorrentFile>();

        public void IgnoreFileWatch(bool ignore = true)
        {
            _ignoreFileWatch = ignore;
        }

        private bool IsWatching
        {
            set
            {
                _isWatching = value;
                if (_isWatching)
                {
                    tsButtonWatch.Image = Properties.Resources.Eye32;
                    tsButtonWatch.ToolTipText = "目录监视中";
                    tsmiToggleWatch.Text = "停止监视";
                    DisplayInfo("目录监视已启动", false, false);
                    return;
                }

                tsButtonWatch.Image = Properties.Resources.EyeStop32;
                tsButtonWatch.ToolTipText = "目录监视停止";
                tsmiToggleWatch.Text = "启动监视";
                DisplayInfo("目录监视已停止", true);
            }
        }
        private FileSystemWatcher _watcher;
        private Timer _watchTimer;

        private void StartFileWatch()
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentFilePath)) return;

            try
            {
                _watcher = new FileSystemWatcher(TorrentFile.TorrentFilePath)
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.FileName,
                    Filter = "*.torrent"
                };
                _watcher.Created += Watcher_File_Created;
                _watcher.Error += Watcher_File__Error;
                _watcher.EnableRaisingEvents = true;
                IsWatching = true;
            }
            catch (Exception e)
            {
                IsWatching = false;
                DisplayInfo($"启动目录监视失败\r\n{e.Message}!", true);
            }

            if (_watchTimer == null)
            {
                _watchTimer = new Timer(CheckWatchStatus, null, 2000, 10000);
            }
            else
                _watchTimer.Change(2000, 10000);

        }
        private void StopFileWatch()
        {
            if (_watchTimer != null && !_watchTimer.Change(Timeout.Infinite, Timeout.Infinite))
            {
                _watchTimer.Dispose();
                _watchTimer = null;
            }

            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }


            IsWatching = false;
        }

        private void Watcher_File__Error(object sender, ErrorEventArgs e)
        {
            IsWatching = false;
            DisplayInfo($"目录监视失败:{e.GetException().Message}", true);
        }

        public void CheckWatchStatus(object state)
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentFilePath)) return;

            if (_monitoredFilesToProcess.Count > 0)
            {
                var filesToProcess = new BlockingCollection<TorrentFile>();

                while (_monitoredFilesToProcess.TryTake(out var torrentFile))
                {
                    filesToProcess.Add(torrentFile);
                }
                filesToProcess.CompleteAdding();

                Task.Run(() => ProcessFileToBeAdded(new CancellationTokenSource().Token, filesToProcess)).ContinueWith(task =>
                {
                    if (!task.IsFaulted)
                    {
                        DoSearch();
                        return;
                    }
                    var ex = task.Exception?.InnerException;
                    if (ex != null)
                        DisplayInfo(ex.Message, true);
                });
            }


            if (_isWatching) return;

            if (!Directory.Exists(TorrentFile.TorrentFilePath)) return;

            StartFileWatch();
        }


        private void tsmiToggleWatch_Click(object sender, EventArgs e)
        {
            if (_isWatching)
                StopFileWatch();
            else
                StartFileWatch();

        }
        private void Watcher_File_Created(object sender, FileSystemEventArgs e)
        {
            if(_ignoreFileWatch) return;

            //添加一个文件会发现生成2个事件，https://blogs.msdn.microsoft.com/ahamza/2006/02/04/filesystemwatcher-generates-duplicate-events-how-to-workaround/ 
            Debug.WriteLine($"File monitored added:{e.FullPath}");

            _monitoredFilesToProcess.Add(TorrentFile.FromFullPath(e.FullPath));

        }
        #endregion

        //辅助函数
        #region 辅助函数



        private void DisplayInfo(string infoMsg = "", bool error = false, bool alert = true)
        {
            Invoke(new Action(() =>
            {
                tssInfo.Text = string.IsNullOrEmpty(infoMsg) ? "空闲" : infoMsg;
                if (error)
                    tssState.Image = Properties.Resources.InfoRed32;
                else
                    tssState.Image = _currentOperation == OperationNone ? Properties.Resources.InfoGree32 : Properties.Resources.InfoYellow32;

                if (alert && !string.IsNullOrEmpty(infoMsg))
                {
                    notifyIcon1.BalloonTipText = infoMsg;
                    notifyIcon1.ShowBalloonTip(2000);
                }



            }));


        }

        private void tssState_Click(object sender, EventArgs e)
        {

            if (_currentOperation == OperationNone)
                return;

            if (MessageBox.Show("确定取消当前操作?", Properties.Resources.TextHint, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                return;

            _operTokenSource?.Cancel();
        }
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedItems[0].Tag;
            torrentFile.OpenDoubanLink();
        }
        #endregion


        //搜索记录
        #region 搜索记录
        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            DoSearch(true);
        }

        private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) DoSearch();
        }


        private void DoSearch(bool checkLastSearch = false)
        {
            var text = tbSearchText.Text.Trim();
            if (checkLastSearch && string.Compare(text, _lastSearchText, StringComparison.InvariantCultureIgnoreCase) == 0) return;
            _lastSearchText = text;

            if (!tsmiFilterRecent.Checked && !tsmiFilterSeelater.Checked)
            {
                if (string.IsNullOrEmpty(text) || (text.Length <= 2 && text.All(x => (int)x <= 127)))
                {
                    DisplayInfo("输入的搜素文字过少", false, false);
                    return;
                }
            }


            if (Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationNone) != OperationNone
                && Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationQueryFile) != OperationQueryFile)

            {
                MessageBox.Show("正在执行其他操作，等待完成后操作", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            if (_queryTokenSource != null)
            {
                _queryTokenSource.Cancel();
                _queryTokenSource.Dispose();
                _queryTokenSource = null;
            }

            _queryTokenSource = new CancellationTokenSource();

            DisplayInfo("正在查询文件", false, false);

            Task.Run( async () =>
            {
                var (torrents,msg) = await TorrentFile.ExecuteSearch(text, _queryTokenSource.Token);
                Interlocked.Exchange(ref _currentOperation, OperationNone);

                if (!string.IsNullOrEmpty(msg))
                {
                    DisplayInfo(msg,true);
                    return;
                }

                BeginInvoke(new Action(() =>
                {
                    UpdateListView(torrents);
                    DisplayInfo();
                }));

            });


        }



        //更新列表
        private void UpdateListView(IEnumerable<TorrentFile> torrentFiles)
        {
            var files = torrentFiles.ToList();
            var totalFiles = files.Count();
            var totalItems = files.Where(x=>!string.IsNullOrEmpty(x.doubanid)).GroupBy(x => x.doubanid).Count();
            lvResults.BeginUpdate();
            lvResults.Items.Clear();
            foreach (var torrentFile in files)
            {
                if (_queryTokenSource != null && _queryTokenSource.IsCancellationRequested)
                    break;

                string[] row = { torrentFile.name,
                                torrentFile.rating.ToString(CultureInfo.InvariantCulture),
                                torrentFile.year,
                                torrentFile.seelater.ToString(),
                                torrentFile.seenowant.ToString(),
                                torrentFile.seeflag.ToString(),
                                torrentFile.seedate,
                                torrentFile.seecomment
                            };
                lvResults.Items.Add(new ListViewItem(row)
                {
                    Tag = torrentFile,
                    ForeColor = torrentFile.ForeColor
                });

                //Debug.Print(torrentFile.GetPurifiedChineseName());
            }


            lvResults.EndUpdate();

            tsSummary.Text = $"加载了{totalFiles} 个文件，{totalItems} 已知项目。";
        }

        #endregion

        //备份数据库文件
        private void BackupDbFile()
        {
            var ret = TorrentFile.BackupDbFile(out var msg);
            
            MessageBox.Show(msg, ret?Properties.Resources.TextHint:"错误", MessageBoxButtons.OK, ret?MessageBoxIcon.Information:MessageBoxIcon.Error);


        }

        //编辑记录
        private void EditRecord()
        {
            if (lvResults.SelectedItems.Count == 0) return;

            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;
            var formRenameTorrent = new FormEdit(torrentFile);
            if (formRenameTorrent.ShowDialog(this) == DialogResult.Cancel) return;

            RefreshSelected(torrentFile);

        }

        //刷新当前选择
        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelected();
        }
        private void RefreshSelected(TorrentFile torrentFile = null)
        {
            lbGenres.Text = lbRating.Text = lbOtherName.Text = lbKeyName.Text = lbZone.Text = null;
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];

            if (torrentFile == null) torrentFile = (TorrentFile)lvItem.Tag;

            lvItem.Text = torrentFile.name;
            lvItem.ForeColor = torrentFile.ForeColor;
            lvItem.SubItems[1].Text = torrentFile.rating.ToString(CultureInfo.InvariantCulture);
            lvItem.SubItems[2].Text = torrentFile.year;
            lvItem.SubItems[3].Text = torrentFile.seelater.ToString();
            lvItem.SubItems[4].Text = torrentFile.seenowant.ToString();
            lvItem.SubItems[5].Text = torrentFile.seeflag.ToString();
            lvItem.SubItems[6].Text = torrentFile.seedate;
            lvItem.SubItems[7].Text = torrentFile.seecomment;

            lbGenres.Text = torrentFile.genres;
            lbKeyName.Text = torrentFile.keyname;
            lbOtherName.Text = torrentFile.otherName;
            lbZone.Text = torrentFile.zone;
            lbRating.Text = Math.Abs(torrentFile.rating) < 0.0001 ? null : torrentFile.rating.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(torrentFile.RealPosterPath) || !File.Exists(torrentFile.RealPosterPath)) return;
            try
            {
                using var stream = new FileStream(torrentFile.RealPosterPath, FileMode.Open, FileAccess.Read);
                pictureBox1.Image = Image.FromStream(stream);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //扫描种子文件
        #region 扫描种子文件
        private void ScanTorrentFile()
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentFilePath))
            {
                MessageBox.Show("种子文件根目录没有配置", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(TorrentFile.TorrentFilePath))
            {
                MessageBox.Show($"种子文件根目录\"{TorrentFile.TorrentFilePath}\"不存在！", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_currentOperation != 0)
            {
                MessageBox.Show("正在执行其他操作，等待完成后操作", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_operTokenSource != null)
            {
                _operTokenSource.Dispose();
                _operTokenSource = null;
            }

            _operTokenSource = new CancellationTokenSource();
            var filesToProcess = new BlockingCollection<TorrentFile>();




            Task.Run(() => DoScanFile(TorrentFile.TorrentFilePath, _operTokenSource.Token, filesToProcess)).ContinueWith(task =>
            {
                if (!task.IsFaulted) return;
                if (task.Exception?.InnerException != null) DisplayInfo($"扫描文件出错：{task.Exception.InnerException.Message}", true);
                //Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
            });

            Task.Run(() => ProcessFileToBeAdded(_operTokenSource.Token, filesToProcess)).ContinueWith(task =>
            {
                if (!task.IsFaulted) return;
                if (task.Exception?.InnerException != null) DisplayInfo($"添加文件记录出错：{task.Exception.InnerException.Message}", true);
            });

        }

        //添加扫描到的种子到队列等待处理
        private void DoScanFile(string dirName, CancellationToken token, BlockingCollection<TorrentFile> filesToProcess)
        {
            var fileScanned = 0;

            var di = new DirectoryInfo(dirName);
            if (!di.Exists) return;

            DisplayInfo("扫描文件中...", false, false);

            var stopwatch = Stopwatch.StartNew();


            foreach (var fi in di.EnumerateFiles("*.torrent", SearchOption.AllDirectories))
            {

                //Debug.WriteLine(fi.Extension);
                try
                {
                    filesToProcess.TryAdd(TorrentFile.FromFullPath(fi.FullName), 2, token);
                }
                catch (OperationCanceledException e)
                {
                    Debug.WriteLine("扫描文件操作取消");
                    break;
                }

                //filesToProcess.Add(new TorrentFile(fi.FullName), token);
                fileScanned++;

            }


            filesToProcess.CompleteAdding();
            stopwatch.Stop();

            DisplayInfo($"完成文件磁盘扫描，共{fileScanned}个文件，耗时{stopwatch.Elapsed.Minutes}分{stopwatch.Elapsed.Seconds}秒,文件处理可能还在进行中，请等待。");

        }

        //处理文件队列
        private void ProcessFileToBeAdded(CancellationToken token, BlockingCollection<TorrentFile> filesToProcess)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _currentOperation, OperationProcessingAddedFile, OperationNone) != OperationNone)
                    Thread.Sleep(1000);
                else
                    break;
            }


            int fileAdded;
            int fileProcessed;

            var stopWatch = Stopwatch.StartNew();
            (fileProcessed, fileAdded) = TorrentFile.InsertToDb(filesToProcess);

            stopWatch.Stop();
            Interlocked.Exchange(ref _currentOperation, OperationNone);

            DisplayInfo($"完成文件处理，共{fileProcessed}个文件，新添加{fileAdded}个，耗时{stopWatch.Elapsed.Minutes}分{stopWatch.Elapsed.Seconds}秒。");

        }

        #endregion

        //清理无效记录

        #region 清理无效记录
        private async void tsmiClearRecords_Click(object sender, EventArgs e)
        {
            if (Interlocked.CompareExchange(ref _currentOperation, OperationClearFile, OperationNone) != OperationNone)
            {
                MessageBox.Show("正在执行其他操作，等待完成后操作", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_operTokenSource != null)
            {
                _operTokenSource.Dispose();
                _operTokenSource = null;
            }

            _operTokenSource = new CancellationTokenSource();

            DisplayInfo("清理无效文件中...", false, false);

            
            await Task.Run(async () =>
            {
                int clearFileCounted;
                int clearFileRead;
                string msg;
                bool error;
                (clearFileRead, clearFileCounted,msg,error) = await TorrentFile.DoClearFile(_operTokenSource.Token);
                DisplayInfo($"{msg}。总共读取{clearFileRead}记录，清理了{clearFileCounted}条无效记录!", error);

            });

            Interlocked.Exchange(ref _currentOperation, OperationNone);

        }



        #endregion


        //通知栏图标
        #region 通知栏图标 
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                ShowWindow();
            }
            base.WndProc(ref message);


        }
        public void ShowWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;

            //WinApi.ForceShowToFront(Handle);
            Activate();

        }

        void MinimizeToTray()
        {
            notifyIcon1.Visible = true;
            //WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }



        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }
        #endregion

        //主菜单
        #region 主菜单
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmiScanFile_Click(object sender, EventArgs e)
        {
            ScanTorrentFile();
        }



        #endregion

        //过滤菜单
        #region 过滤菜单
        private void tsmiFilterRecent_Click(object sender, EventArgs e)
        {
            tsmiFilterRecent.Checked = !tsmiFilterRecent.Checked;
            TorrentFile.Filter.Recent = tsmiFilterRecent.Checked;
            if (tsmiFilterRecent.Checked) tbSearchText.Text = "";
            DoSearch();
        }

        private void tsmiFilterSeelater_Click(object sender, EventArgs e)
        {
            tsmiFilterSeelater.Checked = !tsmiFilterSeelater.Checked;
            if (tsmiFilterSeelater.Checked) tbSearchText.Text = string.Empty;
            TorrentFile.Filter.SeeLater = tsmiFilterSeelater.Checked;

            DoSearch();
        }

        private void tsmiFilterWatched_Click(object sender, EventArgs e)
        {
            tsmiFilterWatched.Checked = !tsmiFilterWatched.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            TorrentFile.Filter.Watched = tsmiFilterWatched.Checked;

            DoSearch();
        }
        private void tsmiFilterNotWatched_Click(object sender, EventArgs e)
        {
            tsmiFilterNotWatched.Checked = !tsmiFilterNotWatched.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            TorrentFile.Filter.NotWatched = tsmiFilterNotWatched.Checked;

            DoSearch();
        }
        private void tsmiHideSameSubject_Click(object sender, EventArgs e)
        {
            tsmiHideSameSubject.Checked = !tsmiHideSameSubject.Checked;
            TorrentFile.Filter.HideSameSubject = tsmiHideSameSubject.Checked;
            DoSearch();
        }

        private void tsmiLimit_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem) sender).Checked = true;
            foreach (var menuItem in _limitMenuItems)
            {
                if (menuItem != sender) menuItem.Checked = false;
            }
            TorrentFile.Filter.RecordsLimit =Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            DoSearch();
        }


        private void tsmiFilterSeeNoWant_Click(object sender, EventArgs e)
        {
            tsmiFilterSeeNoWant.Checked = !tsmiFilterSeeNoWant.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            TorrentFile.Filter.NotWant = tsmiFilterSeeNoWant.Checked;
            DoSearch();
        }
        #endregion
        //排序菜单
        #region 排序菜单
        private void tsmiRatingDescAsc_Click(object sender, EventArgs e)
        {
            if (sender == tsmiRatingDesc)
            {
                tsmiRatingDesc.Checked = !tsmiRatingDesc.Checked;
                if (tsmiRatingDesc.Checked) { tsmiRatingAsc.Checked = false; }
            }
            else if (sender == tsmiRatingAsc)
            {
                tsmiRatingAsc.Checked = !tsmiRatingAsc.Checked;
                if (tsmiRatingAsc.Checked) tsmiRatingDesc.Checked = false;
            }

            if (tsmiRatingDesc.Checked)
                TorrentFile.Filter.OrderRatingDesc = true;
            else if (tsmiRatingAsc.Checked)
                TorrentFile.Filter.OrderRatingDesc = false;
            else
                TorrentFile.Filter.OrderRatingDesc = null;

            DoSearch();
        }


        private void tsmiYearDescAsc_Click(object sender, EventArgs e)
        {
            if (sender == tsmiYearDesc)
            {
                tsmiYearDesc.Checked = !tsmiYearDesc.Checked;
                if (tsmiYearDesc.Checked) tsmiYearAsc.Checked = false;
            }
            else if (sender == tsmiYearDesc)
            {
                tsmiYearAsc.Checked = !tsmiYearAsc.Checked;
                if (tsmiYearAsc.Checked) tsmiYearDesc.Checked = false;
            }
            

            if (tsmiYearDesc.Checked)
                TorrentFile.Filter.OrderRatingDesc = true;
            else if (tsmiRatingAsc.Checked)
                TorrentFile.Filter.OrderRatingDesc = false;
            else
                TorrentFile.Filter.OrderRatingDesc = null;

            DoSearch();
        }


        #endregion

        //右键菜单
        #region
        private void lvResults_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (lvResults.FocusedItem.Bounds.Contains(e.Location))
            {
                lvContextMenu.Show(Cursor.Position);
            }
        }

        private void lvContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) e.Cancel = true;
        }

        private void tsmiShowFileLocation_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;

            var torrentFile = (TorrentFile)lvResults.SelectedItems[0].Tag;
            torrentFile.ShowInExplorer();
        }

        private void tsmiSearchDouban_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;

            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;
            var formSearchDouban = new FormSearchDouban(torrentFile);
            if (formSearchDouban.ShowDialog(this) == DialogResult.Cancel) return;

            RefreshSelected(torrentFile);
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0)
            {
                MessageBox.Show("没有勾选任何记录！", Properties.Resources.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var ret =MessageBox.Show("确定删除勾选的记录？\r\n选择是同时删除文件\r\n否仅删除记录", Properties.Resources.TextHint, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (ret == DialogResult.Cancel) return;
            var deleteFile = ret == DialogResult.Yes;

            var msgs = "";
            foreach (ListViewItem lvItem in lvResults.CheckedItems)
            {
                var torrentFile = (TorrentFile)lvItem.Tag;
                if (!torrentFile.DeleteFromDb(deleteFile, out var msg))
                {
                    msgs += $"{msg}\r\n";
                    continue;
                }
                lvResults.Items.Remove(lvItem);
            }
            if (!string.IsNullOrEmpty(msgs))
                MessageBox.Show(msgs, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            
        }

        private void tsmiMove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentFilePath))
            {
                MessageBox.Show("种子文件根目录没有配置", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (lvResults.SelectedItems.Count == 0) return;

            var formBrowseTorrentFolder = new FormBrowseTorrentFolder();
            if (formBrowseTorrentFolder.ShowDialog(this) == DialogResult.Cancel) return;
            var selectedPath = formBrowseTorrentFolder.SelectedPath;
            if (!selectedPath.StartsWith(TorrentFile.TorrentFilePath, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show($"选择的目录必须是种子文件目录\"{TorrentFile.TorrentFilePath}\"的子目录！", Properties.Resources.TextHint,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var errorMsg = new StringBuilder();
            var moved = 0;
            IgnoreFileWatch();
            foreach (ListViewItem selectedItem in lvResults.SelectedItems)
            {
                var torrentFile = (TorrentFile)selectedItem.Tag;
                if (!torrentFile.MoveTo(formBrowseTorrentFolder.SelectedPath, out var msg))
                    errorMsg.AppendLine(msg);
                else moved++;
            }
            IgnoreFileWatch(false);
            MessageBox.Show($"成功移动{moved}条记录。\r\n{errorMsg}", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tsmiCopyPath_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedItems[0].Tag;
            Clipboard.SetText(torrentFile.FullName);
        }

        //标记为已观看
        private void tsmiSetWatched_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;


            var formSetWatched = new FormSetWatched(torrentFile);
            if (formSetWatched.ShowDialog(this) == DialogResult.Cancel) return;
            RefreshSelected(torrentFile);

            //自动备份
            BackupDbFile();

        }

        //标记稍后看
        private void tsmiSetSeelater_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;

            if (!torrentFile.ToggleSeeLater(out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            torrentFile.seelater = torrentFile.seelater == 1 ? 0 : 1;
            lvItem.SubItems[3].Text = torrentFile.seelater.ToString();

        }

        //切换不想看
        private void tsmiToggleSeeNoWant_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;

            if (!torrentFile.ToggleSeeNoWant(out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            lvItem.SubItems[4].Text = torrentFile.seenowant.ToString();
        }

        private void tsmiCopyName_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            Clipboard.SetText(lvItem.Text);
        }

        private void tsmiCopyFile_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;
            Clipboard.SetFileDropList(new StringCollection { torrentFile.FullName });
        }

        private void lvResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditRecord();
        }

        private void tsmiCopyDouban_Click(object sender, EventArgs e)
        {
            if (lvResults.CheckedItems.Count == 0)
            {
                MessageBox.Show("请勾选要拷贝豆瓣信息的原始项！", Properties.Resources.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }
            if (lvResults.CheckedItems.Count > 1)
            {
                MessageBox.Show("请只勾选一项作为要拷贝豆瓣信息的原始项！", Properties.Resources.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

            var checkedId = ((TorrentFile)lvResults.CheckedItems[0].Tag).fid;
            var selectedIds = lvResults.SelectedItems.Cast<ListViewItem>().Select(x => ((TorrentFile)x.Tag).fid).ToList();
            if (selectedIds.Count - (selectedIds.Contains(checkedId) ? 1 : 0) <= 0)
            {
                MessageBox.Show("请选择除源项以外的更多项操作！", Properties.Resources.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

            if (MessageBox.Show("确认拷贝豆瓣信息到所选记录？", Properties.Resources.TextHint, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

            if (!TorrentFile.CopyDoubanInfo(checkedId, selectedIds, out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DoSearch();
        }


        

        #endregion

        private void tsmiShowStatistics_Click(object sender, EventArgs e)
        {
            var result = TorrentFile.CountStatistics(out var msg);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(result, Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void lvResults_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var data = new DataObject(DataFormats.FileDrop, (from ListViewItem item in lvResults.SelectedItems select ((TorrentFile)item.Tag).FullName).ToArray());
            lvResults.DoDragDrop(data, DragDropEffects.Copy);

        }


        private void tsbSearchDouban_Click(object sender, EventArgs e)
        {
            tsmiSearchDouban_Click(sender, e);
        }

        private void tsmiBtbttDownload_Click(object sender, EventArgs e)
        {
            if (_formBtBtt == null) _formBtBtt = new FormBtBtt();
            _formBtBtt.Show();
            _formBtBtt.WindowState = FormWindowState.Maximized;
            _formBtBtt.BringToFront();
        }

        private void tsbFindSimilar_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;

            tbSearchText.Text = torrentFile.FirstName;
        }

        private void btClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchText.Text = "";
        }
    }
}
