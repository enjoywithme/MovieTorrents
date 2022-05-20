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
using BrightIdeasSoftware;
using MovieTorrents.Common;
using MovieTorrents.WebPWrapper;
using mySharedLib;
using Nito.AsyncEx.Synchronous;
using Exception = System.Exception;

namespace MovieTorrents
{
    public partial class FormMain : Form
    {
        public static FormMain DefaultInstance { get; set; }
        private FormBtBtt _formBtBtt;


        private CancellationTokenSource _queryTokenSource;
        private CancellationTokenSource _operationTokenSource;


        private ToolStripMenuItem[] _limitMenuItems;

        private const int OperationNone = 0;
        private const int OperationQueryFile = 1;
        private const int OperationProcessingAddedFile = 2;
        private const int OperationClearFile = 3;

        private string _searchText;

        private int _currentOperation;
        private string _lastSearchText = string.Empty;


        private FolderWatch _folderWatch;


        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            DefaultInstance = this;

            if (!TorrentFile.CheckTorrentPath(out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            if (!string.IsNullOrEmpty(TorrentFile.TorrentRootPath) && !Directory.Exists(TorrentFile.TorrentRootPath))
            {
                MessageBox.Show(string.Format(Resource.TextRootFolderNotExists, TorrentFile.TorrentRootPath), Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tsCurrentDir.Text = string.Format(Resource.TxtSeedRottDir, TorrentFile.TorrentRootPath);

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = Icon;


            lbYear.Text = lbGenres.Text = lbKeyName.Text = lbOtherName.Text = lbZone.Text = lbRating.Text = string.Empty;
            tsSummary.Text = Resource.TxtLoadZeroFiles;

            //Folder watch
            _folderWatch = new FolderWatch(TorrentFile.TorrentRootPath);
            _folderWatch.FolderWatchEvent += _folderWatch_FolderWatchEvent;
            _folderWatch.Start();


            //查询limit菜单项
            _limitMenuItems = new[] { tsmiLimit100, tsmiLimit200, tsmiLimit500, tsmiLimit1000, tsmiLimit2000 };
            foreach (var menuItem in _limitMenuItems)
            {
                menuItem.Click += tsmiLimit_Click;
            }



            tsbDelete.Click += ToolBarDelete_Click;//删除
            tsbCopyDouban.Click += MenuItemCopyDouBan_Click;//拷贝豆瓣信息
            tsbMove.Click += ToolBarMove_Click;//移动到文件夹
            tsbMovePath.Click += ToolbarMovePath_Click;//移动整个文件夹
            tsbNormalize.Click += TsbNormalizeName;//规范文件名称
            tsmiClearDuplicates.Click += MenuItemClearDuplicates_Click;//清理重复文件

            tsbRating0.Click += (_, _) => { tbSearchText.Text = @"Rating:0"; };
            tsbRating8.Click += (_, _) => { tbSearchText.Text = @"Rating:>8"; };
            tsbRating9.Click += (_, _) => { tbSearchText.Text = @"Rating:>9"; };


            //列表 list view
            lvResults.SelectedIndexChanged += lvResults_SelectedIndexChanged;
            lvResults.ItemDrag += lvResults_ItemDrag;
            lvResults.MouseDoubleClick += lvResults_MouseDoubleClick;
            lvResults.MouseClick += lvResults_MouseClick;
            lvResults.RowFormatter = RowFormatter;
            lvResults.CellToolTipGetter = CellToolTipGetter;
            lvResults.CheckBoxes = true;
            lvResults.FullRowSelect = true;
            lvResults.EmptyListMsg = Resource.TextNoResultsFound;
            lvResults.EmptyListMsgFont = new Font("Microsoft YaHei", 24);
#if DEBUG
            tbSearchText.Text = @"雷神";
#endif
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentOperation == OperationNone || 0 != Interlocked.Exchange(ref BtBtItem.AutoDownloadRunning, 1))
            {
                _formBtBtt?.Close();
                return;
            }
            MessageBox.Show(Resource.TxtWaitForOtherOperation, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }

        //目录监视
        #region 目录监视

        private void _folderWatch_FolderWatchEvent(object sender, FolderWatchEventArgs e)
        {
            if (e == null) return;


            if (!string.IsNullOrEmpty(e.Message))
            {
                DisplayInfo(e.Message);
            }

            //Status
            if (e.StatusChanged)
            {
                tsButtonWatch.Image = _folderWatch.IsWatching ? Resource.Eye32 : Resource.EyeStop32;
                tsButtonWatch.ToolTipText = _folderWatch.IsWatching ? Resource.TextDirInMonitor : Resource.TextDirMonitorStopped;
                tsmiToggleWatch.Text = _folderWatch.IsWatching ? Resource.TxtStopDirMonitor : Resource.TxtStartDirMonitor;

                if (_folderWatch.IsWatching)
                    DisplayInfo("目录监视已启动", false, false);
                else DisplayInfo("目录监视已停止", true);

            }


            if (e.NewFilesToBeProcess)
            {
                var filesToProcess = new BlockingCollection<TorrentFile>();

                while (_folderWatch.FilesAdded.TryDequeue(out var file))
                {
                    filesToProcess.Add(TorrentFile.FromFullPath(file));
                }
                filesToProcess.CompleteAdding();

                Task.Run(() => ProcessFileToBeAdded(filesToProcess)).ContinueWith(task =>
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

        }

        private void MenuItemToggleWatch_Click(object sender, EventArgs e)
        {
            if (_folderWatch.IsWatching)
                _folderWatch.Stop();
            else
                _folderWatch.Start();

        }

        #endregion


        //List view
        #region 列表

        //更新列表
        private void UpdateListView(IList<TorrentFile> torrentFiles)
        {
            var totalFiles = torrentFiles.Count();
            var totalItems = torrentFiles.Where(x => !string.IsNullOrEmpty(x.DoubanId)).GroupBy(x => x.DoubanId).Count();
            lvResults.SetObjects(torrentFiles,true);
            lvResults.SelectedIndices.Clear();
            tsSummary.Text = string.Format(Resource.TextLoadNFiles, totalFiles, totalItems);
        }
        //条目tooltip
        private string CellToolTipGetter(OLVColumn column, object modelObject)
        {
            var torrentFile = (TorrentFile)modelObject;
            var sb = new StringBuilder();
            sb.AppendLine(torrentFile.Name);
            sb.AppendLine($"评分：{torrentFile.Rating:0.0}");
            sb.AppendLine($"年份：{torrentFile.Year}");
            sb.AppendLine($"类型:{torrentFile.Genres}");
            sb.AppendLine($"地区:{torrentFile.Zone}");
            sb.AppendLine($"路径：{torrentFile.Path}");
            sb.AppendLine($"观看评论：{torrentFile.SeeComment}");
            return sb.ToString();
        }
        //格式化条目
        private void RowFormatter(OLVListItem olvItem)
        {
            var torrentFile = (TorrentFile)olvItem.RowObject;
            var i = lvResults.Columns.IndexOf(olvColName);
            if (i < 0) return;
            olvItem.UseItemStyleForSubItems = false;
            olvItem.SubItems[i].ForeColor = torrentFile.ForeColor;
        }

        //刷新当前选择
        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelected();
        }
        private void RefreshSelected()
        {
            lbYear.Text = lbGenres.Text = lbRating.Text = lbOtherName.Text = lbKeyName.Text = lbZone.Text = null;
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            if (lvResults.SelectedObjects.Count == 0) return;
            lvResults.RefreshSelectedObjects();


            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];

            lbGenres.Text = torrentFile.Genres;
            lbYear.Text = torrentFile.Year;
            lbKeyName.Text = torrentFile.KeyName;
            lbOtherName.Text = torrentFile.OtherName;
            lbZone.Text = torrentFile.Zone;
            lbRating.Text = Math.Abs(torrentFile.Rating) < 0.0001 ? null : torrentFile.Rating.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(torrentFile.RealPosterPath) || !File.Exists(torrentFile.RealPosterPath)) return;
            try
            {
                var ext = Path.GetExtension(torrentFile.RealPosterPath);
                if (ext.Equals(@".webp", StringComparison.InvariantCultureIgnoreCase))
                {
                    using var webp = new WebP();
                    pictureBox1.Image = webp.Load(torrentFile.RealPosterPath);
                }
                else
                {
                    using var stream = new FileStream(torrentFile.RealPosterPath, FileMode.Open, FileAccess.Read);
                    pictureBox1.Image = Image.FromStream(stream);
                }

            }
            catch (Exception)
            {
                // ignored
            }




        }

        //拖动条目
        private void lvResults_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var selectedItems = lvResults.SelectedObjects.Cast<TorrentFile>();
            var data = new DataObject(DataFormats.FileDrop, (from TorrentFile item in selectedItems select item.FullName).ToArray());
            lvResults.DoDragDrop(data, DragDropEffects.Copy);

        }
        //双击条目
        private void lvResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditRecord();
        }
        //编辑记录
        private void EditRecord()
        {
            if (lvResults.SelectedObjects.Count == 0) return;

            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            var formRenameTorrent = new FormEdit(torrentFile);
            if (formRenameTorrent.ShowDialog(this) == DialogResult.Cancel) return;

            RefreshSelected();

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
                    tssState.Image = Resource.InfoRed32;
                else
                    tssState.Image = _currentOperation == OperationNone ? Resource.InfoGree32 : Resource.InfoYellow32;

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

            if (MessageBox.Show(Resource.TxtConfirmCancelCurrentOperation, Resource.TextHint, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                return;

            _operationTokenSource?.Cancel();
        }
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (lvResults.SelectedIndices.Count == 0) return;
            var lvItem = lvResults.Items[lvResults.SelectedIndices[0]];
            var torrentFile = (TorrentFile)lvItem.Tag;
            torrentFile.OpenDoubanLink();
        }
        #endregion


        //搜索记录
        #region 搜索记录
        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            _searchText = tbSearchText.Text;
            DoSearch(true);
        }

