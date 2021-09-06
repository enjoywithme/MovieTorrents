using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MovieTorrents.Common;
using mySharedLib;

namespace MovieTorrents
{

    public class TorrentFilter
    {
        public int RecordsLimit { get; set; } = 100;
        public bool Recent { get; set; }
        public bool HideSameSubject { get; set; } = true;
        public bool NotWatched { get; set; }
        public bool Watched { get; set; }
        public bool NotWant { get; set; }
        public bool SeeLater { get; set; }

        public bool? OrderRatingDesc { get; set; } = true;
        public bool? OrderYearDesc { get; set; }

        private readonly List<string> _filterFields = new List<string> { "rating", "year" };

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

            if (string.IsNullOrEmpty(greaterLess))
                greaterLess = "=";
            else switch (greaterLess)
            {
                case ">":
                    greaterLess = ">=";
                    break;
                case "<":
                    greaterLess = "<=";
                    break;
            }
            var pName = $"@p{command.Parameters.Count}";
            sb.Append($" and {fldName}{greaterLess}{pName}");
            command.Parameters.AddWithValue(pName, oValue);
            return true;
        }
        public SQLiteCommand BuildSearchCommand(string text,bool withFilter)
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

            if (!withFilter)
            {
                command.CommandText = sb.ToString();
                return command;
            }

            //过滤
            if (SeeLater) sb.Append(" and seelater=1");
            if (Watched && !NotWatched) sb.Append(" and seeflag=1");
            if (NotWatched && !Watched)
            {
                sb.Append(" and seeflag=0");
                if (HideSameSubject)
                    sb.Append(" and  doubanid not in(select DISTINCT doubanid from tb_file where seeflag=1 and doubanid<>'')");
            }

            if (NotWatched)
            {
                sb.Append(" and seenowant=0");
                if (HideSameSubject)
                    sb.Append(" and  doubanid not in(select DISTINCT doubanid from tb_file where seenowant=1 and doubanid<>'')");
            }

            //限制条数
            var limitClause = $" limit {RecordsLimit}";

            //最近观看特殊处理
            if (Recent)
            {
                sb.Insert(0, "select * from (");
                sb.Append(" order by CreationTime desc");
                sb.Append(limitClause);
                sb.Append(")");
            }

            //排序
            var ordered = false;
            if (OrderRatingDesc.HasValue && OrderRatingDesc.Value)
            {
                sb.Append(" order by rating desc");
                ordered = true;
            }
            else if (OrderRatingDesc.HasValue && !OrderRatingDesc.Value)
            {
                sb.Append(" order by rating asc");
                ordered = true;
            }

            if (OrderYearDesc.HasValue && OrderYearDesc.Value)
                sb.Append(ordered ? ",year desc" : " order by year desc");
            else if (OrderYearDesc.HasValue && !OrderYearDesc.Value)
                sb.Append(ordered ? ",year asc" : " order by year asc");

            if (!Recent)
                sb.Append(limitClause);



            Debug.WriteLine(sb.ToString());

