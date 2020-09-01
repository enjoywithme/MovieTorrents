using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovieTorrents.Common;
using mySharedLib;
using Exception = System.Exception;
using Timer = System.Threading.Timer;

namespace MovieTorrents
{
    public partial class FormMain : Form
    {
        public static string CurrentPath;
        public static string DbConnectionString;
        public static FormMain DefaultInstance { get; set; }
        private bool _minimizedToTray;
        private FormBtBtt _formBtBtt;
        

        private FolderBrowserDialog _folderBrowserDialog1;

        private CancellationTokenSource _queryTokenSource;
        private CancellationTokenSource _operTokenSource;

        private readonly List<string> _filterFields = new List<string> { "rating", "year" };

        public string TorrentFilePath { get; set; }

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

        private byte _hdd_nid;
        public static string Area;
        private string _shortRootPath;

        //private System.Timers.Timer _tt;


        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            DefaultInstance = this;
            //_tt = new System.Timers.Timer(5000);
            //_tt.Elapsed += (o, j) => DisplayInfo("test");
            //_tt.Enabled = true;

            CurrentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists($"{CurrentPath}\\zogvm.db"))
            {
                MessageBox.Show($"数据库文件不存在！\r\n{CurrentPath}\\zogvm.db", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }


            DbConnectionString = $"Data Source ={CurrentPath}//zogvm.db; Version = 3; ";

            if (!CheckTorrentPath(out var msg))
            {
                MessageBox.Show($"数据库找不到种子目录\r\n{msg}", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!string.IsNullOrEmpty(TorrentFilePath) && !Directory.Exists(TorrentFilePath))
            {
                MessageBox.Show($"种子目录“{TorrentFilePath}”不存在!", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tsCurrentDir.Text = $"种子目录[{TorrentFilePath}]";

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = Icon;


            lbGenres.Text = lbKeyName.Text = lbOtherName.Text = lbZone.Text = lbRating.Text = string.Empty;

            StartFileWatch();


            //var d1 = DateTime.MinValue;
            //var d2 = DateTime.Parse("2018-09-10 14:05:04");
            //Debug.WriteLine((d2 - d1).TotalMilliseconds / 1000);
#if DEBUG
            tbSearchText.Text = "雷神";
#endif
        }



        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentOperation == OperationNone)
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
        private BlockingCollection<TorrentFile> _monitoredFilesToProcess = new BlockingCollection<TorrentFile>();

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
            if(string.IsNullOrEmpty(TorrentFilePath)) return;

            try
            {
                _watcher = new FileSystemWatcher(TorrentFilePath)
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
            if(string.IsNullOrEmpty(TorrentFilePath)) return;

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

            if (!Directory.Exists(TorrentFilePath)) return;

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
            //添加一个文件会发现生成2个事件，https://blogs.msdn.microsoft.com/ahamza/2006/02/04/filesystemwatcher-generates-duplicate-events-how-to-workaround/ 
            Debug.WriteLine($"File monitored added:{e.FullPath}");

            _monitoredFilesToProcess.Add(new TorrentFile(e.FullPath));

        }
        #endregion

        //辅助函数
        #region 辅助函数

        private bool CheckTorrentPath(out string msg)
        {
            var ok = true;
            msg = string.Empty;
            using (var connection = new SQLiteConnection(DbConnectionString))
            {
                var sql = $"select d.hdd_nid,area,path from tb_dir as d inner join tb_hdd as h on h.hdd_nid=d.hdd_nid  limit 1";
                try
                {
                    connection.Open();
                    try
                    {
                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    _hdd_nid = (byte)reader.GetByte(0);// ["hdd_nid"];
                                    Area = (string)reader["area"];
                                    _shortRootPath = (string)reader["path"];
                                    TorrentFilePath = Area + _shortRootPath;
                                }
                            }

                        }

                    }
                    catch (Exception e)
                    {
                        ok = false;
                        msg = e.Message;
                    }

                    connection.Close();

                }
                catch (Exception e)
                {
                    ok = false;
                    msg = e.Message;
                }
            }

