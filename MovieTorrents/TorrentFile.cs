using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MovieTorrents.Common;
using mySharedLib;

namespace MovieTorrents
{
    public class TorrentFile
    {
        public long fid { get; set; }
        public byte hdd_nid { get; set; }
        public string area { get; set; }
        public string path { get; set; }
        public string keyname { get; set; }

        private string _name;
        public string name
        {
            get=>_name;
            set
            {
                _name = value;
                PurifiedName = _name.Purify();
                FirstName = PurifiedName.ExtractFirstToken();
            }
        }

        public string otherName { get; set; }
        public string ext { get; set; }
        public long filesize { get; set; }
        public double rating { get; set; }
        public string year { get; set; }
        public long seelater { get; set; }
        public long seeflag { get; set; }
        public long seenowant { get; set; }
        public string seedate { get; set; }
        public string seecomment { get; set; }
        public string genres { get; set; }
        public string zone { get; set; }
        public string posterpath { get; set; }
        public string doubanid { get; set; }

        public string FullName => area + path + name + ext;

        public string PurifiedName { get; set; }

        public string FirstName { get; set; }

        public DateTime SeeDate
        {
            get
            {
                var seeDate = DateTime.Today;
                if (!string.IsNullOrEmpty(seedate) && DateTime.TryParse(seedate, out var d))
                    seeDate = d;
                return seeDate;
            }
        }

        public string RealPosterPath
        {
            get
            {
                if (string.IsNullOrEmpty(posterpath)) return null;
                return FormMain.CurrentPath + "\\poster\\douban\\" + Path.GetFileName(posterpath);
            }
        }

        public Color ForeColor => string.IsNullOrWhiteSpace(doubanid) ? Color.Firebrick : Color.Black;

        public static TorrentFile FromFullPath(string fullName)
        {
            var torrentFile = new TorrentFile {name = Path.GetFileNameWithoutExtension(fullName)};
            var dir = Path.GetDirectoryName(fullName);
            torrentFile.path = dir.Substring(Path.GetPathRoot(dir).Length) + "\\";
            torrentFile.ext = Path.GetExtension(fullName);
            torrentFile.filesize = new FileInfo(fullName).Length;

            var yr = torrentFile.name.ExtractYear();
            torrentFile.year = yr == 0 ? "" : yr.ToString();
            return torrentFile;
        }


        public void ShowInExplorer()
        {
            if (File.Exists(FullName))
            {
                Process.Start("explorer.exe", "/select, " + FullName);
            }
        }


        private static SQLiteCommand BuildSearchCommand(string searchText,string selectQuery="")
        {
            var command = new SQLiteCommand();

            var sb = new StringBuilder();
            sb.Append(string.IsNullOrEmpty(selectQuery) ? "select * from filelist_view where 1=1" : selectQuery);

            //处理搜索关键词
            if (!string.IsNullOrEmpty(searchText))
            {
                var splits = searchText.Split(null);

                for (var i = 0; i < splits.Length; i++)
                {
                    var pName = $"@p{command.Parameters.Count}";
                    command.Parameters.AddWithValue(pName, $"%{splits[i]}%");
                    sb.Append($" and (name like {pName} or keyname like{pName} or othername like {pName} or genres like {pName} or seecomment like {pName})");

                }
            }


            //Debug.WriteLine(sb.ToString());

            command.CommandText = sb.ToString();
            return command;
        }
        //在数据库中搜索
        public static IEnumerable<TorrentFile> Search(string dbConnString, string text, out string msg)
        {
            var result = new List<TorrentFile>();
            var ok = true;
            msg = "";
            if (string.IsNullOrEmpty(text) || text.Length < 2)
            {
                msg = "搜索的内容太少！";
                return null;
            }


            try
            {
                var connection = new SQLiteConnection(dbConnString);
                connection.Open();

                try
                {
                    var command = BuildSearchCommand(text);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new TorrentFile
                            {
                                fid = (long)reader["file_nid"],
                                area = FormMain.Area,
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
                catch (Exception e)
                {
                    msg = e.Message;
                    ok = false;
                }
                connection.Close();
            }
            catch (Exception e)
            {
                msg = e.Message;
                ok = false;
            }



            return ok?result:null;
        }
        //检查数据库是否存在
        public static bool ExistInDb(string dbConnString, string text,int? year=null)
        {
            if (string.IsNullOrEmpty(text)) return false;

            var ok = true;
            long result = 0;
            try
            {
                var connection = new SQLiteConnection(dbConnString);
                
                connection.Open();

                try
                {
                    var sql =
                        "select count(*) from filelist_view where (keyname like $pName or othername like $pName)";
                    if (year != null)
                        sql += $" and year={year.Value}";

                    var command = new SQLiteCommand(sql,connection);
                    command.Parameters.AddWithValue("$pName", $"%{text}%");

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = (long) reader[0];
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    ok = false;
                }
                connection.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                ok = false;
            }


            return ok && result>0;
        }


        //返回归档年份子目录
        public static string ArchiveYearSubPath(int year)
        {
            if (year >= 1910 && year <= 1930) return "1910-1930";
            if (year >= 1931 && year <= 1950) return "1931-1950";
            if (year >= 1951 && year <= 1960) return "1951-1960";
            if (year >= 1961 && year <= 1970) return "1961-1970";
            if (year >= 1971 && year <= 1980) return "1971-1980";
            if (year >= 1981 && year <= 1990) return "1981-1990";
            if (year >= 1991 && year <= 2000) return "1991-2000";
            if (year >= 2001 && year <= 2010) return "2001-2010";

            return year.ToString();

        }

        public bool ToggleSeeLater(string dbConnString, out string msg)
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
                msg = e.Message;
                ok = false;
            }

            return ok;
        }

