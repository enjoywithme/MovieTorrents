using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormMain : Form
    {
        public static string CurrentPath;
        public static string DbConnection;

        private CancellationTokenSource _quernyTokenSource;
        private CancellationTokenSource _operTokenSource;

        private string _torrentFilePath;
        private string TorrentFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_torrentFilePath))
                    return _torrentFilePath;

                using (var connection = new SQLiteConnection(DbConnection))
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
                                        _torrentFilePath = _area + _shortRootPath;
                                    }
                                }

                            }

                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        connection.Close();

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


                return _torrentFilePath;
            }
        }

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


        private byte _hdd_nid;
        private string _area;
        private string _shortRootPath;

        private FileSystemWatcher watcher = new FileSystemWatcher();


        public FormMain()
        {
            InitializeComponent();
            CurrentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DbConnection = $"Data Source ={CurrentPath}//zogvm.db; Version = 3; ";


        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TorrentFilePath) && Directory.Exists(TorrentFilePath))
            {
                watcher.Path = TorrentFilePath;
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.Filter = "*.torrent";
                watcher.Created += Watcher_File_Created;
                watcher.EnableRaisingEvents = true;
            }

            //var d1 = DateTime.MinValue;
            //var d2 = DateTime.Parse("2018-09-10 14:05:04");
            //Debug.WriteLine((d2 - d1).TotalMilliseconds / 1000);
#if DEBUG
            tbSearchText.Text = "雷神";