            return ok;

        }

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

            Task.Run(() => ExecuteSearch(text, _queryTokenSource.Token))
                .ContinueWith(task =>
                {
                    Interlocked.Exchange(ref _currentOperation, OperationNone);

                    if (task.IsFaulted)
                    {
                        var ex = task.Exception?.InnerException;
                        if (ex != null)
                            DisplayInfo(ex.Message, true);
                        //        Invoke(new Action(() => MessageBox.Show(ex.Message, Properties.Resources.TextError,
                        //            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }

                    else if (task.IsCanceled) { }
                    else Invoke(new Action(() =>
                    {
                        UpdateListView(task.Result);
                        DisplayInfo();
                    }));

                });

        }
        //构造搜索SQL
        private bool ProcessFilterFields(SQLiteCommand command, StringBuilder sb, string text)
        {
            if (!text.Contains(":")) return false;
            var splits = text.Split(':');
            if (splits.Length != 2) return false;
            var fldName = splits[0].ToLower();
            if (!_filterFields.Contains(fldName)) return false;
            var greaterLess = string.Empty;
            var fldValue = splits[1];
            if (fldValue.StartsWith(">") || fldValue.StartsWith("<"))
            {
                greaterLess = fldValue.Substring(0, 1);
                fldValue = fldValue.Substring(1, fldValue.Length - 1);
            }

            object oValue = null;
            switch (fldName)
            {
                case "rating":
                    if (!double.TryParse(fldValue, out var d) || d < 0)
                        throw new Exception($"错误的查询格式：“{text}”");
                    oValue = d;
                    break;
                case "year":
                    if (!int.TryParse(fldValue, out var i) || i < 1900)
                        throw new Exception($"错误的查询格式：“{text}”");
                    oValue = i.ToString();
                    break;
            }

            if (string.IsNullOrEmpty(greaterLess)) greaterLess = "=";
            var pName = $"@p{command.Parameters.Count}";
            sb.Append($" and {fldName}{greaterLess}{pName}");
            command.Parameters.AddWithValue(pName, oValue);
            return true;
        }
        private SQLiteCommand BuildSearchCommand(string text)
        {
            var command = new SQLiteCommand();

            var sb = new StringBuilder("select * from filelist_view where 1=1");

            //处理搜索关键词
            if (!string.IsNullOrEmpty(text))
            {
                var splits = text.Split(null);

                for (var i = 0; i < splits.Length; i++)
                {
                    if (ProcessFilterFields(command, sb, splits[i])) continue;

                    var pName = $"@p{command.Parameters.Count}";
                    command.Parameters.AddWithValue(pName, $"%{splits[i]}%");
                    sb.Append($" and (name like {pName} or keyname like{pName} or othername like {pName} or genres like {pName} or seecomment like {pName})");

                }
            }


            //过滤
            if (tsmiFilterSeelater.Checked) sb.Append(" and seelater=1");
            if (tsmiFilterWatched.Checked && !tsmiFilterNotWatched.Checked) sb.Append(" and seeflag=1");
            if (tsmiFilterNotWatched.Checked && !tsmiFilterWatched.Checked)
            {
                sb.Append(" and seeflag=0");
                if (tsmiHideSameSubject.Checked)
                    sb.Append(" and  doubanid not in(select DISTINCT doubanid from tb_file where seeflag=1 and doubanid<>'')");
            }

            if (tsmiFilterSeeNoWant.Checked)
            {
                sb.Append(" and seenowant=0");
                if (tsmiHideSameSubject.Checked)
                    sb.Append(" and  doubanid not in(select DISTINCT doubanid from tb_file where seenowant=1 and doubanid<>'')");
            }

            //限制条数
            var limitClause="";
            if (tsmiLimit100.Checked)
                limitClause = " limit 100";
            else if (tsmiLimit200.Checked)
                limitClause = " limit 200";
            else if (tsmiLimit500.Checked)
                limitClause = " limit 500";
            else if (tsmiLimit1000.Checked)
                limitClause = " limit 1000";

            //最近观看特殊处理
            if (tsmiFilterRecent.Checked)
            {
                sb.Insert(0, "select * from (");
                sb.Append(" order by CreationTime desc");
                sb.Append(limitClause);
                sb.Append(")");
            }

            //排序
            var ordered = false;
            if (tsmiRatingDesc.Checked)
            {
                sb.Append(" order by rating desc");
                ordered = true;
            }
            else if (tsmiRatingAsc.Checked)
            {
                sb.Append(" order by rating asc");
                ordered = true;
            }

            if (tsmiYearDesc.Checked)
                sb.Append(ordered ? ",year desc" : " order by year desc");
            else if (tsmiYearAsc.Checked)
                sb.Append(ordered ? ",year asc" : " order by year asc");

            if (!tsmiFilterRecent.Checked)
                sb.Append(limitClause);
            


            Debug.WriteLine(sb.ToString());

            command.CommandText = sb.ToString();
            return command;
        }
       
        //执行搜索
        public async Task<IEnumerable<TorrentFile>> ExecuteSearch(string text, CancellationToken cancelToken)
        {
            var result = new List<TorrentFile>();

            using (var connection = new SQLiteConnection(DbConnectionString))
            {

                using (var command = BuildSearchCommand(text))
                {
                    command.Connection = connection;

                    await connection.OpenAsync(cancelToken);

                    using (var reader = await command.ExecuteReaderAsync(cancelToken))
                    {
                        while (await reader.ReadAsync(cancelToken))
                        {
                            result.Add(new TorrentFile
                            {
                                fid = (long)reader["file_nid"],
                                area = Area,
                                path = (string)reader["path"],
                                name = (string)reader["name"],
                                keyname = reader.GetReaderFieldString("keyname"),
                                otherName = reader.GetReaderFieldString("othername"),
                                ext = (string)reader["ext"],
                                rating = (double)reader["rating"],
                                year = reader.GetReaderFieldString("year"),
                                zone = reader.GetReaderFieldString("zone"),
                                seelater = (long)reader["seelater"],
                                seenowant = (long)reader["seenowant"],
                                seeflag = (long)reader["seeflag"],
                                posterpath = reader.GetReaderFieldString("posterpath"),
                                genres = reader.GetReaderFieldString("genres"),
                                doubanid = reader.GetReaderFieldString("doubanid"),
                                seedate = reader.GetReaderFieldString("seedate"),
                                seecomment = reader.GetReaderFieldString("seecomment")
                            });

                        }
                    }
                }
            }

            return result;
        }
        //更新列表
        private void UpdateListView(IEnumerable<TorrentFile> torrentFiles)
        {
            lvResults.BeginUpdate();
            lvResults.Items.Clear();
            foreach (var torrentFile in torrentFiles)
            {
                if (_queryTokenSource != null && _queryTokenSource.IsCancellationRequested)
                    break;

                string[] row = { torrentFile.name,
                                torrentFile.rating.ToString(),
                                torrentFile.year,
                                torrentFile.seelater.ToString(),
                                torrentFile.seenowant.ToString(),
                                torrentFile.seeflag.ToString(),
                                torrentFile.seedate,
                                torrentFile.seecomment
                            };
                lvResults.Items.Add(new ListViewItem(row)
                {
                    Tag = torrentFile
                });

                //Debug.Print(torrentFile.GetPurifiedChineseName());
            }


            lvResults.EndUpdate();

        }

        #endregion

        //备份数据库文件
        private void BackupDbFile()
        {
            var watched = TorrentFile.CountWatched(DbConnectionString, out var msg);
            if (!string.IsNullOrEmpty(msg))
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (watched == -1) return;

            try
            {
                File.Copy($"{ CurrentPath}\\zogvm.db", "E:\\MyWinDoc\\My Movies\\zogvm.db", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            MessageBox.Show($"备份--OK\r\n观看了{watched}个", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        //编辑记录
        private void EditRecord()
        {
            if (lvResults.SelectedItems.Count == 0) return;

            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;
            var formRenameTorrent = new FormEdit(torrentFile);
            if (formRenameTorrent.ShowDialog() == DialogResult.Cancel) return;

            RefreshSelected(torrentFile);

        }

        //刷新当前选择
        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelected();
        }
        private void RefreshSelected(TorrentFile torrentFile = null)
        {
            lbGenres.Text = lbRating.Text = lbOtherName.Text = lbKeyName.Text =lbZone.Text = null;
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];

            if (torrentFile == null) torrentFile = (TorrentFile)lvItem.Tag;

            lvItem.Text = torrentFile.name;

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
            using (var stream = new FileStream(torrentFile.RealPosterPath, FileMode.Open, FileAccess.Read))
            {
                pictureBox1.Image = Image.FromStream(stream);
            }
        }

        //扫描种子文件
        #region 扫描种子文件
        private void ScanTorrentFile()
        {
            if (string.IsNullOrEmpty(TorrentFilePath))
            {
                MessageBox.Show("种子文件根目录没有配置", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(TorrentFilePath))
            {
                MessageBox.Show($"种子文件根目录\"{TorrentFilePath}\"不存在！", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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




            Task.Run(() => DoScanFile(TorrentFilePath, _operTokenSource.Token, filesToProcess)).ContinueWith(task =>
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
                    filesToProcess.TryAdd(new TorrentFile(fi.FullName), 2, token);
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
        private void ProcessFileToBeAdded(CancellationToken token, BlockingCollection<TorrentFile> filesToProces)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _currentOperation, OperationProcessingAddedFile, OperationNone) != OperationNone)
                    Thread.Sleep(1000);
                else
                    break;
            }


            var fileAdded = 0;
            var fileProcessed = 0;

            var stopWatch = Stopwatch.StartNew();

            using (var connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();

                using (var commandInsert = new SQLiteCommand($@"insert into tb_file
(hdd_nid,path,name,ext,year,filesize,CreationTime,LastWriteTime,LastOpenTime,maintype,resolutionW,resolutionH,filetime,bitrateKbps,zidian_sound,zidian_sub)
select {_hdd_nid},$path,$name,$ext,$year,$filesize,$n,$n,$n,0,0,0,0,0,'',''
where not exists (select 1 from tb_file where hdd_nid={_hdd_nid} and path=$path and name=$name and ext=$ext)", connection))
                {
                    commandInsert.Parameters.Add("$path", DbType.String, 520);
                    commandInsert.Parameters.Add("$name", DbType.String, 520);
                    commandInsert.Parameters.Add("$ext", DbType.String, 32);
                    commandInsert.Parameters.Add("$year", DbType.String, 16);
                    commandInsert.Parameters.Add("$filesize", DbType.Int64);
                    commandInsert.Parameters.Add("$n", DbType.Int64);

                    commandInsert.Prepare();

                    var refDate = DateTime.Parse("2016-12-02 10:53:38");
                    long refDateInt = 13125120818;

                    while (!filesToProces.IsCompleted)
                    {
                        TorrentFile torrentFile = null;
                        try
                        {
                            torrentFile = filesToProces.Take();
                        }
                        catch (InvalidOperationException) { }

                        if (torrentFile == null) continue;

                        Debug.WriteLine($"Processing:{torrentFile.name}");

                        fileProcessed++;
                        long n = ((long)(DateTime.Now - refDate).TotalSeconds + refDateInt) * 10000000;

                        commandInsert.Parameters["$path"].Value = torrentFile.path;
                        commandInsert.Parameters["$name"].Value = torrentFile.name;
                        commandInsert.Parameters["$ext"].Value = torrentFile.ext;
                        commandInsert.Parameters["$year"].Value = torrentFile.year;
                        commandInsert.Parameters["$filesize"].Value = torrentFile.filesize;
                        commandInsert.Parameters["$n"].Value = n;

                        fileAdded += commandInsert.ExecuteNonQuery();
                    }

                    filesToProces.Dispose();
                    filesToProces = null;

                }

            }

            stopWatch.Stop();
            Interlocked.Exchange(ref _currentOperation, OperationNone);

            DisplayInfo($"完成文件处理，共{fileProcessed}个文件，新添加{fileAdded}个，耗时{stopWatch.Elapsed.Minutes}分{stopWatch.Elapsed.Seconds}秒。");

        }

        #endregion

        //清理无效记录

        #region 清理无效记录
        private int _clearFileCounted;
        private int _clearFileReaded;
        private async void tsmiClearRecords_Click(object sender, EventArgs e)
        {
            if (Interlocked.CompareExchange(ref _currentOperation, OperationClearFile, OperationNone) != OperationNone)
            {
                MessageBox.Show("正在执行其他操作，等待完成后操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_operTokenSource != null)
            {
                _operTokenSource.Dispose();
                _operTokenSource = null;
            }

            _operTokenSource = new CancellationTokenSource();

            DisplayInfo("清理无效文件中...", false, false);


            await Task.Run(() => DoClearFile(_operTokenSource.Token), _operTokenSource.Token)
                .ContinueWith(task =>
                {
                    Interlocked.Exchange(ref _currentOperation, OperationNone);

                    var msg = string.Empty;

                    if (task.IsFaulted)
                    {
                        if (task.Exception?.InnerException != null) msg = $"清理无效出错：{task.Exception.InnerException.Message}";
                    }
                    else if (task.IsCanceled)
                        msg = "清理无效文件已取消";

                    DisplayInfo($"{msg}。总共读取{_clearFileReaded}记录，清理了{_clearFileCounted}条无效记录!", task.IsFaulted);


                });


        }


        private async Task DoClearFile(CancellationToken cancelToken)
        {
            _clearFileCounted = 0;
            _clearFileReaded = 0;

            var fileIdToClear = new List<long>();

            using (var connection = new SQLiteConnection(DbConnectionString))
            {
                await connection.OpenAsync(cancelToken);
                using (var command = new SQLiteCommand("select file_nid,h.area || f.path || f.name || f.ext as fullname from tb_file as f INNER join tb_hdd as h on h.hdd_nid=f.hdd_nid", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync(cancelToken))
                    {
                        while (await reader.ReadAsync(cancelToken))
                        {
                            //if (cancelToken.IsCancellationRequested) break;

                            var nid = (long)reader["file_nid"];
                            var fullname = (string)reader["fullname"];
                            Debug.WriteLine(fullname);

                            _clearFileReaded++;

                            if (!File.Exists(fullname)) fileIdToClear.Add(nid);
                            else if (new FileInfo(fullname).Length == 0)
                            {
                                File.Delete(fullname);
                                fileIdToClear.Add(nid);
                            }
                        }
                        reader.Close();
                    }
                }


                using (var command = new SQLiteCommand("delete from tb_file where file_nid=$nid", connection))
                {
                    command.Parameters.Add("$nid", DbType.Int32);
                    command.Prepare();

                    foreach (var nid in fileIdToClear)
                    {
                        command.Parameters["$nid"].Value = nid;
                        await command.ExecuteNonQueryAsync(cancelToken);
                        if (cancelToken.IsCancellationRequested) break;

                        _clearFileCounted++;
                    }
                }

            }


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
            _minimizedToTray = false;

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
            _minimizedToTray = true;
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
            if (tsmiFilterRecent.Checked) tbSearchText.Text = "";
            DoSearch();
        }

        private void tsmiFilterSeelater_Click(object sender, EventArgs e)
        {
            tsmiFilterSeelater.Checked = !tsmiFilterSeelater.Checked;
            if (tsmiFilterSeelater.Checked) tbSearchText.Text = string.Empty;
            DoSearch();
        }

        private void tsmiFilterWatched_Click(object sender, EventArgs e)
        {
            tsmiFilterWatched.Checked = !tsmiFilterWatched.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            DoSearch();
        }
        private void tsmiFilterNotWatched_Click(object sender, EventArgs e)
        {
            tsmiFilterNotWatched.Checked = !tsmiFilterNotWatched.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            DoSearch();
        }
        private void tsmiHideSameSubject_Click(object sender, EventArgs e)
        {
            tsmiHideSameSubject.Checked = !tsmiHideSameSubject.Checked;
            DoSearch();
        }
        private void tsmiLimit100_Click(object sender, EventArgs e)
        {
            tsmiLimit100.Checked = true;
            tsmiLimit200.Checked = tsmiLimit500.Checked = tsmiLimit1000.Checked = false;
            DoSearch();
        }

        private void tsmiLimit200_Click(object sender, EventArgs e)
        {
            tsmiLimit200.Checked = true;
            tsmiLimit100.Checked = tsmiLimit500.Checked = tsmiLimit1000.Checked = false;
            DoSearch();
        }

        private void tsmiLimit300_Click(object sender, EventArgs e)
        {
            tsmiLimit500.Checked = true;
            tsmiLimit100.Checked = tsmiLimit200.Checked = tsmiLimit1000.Checked = false;
            DoSearch();
        }

        private void tsmiLimit1000_Click(object sender, EventArgs e)
        {
            tsmiLimit1000.Checked = true;
            tsmiLimit100.Checked = tsmiLimit200.Checked = tsmiLimit500.Checked = false;
            DoSearch();
        }
        #endregion
        //排序菜单
        #region 排序菜单
        private void tsmiRatingDesc_Click(object sender, EventArgs e)
        {
            tsmiRatingDesc.Checked = !tsmiRatingDesc.Checked;
            if (tsmiRatingDesc.Checked) tsmiRatingAsc.Checked = false;
            DoSearch();
        }

        private void tsmiRatingAsc_Click(object sender, EventArgs e)
        {
            tsmiRatingAsc.Checked = !tsmiRatingAsc.Checked;
            if (tsmiRatingAsc.Checked) tsmiRatingDesc.Checked = false;
            DoSearch();
        }

        private void tsmiYearDesc_Click(object sender, EventArgs e)
        {
            tsmiYearDesc.Checked = !tsmiYearDesc.Checked;
            if (tsmiYearDesc.Checked) tsmiYearAsc.Checked = false;
            DoSearch();
        }

        private void tsmiYearAsc_Click(object sender, EventArgs e)
        {
            tsmiYearAsc.Checked = !tsmiYearAsc.Checked;
            if (tsmiYearAsc.Checked) tsmiYearDesc.Checked = false;
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
            if (formSearchDouban.ShowDialog() == DialogResult.Cancel) return;

            RefreshSelected(torrentFile);
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;

            if (MessageBox.Show("确定删除选中的记录？", Properties.Resources.TextHint, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;
            var deleteFile = (MessageBox.Show("同时删除文件？", Properties.Resources.TextHint, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
            if (!torrentFile.DeleteFromDb(DbConnectionString, deleteFile, out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            lvResults.Items.Remove(lvItem);

        }

        private void tsmiMove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TorrentFilePath))
            {
                MessageBox.Show("种子文件根目录没有配置", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (lvResults.SelectedItems.Count == 0) return;
            if (_folderBrowserDialog1 == null) _folderBrowserDialog1 = new FolderBrowserDialog();
            _folderBrowserDialog1.SelectedPath = TorrentFilePath;
            if (_folderBrowserDialog1.ShowDialog(this) == DialogResult.Cancel) return;
            var selectedPath = _folderBrowserDialog1.SelectedPath;
            if (!selectedPath.StartsWith(TorrentFilePath, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show($"选择的目录必须是种子文件目录\"{TorrentFilePath}\"的子目录！", Properties.Resources.TextHint,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var errorMsg = "";
            var moved = 0;
            foreach (ListViewItem selectedItem in lvResults.SelectedItems)
            {
                var torrentFile = (TorrentFile)selectedItem.Tag;
                if (!torrentFile.MoveTo(DbConnectionString, _folderBrowserDialog1.SelectedPath, out var msg))
                    errorMsg += msg;
                else moved++;
            }

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

            if (!torrentFile.ToggleSeeLater(DbConnectionString, out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            torrentFile.seelater = torrentFile.seelater == 1 ? 0 : 1;
            lvItem.SubItems[3].Text = torrentFile.seelater.ToString();

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
            var selectdIds = lvResults.SelectedItems.Cast<ListViewItem>().Select(x => ((TorrentFile)x.Tag).fid).ToList();
            if (selectdIds.Count - (selectdIds.Contains(checkedId) ? 1 : 0) <= 0)
            {
                MessageBox.Show("请选择除源项以外的更多项操作！", Properties.Resources.TextHint, MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

            if (MessageBox.Show("确认拷贝豆瓣信息到所选记录？", Properties.Resources.TextHint, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

            if (!TorrentFile.CopyDoubanInfo(DbConnectionString, checkedId, selectdIds, out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DoSearch();
        }







        #endregion

        private void tsmiShowStatistics_Click(object sender, EventArgs e)
        {
            var result = TorrentFile.CountStatistics(DbConnectionString, out var msg);
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
            var data = new DataObject(DataFormats.FileDrop, (from ListViewItem item in lvResults.SelectedItems select ((TorrentFile) item.Tag).FullName).ToArray());
            lvResults.DoDragDrop(data, DragDropEffects.Copy);

        }

        private void tsmiToggleSeeNoWant_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;

            if (!torrentFile.ToggleSeeNoWant(DbConnectionString, out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            lvItem.SubItems[4].Text = torrentFile.seenowant.ToString();
        }

        private void tsmiFilterSeeNoWant_Click(object sender, EventArgs e)
        {
            tsmiFilterSeeNoWant.Checked = !tsmiFilterSeeNoWant.Checked;
            tsmiHideSameSubject.Enabled = (tsmiFilterNotWatched.Checked || !tsmiFilterSeeNoWant.Checked) && !tsmiFilterWatched.Checked;
            DoSearch();
        }

        private void tsbSearchDouban_Click(object sender, EventArgs e)
        {
            tsmiSearchDouban_Click(sender, e);
        }

        private void tsmiBtbttDownload_Click(object sender, EventArgs e)
        {
            if(_formBtBtt==null) _formBtBtt = new FormBtBtt();
            _formBtBtt?.Show();
        }
    }
}