        //搜索的主函数
        private void DoSearch(bool checkLastSearch = false)
        {
            if (checkLastSearch && string.Compare(_searchText, _lastSearchText, StringComparison.InvariantCultureIgnoreCase) == 0) return;
            _lastSearchText = _searchText;

            if (!tsmiFilterRecent.Checked && !tsmiFilterSeelater.Checked)
            {
                if (string.IsNullOrEmpty(_searchText) || (_searchText.Length <= 2 && _searchText.All(x => x <= 127)))
                {
                    DisplayInfo("输入的搜素文字过少", false, false);
                    return;
                }
            }


            if (Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationNone) != OperationNone
                && Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationQueryFile) != OperationQueryFile)

            {
                MessageBox.Show(Resource.TxtWaitForOtherOperation, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            Task.Run(() =>
            {
                var task = TorrentFile.ExecuteSearch(_searchText, _queryTokenSource.Token);
                Interlocked.Exchange(ref _currentOperation, OperationNone);
                //https://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c
                var result = task.WaitAndUnwrapException();

                if (result.Cancelled) return;

                BeginInvoke(new Action(() =>
                {
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        DisplayInfo(result.Message, true);
                        return;
                    }

                    UpdateListView(result.TorrentFiles);
                    DisplayInfo();
                }));

            });


        }


