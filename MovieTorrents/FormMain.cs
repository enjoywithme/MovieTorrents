using System;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormMain : Form
    {
        private string _currentPath;
        private string _dbConnString;

        private CancellationTokenSource _quernyTokenSource;
        private CancellationTokenSource _operTokenSource;

        private string _torrentFilePath;
        private string TorrentFilePath
        {
            get {
                if (!string.IsNullOrEmpty(_torrentFilePath))
                    return _torrentFilePath;

                var m_dbConnection = new SQLiteConnection(_dbConnString);
                
                var sql = $"select area,path from tb_dir as d inner join tb_hdd as h on h.hdd_nid=d.hdd_nid  limit 1";
                try
                {
                    m_dbConnection.Open();
                    try
                    {
                        var command = new SQLiteCommand(sql, m_dbConnection);
                        var reader = command.ExecuteReader();
                        if(reader.Read())
                        {
                            _torrentFilePath = (string)reader["area"] + (string)reader["path"];
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    m_dbConnection.Close();

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return _torrentFilePath;
            }
        }

        private enum OperationType
        {
            None,
            QueryFile,
            ScanFile,
            ClearFile
        };

        private OperationType _currentOperation = OperationType.None;

        private long _fileScanned;


        public FormMain()
        {
            InitializeComponent();
            _currentPath = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location);
            _dbConnString = $"Data Source ={_currentPath}//zogvm.db; Version = 3; ";

        }

        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbSearchText.Text.Trim()))
            {
                lvResults.Items.Clear();
                return;
            }

            lock(this)
            {
                if(_currentOperation!=OperationType.None && _currentOperation!=OperationType.QueryFile)
                {
                    MessageBox.Show("正在执行其他操作，等待完成后操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            if(_quernyTokenSource!=null)
            {
                _quernyTokenSource.Cancel();
                _quernyTokenSource.Dispose();
                _quernyTokenSource = null;
            }

            UpdateCurrentOperation(OperationType.QueryFile);

            _quernyTokenSource = new CancellationTokenSource();
            Task.Run(() => ExecuteSearch(tbSearchText.Text.Trim(), _quernyTokenSource.Token))
                .ContinueWith(task =>
                {
                    if (task.IsFaulted) Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                    else if (task.IsCanceled) { }
                    else Invoke(new Action(() => updateListView(task.Result)));

                    Invoke(new Action(() => UpdateCurrentOperation()));
                });

        }

       
        private void updateListView(IEnumerable<TorrentFile> torrentFiles)
        {
            lvResults.BeginUpdate();
            lvResults.Items.Clear();
            foreach(var torrentFile in torrentFiles)
            {
                string[] row = { torrentFile.name,
                                torrentFile.rating.ToString(),
                                torrentFile.year,
                                torrentFile.seeflag.ToString(),
                                torrentFile.seedate
                            };
                lvResults.Items.Add(new ListViewItem(row)
                {
                    Tag = torrentFile.fid
                });
            }


            lvResults.EndUpdate();
            
        }

        public async Task<IEnumerable<TorrentFile>> ExecuteSearch(string text, CancellationToken cancelToken)
        {
            var result = new List<TorrentFile>();

            using (var m_dbConnection = new SQLiteConnection(_dbConnString))
            {
                var sql = $"select * from filelist_view where name like '%{text}%' order by rating desc";
                using (var command = new SQLiteCommand(sql, m_dbConnection))
                {
                    await m_dbConnection.OpenAsync(cancelToken);

                    using (var reader = await command.ExecuteReaderAsync(cancelToken))
                    {
                        while (await reader.ReadAsync(cancelToken))
                        {
                            result.Add(new TorrentFile
                            {
                                fid = (long)reader["file_nid"],
                                name = (string)reader["name"],
                                rating = (double)reader["rating"],
                                year = (string)reader["year"],
                                seeflag = (long)reader["seeflag"],
                                seedate = Convert.IsDBNull(reader["seedate"]) ? string.Empty : (string)reader["seedate"]
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

        private void tsmiSetWatched_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var lvItem = lvResults.SelectedItems[0];
            if(SetWatched(lvItem.Tag,out var seedate))
            {
                lvItem.SubItems[4].Text = seedate;
                lvItem.SubItems[3].Text = "1";
            }
           
        }

        private bool SetWatched(object fileid,out string seedate)
        {
            var m_dbConnection = new SQLiteConnection(_dbConnString);
            seedate = DateTime.Today.ToString("yyyy-MM-dd");
            var sql = $"update tb_file set seeflag=1,seedate='{seedate}' where file_nid=$fid";
            var ok = true;
            try
            {
                m_dbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("$fid", fileid);
                    command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ok = false;
                }

                m_dbConnection.Close();

            }catch(Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ok = false;
            }

            return ok;
        }

        //扫描种子文件
        private void ScanTorrentFile()
        {
            if(string.IsNullOrEmpty(TorrentFilePath))
            {
                MessageBox.Show("种子文件根目录没有配置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!Directory.Exists(TorrentFilePath))
            {
                MessageBox.Show($"种子文件根目录\"{TorrentFilePath}\"不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lock (this)
            {
                if (_currentOperation != OperationType.None)
                {
                    MessageBox.Show("正在执行其他操作，等待完成后操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if(_operTokenSource!=null)
            {
                _operTokenSource.Dispose();
                _operTokenSource = null;
            }
            _operTokenSource = new CancellationTokenSource();

            UpdateCurrentOperation(OperationType.ScanFile);

            Task.Run(() => DoScanFile(TorrentFilePath, _operTokenSource.Token)).ContinueWith(task =>
            {
                Invoke(new Action(() => {
                    UpdateCurrentOperation(OperationType.None,$"完成文件扫描，共{_fileScanned}个文件");
                }));
            });

        }

        private void UpdateCurrentOperation(OperationType operation= OperationType.None, string infoMsg = "")
        {
            lock (this)
            {
                _currentOperation = operation;
            }

            switch(_currentOperation)
            {
                case OperationType.None:
                    tssInfo.Text =string.IsNullOrEmpty(infoMsg)?"空闲":infoMsg;
                    tssState.BackColor = Color.LimeGreen;
                    break;
                case OperationType.QueryFile:
                    tssInfo.Text = string.IsNullOrEmpty(infoMsg) ? "查询文件中" : infoMsg;
                    tssState.BackColor = Color.OrangeRed;
                    break;
                case OperationType.ScanFile:
                    tssInfo.Text = string.IsNullOrEmpty(infoMsg) ? "扫描文件中" : infoMsg;
                    tssState.BackColor = Color.OrangeRed;
                    break;
            }
        }

        private void DoScanFile(string dirName,CancellationToken token)
        {
            _fileScanned = 0;
            var di = new DirectoryInfo(dirName);
            if (di != null && di.Exists)
            {
                foreach (FileInfo fi in di.EnumerateFiles("*.torrent", SearchOption.AllDirectories))
                {
                    if (token.IsCancellationRequested) return;

                    _fileScanned++;

                    Debug.WriteLine(fi.Extension);
                    Debug.WriteLine(fi.FullName);
                }
            }
        }

        private void tsmiScanFile_Click(object sender, EventArgs e)
        {
            ScanTorrentFile();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock(this)
            {
                if(_currentOperation!=OperationType.None)
                {
                    MessageBox.Show("尚有操作在进行中，不能退出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void tssInfo_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void tssState_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (_currentOperation == OperationType.None)
                    return;
            }

            if (MessageBox.Show("确定取消当前操作?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                return;

            if (_operTokenSource != null) _operTokenSource.Cancel();
        }
    }
}
