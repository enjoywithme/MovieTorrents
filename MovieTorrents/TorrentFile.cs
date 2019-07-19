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
        public string keyname { get; set; }
        public string name { get; set; }
        public string otherName { get; set; }
        public string ext { get; set; }
        public long filesize { get; set; }
        public double rating { get; set; }
        public string year { get; set; }
        public long seelater { get; set; }
        public long seeflag { get; set; }
        public string seedate { get; set; }
        public string seecomment { get; set; }
        public string genres { get; set; }
        public string posterpath { get; set; }
        public string doubanid { get; set; }

        public string FullName => area + path + name + ext;

        public DateTime SeeDate { get {
            var seeDate = DateTime.Today;
            if (!string.IsNullOrEmpty(seedate) && DateTime.TryParse(seedate, out var d))
                seeDate = d;
            return seeDate;
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

        public bool ToggleSeelater(string dbConnString,out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(dbConnString);

            var sql = $"update tb_file set seelater = case seelater when 1 then 0 else 1 end where file_nid=$fid";
            var ok = true;
            try
            {
                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", fid);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                mDbConnection.Close();

            }
            catch (Exception e)
            {
                msg=e.Message;
                ok = false;
            }

            return ok;
        }


        public bool SetWatched(string dbConnString, DateTime watchDate,string comment,out string msg)
        {
            msg = string.Empty;
            seedate =watchDate.ToString("yyyy-MM-dd");
            seecomment = comment;
            var mDbConnection = new SQLiteConnection(dbConnString);

            var sql = $"update tb_file set seelater=0,seeflag=1,seedate=$seedate,seecomment=$comment where file_nid=$fid";
            var ok = true;
            try
            {
                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$seedate", seedate);
                    command.Parameters.AddWithValue("$comment", comment);
                    command.Parameters.AddWithValue("$fid", fid);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                mDbConnection.Close();

            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
            }

            return ok;
        }

        public bool DeleteFromDb(string dbConnString,bool deleteFile,out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(dbConnString);

            var sql = $"delete from tb_file where file_nid=$fid";
            var ok = true;
            try
            {
                mDbConnection.Open();
                try
                {
                    if(deleteFile)
                        File.Delete(FullName);

                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", fid);
                    ok = command.ExecuteNonQuery()>0;
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                mDbConnection.Close();

            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
            }

            return ok;
        }

        public bool EditRecord(string dbConnString,string newName,string newYear,
            string newKeyName,string newOtherName,string newGenres,
            bool newWatched,DateTime newWatchDate,string newSeeComment,
            out string msg)
        {
            msg = string.Empty;
            var reNameFile = false;
            newName = newName.Trim();
            var newFullName = area + path + newName + ext;

            if (string.Compare(name, newName, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                reNameFile = true;
                if (File.Exists(newFullName))
                {
                    msg = "要命名的新文件已经存在！";
                    return false;
                }
            }

            var watchDate =newWatched?newWatchDate.ToString("yyyy-MM-dd"):"";
            var newSeelater = newWatched ? 0 : seelater;

            var mDbConnection = new SQLiteConnection(dbConnString);
            var sql = @"update tb_file set name=$name,year=$year,
keyname=$keyname,othername=$othername,genres=$genres,
seelater=$seelater,seeflag=$seeflag,seedate=$seedate,seecomment=$comment 
where file_nid=$fid";
            var ok = true;
            try
            {
                
               
                mDbConnection.Open();
                try
                {
                    if(reNameFile) File.Move(FullName, newFullName);

                    var command = new SQLiteCommand(sql, mDbConnection);
                     
                    command.Parameters.AddWithValue("$name", newName);
                    command.Parameters.AddWithValue("$year", newYear);
                    command.Parameters.AddWithValue("$keyname", newKeyName);
                    command.Parameters.AddWithValue("$othername", newOtherName);
                    command.Parameters.AddWithValue("$genres", newGenres);
                    command.Parameters.AddWithValue("seelater", newSeelater);
                    command.Parameters.AddWithValue("$seeflag", newWatched?1:0);
                    command.Parameters.AddWithValue("$seedate", watchDate);
                    command.Parameters.AddWithValue("$comment", newSeeComment);
                    command.Parameters.AddWithValue("$fid", fid);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                mDbConnection.Close();



            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
            }

            if (!ok) return false;

            name = newName;
            year = newYear;
            keyname = newKeyName;
            otherName = newOtherName;
            genres = newGenres;
            seelater = newSeelater;
            seeflag = newWatched ? 1 : 0;
            seedate = watchDate;
            seecomment = newSeeComment;

            return true;
        }

        public static long CountWatched(string dbConnString,out string msg)
        {
            msg = string.Empty;
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
                    msg=e.Message;
                    watched = -1;
                }

                connection.Close();

            }
            catch (Exception e)
            {
                msg=e.Message;
                watched = -1;
            }

            return watched;
        }

        public bool UpdateDoubanInfo(string dbConnString,DoubanSubject subject,out string msg)
        {
            msg = string.Empty;
            var posterImageFileName = string.Empty;

            var mDbConnection = new SQLiteConnection(dbConnString);
            var sql = @"update tb_file set year=$year,keyname=$keyname,othername=$othername,doubanid=$doubanid,posterpath=$posterpath,
rating=$rating,genres=$genres,directors=$directors,casts=$casts where file_nid=$fid";
            var ok = true;
            try
            {

                if (!string.IsNullOrEmpty(subject.img_local))
                {
                    posterImageFileName = FormMain.CurrentPath + "\\poster\\douban\\" + Path.GetFileName(subject.img_local);
                    File.Copy(subject.img_local, posterImageFileName,true);
                    posterImageFileName = posterImageFileName.Replace("\\", "/");//zogvm的路径使用正斜杠
                }

                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$year", string.IsNullOrEmpty(subject.year)?year:subject.year);
                    command.Parameters.AddWithValue("$keyname", subject.name);
                    command.Parameters.AddWithValue("$othername", string.IsNullOrEmpty(subject.othername)?otherName:subject.othername);
                    command.Parameters.AddWithValue("$doubanid", subject.id);
                    command.Parameters.AddWithValue("$posterpath", posterImageFileName);
                    command.Parameters.AddWithValue("$rating", subject.Rating);
                    command.Parameters.AddWithValue("$genres", subject.genres);
                    command.Parameters.AddWithValue("$directors",subject.directors);
                    command.Parameters.AddWithValue("$casts", subject.casts);
                    command.Parameters.AddWithValue("$fid", fid);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                mDbConnection.Close();



            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
            }

            if (!ok) return false;
            genres = subject.genres;
            keyname = subject.name;
            otherName = string.IsNullOrEmpty(subject.othername) ? otherName : subject.othername;
            year = string.IsNullOrEmpty(subject.year) ? year : subject.year;
            posterpath = posterImageFileName;
            doubanid = subject.id;
            rating = subject.Rating;


            return true;
        }

        public static bool CopyDoubanInfo(string dbConnString,long sFid, List<long> dFids,out string msg)
        {
            msg = string.Empty;

            if (dFids.Contains(sFid)) dFids.Remove(sFid);
            var sFids = string.Join(",", dFids.Select(x => x.ToString()));


            var mDbConnection = new SQLiteConnection(dbConnString);
            var sql = $@"update tb_file set
year=(select year from tb_file where file_nid=$fid),
keyname=(select keyname from tb_file where file_nid=$fid),
othername=(select othername from tb_file where file_nid=$fid),
posterpath=(select posterpath from tb_file where file_nid=$fid),
doubanid=(select doubanid from tb_file where file_nid=$fid),
rating=(select rating from tb_file where file_nid=$fid),
casts=(select casts from tb_file where file_nid=$fid),
directors=(select directors from tb_file where file_nid=$fid),
genres=(select genres from tb_file where file_nid=$fid)
where file_nid in ({sFids})";
            var ok = true;
            try
            {

                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", sFid);
                    ok = command.ExecuteNonQuery()>0;
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }

                mDbConnection.Close();



            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
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
