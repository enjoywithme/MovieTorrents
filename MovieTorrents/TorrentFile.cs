using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public class TorrentFile
    {
        public long fid { get; set;}
        public byte hdd_nid { get; set; }
        public string area { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public string ext { get; set; }
        public long filesize { get; set; }
        public double rating { get; set; }
        public string year { get; set; }
        public long seeflag { get; set; }
        public string seedate { get; set; }
        public string seecomment { get; set; }

        public string FullName { get
            {
                return area + path + name + ext;
            } }

        private static Regex regex = new Regex(@"\d{4}");

        public TorrentFile()
        {
            
        }

        public TorrentFile(string fullName)
        {
            name = Path.GetFileNameWithoutExtension(fullName);
            var dir = Path.GetDirectoryName(fullName);
            path = dir.Substring(Path.GetPathRoot(dir).Length)+"\\";
            ext = Path.GetExtension(fullName);
            filesize = new FileInfo(fullName).Length;

            year = string.Empty;
            var match = regex.Match(name);
            if (match.Success) year = match.Value;
        }

        public static bool SetWatched(string dbConnString, object fileid, DateTime watchDate,string comment, out string seedate)
        {
            var m_dbConnection = new SQLiteConnection(dbConnString);
            seedate =watchDate.ToString("yyyy-MM-dd");
            var sql = $"update tb_file set seeflag=1,seedate=$seedate,seecomment=$comment where file_nid=$fid";
            var ok = true;
            try
            {
                m_dbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("$seedate", seedate);
                    command.Parameters.AddWithValue("$comment", comment);
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
