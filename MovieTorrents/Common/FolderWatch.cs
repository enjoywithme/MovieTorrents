using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MovieTorrents.Common
{

    public class FolderWatchEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool NewFilesToBeProcess { get; set; }
        public bool StatusChanged { get; set; }
    }

    public class FolderWatch
    {
        private FileSystemWatcher _watcher;
        private Timer _watchTimer;
        private bool _isWatching;
        private bool _ignoreFileWatch;
        private readonly string _watchPath;

        public ConcurrentQueue<string> FilesAdded { get; } = new();

        public event EventHandler<FolderWatchEventArgs> FolderWatchEvent;

        public FolderWatch(string watchPath)
        {
            _watchPath = watchPath;
        }

        public bool IsWatching
        {
            set
            {
                if (_isWatching == value) return;
                _isWatching = value;
                FolderWatchEvent?.Invoke(this, new FolderWatchEventArgs {StatusChanged = true});

            }
            get => _isWatching;
        }

        public void IgnoreFileWatch(bool ignore = true)
        {
            _ignoreFileWatch = ignore;
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_watchPath))
            {
                FolderWatchEvent?.Invoke(this,new FolderWatchEventArgs(){Message = "Watch folder is empty."});
                return;
            }

            try
            {
                _watcher = new FileSystemWatcher(TorrentFile.TorrentRootPath)
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.FileName,
                    Filter = "*.torrent"
                };
                _watcher.Created += Watcher_File_Created;
                _watcher.Error += Watcher_File__Error;
                _watcher.EnableRaisingEvents = true;
                IsWatching = true;
            }
            catch (Exception e)
            {
                IsWatching = false;
                FolderWatchEvent?.Invoke(this, new FolderWatchEventArgs() { Message = $"启动目录监视失败\r\n{e.Message}!" });

            }

            if (_watchTimer == null)
            {
                _watchTimer = new Timer(CheckWatchStatus, null, 2000, 10000);
            }
            else
                _watchTimer.Change(2000, 10000);

        }

        public void Stop()
        {
            if (_watchTimer != null && !_watchTimer.Change(Timeout.Infinite, Timeout.Infinite))
            {
                _watchTimer.Dispose();
                _watchTimer = null;
            }

            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }


            IsWatching = false;
        }

        private void Watcher_File_Created(object sender, FileSystemEventArgs e)
        {
            if (_ignoreFileWatch) return;

            //添加一个文件会发现生成2个事件，https://blogs.msdn.microsoft.com/ahamza/2006/02/04/filesystemwatcher-generates-duplicate-events-how-to-workaround/ 
            Debug.WriteLine($"File monitored added:{e.FullPath}");
            FilesAdded.Enqueue(e.FullPath);

        }

        private void Watcher_File__Error(object sender, ErrorEventArgs e)
        {
            IsWatching = false;
            FolderWatchEvent?.Invoke(this,new FolderWatchEventArgs(){Message = $"目录监视失败:{e.GetException().Message}" });
        }

        private void CheckWatchStatus(object state)
        {
            if (string.IsNullOrEmpty(_watchPath)|| !Directory.Exists(TorrentFile.TorrentRootPath)) return;

            if (FilesAdded.Count > 0)
            {
                FolderWatchEvent?.Invoke(this, new FolderWatchEventArgs {NewFilesToBeProcess = true});

            }
            
            if (_isWatching) return;

            Start();
        }

    }
}
