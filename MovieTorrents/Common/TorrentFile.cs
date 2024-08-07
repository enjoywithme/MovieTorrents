﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using mySharedLib;

namespace MovieTorrents.Common
{
    public class TorrentFile
    {
        public long Fid { get; set; }
        //public byte hdd_nid { get; set; }
        public int DirNid { get; set; }
        public string DirPath { get; set; }
        public string Path { get; set; }
        //public string KeyName { get; set; }
        public string Casts { get; set; }
        public string Directors { get; set; }
        public string Name { get; set; }

        public string OtherName { get; set; }
        public string Ext { get; set; }
        public long FileSize { get; set; }
        public double Rating { get; set; }
        public string Year { get; set; }
        public long SeeLater { get; set; }
        public long SeeFlag { get; set; }
        public long SeeNoWant { get; set; }
        public string SeeDate { get; set; }
        public string SeeComment { get; set; }
        public string Genres { get; set; }
        public string Zone { get; set; }
        public string PosterPath { get; set; }
        public string DoubanId { get; set; }
        public string DoubanTitle { get; set; }
        public string DoubanSubTitle { get; set; }
        public string FullName => DirPath + Path + Name + Ext;
        private long? _creationTime;
        public string CreationTime => _creationTime.SqlLiteToDateTime();
        public string PurifiedName => Name.Purify();

        public string RatingString => Math.Abs(Rating) < 0.0001 ? null : Rating.ToString(CultureInfo.InvariantCulture);

        public string FirstName => PurifiedName.ExtractFirstToken();
        public string Title => DoubanTitle + " " + DoubanSubTitle;

        public DateTime SeeDateDate
        {
            get
            {
                var seeDate = DateTime.Today;
                if (!string.IsNullOrEmpty(SeeDate) && DateTime.TryParse(SeeDate, out var d))
                    seeDate = d;
                return seeDate;
            }
        }

        public string RealPosterPath
        {
            get
            {
                if (string.IsNullOrEmpty(PosterPath)) return null;
                return _currentPath + "\\poster\\douban\\" + System.IO.Path.GetFileName(PosterPath);
            }
        }

        public Color ForeColor => string.IsNullOrWhiteSpace(DoubanId) || DoubanId=="0"? Color.Firebrick : Color.Black;