        public bool ToggleSeeNoWant(string dbConnString, out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(dbConnString);

            var sql = $"update tb_file set seenowant = case seenowant when 1 then 0 else 1 end where file_nid=$fid";
            var ok = true;
            try
            {
                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", fid);
                    command.ExecuteNonQuery();

                    seenowant = seenowant == 1 ? 0 : 1;
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


        public bool SetWatched(string dbConnString, DateTime watchDate, string comment, out string msg)
        {
            msg = string.Empty;
            seedate = watchDate.ToString("yyyy-MM-dd");
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

        public bool MoveTo(string dbConnString, string destFolder, out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(dbConnString);

            var sql = $"update tb_file set path=$path where file_nid=$fid";
            bool ok;
            var newPath = destFolder.Substring(Path.GetPathRoot(destFolder).Length) + "\\";

            try
            {
                mDbConnection.Open();
                try
                {

                    var destFullName = destFolder + "\\" + name + ext;
                    File.Move(FullName, destFullName);

                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$path", newPath);
                    command.Parameters.AddWithValue("$fid", fid);
                    ok = command.ExecuteNonQuery() > 0;
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

            if (ok) path = newPath;

            return ok;
        }

        public bool DeleteFromDb(string dbConnString, bool deleteFile, out string msg)
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
                    if (deleteFile)
                        File.Delete(FullName);

                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", fid);
                    ok = command.ExecuteNonQuery() > 0;
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

        public bool EditRecord(string dbConnString, string newName, string newYear, string newZone,
            string newKeyName, string newOtherName, string newGenres,
            bool newWatched, DateTime newWatchDate, string newSeeComment,
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

            var watchDate = newWatched ? newWatchDate.ToString("yyyy-MM-dd") : "";
            var newSeelater = newWatched ? 0 : seelater;

            var mDbConnection = new SQLiteConnection(dbConnString);
            var sql = @"update tb_file set name=$name,year=$year,zone=$zone,
keyname=$keyname,othername=$othername,genres=$genres,
seelater=$seelater,seeflag=$seeflag,seedate=$seedate,seecomment=$comment 
where file_nid=$fid";
            var ok = true;
            try
            {


                mDbConnection.Open();
                try
                {
                    if (reNameFile) File.Move(FullName, newFullName);

                    var command = new SQLiteCommand(sql, mDbConnection);

                    command.Parameters.AddWithValue("$name", newName);
                    command.Parameters.AddWithValue("$year", newYear);
                    command.Parameters.AddWithValue("$zone", newZone);
                    command.Parameters.AddWithValue("$keyname", newKeyName);
                    command.Parameters.AddWithValue("$othername", newOtherName);
                    command.Parameters.AddWithValue("$genres", newGenres);
                    command.Parameters.AddWithValue("seelater", newSeelater);
                    command.Parameters.AddWithValue("$seeflag", newWatched ? 1 : 0);
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
            zone = newZone;
            keyname = newKeyName;
            otherName = newOtherName;
            genres = newGenres;
            seelater = newSeelater;
            seeflag = newWatched ? 1 : 0;
            seedate = watchDate;
            seecomment = newSeeComment;

            return true;
        }

        public static long CountWatched(string dbConnString, out string msg)
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
                    watched = (long)command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    watched = -1;
                }

                connection.Close();

            }
            catch (Exception e)
            {
                msg = e.Message;
                watched = -1;
            }

            return watched;
        }

        public static string CountStatistics(string dbConnString, out string msg)
        {
            msg = string.Empty;
            var sb = new StringBuilder();
            var connection = new SQLiteConnection(dbConnString);
            var sql = $"select count(*) as watched from filelist_view where seeflag=1";
            try
            {
                connection.Open();
                try
                {
                    var command = new SQLiteCommand("select count(*) as total from filelist_view", connection);
                    sb.AppendLine($"总共 {command.ExecuteScalar()} 种子文件");

                    command = new SQLiteCommand("select count(*) from (select DISTINCT doubanid from filelist_view where doubanid<>'' and doubanid<>'0' and doubanid is not null)", connection);
                    sb.AppendLine($"已知电影数 {command.ExecuteScalar()}（有豆瓣编号）");

                    command = new SQLiteCommand("select count(*) as watched from filelist_view where seeflag=1", connection);
                    sb.AppendLine($"已看 {command.ExecuteScalar()}");

                    sb.AppendLine("过去5年观看数：");

                    var thisYear = DateTime.Now.Year;
                    command = new SQLiteCommand("select count(*) from filelist_view where strftime('%Y', seedate)=@year ", connection);
                    command.Parameters.Add("@year", DbType.String, 10);
                    command.Prepare();
                    for (var i = 0; i < 5; i++)
                    {
                        command.Parameters["@year"].Value = (thisYear - i).ToString();
                        sb.AppendLine($"{ thisYear - i} / {command.ExecuteScalar()}");
                    }
                }
                catch (Exception e)
                {
                    msg = e.Message;
                }

                connection.Close();

            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return sb.ToString();
        }

        public bool UpdateDoubanInfo(string dbConnString, DoubanSubject subject, out string msg)
        {
            msg = string.Empty;
            var posterImageFileName = string.Empty;

            var mDbConnection = new SQLiteConnection(dbConnString);
            var sql = @"update tb_file set year=$year,zone=$zone,keyname=$keyname,othername=$othername,doubanid=$doubanid,posterpath=$posterpath,
rating=$rating,genres=$genres,directors=$directors,casts=$casts where file_nid=$fid";
            var ok = true;
            try
            {

                if (!string.IsNullOrEmpty(subject.img_local))
                {
                    posterImageFileName = FormMain.CurrentPath + "\\poster\\douban\\" + Path.GetFileName(subject.img_local);
                    File.Copy(subject.img_local, posterImageFileName, true);
                    posterImageFileName = posterImageFileName.Replace("\\", "/");//zogvm的路径使用正斜杠
                }

                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$year", string.IsNullOrEmpty(subject.year) ? year : subject.year);
                    command.Parameters.AddWithValue("$zone", subject.zone);
                    command.Parameters.AddWithValue("$keyname", subject.name);
                    command.Parameters.AddWithValue("$othername", string.IsNullOrEmpty(subject.othername) ? otherName : subject.othername);
                    command.Parameters.AddWithValue("$doubanid", subject.id);
                    command.Parameters.AddWithValue("$posterpath", posterImageFileName);
                    command.Parameters.AddWithValue("$rating", subject.Rating);
                    command.Parameters.AddWithValue("$genres", subject.genres);
                    command.Parameters.AddWithValue("$directors", subject.directors);
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
            zone = string.IsNullOrEmpty(subject.zone) ? zone : subject.zone;
            posterpath = posterImageFileName;
            doubanid = subject.id;
            rating = subject.Rating;


            return true;
        }

        public static bool CopyDoubanInfo(string dbConnString, long sFid, List<long> dFids, out string msg)
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
genres=(select genres from tb_file where file_nid=$fid),
zone=(select zone from tb_file where file_nid=$fid)
where file_nid in ({sFids})";
            var ok = true;
            try
            {

                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", sFid);
                    ok = command.ExecuteNonQuery() > 0;
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
