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
using Infragistics.Win.DataVisualization;
using Exception = System.Exception;

namespace MovieTorrents
{
    public partial class FormMain : Form
    {
        public static string CurrentPath;
        public static string DbConnectionString;
        private bool _minimizedToTray;

        private CancellationTokenSource _quernyTokenSource;
        private CancellationTokenSource _operTokenSource;

        private readonly List<string> _filterFields = new List<string> { "rating", "year" };

        private string TorrentFilePath { get; set; }

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
        private string _area;
        private string _shortRootPath;

        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();


        public FormMain()
        {
            InitializeComponent();


        }

        private void FormMain_Load(object sender, EventArgs e)
        {
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
                Close();
                return;
            }

            if (!Directory.Exists(TorrentFilePath))
            {
                MessageBox.Show($"种子目录“{TorrentFilePath}”不存在!", Properties.Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            lbGenres.Text = lbKeyName.Text = lbOtherName.Text = lbRating.Text = string.Empty;


            _watcher.Path = TorrentFilePath;
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.FileName;
            _watcher.Filter = "*.torrent";
            _watcher.Created += Watcher_File_Created;
            _watcher.EnableRaisingEvents = true;

            //var d1 = DateTime.MinValue;
            //var d2 = DateTime.Parse("2018-09-10 14:05:04");
            //Debug.WriteLine((d2 - d1).TotalMilliseconds / 1000);
#if DEBUG
            tbSearchText.Text = "雷神";
#endif
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentOperation == OperationNone) return;
            MessageBox.Show("尚有操作在进行中，不能退出！", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }



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
                                    _area = (string)reader["area"];
                                    _shortRootPath = (string)reader["path"];
                                    TorrentFilePath = _area + _shortRootPath;
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
        private void Watcher_File_Created(object sender, FileSystemEventArgs e)
        {
            Debug.Print(e.FullPath);

            var filesToProcess = new BlockingCollection<TorrentFile>
            {
                new TorrentFile(e.FullPath)
            };

            filesToProcess.CompleteAdding();

            Task.Run(() => ProcessFileToBeAdded(new CancellationTokenSource().Token, filesToProcess)).ContinueWith(task =>
            {
                if (!task.IsFaulted) return;
                var ex = task.Exception?.InnerException;
                if (ex != null)
                    DisplayInfo(ex.Message, true);
            }); ;
        }
        private void DisplayInfo(string infoMsg = "空闲", bool error = false)
        {
            Invoke(new Action(() =>
            {
                tssInfo.Text = infoMsg;
                if (error)
                    tssState.BackColor = Color.Red;
                else
                    tssState.BackColor = _currentOperation == OperationNone ? Color.LimeGreen : Color.Blue;

                if (notifyIcon1.Visible)
                    notifyIcon1.ShowBalloonTip(2000, "Movie torrents", infoMsg, error ? ToolTipIcon.Error : ToolTipIcon.Info);
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


        private void DoSearch(bool checkLastSearch=false)
        {
            var text = tbSearchText.Text.Trim();
            if (checkLastSearch && string.Compare(text, _lastSearchText, StringComparison.InvariantCultureIgnoreCase) == 0) return;
            _lastSearchText = text;
            lvResults.Items.Clear();

            if (!tsmiFilterRecent.Checked)
            {
                if (string.IsNullOrEmpty(text) || (text.Length <= 2 && text.All(x => (int)x <= 127)))
                {
                    DisplayInfo("输入的搜素文字过少");
                    return;
                }
            }


            if (Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationNone) != OperationNone
                && Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationQueryFile) != OperationQueryFile)

            {
                MessageBox.Show("正在执行其他操作，等待完成后操作", Properties.Resources.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            if (_quernyTokenSource != null)
            {
                _quernyTokenSource.Cancel();
                _quernyTokenSource.Dispose();
                _quernyTokenSource = null;
            }

            _quernyTokenSource = new CancellationTokenSource();

            DisplayInfo("正在查询文件");

            Task.Run(() => ExecuteSearch(text, _quernyTokenSource.Token))
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
                    sb.Append($" and (name like {pName} or othername like {pName} or genres like {pName})");

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

            //排序
            var ordered = false;
            if (tsmiFilterRecent.Checked)
            {
                sb.Append(" order by CreationTime desc");
                ordered = true;
            }

            if (tsmiRatingDesc.Checked)
            {
                sb.Append(ordered ? ",rating desc" : " order by rating desc");
                ordered = true;
            }

            if (tsmiRatingAsc.Checked)
            {
                sb.Append(ordered ? ",rating asc" : " order by rating asc");
                ordered = true;
            }

            if (tsmiYearDesc.Checked)
            {
                sb.Append(ordered ? ",year desc" : " order by year desc");
                ordered = true;
            }
            if (tsmiYearAsc.Checked) sb.Append(ordered ? ",year asc" : " order by year asc");

            //限制条数
            if (tsmiLimit100.Checked)
                sb.Append(" limit 100");
            else if (tsmiLimit200.Checked)
                sb.Append(" limit 200");
            else if (tsmiLimit500.Checked)
                sb.Append(" limit 500");
            else if (tsmiLimit1000.Checked)
                sb.Append(" limit 1000");


            Debug.WriteLine(sb.ToString());

            command.CommandText = sb.ToString();
            return command;
        }
        //更新列表
        private void UpdateListView(IEnumerable<TorrentFile> torrentFiles)
        {
            lvResults.BeginUpdate();
            lvResults.Items.Clear();
            foreach (var torrentFile in torrentFiles)
            {
                if (_quernyTokenSource != null && _quernyTokenSource.IsCancellationRequested)
                    break;

                string[] row = { torrentFile.name,
                                torrentFile.rating.ToString(),
                                torrentFile.year,
                                torrentFile.seelater.ToString(),
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


        private string GetReaderFieldString(DbDataReader reader, string fieldName)
        {
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : (string)reader[fieldName];
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
                                area = _area,
                                path = (string)reader["path"],
                                name = (string)reader["name"],
                                keyname = GetReaderFieldString(reader, "keyname"),
                                otherName = GetReaderFieldString(reader, "othername"),
                                ext = (string)reader["ext"],
                                rating = (double)reader["rating"],
                                year = GetReaderFieldString(reader, "year"),
                                seelater = (long)reader["seelater"],
                                seeflag = (long)reader["seeflag"],
                                posterpath = GetReaderFieldString(reader, "posterpath"),
                                genres = GetReaderFieldString(reader, "genres"),
                                doubanid = GetReaderFieldString(reader, "doubanid"),
                                seedate = GetReaderFieldString(reader, "seedate"),
                                seecomment = GetReaderFieldString(reader, "seecomment")
                            });

                        }
                    }
                }
            }

            return result;
        }

        #endregion

        //备份数据库文件
        private void BackupDbFile()
        {
            var watched = TorrentFile.CountWatched(DbConnectionString);
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

            lvItem.Text = torrentFile.name;
            lvItem.SubItems[1].Text = torrentFile.year;

            lbKeyName.Text = torrentFile.keyname;
            lbOtherName.Text = torrentFile.otherName;
            lbGenres.Text = torrentFile.genres;

        }

        //刷新当前选择
        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelected();
        }
        private void RefreshSelected(TorrentFile torrentFile = null)
        {
            lbGenres.Text = lbRating.Text = lbOtherName.Text = lbKeyName.Text = null;
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

            lbGenres.Text = torrentFile.genres;
            lbKeyName.Text = torrentFile.keyname;
            lbOtherName.Text = torrentFile.otherName;
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
                if (task.IsFaulted)
                {
                    Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                }
            });

