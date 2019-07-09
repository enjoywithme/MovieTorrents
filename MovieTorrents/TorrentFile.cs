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
        public string genres { get; set; }
        public string posterpath { get; set; }
        public string doubanid { get; set; }

        public string FullName { get
            {
                return area + path + name + ext;
            } }


        public string RealPosterPath
        {
            get
            {
                if (string.IsNullOrEmpty(posterpath)) return null;
                return FormMain.CurrentPath + "\\poster\\douban\\" + Path.GetFileName(posterpath);
            }
        }

        private static Regex regex = new Regex(@"\d{4}");
        private static string[] PRE_CLEARS = { "WEB_DL", "DD5.1", "[0-9]*(?:\\.[0-9]*)?[GM]B?" };
        private static Regex BAD_CHARS = new Regex(@"[\[.\]()_-]");
        private static string[] BAD_WORDS = { "1080P", "720P", "x26[45]", "H26[45]", "BluRay", "AC3", " DTS ", "2Audios?", "FLAME", "IMAX",
            "中英字幕", "国英双语","国英音轨" };
        private static string[] RELEASE_GROUPS = { "SPARKS", "CMCT", "FGT", "KOOK" };
        public string PurifiedName
        {
            get
            {
                var cleaned = Regex.Replace(name, "\\b" + string.Join("\\b|\\b", PRE_CLEARS) + "\\b", "", RegexOptions.IgnoreCase);
                cleaned = BAD_CHARS.Replace(cleaned, " ");
                cleaned = Regex.Replace(cleaned, "\\b" + string.Join("\\b|\\b", BAD_WORDS) + "\\b", "", RegexOptions.IgnoreCase);
                cleaned = Regex.Replace(cleaned, "\\b" + string.Join("\\b|\\b", RELEASE_GROUPS) + "\\b", "", RegexOptions.IgnoreCase);
                return cleaned.Trim();
            }
        }
        public string ChineseName
        {
            get
            {
                var matches = Regex.Matches(PurifiedName, "[\u4e00-\u9fa5]+");
                return string.Join(" ", matches.OfType<Match>().Where(m=>m.Success));
            }
        }

        

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

        
        public void ShowInExplorer()
        {
            if (File.Exists(FullName))
            {
                Process.Start("explorer.exe", "/select, " + FullName);
            }
        }

        public bool SetWatched(string dbConnString, DateTime watchDate,string comment)
        {
            seedate =watchDate.ToString("yyyy-MM-dd");
            seecomment = comment;
            var m_dbConnection = new SQLiteConnection(dbConnString);

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
                    command.Parameters.AddWithValue("$fid", fid);
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

        public bool DeleteFromDb(string dbConnString)
        {
            var m_dbConnection = new SQLiteConnection(dbConnString);

            var sql = $"delete from tb_file where file_nid=$fid";
            var ok = true;
            try
            {
                m_dbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("$fid", fid);
                    ok = command.ExecuteNonQuery()>0;
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

        public bool Rename(string dbConnString,string newName,out string msg)
        {
            msg = string.Empty;

            var newFullName = area + path + newName + ext;
            if(File.Exists(newFullName))
            {
                msg = "要命名的新文件已经存在！";
                return false;
            }

            var m_dbConnection = new SQLiteConnection(dbConnString);
            var sql = @"update tb_file set name=$name where file_nid=$fid";
            var ok = true;
            try
            {
                
               
                m_dbConnection.Open();
                try
                {
                    File.Move(FullName, newFullName);

                    var command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("$name", newName);
                    command.Parameters.AddWithValue("$fid", fid);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                m_dbConnection.Close();



            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
            }

            if (ok)
            {
                name = newName;
            }


            return ok;
        }

        public static long CountWatched(string dbConnString)
        {
            var connection = new SQLiteConnection(dbConnString);
            var sql = $"select count(*) as watched from filelist_view where seeflag=1";
            long watched = 0;
            try
            {
                connection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, connection);
                    watched =(long) command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    watched = -1;
                }

                connection.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                watched = -1;
            }

            return watched;
        }

        public bool UpdateDoubanInfo(string dbConnString,DoubanSubject subject)
        {
            var posterImageFileName = string.Empty;

            var m_dbConnection = new SQLiteConnection(dbConnString);
            var sql = @"update tb_file set year=$year,keyname=$keyname,othername=$othername,doubanid=$doubanid,posterpath=$posterpath,rating=$rating,genres=$genres where file_nid=$fid";
            var ok = true;
            try
            {

                if (!string.IsNullOrEmpty(subject.img_local))
                {
                    posterImageFileName = FormMain.CurrentPath + "\\poster\\douban\\" + Path.GetFileName(subject.img_local);
                    File.Copy(subject.img_local, posterImageFileName,true);
                    posterImageFileName = posterImageFileName.Replace("\\", "/");//zogvm的路径使用正斜杠
                }

                m_dbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("$year", string.IsNullOrEmpty(subject.year)?year:subject.year);
                    command.Parameters.AddWithValue("$keyname", subject.name);
                    command.Parameters.AddWithValue("$othername", subject.othername);
                    command.Parameters.AddWithValue("$doubanid", subject.id);
                    command.Parameters.AddWithValue("$posterpath", posterImageFileName);
                    command.Parameters.AddWithValue("$rating", subject.Rating);
                    command.Parameters.AddWithValue("$genres", subject.genres);
                    command.Parameters.AddWithValue("$fid", fid);
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

            if(ok)
            {
                genres = subject.genres;
                posterpath = posterImageFileName;
                doubanid = subject.id;
                rating = subject.Rating;
            }


            return ok;
        }

        public void OpenDoubanLink()
        {
            if (string.IsNullOrEmpty(doubanid)) return;
            Process.Start($"https://movie.douban.com/subject/{doubanid}/");
        }
    }
}
