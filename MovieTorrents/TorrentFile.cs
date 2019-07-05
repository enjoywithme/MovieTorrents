using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public class TorrentFile
    {
        public long fid { get; set;}
        public byte hdd_nid { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public string ext { get; set; }
        public double rating { get; set; }
        public string year { get; set; }
        public long seeflag { get; set; }
        public string seedate { get; set; }

        public static bool SetWatched(string dbConnString, object fileid, out string seedate)
        {
            var m_dbConnection = new SQLiteConnection(dbConnString);
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
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ok = false;
                }

                m_dbConnection.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ok = false;
            }

            return ok;
        }


    }
}
