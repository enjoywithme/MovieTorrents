using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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
        private CancellationTokenSource _tokenSource;

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

            if(_tokenSource!=null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
            }

            _tokenSource = new CancellationTokenSource();
            Task.Run(() => ExecuteSearch(tbSearchText.Text.Trim(), _tokenSource.Token))
                .ContinueWith(task =>
                {
                    if (task.IsFaulted) Invoke(new Action(() => MessageBox.Show(task.Exception.InnerException.Message)));
                    else if (task.IsCanceled) { }
                    else Invoke(new Action(() => updateListView(task.Result)));
                });

        }

       
        private void updateListView(IEnumerable<TorrentFile> torrentFiles)
        {
            lvResults.BeginUpdate();
            lvResults.Items.Clear();
            foreach(var torrentFile in torrentFiles)
            {
                if (_tokenSource != null && _tokenSource.IsCancellationRequested) return;

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
    }
}