            Task.Run(() => ProcessFileToBeAdded(_operTokenSource.Token, filesToProcess)).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Invoke(new Action(() =>
                    {
                        if (task.Exception?.InnerExceptions != null) MessageBox.Show(task.Exception.InnerException.Message);
                    }));
                }
            });

        }

        //添加扫描到的种子到队列等待处理
        private void DoScanFile(string dirName, CancellationToken token, BlockingCollection<TorrentFile> filesToProcess)
        {
            var fileScanned = 0;

            var di = new DirectoryInfo(dirName);
            if (!di.Exists) return;

            var stopwatch = Stopwatch.StartNew();

            foreach (var fi in di.EnumerateFiles("*.torrent", SearchOption.AllDirectories))
            {
                if (token.IsCancellationRequested) break;

                fileScanned++;

                //Debug.WriteLine(fi.Extension);

                filesToProcess.Add(new TorrentFile(fi.FullName));

            }

            filesToProcess.CompleteAdding();

            stopwatch.Stop();
            DisplayInfo($"完成文件磁盘扫描，共{fileScanned}个文件，耗时{stopwatch.Elapsed.Minutes}分{stopwatch.Elapsed.Seconds}秒。");



        }

        //处理文件队列
        private void ProcessFileToBeAdded(CancellationToken token, BlockingCollection<TorrentFile> _filesToProces)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _currentOperation, OperationProcessingAddedFile, OperationNone) != OperationNone)
                    Thread.Sleep(200);
                else
                    break;
            }

            DisplayInfo("处理文件队列");

            var _fileAdded = 0;
            var _fileProcessed = 0;

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

                    foreach (var torrentFile in _filesToProces.GetConsumingEnumerable())
                    {
                        if (token.IsCancellationRequested) break;

                        Debug.WriteLine($"Processing:{torrentFile.name}");

                        _fileProcessed++;
                        long n = ((long)(DateTime.Now - refDate).TotalSeconds + refDateInt) * 10000000;

                        commandInsert.Parameters["$path"].Value = torrentFile.path;
                        commandInsert.Parameters["$name"].Value = torrentFile.name;
                        commandInsert.Parameters["$ext"].Value = torrentFile.ext;
                        commandInsert.Parameters["$year"].Value = torrentFile.year;
                        commandInsert.Parameters["$filesize"].Value = torrentFile.filesize;
                        commandInsert.Parameters["$n"].Value = n;

                        _fileAdded += commandInsert.ExecuteNonQuery();
                    }


                }

            }

            stopWatch.Stop();
            Interlocked.Exchange(ref _currentOperation, OperationNone);

            DisplayInfo($"完成文件处理，共{_fileProcessed}个文件，新添加{_fileAdded}个，耗时{stopWatch.Elapsed.Minutes}分{stopWatch.Elapsed.Seconds}秒。");

        }

        #endregion

        //清理无效记录
        private async void DoClearFile(CancellationToken cancelToken)
        {
            var filesCleard = 0;
            var filesCount = 0;

            var fileIdToClear = new List<long>();

            using (var connection = new SQLiteConnection(DbConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand("select file_nid,h.area || f.path || f.name || f.ext as fullname from tb_file as f INNER join tb_hdd as h on h.hdd_nid=f.hdd_nid", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (cancelToken.IsCancellationRequested) break;

                            var nid = (long)reader["file_nid"];
                            var fullname = (string)reader["fullname"];
                            Debug.WriteLine(fullname);

                            filesCount++;

                            if (!File.Exists(fullname)) fileIdToClear.Add(nid);
                        }
                        reader.Close();
                    }
                }

                if (cancelToken.IsCancellationRequested) goto exit;

                using (var command = new SQLiteCommand("delete from tb_file where file_nid=$nid", connection))
                {
                    command.Parameters.Add("$nid", DbType.Int32);
                    command.Prepare();

                    foreach (var nid in fileIdToClear)
                    {
                        command.Parameters["$nid"].Value = nid;
                        await command.ExecuteNonQueryAsync(cancelToken);
                        if (cancelToken.IsCancellationRequested) break;

                        filesCleard++;
                    }
                }

            }

            exit:
            Interlocked.Exchange(ref _currentOperation, OperationNone);
            DisplayInfo($"总共{filesCount}记录，清理了{filesCleard}条无效记录!");
        }

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
            if (_minimizedToTray)
            {
                notifyIcon1.Visible = false;
                Show();
                Activate();
                WindowState = FormWindowState.Normal;
                _minimizedToTray = false;
            }
            else
            {
                WinApi.ShowToFront(Handle);
            }
        }

        void MinimizeToTray()
        {
            notifyIcon1.Icon = Icon;
            notifyIcon1.Visible = true;
            WindowState = FormWindowState.Minimized;
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
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
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

        private void tsmiClearRecords_Click(object sender, EventArgs e)
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

            DisplayInfo("正在清理无效文件");

            Task.Run(() => DoClearFile(_operTokenSource.Token))
                .ContinueWith(task =>
                {
                    if (task.IsFaulted) Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                });
        }



        #endregion

        //过滤菜单
        #region 过滤菜单
        private void tsmiFilterRecent_Click(object sender, EventArgs e)
        {
            tsmiFilterRecent.Checked = !tsmiFilterRecent.Checked;
            DoSearch();
        }

        private void tsmiFilterSeelater_Click(object sender, EventArgs e)
        {
            tsmiFilterSeelater.Checked = !tsmiFilterSeelater.Checked;
            DoSearch();
        }

        private void tsmiFilterWatched_Click(object sender, EventArgs e)
        {
            tsmiFilterWatched.Checked = !tsmiFilterWatched.Checked;
            tsmiHideSameSubject.Enabled = tsmiFilterNotWatched.Checked && !tsmiFilterWatched.Checked;
            DoSearch();
        }
        private void tsmiFilterNotWatched_Click(object sender, EventArgs e)
        {
            tsmiFilterNotWatched.Checked = !tsmiFilterNotWatched.Checked;
            tsmiHideSameSubject.Enabled = tsmiFilterNotWatched.Checked && !tsmiFilterWatched.Checked;
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
            if (e.Button == MouseButtons.Right)
            {
                if (lvResults.FocusedItem.Bounds.Contains(e.Location))
                {
                    lvContextMenu.Show(Cursor.Position);
                }
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
            if (torrentFile.DeleteFromDb(DbConnectionString, deleteFile))
            {
                lvResults.Items.Remove(lvItem);
            }
        }



        //标记为已观看
        private void tsmiSetWatched_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;


            var formSetWatched = new FormSetWatched(torrentFile);
            if (formSetWatched.ShowDialog(this) == DialogResult.Cancel) return;
            lvItem.SubItems[3].Text = "0";
            lvItem.SubItems[4].Text = "1";
            lvItem.SubItems[5].Text = torrentFile.seedate;
            lvItem.SubItems[6].Text = torrentFile.seecomment;


            //自动备份
            BackupDbFile();

        }

        //标记稍后看
        private void tsmiSetSeelater_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;

            if (!torrentFile.MarkSeelater(DbConnectionString)) return;
            lvItem.SubItems[3].Text = "1";

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





        #endregion

       
    }
}
