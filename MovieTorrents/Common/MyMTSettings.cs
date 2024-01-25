using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieTorrents.Common
{
    internal class MyMtSettings
    {
        public static MyMtSettings Instance { get; private set; }
        public string CurrentPath { get; set; }
        public string BtBtHomeUrl { get; set; }
        public string DownLoadRootPath { get; set; }
        public string WebProxy { get; set; }
        public int AutoDownloadInterval { get; set; } = 20; //自动搜索时间间隔（分钟）
        public int AutoDownloadSearchPages { get; set; } = 8; //自动搜索的页数
        public int AutoDownloadSearchHours { get; set; } = 24;//自动搜索的小时

        public int AutoDownloadLastTid { get; set; }
        public DateTime? AutoDownloadLastPostDateTime { get; set; }

        public string CurrentMonitorComputer { get; private set; } //当前监视的电脑

        public static bool InitInstance(string currentPath,out string message)
        {
            message = string.Empty;

            var settingFile = Path.Combine(currentPath, "mtSettings.json");
            if (!File.Exists(settingFile))
            {
                message = "找不到程序设置文件 mtSettings.json";
                return false;
            }

            try
            {
                Instance = new MyMtSettings(currentPath,settingFile);

            }
            catch (Exception e)
            {
                message = e.Message; return false;
            }

            return true;
        }

        private readonly string _settingFile;
        public MyMtSettings(string currentPath,string settingFile)
        {
            CurrentPath = currentPath;
            _settingFile = settingFile;

            if (!File.Exists(_settingFile)) return;

            var jo = JObject.Parse(File.ReadAllText(_settingFile));
            if (jo.ContainsKey(nameof(BtBtHomeUrl)))
                BtBtHomeUrl = jo.Value<string>(nameof(BtBtHomeUrl));

            if (jo.ContainsKey(nameof(DownLoadRootPath)))
                DownLoadRootPath = jo.Value<string>(nameof(DownLoadRootPath));

            if (jo.ContainsKey(nameof(WebProxy)))
                WebProxy = jo.Value<string>(nameof(WebProxy));

            if (jo.ContainsKey(nameof(AutoDownloadInterval)))
                AutoDownloadInterval = jo.Value<int>(nameof(AutoDownloadInterval));

            if (jo.ContainsKey(nameof(AutoDownloadSearchPages)))
                AutoDownloadSearchPages = jo.Value<int>(nameof(AutoDownloadSearchPages));

            if (jo.ContainsKey(nameof(AutoDownloadSearchHours)))
                AutoDownloadSearchHours = jo.Value<int>(nameof(AutoDownloadSearchHours));

            if (jo.ContainsKey(nameof(AutoDownloadLastTid)))
                AutoDownloadLastTid = jo.Value<int>(nameof(AutoDownloadLastTid));

            if (jo.ContainsKey(nameof(AutoDownloadLastPostDateTime)))
                AutoDownloadLastPostDateTime = jo.Value<DateTime>(nameof(AutoDownloadLastPostDateTime));

            if (jo.ContainsKey(nameof(CurrentMonitorComputer)))
                CurrentMonitorComputer = jo.Value<string>(nameof(CurrentMonitorComputer));
        }


        public bool RegisterMonitor()
        {
            if (!string.IsNullOrEmpty(CurrentMonitorComputer))
            {
                return CurrentMonitorComputer == Environment.MachineName;
            }

            CurrentMonitorComputer = Environment.MachineName;
            Save();
            return true;
        }

        public void UnRegisterMonitor()
        {
            if (!string.IsNullOrEmpty(CurrentMonitorComputer) && CurrentMonitorComputer == Environment.MachineName)
            {
                CurrentMonitorComputer =string.Empty;
            }

           
        }

        public bool IsCurrentMonitor()
        {
            if(CurrentMonitorComputer == null) return false;
            return CurrentMonitorComputer == Environment.MachineName;
        }


        public void Save()
        {
            try
            {
                var jo = new JObject
                {
                    [nameof(BtBtHomeUrl)] = BtBtHomeUrl,
                    [nameof(DownLoadRootPath)] = DownLoadRootPath,
                    [nameof(WebProxy)] = WebProxy,
                    [nameof(AutoDownloadInterval)] = AutoDownloadInterval,
                    [nameof(AutoDownloadSearchPages)] = AutoDownloadSearchPages,
                    [nameof(AutoDownloadSearchHours)] = AutoDownloadSearchHours,
                    [nameof(AutoDownloadLastTid)] = AutoDownloadLastTid,
                    [nameof(AutoDownloadLastPostDateTime)] = AutoDownloadLastPostDateTime,
                    [nameof(CurrentMonitorComputer)] = CurrentMonitorComputer,
                };

                File.WriteAllText(_settingFile,jo.ToString(Formatting.Indented));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }
    }
}