        public static TorrentFile FromFullPath(string fullName)
        {
            var torrentFile = new TorrentFile { Name = System.IO.Path.GetFileNameWithoutExtension(fullName) };
            var dir = System.IO.Path.GetDirectoryName(fullName);
            torrentFile.Path = dir?.Substring(TorrentRootPath.Length) + "\\";
            torrentFile.Ext = System.IO.Path.GetExtension(fullName);
            torrentFile.FileSize = new FileInfo(fullName).Length;

            var yr = torrentFile.Name.ExtractYear();
            torrentFile.Year = yr == 0 ? "" : yr.ToString();
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
        public static TorrentFilter Filter { get; } = new();
        //public static string DefaultArea { get; set; }
        //private static string _shortRootPath;
        private static string _currentPath;
        public static string TorrentRootPath { get; set; }
        public static int TorrentRootPathNid { get; private set; } //tb_dir表中的dir_nid

        public static string DbConnectionString;
        public static bool CheckTorrentPath(out string msg,string currentPath)
        {
            var ok = true;
            msg = string.Empty;

            if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                _currentPath = currentPath;
            else
                _currentPath = Utility.ExecutingAssemblyPath();

            if (!File.Exists($"{_currentPath}\\zogvm.db"))
            {
                msg = $"数据库文件不存在！\r\n{_currentPath}\\zogvm.db";
                return false;
            }


            DbConnectionString = $"Data Source ={_currentPath}//zogvm.db; Version = 3; ";

            using var connection = new SQLiteConnection(DbConnectionString);
            const string sql = "select d.dir_nid,area,path from tb_dir as d inner join tb_hdd as h on h.hdd_nid=d.hdd_nid  limit 1";
            try
            {
                connection.Open();
                try
                {
                    using var command = new SQLiteCommand(sql, connection);
                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        TorrentRootPathNid = reader.GetByte(0);// ["hdd_nid"];
                        //DefaultArea = (string)reader["area"];
                        //_shortRootPath = (string)reader["path"];
                        TorrentRootPath = (string)reader["path"];
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

        public TorrentFile()
        {

        }

        public TorrentFile(DbDataReader reader)
        {
            InitializeFromDbReader(reader);
        }

        /// <summary>
        /// 从DbDataReader初始化属性
        /// </summary>
        /// <param name="reader"></param>
        private void InitializeFromDbReader(DbDataReader reader)
        {
            Fid = (long)reader["file_nid"];
            DirPath = (string)reader["DirPath"];
            Path = (string)reader["path"];
            Name = (string)reader["name"];
            //KeyName = reader.GetReaderFieldString("keyname");
            DoubanTitle = reader.GetReaderFieldString("doubantitle");
            DoubanSubTitle = reader.GetReaderFieldString("doubansubtitle");
            OtherName = reader.GetReaderFieldString("othername");
            Ext = (string)reader["ext"];
            Rating = (double)reader["rating"];
            Year = reader.GetReaderFieldString("year");
            Zone = reader.GetReaderFieldString("zone");
            SeeLater = (long)reader["seelater"];
            SeeNoWant = (long)reader["seenowant"];
            SeeFlag = (long)reader["seeflag"];
            PosterPath = reader.GetReaderFieldString("posterpath");
            Genres = reader.GetReaderFieldString("genres");
            DoubanId = reader.GetReaderFieldString("doubanid");
            SeeDate = reader.GetReaderFieldString("seedate");
            SeeComment = reader.GetReaderFieldString("seecomment");
            Casts = reader.GetReaderFieldString("casts");
            Directors = reader.GetReaderFieldString("directors");
            _creationTime = Convert.IsDBNull(reader["CreationTime"])?null: reader.GetInt64(reader.GetOrdinal("CreationTime"));//int8 表示字节，实际上存的是 int64 https://stackoverflow.com/questions/681653/can-you-get-the-column-names-from-a-sqldatareader
        }


        public class SearchResult
        {
            public IList<TorrentFile> TorrentFiles=new List<TorrentFile>();
            public string Message=string.Empty;
            public bool Cancelled;
        }
        //执行异步搜索
        public static async Task<SearchResult> ExecuteSearch(string searchText, CancellationToken cancelToken, bool withFilter=true)
        {
            var result = new SearchResult();

            try
            {
                await using var connection = new SQLiteConnection(DbConnectionString);
                await using var command = Filter.BuildSearchCommand(searchText, withFilter);
                command.Connection = connection;

                await connection.OpenAsync(cancelToken);
                await using var reader = await command.ExecuteReaderAsync(cancelToken);
                while (await reader.ReadAsync(cancelToken))
                {
                    result.TorrentFiles.Add(new TorrentFile(reader));
                }
            }
            catch (OperationCanceledException)
            {
                result.Cancelled = true;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            

            return result;
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

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new TorrentFile(reader));

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

        /// <summary>
        /// 根据文件名查找
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TorrentFile FindByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            TorrentFile result = null;
            var ok = true;
            try
            {
                var connection = new SQLiteConnection(DbConnectionString);

                connection.Open();

                try
                {
                    var sql = "select * from filelist_view where name=$pName";

                    var command = new SQLiteCommand(sql, connection);
                    command.Parameters.AddWithValue("$pName", name);

                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = new TorrentFile(reader);
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
                        "select count(*) from filelist_view where (name like $pName or othername like $pName)";
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
                File.Copy($"{ _currentPath}\\zogvm.db", $"{ _currentPath}\\zogvm.db.bak", true);
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

                using var commandInsert = new SQLiteCommand($@"insert into tb_file
(dir_nid,path,name,ext,year,filesize,CreationTime,LastWriteTime,LastOpenTime,maintype,resolutionW,resolutionH,filetime,bitrateKbps,zidian_sound,zidian_sub)
select {TorrentRootPathNid},$path,$name,$ext,$year,$filesize,$n,$n,$n,0,0,0,0,0,'',''
where not exists (select 1 from tb_file where dir_nid={TorrentRootPathNid} and path=$path and name=$name and ext=$ext)", connection);
                commandInsert.Parameters.Add("$path", DbType.String, 520);
                commandInsert.Parameters.Add("$name", DbType.String, 520);
                commandInsert.Parameters.Add("$ext", DbType.String, 32);
                commandInsert.Parameters.Add("$year", DbType.String, 16);
                commandInsert.Parameters.Add("$filesize", DbType.Int64);
                commandInsert.Parameters.Add("$n", DbType.Int64);

                commandInsert.Prepare();

                while (!filesToProcess.IsCompleted)
                {
                    TorrentFile torrentFile = null;
                    try
                    {
                        torrentFile = filesToProcess.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (torrentFile == null) continue;

                    Debug.WriteLine($"Processing:{torrentFile.Name}");

                    fileProcessed++;
                    var n = DateTime.Now.ToSqlLiteInt64();

                    commandInsert.Parameters["$path"].Value = torrentFile.Path;
                    commandInsert.Parameters["$name"].Value = torrentFile.Name;
                    commandInsert.Parameters["$ext"].Value = torrentFile.Ext;
                    commandInsert.Parameters["$year"].Value = torrentFile.Year;
                    commandInsert.Parameters["$filesize"].Value = torrentFile.FileSize;
                    commandInsert.Parameters["$n"].Value = n;

                    fileAdded += commandInsert.ExecuteNonQuery();
                }

                filesToProcess.Dispose();
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

                //update the file not exists field
                using var command = new SQLiteCommand("update tb_file set filenotexists=1 where file_nid=$nid ", connection);
                command.Parameters.Add("$nid", DbType.Int32);
                command.Prepare();

                foreach (var nid in fileIdToClear)
                {
                    command.Parameters["$nid"].Value = nid;
                    await command.ExecuteNonQueryAsync(cancelToken);
                    if (cancelToken.IsCancellationRequested) break;

                }

                //delete the record
                using var deleteCommand =
                    new SQLiteCommand(
                        "delete from tb_file where filenotexists=1 and doubanid<>0 and seeflag=0 and seecomment='' and seelater=0",
                        connection);
                counterCleared = await deleteCommand.ExecuteNonQueryAsync(cancelToken);


            }
            catch
                (Exception e)
            {
                msg = $"{e.Message}。";
                error = true;
            }

            return (counterRead, counterCleared, msg, error);

        }

        //清理重复项
        public static async Task<(int counterCleared, string msg, bool error)> DoClearDuplicate(CancellationToken cancelToken)
        {

            var counterCleared = 0;
            var msg = string.Empty;
            var error = false;
            
            try
            {
                using var connection = new SQLiteConnection(DbConnectionString);

                //Find the duplicated names
                await connection.OpenAsync(cancelToken);
     

                //merge the information
                var torrentUpdates = new List<TorrentFile>();
                var torrentDeletes = new List<TorrentFile>();

                //foreach (var name in duplicateNames)
                {
                    TorrentFile firstTorrent = null;
                    var lastName = "";
                    using var commandMerge =
                        new SQLiteCommand(
                            "select * from filelist_view where name in (select name from tb_file group by name having count(name)>1) order by name,filenotexists,file_nid",
                            connection);
                    using var readerMerge = await commandMerge.ExecuteReaderAsync(cancelToken);
                    while (await readerMerge.ReadAsync(cancelToken))
                    {
                        var torrentFile = new TorrentFile(readerMerge);

                        if (lastName != (string)readerMerge["name"])
                        {
                            firstTorrent = null;
                            lastName= (string)readerMerge["name"];
                            Debug.WriteLine(lastName);
                        }

                        if (firstTorrent == null)
                        {
                            firstTorrent = torrentFile;
                            torrentUpdates.Add(firstTorrent);

                            continue;
                        }

                        #region Merge fileds


                        if (string.IsNullOrEmpty(firstTorrent.DoubanId) && !string.IsNullOrEmpty(torrentFile.DoubanId))
                            firstTorrent.DoubanId = torrentFile.DoubanId;

                        if (string.IsNullOrEmpty(firstTorrent.DoubanTitle) && !string.IsNullOrEmpty(torrentFile.DoubanTitle))
                            firstTorrent.DoubanTitle = torrentFile.DoubanTitle;

                        if (string.IsNullOrEmpty(firstTorrent.DoubanSubTitle) && !string.IsNullOrEmpty(torrentFile.DoubanSubTitle))
                            firstTorrent.DoubanSubTitle = torrentFile.DoubanSubTitle;

                        if (string.IsNullOrEmpty(firstTorrent.Casts) && !string.IsNullOrEmpty(torrentFile.Casts))
                            firstTorrent.Casts = torrentFile.Casts;

                        if (string.IsNullOrEmpty(firstTorrent.Directors) && !string.IsNullOrEmpty(torrentFile.Directors))
                            firstTorrent.Directors = torrentFile.Directors;

                        if (string.IsNullOrEmpty(firstTorrent.OtherName) && !string.IsNullOrEmpty(torrentFile.OtherName))
                            firstTorrent.OtherName = torrentFile.OtherName; 
                        
                        if (firstTorrent.Rating==0 && torrentFile.Rating!=0)
                            firstTorrent.Rating = torrentFile.Rating; 
                        
                        if (string.IsNullOrEmpty(firstTorrent.Year) && !string.IsNullOrEmpty(torrentFile.Year))
                            firstTorrent.Year = torrentFile.Year;

                        if (firstTorrent.SeeLater == 0 && torrentFile.SeeLater != 0)
                            firstTorrent.SeeLater = torrentFile.SeeLater;

                        if (firstTorrent.SeeFlag == 0 && torrentFile.SeeFlag != 0)
                            firstTorrent.SeeFlag = torrentFile.SeeFlag;

                        if (firstTorrent.SeeNoWant == 0 && torrentFile.SeeNoWant != 0)
                            firstTorrent.SeeNoWant = torrentFile.SeeNoWant;

                        if (string.IsNullOrEmpty(firstTorrent.SeeDate) && !string.IsNullOrEmpty(torrentFile.SeeDate))
                            firstTorrent.SeeDate = torrentFile.SeeDate; 
                        
                        if (string.IsNullOrEmpty(firstTorrent.SeeComment) && !string.IsNullOrEmpty(torrentFile.SeeComment))
                            firstTorrent.SeeComment = torrentFile.SeeComment;

                        if (string.IsNullOrEmpty(firstTorrent.Genres) && !string.IsNullOrEmpty(torrentFile.Genres))
                            firstTorrent.Genres = torrentFile.Genres; 
                        
                        if (string.IsNullOrEmpty(firstTorrent.Zone) && !string.IsNullOrEmpty(torrentFile.Zone))
                            firstTorrent.Zone = torrentFile.Zone; 
                        
                        if (string.IsNullOrEmpty(firstTorrent.PosterPath) && !string.IsNullOrEmpty(torrentFile.PosterPath))
                            firstTorrent.PosterPath = torrentFile.PosterPath;
                        #endregion

                        torrentDeletes.Add(torrentFile);

                    }

                    readerMerge.Close();
                }

                

                //update 
                using var commandUpdate = new SQLiteCommand(@"update tb_file set 
year =$year, 
zone =$zone,
othername =$othername, 
genres =$genres,
seelater =$seelater, 
seeflag =$seeflag, 
seedate =$seedate,
seenowant=$seenowant, 
seecomment =$seecomment,
doubanid =$doubanid, 
doubantitle = $doubantitle,
doubansubtitle = $doubansubtitle,
rating =$rating, 
posterpath =$posterpath, 
casts =$casts, 
directors =$directors 
where file_nid =$fid", connection);
                commandUpdate.Parameters.Add("$fid", DbType.Int32);
                commandUpdate.Parameters.Add("$year", DbType.String);
                commandUpdate.Parameters.Add("$zone", DbType.String);
                commandUpdate.Parameters.Add("$doubantitle", DbType.String);
                commandUpdate.Parameters.Add("$doubansubtitle", DbType.String);
                commandUpdate.Parameters.Add("$othername", DbType.String);
                commandUpdate.Parameters.Add("$genres", DbType.String);
                commandUpdate.Parameters.Add("$seelater", DbType.Int32);
                commandUpdate.Parameters.Add("$seeflag", DbType.Int32);
                commandUpdate.Parameters.Add("$seenowant", DbType.Int32);
                commandUpdate.Parameters.Add("$seedate", DbType.String);
                commandUpdate.Parameters.Add("$seecomment", DbType.String);
                commandUpdate.Parameters.Add("$doubanid", DbType.String);
                commandUpdate.Parameters.Add("$rating", DbType.Double);
                commandUpdate.Parameters.Add("$posterpath", DbType.String);
                commandUpdate.Parameters.Add("$casts", DbType.String);
                commandUpdate.Parameters.Add("$directors", DbType.String);


                commandUpdate.Prepare();

                foreach (var torrent in torrentUpdates)
                {
                    commandUpdate.Parameters["$fid"].Value = torrent.Fid;
                    commandUpdate.Parameters["$year"].Value = torrent.Year;
                    commandUpdate.Parameters["$zone"].Value = torrent.Zone;
                    //commandUpdate.Parameters["$keyname"].Value = torrent.KeyName;
                    commandUpdate.Parameters["$othername"].Value = torrent.OtherName;
                    commandUpdate.Parameters["$genres"].Value = torrent.Genres;
                    commandUpdate.Parameters["$seelater"].Value = torrent.SeeLater;
                    commandUpdate.Parameters["$seeflag"].Value = torrent.SeeFlag;
                    commandUpdate.Parameters["$seenowant"].Value = torrent.SeeNoWant;
                    commandUpdate.Parameters["$seedate"].Value = torrent.SeeDate;
                    commandUpdate.Parameters["$seecomment"].Value = torrent.SeeComment;
                    commandUpdate.Parameters["$doubanid"].Value = torrent.DoubanId;
                    commandUpdate.Parameters["$doubantitle"].Value = torrent.DoubanTitle;
                    commandUpdate.Parameters["$doubansubtitle"].Value = torrent.DoubanSubTitle;
                    commandUpdate.Parameters["$rating"].Value = torrent.Rating;
                    commandUpdate.Parameters["$posterpath"].Value = torrent.PosterPath;
                    commandUpdate.Parameters["$casts"].Value = torrent.Casts;
                    commandUpdate.Parameters["$directors"].Value = torrent.Directors;
                    
                    Debug.WriteLine($"Update:{torrent.FullName}");

                    await commandUpdate.ExecuteNonQueryAsync(cancelToken);
                    if (cancelToken.IsCancellationRequested) break;

                }

                //delete
                using var deleteCommand = new SQLiteCommand("delete from tb_file where file_nid =$fid", connection);
                deleteCommand.Parameters.Add("$fid", DbType.Int32);
                deleteCommand.Prepare();
                foreach (var torrentFile in torrentDeletes)
                {
                    if (File.Exists(torrentFile.FullName))
                        File.Delete(torrentFile.FullName);
                    Debug.WriteLine($"Delete:{torrentFile.FullName}");

                    deleteCommand.Parameters["$fid"].Value = torrentFile.Fid;
                    counterCleared += await deleteCommand.ExecuteNonQueryAsync(cancelToken);

                }

            }
            catch
                (Exception e)
            {
                msg = $"{e.Message}。";

                error = true;
            }

            return (counterCleared, msg, error);

        }


        struct PosterUpdate
        {
            public TorrentFile TorrentFile;
            public string NewPosterFileName;
        }
        //清理海报
        public static async Task<(int counterCleared, string msg, bool error)> DoClearPoster(CancellationToken cancelToken)
        {

            var counterCleared = 0;
            var msg = string.Empty;
            var error = false;

            try
            {
                using var connection = new SQLiteConnection(DbConnectionString);

                //Find the duplicated names
                await connection.OpenAsync(cancelToken);


                //merge the information
                var torrentUpdates = new List<PosterUpdate>();

                //foreach (var name in duplicateNames)
                {
                    using var commandMerge =
                        new SQLiteCommand(
                            "select * from filelist_view where length(doubanid)<>0 and length(posterpath)<>0",
                            connection);
                    using var reader = await commandMerge.ExecuteReaderAsync(cancelToken);
                    while (await reader.ReadAsync(cancelToken))
                    {
                        var torrentFile = new TorrentFile(reader);

                        var posterPath = torrentFile.RealPosterPath;

                        var fileName = System.IO.Path.GetFileNameWithoutExtension(posterPath);
                        var fileExt = System.IO.Path.GetExtension(posterPath);
                        var newFileName = $"d{torrentFile.DoubanId}";
                        if (!string.Equals(fileName, newFileName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            torrentUpdates.Add(new PosterUpdate { TorrentFile = torrentFile, NewPosterFileName = newFileName + fileExt });
                        }


                    }

                    reader.Close();
                }



                //update 
                using var commandUpdate = new SQLiteCommand(@"update tb_file set posterpath =$posterpath where file_nid =$fid", connection);
                commandUpdate.Parameters.Add("$fid", DbType.Int32);
                commandUpdate.Parameters.Add("$posterpath", DbType.String);
                commandUpdate.Prepare();

                foreach (var posterUpdate in torrentUpdates)
                {

                    try
                    {
                        if (!string.IsNullOrEmpty(posterUpdate.NewPosterFileName))
                        {
                            var newFullName = System.IO.Path.Combine(_currentPath, "poster\\douban\\" , posterUpdate.NewPosterFileName);
                            if (!File.Exists(newFullName))
                            {
                                if(!File.Exists(posterUpdate.TorrentFile.RealPosterPath)) continue;
                                File.Move(posterUpdate.TorrentFile.RealPosterPath,newFullName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Failed to rename file:{e.Message}");
                        continue;
                    }

                    commandUpdate.Parameters["$fid"].Value = posterUpdate.TorrentFile.Fid;
                    commandUpdate.Parameters["$posterpath"].Value = posterUpdate.NewPosterFileName;

                    commandUpdate.ExecuteNonQuery();
                    counterCleared++;
                    Debug.WriteLine($"Update:{posterUpdate.NewPosterFileName}");

                    if (cancelToken.IsCancellationRequested) break;

                }


            }
            catch
                (Exception e)
            {
                msg = $"{e.Message}。";

                error = true;
            }

            return (counterCleared, msg, error);

        }

        #endregion


        /// <summary>
        ///匹配豆瓣影视信息
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public static async Task<(int, string msg, bool error)> DoMatchMovieInfo(CancellationToken cancelToken)
        {

            var countUpdated = 0;
            var msg = string.Empty;
            var error = false;

            var unknownTorrents = new List<TorrentFile>();

            try
            {
                await using var connection = new SQLiteConnection(DbConnectionString);
                await connection.OpenAsync(cancelToken);

                //读取未知豆瓣信息的条目到列表
                await using var commandRead = new SQLiteCommand("select * from filelist_view where doubanid is null order by file_nid DESC", connection);
                await using var reader = await commandRead.ExecuteReaderAsync(cancelToken);
                while (await reader.ReadAsync(cancelToken))
                {
                    var tf = new TorrentFile(reader);
                    unknownTorrents.Add(tf);
                }

                await reader.CloseAsync();

                //update the file not exists field
                for (var i = 0; i < unknownTorrents.Count; i++)
                {
                    var tf = unknownTorrents[i];
                    var otherName = tf.FirstName;
                    Debug.WriteLine(otherName);
                    
                    //使用净化后的名称搜索条目豆瓣名称
                    await using var cmdCountMovieId =
                        new SQLiteCommand(
                            "select count(DISTINCT doubanid) as cnt from filelist_view where doubantitle=$name",connection);
                    cmdCountMovieId.Parameters.AddWithValue("$name", otherName);
                    await using var readerCountMovieId = await cmdCountMovieId.ExecuteReaderAsync(cancelToken);
                    if(!await readerCountMovieId.ReadAsync(cancelToken)) continue;
                    var countIds = readerCountMovieId.GetInt32(0);
                    await readerCountMovieId.CloseAsync();

                    if(countIds !=1) continue;

                    //取得第一条同豆瓣ID的FID
                    await using var cmdQueryMovieId =
                        new SQLiteCommand(
                            "select file_nid from filelist_view where doubantitle=$name and doubanid is not null order by file_nid DESC limit 1", connection);
                    cmdQueryMovieId.Parameters.AddWithValue("$name", otherName);
                    await using var readerQueryMovieId = await cmdQueryMovieId.ExecuteReaderAsync(cancelToken);
                    if (!await readerQueryMovieId.ReadAsync(cancelToken)) continue;
                    var fid = readerQueryMovieId.GetInt64(0);
                    await readerCountMovieId.CloseAsync();

                    //更新到当前条目
                    await using var cmdUpdateMovieInfo = new SQLiteCommand($@"update tb_file set
year=(select year from tb_file where file_nid=$fid),
posterpath=(select posterpath from tb_file where file_nid=$fid),
doubanid=(select doubanid from tb_file where file_nid=$fid),
doubantitle=(select doubantitle from tb_file where file_nid=$fid),
doubansubtitle=(select doubansubtitle from tb_file where file_nid=$fid),
rating=(select rating from tb_file where file_nid=$fid),
casts=(select casts from tb_file where file_nid=$fid),
directors=(select directors from tb_file where file_nid=$fid),
genres=(select genres from tb_file where file_nid=$fid),
zone=(select zone from tb_file where file_nid=$fid)
where file_nid =$this_fid", connection);
                    cmdUpdateMovieInfo.Parameters.AddWithValue("$fid", fid);
                    cmdUpdateMovieInfo.Parameters.AddWithValue("$this_fid", tf.Fid);
                    await cmdUpdateMovieInfo.ExecuteNonQueryAsync(cancelToken);

                    Debug.WriteLine("---matched");
                    countUpdated++;
                }

                await connection.CloseAsync();

            }
            catch
                (Exception e)
            {
                msg = $"{e.Message}。";
                error = true;
            }

            return (countUpdated, msg, error);

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

        public bool ToggleSeeLater(out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

            const string sql = $"update tb_file set seelater = case seelater when 1 then 0 else 1 end where file_nid=$fid";
            var ok = true;
            try
            {
                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", Fid);
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
                    command.Parameters.AddWithValue("$fid", Fid);
                    command.ExecuteNonQuery();

                    SeeNoWant = SeeNoWant == 1 ? 0 : 1;
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
            SeeDate = watchDate.ToString("yyyy-MM-dd");
            SeeComment = comment;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

            var sql = $"update tb_file set seelater=0,seeflag=1,seedate=$seedate,seecomment=$comment where file_nid=$fid";
            var ok = true;
            try
            {
                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$seedate", SeeDate);
                    command.Parameters.AddWithValue("$comment", comment);
                    command.Parameters.AddWithValue("$fid", Fid);
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


        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="destFolder">完整的文件夹路径,带末尾\\</param>
        /// <param name="destName">可以重新命名</param>
        /// <returns></returns>
        public (bool,string) MoveTo(string destFolder,string destName=null)
        {

            if (!File.Exists(FullName))
            {
                return (false, $"文件{FullName} 不存在。");
            }

            if (!destFolder.StartsWith(TorrentRootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, "目标目录不在Torrent根目录下！");
            }
            if (!destFolder.EndsWith("\\")) destFolder += "\\";


            var mDbConnection = new SQLiteConnection(DbConnectionString);

            var ok=true;
            var newPath = string.IsNullOrEmpty(destFolder)?Path:destFolder.Substring(TorrentRootPath.Length);//取根目录下的相对位置
            var newName = string.IsNullOrEmpty(destName) ? Name : destName;
            var newFullName =  destFolder  + newName + Ext;
            

            var msg = "";

            try
            {
                mDbConnection.Open();
                try
                {


                    if (!newFullName.Equals(FullName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        File.Move(FullName, newFullName);

                        var command = new SQLiteCommand("update tb_file set path=$path,name=$name where file_nid=$fid", mDbConnection);
                        command.Parameters.AddWithValue("$path", newPath);
                        command.Parameters.AddWithValue("$name", newName);
                        command.Parameters.AddWithValue("$fid", Fid);
                        ok = command.ExecuteNonQuery() > 0;
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

            if (ok) Path = newPath;

            return (ok,msg);
        }

        /// <summary>
        /// 移动整个路径
        /// </summary>
        /// <param name="destFolder">完整的文件夹，末尾带\\</param>
        /// <param name="sourcePath">相对torrent根目录的相对文件夹</param>
        /// <param name="rename"></param>
        /// <returns></returns>
        public static (bool, string) MovePath(string destFolder, string sourcePath,bool rename=false)
        {
            if (string.IsNullOrEmpty(sourcePath))
                return (false, "源路径空");

            if (!destFolder.StartsWith(TorrentRootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, "目标目录不在Torrent根目录下！");
            }
            if (!destFolder.EndsWith("\\")) destFolder += "\\";

            var ret = true;
            var msg = "";

            var mDbConnection = new SQLiteConnection(DbConnectionString);
            try
            {
                mDbConnection.Open();
                var tran = mDbConnection.BeginTransaction();

                try
                {
                    var sourceFullPath = System.IO.Path.Combine(TorrentRootPath, sourcePath);
                    var sourceDirName = new DirectoryInfo(sourceFullPath).Name;
                    var destFullPath = rename?destFolder: System.IO.Path.Combine(destFolder, sourceDirName)+"\\";

                    var newPath = destFullPath.Substring(TorrentRootPath.Length);//取根目录下的相对位置
                    var command = new SQLiteCommand("update tb_file set path=$newPath where path=$path", mDbConnection);
                    command.Parameters.AddWithValue("$path", sourcePath);
                    command.Parameters.AddWithValue("$newPath", newPath);
                    command.ExecuteNonQuery();

                    if (Directory.Exists(destFullPath))
                    {
                        

                        var files = Directory.GetFiles(sourceFullPath);
                        foreach (var file in files)
                        {
                            var destFile = System.IO.Path.Combine(destFullPath, System.IO.Path.GetFileName(file));
                            if (File.Exists(destFile)) continue;
                            File.Move(file,destFile);
                        }

                        Directory.Delete(sourceFullPath,true);
                    }
                    else
                        Directory.Move(sourceFullPath,destFullPath);

                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    ret=false;
                    msg = e.Message;
                }

                mDbConnection.Close();
            }
            catch (Exception e)
            {
                ret = false;
                msg = e.Message;
            }

            return (ret,msg);
        }

        public bool DeleteFromDb(bool deleteFile, out string msg)
        {
            msg = string.Empty;
            var mDbConnection = new SQLiteConnection(DbConnectionString);

            const string sql = $"delete from tb_file where file_nid=$fid";
            bool ok;
            try
            {
                mDbConnection.Open();
                try
                {
                    if (deleteFile && File.Exists(FullName))
                        File.Delete(FullName);

                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$fid", Fid);
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

        public bool EditRecord(TorrentFile ediTorrentFile, out string msg)
        {
            msg = string.Empty;
            var reNameFile = false;
            ediTorrentFile.Name = ediTorrentFile.Name.Trim();
            var newFullName = TorrentRootPath + Path + ediTorrentFile.Name + Ext;

            if (string.Compare(Name, ediTorrentFile.Name, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                reNameFile = true;
                if (File.Exists(newFullName))
                {
                    msg = "要命名的新文件已经存在！";
                    return false;
                }
            }

            var mDbConnection = new SQLiteConnection(DbConnectionString);
            var sql = @"update tb_file set name=$name,year=$year,zone=$zone,
othername=$othername,genres=$genres,
seelater=$seelater,seeflag=$seeflag,seedate=$seedate,seecomment=$comment,
doubanid=$doubanid,doubantitle=$doubantitle,doubansubtitle=$doubansubtitle,
rating=$rating,posterpath=$posterpath,casts=$casts,directors=$directors
where file_nid=$fid";
            var ok = true;
            try
            {


                mDbConnection.Open();
                try
                {
                    if (reNameFile) File.Move(FullName, newFullName);

                    var command = new SQLiteCommand(sql, mDbConnection);

                    command.Parameters.AddWithValue("$name", ediTorrentFile.Name);
                    command.Parameters.AddWithValue("$year", ediTorrentFile.Year);
                    command.Parameters.AddWithValue("$zone", ediTorrentFile.Zone);
                    //command.Parameters.AddWithValue("$keyname", newKeyName);
                    command.Parameters.AddWithValue("$othername", ediTorrentFile.OtherName);
                    command.Parameters.AddWithValue("$genres", ediTorrentFile.Genres);
                    command.Parameters.AddWithValue("seelater", ediTorrentFile.SeeLater);
                    command.Parameters.AddWithValue("$seeflag", ediTorrentFile.SeeFlag);
                    command.Parameters.AddWithValue("$seedate", ediTorrentFile.SeeDate);
                    command.Parameters.AddWithValue("$comment", ediTorrentFile.SeeComment);

                    command.Parameters.AddWithValue("$doubanid", ediTorrentFile.DoubanId);
                    command.Parameters.AddWithValue("$doubantitle", ediTorrentFile.DoubanTitle);
                    command.Parameters.AddWithValue("$doubansubtitle", ediTorrentFile.DoubanSubTitle);

                    command.Parameters.AddWithValue("$rating", ediTorrentFile.Rating);
                    command.Parameters.AddWithValue("$posterpath", ediTorrentFile.PosterPath);
                    command.Parameters.AddWithValue("$casts", ediTorrentFile.Casts);
                    command.Parameters.AddWithValue("$directors", ediTorrentFile.Directors);

                    command.Parameters.AddWithValue("$fid", Fid);
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

            Name = ediTorrentFile.Name;
            Year = ediTorrentFile.Year;
            Zone = ediTorrentFile.Zone;
            //KeyName = newKeyName;
            DoubanTitle = ediTorrentFile.DoubanTitle;
            DoubanSubTitle = ediTorrentFile.DoubanSubTitle;
            OtherName = ediTorrentFile.OtherName;
            Genres = ediTorrentFile.Genres;
            SeeLater = ediTorrentFile.SeeLater;
            SeeFlag = ediTorrentFile.SeeFlag;
            SeeDate = ediTorrentFile.SeeDate;
            SeeComment = ediTorrentFile.SeeComment;
            DoubanId = ediTorrentFile.DoubanId;
            Rating = ediTorrentFile.Rating;
            PosterPath = ediTorrentFile.PosterPath;
            Casts = ediTorrentFile.Casts;
            Directors = ediTorrentFile.Directors;

            return true;
        }

        public static long CountWatched(string dbConnString, out string msg)
        {
            msg = string.Empty;
            var connection = new SQLiteConnection(dbConnString);
            const string sql = $"select count(*) as watched from filelist_view where seeflag=1";
            long watched;
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
            //var sql = $"select count(*) as watched from filelist_view where seeflag=1";
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
                    sb.AppendLine($"已看 {command.ExecuteScalar()}：");

                    command = new SQLiteCommand(@"
select year_month,count(*) from (
select strftime('%Y', seedate) year_month
from filelist_view
where seedate<>'' and seedate not null)
group by year_month 
order by year_month desc
", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        sb.AppendLine($"{reader[0]} | {reader[1]}");
                    }
                    reader.Close();

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

        public bool UpdateDoubanInfo(DouBanSubject subject, out string msg)
        {
            msg = string.Empty;
            var posterImageFileName = string.Empty;

            var mDbConnection = new SQLiteConnection(DbConnectionString);
            var sql = @"update tb_file set year=$year,zone=$zone,othername=$othername,
doubanid=$doubanid,doubantitle=$doubantitle,doubansubtitle=$doubansubtitle,
posterpath=$posterpath,
rating=$rating,genres=$genres,directors=$directors,casts=$casts where file_nid=$fid";
            var ok = true;
            try
            {

                if (!string.IsNullOrEmpty(subject.img_local))
                {
                    var fileExt = System.IO.Path.GetExtension(subject.img_local);

                    posterImageFileName = _currentPath + "\\poster\\douban\\d" + subject.id + fileExt;
                    File.Copy(subject.img_local, posterImageFileName, true);
                    posterImageFileName = $"d{subject.id}{fileExt}";//使用豆瓣ID命名海报文件
                }

                mDbConnection.Open();
                try
                {
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.Parameters.AddWithValue("$year", string.IsNullOrEmpty(subject.year) ? Year : subject.year);
                    command.Parameters.AddWithValue("$zone", subject.zone);
                    command.Parameters.AddWithValue("$doubantitle", subject.title);
                    command.Parameters.AddWithValue("$doubansubtitle", subject.sub_title);
                    command.Parameters.AddWithValue("$othername", string.IsNullOrEmpty(subject.othername) ? OtherName : subject.othername);
                    command.Parameters.AddWithValue("$doubanid", subject.id);
                    command.Parameters.AddWithValue("$posterpath", posterImageFileName);
                    command.Parameters.AddWithValue("$rating", subject.Rating);
                    command.Parameters.AddWithValue("$genres", subject.genres);
                    command.Parameters.AddWithValue("$directors", subject.directors);
                    command.Parameters.AddWithValue("$casts", subject.casts);
                    command.Parameters.AddWithValue("$fid", Fid);
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
            Genres = subject.genres;
            DoubanTitle = subject.title;//豆瓣标题
            DoubanSubTitle = subject.sub_title;
            OtherName = string.IsNullOrEmpty(subject.othername) ? OtherName : subject.othername;
            Year = string.IsNullOrEmpty(subject.year) ? Year : subject.year;
            Zone = string.IsNullOrEmpty(subject.zone) ? Zone : subject.zone;
            PosterPath = posterImageFileName;
            DoubanId = subject.id;
            Rating = subject.Rating;


            return true;
        }

        /// <summary>
        /// 拷贝指定条目的豆瓣信息到选定的列表
        /// </summary>
        /// <param name="sFid">指定豆瓣信息的条目</param>
        /// <param name="dFids">选定的条目</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CopyDoubanInfo(long sFid, List<long> dFids, out string msg)
        {
            msg = string.Empty;

            if (dFids.Contains(sFid)) dFids.Remove(sFid);
            var sFids = string.Join(",", dFids.Select(x => x.ToString()));


            var mDbConnection = new SQLiteConnection(DbConnectionString);
            var sql = $@"update tb_file set
year=(select year from tb_file where file_nid=$fid),
doubantitle=(select doubantitle from tb_file where file_nid=$fid),
doubansubtitle=(select doubansubtitle from tb_file where file_nid=$fid),
othername=(select othername from tb_file where file_nid=$fid),
posterpath=(select posterpath from tb_file where file_nid=$fid),
doubanid=(select doubanid from tb_file where file_nid=$fid),
rating=(select rating from tb_file where file_nid=$fid),
casts=(select casts from tb_file where file_nid=$fid),
directors=(select directors from tb_file where file_nid=$fid),
genres=(select genres from tb_file where file_nid=$fid),
zone=(select zone from tb_file where file_nid=$fid)
where file_nid in ({sFids})";
            bool ok;
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

        public bool OpenDoubanLink(out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty(DoubanId)) return true;
            try
            {
                var p = new Process()
                {
                    StartInfo = new ProcessStartInfo($"https://movie.douban.com/subject/{DoubanId}/") { UseShellExecute = true }
                };
                p.Start();
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }
            return true;
        }



    }
}