            command.CommandText = sb.ToString();
            return command;
        }
    }

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
            get => _name;
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
                return CurrentPath + "\\poster\\douban\\" + Path.GetFileName(posterpath);
            }
        }

        public Color ForeColor => string.IsNullOrWhiteSpace(doubanid) ? Color.Firebrick : Color.Black;


        public static TorrentFile FromFullPath(string fullName)
        {
            var torrentFile = new TorrentFile { name = Path.GetFileNameWithoutExtension(fullName) };
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

        #region 静态变量/函数
        public static TorrentFilter Filter { get; } = new TorrentFilter();
        private static byte _hdd_nid;
        public static string Area;
        private static string _shortRootPath;
        public static string CurrentPath;
        public static string TorrentFilePath { get; set; }
        public static string DbConnectionString;
        public static bool CheckTorrentPath(out string msg)
        {
            var ok = true;
            msg = string.Empty;


            CurrentPath = Utility.ExecutingAssemblyPath();
            if (!File.Exists($"{CurrentPath}\\zogvm.db"))
            {
                msg = $"数据库文件不存在！\r\n{CurrentPath}\\zogvm.db";
                return false;
            }


            DbConnectionString = $"Data Source ={CurrentPath}//zogvm.db; Version = 3; ";

            using var connection = new SQLiteConnection(DbConnectionString);
            var sql = $"select d.hdd_nid,area,path from tb_dir as d inner join tb_hdd as h on h.hdd_nid=d.hdd_nid  limit 1";
            try
            {
                connection.Open();
                try
                {
                    using var command = new SQLiteCommand(sql, connection);
                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        _hdd_nid = (byte)reader.GetByte(0);// ["hdd_nid"];
                        Area = (string)reader["area"];
                        _shortRootPath = (string)reader["path"];
                        TorrentFile.TorrentFilePath = Area + _shortRootPath;
                    }
                }
                catch (Exception e)
                {
                    ok = false;
                    msg = $"查找种子保存目录失败：{e.Message}";
                }

                connection.Close();

            }
            catch (Exception e)
            {
                ok = false;
                msg = $"查找种子保存目录失败：{e.Message}";
            }

            return ok;

        }

        //执行异步搜索
        public static async Task<(IEnumerable<TorrentFile>,string msg)> ExecuteSearch(string searchText, CancellationToken cancelToken, bool withFilter=true)
        {
            var result = new List<TorrentFile>();
            var msg = string.Empty;

            try
            {
                using var connection = new SQLiteConnection(DbConnectionString);
                using var command = Filter.BuildSearchCommand(searchText, withFilter);
                command.Connection = connection;

                await connection.OpenAsync(cancelToken);

                using var reader = await command.ExecuteReaderAsync(cancelToken);
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
            catch (Exception e)
            {
                msg = e.Message;
            }

            

            return (result,msg);
        }

        //在数据库中搜索
        public static IEnumerable<TorrentFile> Search(string text, out string msg)
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
                var connection = new SQLiteConnection(DbConnectionString);
                connection.Open();

                try
                {
                    var command = Filter.BuildSearchCommand(text,false);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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



            return ok ? result : null;
        }
        //检查数据库是否存在
        public static bool ExistInDb(string text, int? year = null)
        {
            if (string.IsNullOrEmpty(text)) return false;

            var ok = true;
            long result = 0;
            try
            {
                var connection = new SQLiteConnection(DbConnectionString);

                connection.Open();

                try
                {
                    var sql =
                        "select count(*) from filelist_view where (keyname like $pName or othername like $pName)";
                    if (year != null)
                        sql += $" and year={year.Value}";

                    var command = new SQLiteCommand(sql, connection);
                    command.Parameters.AddWithValue("$pName", $"%{text}%");

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = (long)reader[0];
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


            return ok && result > 0;
        }
        //备份数据库文件
        public static bool BackupDbFile(out string msg)
        {
            var watched = TorrentFile.CountWatched(DbConnectionString, out msg);
            if (!string.IsNullOrEmpty(msg)) return false;
            if (watched == -1) return false;

            try
            {
                File.Copy($"{ CurrentPath}\\zogvm.db", "E:\\MyWinDoc\\My Movies\\zogvm.db", true);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }


            msg = $"备份--OK\r\n观看了{watched}个";

            return true;
        }
        //添加文件
        public static (int processed, int added) InsertToDb(BlockingCollection<TorrentFile> filesToProcess)
        {
            var fileProcessed = 0;
            var fileAdded = 0;
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

                    while (!filesToProcess.IsCompleted)
                    {
                        TorrentFile torrentFile = null;
                        try
                        {
                            torrentFile = filesToProcess.Take();
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

                    filesToProcess.Dispose();
                }

            }

            return (fileProcessed, fileAdded);
        }
        //清理文件
        public static async Task<(int counterRead, int counterCleared, string msg, bool error)> DoClearFile(CancellationToken cancelToken)
        {

            var counterCleared = 0;
            var counterRead = 0;
            var msg = string.Empty;
            var error = false;

            var fileIdToClear = new List<long>();

            try
            {
                using var connection = new SQLiteConnection(DbConnectionString);

                await connection.OpenAsync(cancelToken);
                using var commandRead =
                    new SQLiteCommand(
                        "select file_nid,h.area || f.path || f.name || f.ext as fullname from tb_file as f INNER join tb_hdd as h on h.hdd_nid=f.hdd_nid",
                        connection);
                using var reader = await commandRead.ExecuteReaderAsync(cancelToken);
                while (await reader.ReadAsync(cancelToken))
                {
                    //if (cancelToken.IsCancellationRequested) break;

                    var nid = (long)reader["file_nid"];
                    var fullname = (string)reader["fullname"];
                    Debug.WriteLine(fullname);

                    counterRead++;

                    if (!File.Exists(fullname)) fileIdToClear.Add(nid);
                    else if (new FileInfo(fullname).Length == 0)
                    {
                        File.Delete(fullname);
                        fileIdToClear.Add(nid);
                    }
                }

                reader.Close();


                using var command = new SQLiteCommand("delete from tb_file where file_nid=$nid", connection);
                command.Parameters.Add("$nid", DbType.Int32);
                command.Prepare();

                foreach (var nid in fileIdToClear)
                {
                    command.Parameters["$nid"].Value = nid;
                    await command.ExecuteNonQueryAsync(cancelToken);
                    if (cancelToken.IsCancellationRequested) break;

                    counterCleared++;
                }


            }
            catch
                (Exception e)
            {
                msg = e.Message;
                error = true;
            }

            return (counterRead, counterCleared, msg, error);

        }
        #endregion


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

        public bool ToggleSeeLater(out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

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

        public bool ToggleSeeNoWant(out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

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


        public bool SetWatched(DateTime watchDate, string comment, out string msg)
        {
            msg = string.Empty;
            seedate = watchDate.ToString("yyyy-MM-dd");
            seecomment = comment;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

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

        public bool MoveTo(string destFolder, out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

            var ok=true;
            var newPath = destFolder.Substring(Path.GetPathRoot(destFolder).Length) + "\\";

            try
            {
                mDbConnection.Open();
                try
                {

                    var command = new SQLiteCommand("select path from tb_file where file_nid=$fid",mDbConnection);
                    command.Parameters.AddWithValue("$fid", fid);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var currentPath = reader.GetString(0);
                        if (!currentPath.Equals(newPath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var destFullName = destFolder + "\\" + name + ext;
                            File.Move(FullName, destFullName);

                            command = new SQLiteCommand("update tb_file set path=$path where file_nid=$fid", mDbConnection);
                            command.Parameters.AddWithValue("$path", newPath);
                            command.Parameters.AddWithValue("$fid", fid);
                            ok = command.ExecuteNonQuery() > 0;
                        }
                    }

                    
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

        public bool DeleteFromDb(bool deleteFile, out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

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

        public bool EditRecord(string newName, string newYear, string newZone,
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

            var mDbConnection = new SQLiteConnection(DbConnectionString);
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

        public static string CountStatistics(out string msg)
        {
            msg = string.Empty;
            var sb = new StringBuilder();
            var connection = new SQLiteConnection(DbConnectionString);
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

        public bool UpdateDoubanInfo(DoubanSubject subject, out string msg)
        {
            msg = string.Empty;
            var posterImageFileName = string.Empty;

            var mDbConnection = new SQLiteConnection(DbConnectionString);
            var sql = @"update tb_file set year=$year,zone=$zone,keyname=$keyname,othername=$othername,doubanid=$doubanid,posterpath=$posterpath,
rating=$rating,genres=$genres,directors=$directors,casts=$casts where file_nid=$fid";
            var ok = true;
            try
            {

                if (!string.IsNullOrEmpty(subject.img_local))
                {
                    posterImageFileName = CurrentPath + "\\poster\\douban\\" + Path.GetFileName(subject.img_local);
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

        public static bool CopyDoubanInfo(long sFid, List<long> dFids, out string msg)
        {
            msg = string.Empty;

            if (dFids.Contains(sFid)) dFids.Remove(sFid);
            var sFids = string.Join(",", dFids.Select(x => x.ToString()));


            var mDbConnection = new SQLiteConnection(DbConnectionString);
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