#endif
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
                if (task.IsFaulted)
                {
                    Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                }
            }); ;
        }

        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            SearchRecords(tbSearchText.Text.Trim());
        }

        private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) SearchRecords(tbSearchText.Text);
        }


        private void SearchRecords(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                lvResults.Items.Clear();
                return;
            }


            if (Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationNone) != OperationNone
                && Interlocked.CompareExchange(ref _currentOperation, OperationQueryFile, OperationQueryFile) != OperationQueryFile)

            {
                MessageBox.Show("正在执行其他操作，等待完成后操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            Task.Run(() => ExecuteSearch(tbSearchText.Text.Trim(), _quernyTokenSource.Token))
                .ContinueWith(task =>
                {
                    if (task.IsFaulted) Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                    else if (task.IsCanceled) { }
                    else Invoke(new Action(() => updateListView(task.Result)));

                    Interlocked.Exchange(ref _currentOperation, OperationNone);
                    DisplayInfo();
                });

        }

        private void updateListView(IEnumerable<TorrentFile> torrentFiles)
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
                                torrentFile.seeflag.ToString(),
                                torrentFile.seedate,
                                torrentFile.seecomment,
                                torrentFile.path
                            };
                lvResults.Items.Add(new ListViewItem(row)
                {
                    Tag = torrentFile
                });

                //Debug.Print(torrentFile.GetPurifiedChineseName());
            }


            lvResults.EndUpdate();

        }

        public async Task<IEnumerable<TorrentFile>> ExecuteSearch(string text, CancellationToken cancelToken)
        {
            var result = new List<TorrentFile>();

            using (var connection = new SQLiteConnection(DbConnection))
            {
                var splits = text.Split(null);
                var sb = new StringBuilder("select * from filelist_view where ");

                for (var i = 0; i < splits.Length; i++)
                {
                    if (i > 0) sb.Append(" and ");
                    sb.Append($"(name like '%{splits[i]}%' or othername like '%{splits[i]}%' or genres like '%{splits[i]}%')");
                }
                sb.Append(" order by rating desc");

                Debug.WriteLine(sb.ToString());

                using (var command = new SQLiteCommand(sb.ToString(), connection))
                {
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
                                ext =(string)reader["ext"],
                                rating = (double)reader["rating"],
                                year = (string)reader["year"],
                                seeflag = (long)reader["seeflag"],
                                posterpath =(string)reader["posterpath"],
                                genres = (string)reader["genres"],
                                doubanid =(string)reader["doubanid"],
                                seedate = Convert.IsDBNull(reader["seedate"]) ? string.Empty : (string)reader["seedate"],
                                seecomment = Convert.IsDBNull(reader["seecomment"]) ? string.Empty : (string)reader["seecomment"]
                            });

                        }
                    }
                }
            }

            return result;
        }


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

        //标记为已观看
        private void tsmiSetWatched_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;


            var formSetWatched = new FormSetWatched(torrentFile);
            if (formSetWatched.ShowDialog(this) == DialogResult.Cancel) return;
            lvItem.SubItems[3].Text = "1";
            lvItem.SubItems[4].Text = torrentFile.seedate;
            lvItem.SubItems[5].Text = torrentFile.seecomment;


            //自动备份
            BackupDbFile();

        }

        //备份数据库文件
        private void BackupDbFile()
        {
            var watched = TorrentFile.CountWatched(DbConnection);
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

        //扫描种子文件
        private void ScanTorrentFile()
        {
            if (string.IsNullOrEmpty(TorrentFilePath))
            {
                MessageBox.Show("种子文件根目录没有配置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(TorrentFilePath))
            {
                MessageBox.Show($"种子文件根目录\"{TorrentFilePath}\"不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_currentOperation != 0)
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
                    Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                }
            });

        }
        //添加扫描到的种子到队列等待处理
        private void DoScanFile(string dirName, CancellationToken token, BlockingCollection<TorrentFile> _filesToProcess)
        {
            var _fileScanned = 0;

            var di = new DirectoryInfo(dirName);
            if (di == null || !di.Exists) return;

            var stopwatch = Stopwatch.StartNew();

            foreach (FileInfo fi in di.EnumerateFiles("*.torrent", SearchOption.AllDirectories))
            {
                if (token.IsCancellationRequested) break;

                _fileScanned++;

                //Debug.WriteLine(fi.Extension);

                _filesToProcess.Add(new TorrentFile(fi.FullName));

            }

            _filesToProcess.CompleteAdding();

            stopwatch.Stop();
            DisplayInfo($"完成文件磁盘扫描，共{_fileScanned}个文件，耗时{stopwatch.Elapsed.Minutes}分{stopwatch.Elapsed.Seconds}秒。");



        }
        //启动文件队列的处理

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

            using (var connection = new SQLiteConnection(DbConnection))
            {
                connection.Open();

                using (var commandInsert = new SQLiteCommand($@"insert into tb_file(hdd_nid,path,name,ext,year,filesize,CreationTime,LastWriteTime,LastOpenTime)
select {_hdd_nid},$path,$name,$ext,$year,$filesize,$n,$n,$n
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
                        if (token != null && token.IsCancellationRequested) break;

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

        private void DisplayInfo(string infoMsg = "空闲")
        {
            Invoke(new Action(() =>
            {
                tssInfo.Text = infoMsg;
                tssState.BackColor = _currentOperation == OperationNone ? Color.LimeGreen : Color.OrangeRed;
                if (notifyIcon1.Visible)
                    notifyIcon1.ShowBalloonTip(2000, "Movie torrents", infoMsg, ToolTipIcon.Info);
            }));


        }


        private void tsmiScanFile_Click(object sender, EventArgs e)
        {
            ScanTorrentFile();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (_currentOperation != OperationNone)
            {
                MessageBox.Show("尚有操作在进行中，不能退出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void tssState_Click(object sender, EventArgs e)
        {

            if (_currentOperation == OperationNone)
                return;

            if (MessageBox.Show("确定取消当前操作?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                return;

            if (_operTokenSource != null) _operTokenSource.Cancel();
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

        private async void DoClearFile(CancellationToken cancelToken)
        {
            var filesCleard = 0;
            var filesCount = 0;

            var fileIdToClear = new List<long>();

            using (var connection = new SQLiteConnection(DbConnection))
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

        private void tsmiCopyName_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            Clipboard.SetText(lvItem.Text);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        private void tsmiShowFileLocation_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;

            var torrentFile  = (TorrentFile) lvResults.SelectedItems[0].Tag;
            torrentFile.ShowInExplorer();
        }

        private void tsmiSearchDouban_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;

            var lvItem = lvResults.SelectedItems[0];
            var torrentFile = (TorrentFile)lvItem.Tag;
            var formSearchDouban = new FormSearchDouban(torrentFile);
            if (formSearchDouban.ShowDialog() == DialogResult.Cancel) return;

            lvItem.SubItems[1].Text = formSearchDouban.DoubanSubject.rating;
            lvItem.SubItems[2].Text = formSearchDouban.DoubanSubject.year;
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbGenres.Text = null;
            if(pictureBox1.Image!=null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            if (lvResults.SelectedItems.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedItems[0].Tag;
            lbGenres.Text = torrentFile.genres;
            if (!string.IsNullOrEmpty(torrentFile.RealPosterPath) && File.Exists(torrentFile.RealPosterPath))
            {
                using (var stream = new FileStream(torrentFile.RealPosterPath, FileMode.Open, FileAccess.Read))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var torrentFile = (TorrentFile)lvResults.SelectedItems[0].Tag;
            torrentFile.OpenDoubanLink();
        }
    }
}