        #endregion

        //备份数据库文件
        private void BackupDbFile()
        {
            var ret = TorrentFile.BackupDbFile(out var msg);

            MessageBox.Show(msg, ret ? Resource.TextHint : "错误", MessageBoxButtons.OK, ret ? MessageBoxIcon.Information : MessageBoxIcon.Error);


        }

        //扫描种子文件
        #region 扫描种子文件
        private void ScanTorrentFile()
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentRootPath))
            {
                MessageBox.Show(Resource.TextNoRootFolder, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(TorrentFile.TorrentRootPath))
            {
                MessageBox.Show(string.Format(Resource.TextRootFolderNotExists, TorrentFile.TorrentRootPath), Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_currentOperation != 0)
            {
                MessageBox.Show(Resource.TxtWaitForOtherOperation, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_operationTokenSource != null)
            {
                _operationTokenSource.Dispose();
                _operationTokenSource = null;
            }

            _operationTokenSource = new CancellationTokenSource();
            var filesToProcess = new BlockingCollection<TorrentFile>();




            Task.Run(() => DoScanFile(TorrentFile.TorrentRootPath, _operationTokenSource.Token, filesToProcess)).ContinueWith(task =>
            {
                if (!task.IsFaulted) return;
                if (task.Exception?.InnerException != null) DisplayInfo($"扫描文件出错：{task.Exception.InnerException.Message}", true);
                //Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
            });

            Task.Run(() => ProcessFileToBeAdded(filesToProcess)).ContinueWith(task =>
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
                catch (OperationCanceledException)
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
        private void ProcessFileToBeAdded(BlockingCollection<TorrentFile> filesToProcess)
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
        private async void MenuItemClearRecords_Click(object sender, EventArgs e)
        {
            if (Interlocked.CompareExchange(ref _currentOperation, OperationClearFile, OperationNone) != OperationNone)
            {
                MessageBox.Show(Resource.TxtWaitForOtherOperation, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_operationTokenSource != null)
            {
                _operationTokenSource.Dispose();
                _operationTokenSource = null;
            }

            _operationTokenSource = new CancellationTokenSource();

            DisplayInfo("清理无效文件中...", false, false);


            await Task.Run(async () =>
            {
                int clearFileCounted;
                int clearFileRead;
                string msg;
                bool error;
                (clearFileRead, clearFileCounted, msg, error) = await TorrentFile.DoClearFile(_operationTokenSource.Token);
                DisplayInfo($"{msg}总共读取{clearFileRead}记录，清理了{clearFileCounted}条无效记录!", error);

            });

            Interlocked.Exchange(ref _currentOperation, OperationNone);

        }

        //清理重复选项
        private async void MenuItemClearDuplicates_Click(object sender, EventArgs e)
        {
            if (Interlocked.CompareExchange(ref _currentOperation, OperationClearFile, OperationNone) != OperationNone)
            {
                MessageBox.Show(Resource.TextWaitOtherOperation, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_operationTokenSource != null)
            {
                _operationTokenSource.Dispose();
                _operationTokenSource = null;
            }

            _operationTokenSource = new CancellationTokenSource();

            DisplayInfo("清理重复文件中...", false, false);


            await Task.Run(async () =>
            {
                int clearFileCounted;
                string msg;
                bool error;
                (clearFileCounted, msg, error) = await TorrentFile.DoClearDuplicate(_operationTokenSource.Token);
                DisplayInfo($"{msg}清理了{clearFileCounted}条重复记录!", error);

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
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            ShowWindow();
        }

        #endregion

        //主菜单
        #region 主菜单
        private void MenuitemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuItemScanFile_Click(object sender, EventArgs e)
        {
            ScanTorrentFile();
        }

        private void MenuItemShowStatistics_Click(object sender, EventArgs e)
        {
            var result = TorrentFile.CountStatistics(out var msg);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(result, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            ((ToolStripMenuItem)sender).Checked = true;
            foreach (var menuItem in _limitMenuItems)
            {
                if (menuItem != sender) menuItem.Checked = false;
            }
            TorrentFile.Filter.RecordsLimit = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            DoSearch();
        }


        private void tsmiFilterSeeNoWant_Click(object sender, EventArgs e)
        {
            tsmiFilterSeeNoWant.Checked = !tsmiFilterSeeNoWant.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            TorrentFile.Filter.NotWant = tsmiFilterSeeNoWant.Checked;
            DoSearch();
        }

        private void tsmiHaveDoubanId_Click(object sender, EventArgs e)
        {
            tsmiHaveDoubanId.Checked = !tsmiHaveDoubanId.Checked;
            TorrentFile.Filter.HasDoubanId = tsmiHaveDoubanId.Checked;
            DoSearch();

        }

        private void tsmiNoDoubanId_Click(object sender, EventArgs e)
        {
            tsmiNoDoubanId.Checked = !tsmiNoDoubanId.Checked;
            TorrentFile.Filter.NoDoubanId = tsmiNoDoubanId.Checked;
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
            if (lvResults.SelectedIndices.Count == 0) e.Cancel = true;
        }
        //显示条目文件路径
        private void MenuItemShowFileLocation_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            torrentFile.ShowInExplorer();
        }

        //搜索豆瓣信息
        private void MenuItemSearchDouBan_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            var formDouBan = new FormSearchDouban(torrentFile);
            if (formDouBan.ShowDialog(this) == DialogResult.Cancel) return;

            RefreshSelected();
        }

        //拷贝文件路径
        private void MenuItemCopyPath_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            Clipboard.SetText(torrentFile.FullName);
        }

        //标记为已观看
        private void MenuItemSetWatched_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];


            var formSetWatched = new FormSetWatched(torrentFile);
            if (formSetWatched.ShowDialog(this) == DialogResult.Cancel) return;
            lvResults.RefreshObject(torrentFile);

            //自动备份
            BackupDbFile();

        }

        //切换稍后看
        private void MenuItemSetSeeLater_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];

            if (!torrentFile.ToggleSeeLater(out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            lvResults.RefreshObject(torrentFile);

        }

        //切换不想看
        private void MenuItemToggleSeeNoWant_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];

            if (!torrentFile.ToggleSeeNoWant(out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            lvResults.RefreshObject(torrentFile);
        }


        //拷贝文件名
        private void MenuItemCopyName_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;

            Clipboard.SetText(((TorrentFile)lvResults.SelectedObjects[0]).Name);
        }

        //拷贝文件
        private void MenuItemCopyFile_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;

            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            Clipboard.SetFileDropList(new StringCollection { torrentFile.FullName });
        }



        // 打开文件
        private void MenuItemOpenFile_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;

            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            if (!File.Exists(torrentFile.FullName))
            {
                MessageBox.Show(string.Format(Resource.TxtFileNotExists, torrentFile.FullName), Resource.TextError, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(torrentFile.FullName);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Resource.TextError, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        //工具栏
        #region 工具栏
        //搜索豆瓣条目
        private void tsbSearchDouBan_Click(object sender, EventArgs e)
        {
            MenuItemSearchDouBan_Click(sender, e);
        }
        //删除按钮
        private void ToolBarDelete_Click(object sender, EventArgs e)
        {
            if (lvResults.CheckedObjects.Count == 0)
            {
                MessageBox.Show(Resource.TextNoCheckedItems, Resource.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var ret = MessageBox.Show(Resource.TextConfirmDelete, Resource.TextHint, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (ret == DialogResult.Cancel) return;
            var deleteFile = ret == DialogResult.Yes;

            var msgs =new StringBuilder();
            var checkedObjects = lvResults.CheckedObjects;
            foreach (var item in checkedObjects)
            {
                var torrentFile = (TorrentFile)item;
                if (!torrentFile.DeleteFromDb(deleteFile, out var msg))
                {
                    msgs.AppendLine($"{msg}");
                    continue;
                }
                lvResults.RemoveObject(torrentFile);
            }

            if (msgs.Length>0)
                MessageBox.Show(msgs.ToString(), Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);


        }

        //移动文件按钮
        private void ToolBarMove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentRootPath))
            {
                MessageBox.Show(Resource.TextNoRootFolder, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (lvResults.SelectedObjects.Count == 0) return;

            var formBrowseTorrentFolder = new FormBrowseTorrentFolder();
            if (formBrowseTorrentFolder.ShowDialog(this) == DialogResult.Cancel) return;
            var selectedPath = formBrowseTorrentFolder.SelectedPath;
            if (!selectedPath.StartsWith(TorrentFile.TorrentRootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show(string.Format(Resource.TextPathMustInRoot, TorrentFile.TorrentRootPath), Resource.TextHint,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var errorMsg = new StringBuilder();
            var moved = 0;
            _folderWatch.IgnoreFileWatch();
            var selectedItems = lvResults.SelectedObjects;
            foreach (var selectedItem in selectedItems)
            {
                var torrentFile = (TorrentFile)selectedItem;
                var (ret, msg) = torrentFile.MoveTo(formBrowseTorrentFolder.SelectedPath);
                if (!ret)
                    errorMsg.AppendLine(msg);
                else moved++;
            }
            _folderWatch.IgnoreFileWatch(false);
            MessageBox.Show(string.Format(Resource.TextFilesMoved, moved, errorMsg), Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshSelected();
        }

        //移动整个目录
        private void ToolbarMovePath_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TorrentFile.TorrentRootPath))
            {
                MessageBox.Show(Resource.TextNoRootFolder, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];
            var sourcePath = Path.Combine(TorrentFile.DefaultArea, torrentFile.Path);
            if (!Directory.Exists(sourcePath))
            {
                MessageBox.Show(string.Format(Resource.TextPathNotExists, sourcePath), Resource.TextError,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.GetDirectories(sourcePath).Length > 0)
            {
                MessageBox.Show(string.Format(Resource.TextPathHasChildDirectory, sourcePath), Resource.TextError,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dirName = new DirectoryInfo(sourcePath).Name;
            if (int.TryParse(dirName, out var _) || Directory.GetFiles(sourcePath).Length > 10)
            {
                if (MessageBox.Show(string.Format(Resource.TextPathHavePlentyFiles, sourcePath), Resource.TextHint, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }


            var formBrowseTorrentFolder = new FormBrowseTorrentFolder(true);
            if (formBrowseTorrentFolder.ShowDialog(this) == DialogResult.Cancel) return;
            var selectedPath = formBrowseTorrentFolder.SelectedPath;
            if (!selectedPath.StartsWith(TorrentFile.TorrentRootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show(string.Format(Resource.TextPathMustInRoot, TorrentFile.TorrentRootPath), Resource.TextHint,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _folderWatch.IgnoreFileWatch();
            var (ret, msg) = TorrentFile.MovePath(formBrowseTorrentFolder.SelectedPath, torrentFile.Path, formBrowseTorrentFolder.FolderRename);

            _folderWatch.IgnoreFileWatch(false);
            MessageBox.Show(ret ? $"成功移动目录 {torrentFile.Path}。" : $"移动目录 {torrentFile.Path} 失败。\r\n{msg}",
                Resource.TextHint, MessageBoxButtons.OK,
                ret ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            DoSearch();
        }

        //规范文件名称
        private void TsbNormalizeName(object sender, EventArgs e)
        {

            if (lvResults.SelectedObjects.Count == 0)
            {
                MessageBox.Show(Resource.TextNoSelectedItems, Resource.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            _folderWatch.IgnoreFileWatch();
            var errorMsg = new StringBuilder();
            var renamed = 0;
            var selectedItems = lvResults.SelectedObjects;
            foreach (var selectedItem in selectedItems)
            {
                var torrentFile = (TorrentFile)selectedItem;
                Utility.ReadBadWords();
                var newName = torrentFile.Name.PurifyName().NormalizeTorrentFileName();
                var (ret, msg) = torrentFile.MoveTo(null, newName);
                if (!ret)
                    errorMsg.AppendLine(msg);
                else renamed++;
            }


            _folderWatch.IgnoreFileWatch(false);
            MessageBox.Show(string.Format(Resource.TextFilesRenamed, renamed, errorMsg), Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshSelected();
        }

        //拷贝豆瓣信息
        private void MenuItemCopyDouBan_Click(object sender, EventArgs e)
        {
            switch (lvResults.CheckedObjects.Count)
            {
                case 0:
                    MessageBox.Show(Resource.TextNoCheckedItems, Resource.TextHint, MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                    return;
                case > 1:
                    MessageBox.Show(Resource.TextCheckOnlyOneItem, Resource.TextHint, MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                    return;
            }

            var checkedId = ((TorrentFile)lvResults.CheckedObjects[0]).Fid;
            var selectedItems = lvResults.SelectedObjects.Cast<TorrentFile>();
            
            var selectedIds = selectedItems.Select(x => x.Fid).ToList();
            if (selectedIds.Count - (selectedIds.Contains(checkedId) ? 1 : 0) <= 0)
            {
                MessageBox.Show(Resource.TextSelectItemsExcludeSource, Resource.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

            if (MessageBox.Show(Resource.TextConfirmCopDoubanInfo, Resource.TextHint, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

            if (!TorrentFile.CopyDoubanInfo(checkedId, selectedIds, out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DoSearch();
        }
        private void MenuItemBtBtDownload_Click(object sender, EventArgs e)
        {
            _formBtBtt ??= new FormBtBtt();
            _formBtBtt.Show();
            _formBtBtt.WindowState = FormWindowState.Maximized;
            _formBtBtt.BringToFront();
        }
        //查找相似项
        private void tsbFindSimilar_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedObjects.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedObjects[0];

            tbSearchText.Text = torrentFile.FirstName;
        }

        private void btClearSearch_Click(object sender, EventArgs e)
        {
            tbSearchText.Text = "";
        }

        #endregion





    }
}
