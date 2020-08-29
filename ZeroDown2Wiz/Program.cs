using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using CefSharp;
using CefSharp.OffScreen;
using ZeroDownLib;

namespace ZeroDown2Wiz
{
    class Program
    {
        private static MainBrowser _mainBrowser;
        private static int _runningState=0;

        static void Main(string[] args)
        {

            Console.WriteLine("This application will save 0daydown articles to WizNote.");
            Console.WriteLine("You may see Chromium debugging output, please wait...");
            Console.WriteLine();

            //配置wizdb
            ZeroDayDownArticle.WizDbPath = System.Configuration.ConfigurationManager.AppSettings["IndexDbPath"];
            ZeroDayDownArticle.WizDefaultFolder = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];

            //配置cef
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);


            // 初始化浏览器
            _mainBrowser = new MainBrowser();
            var t = new Timer(TimerCallback, null, 10*1000, 20*60*1000);//每隔20分钟搜索一次

            // We have to wait for something, otherwise the process will exit too soon.
            Console.ReadKey();

            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();

        }
        private static void TimerCallback(object o)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked?view=netcore-3.1
            //0 indicates that the method is in use.
            if (0 != Interlocked.Exchange(ref _runningState, 1))
                return;
            Log("------------Starting search-------------");
            _mainBrowser.StartSearch();
            Interlocked.Exchange(ref _runningState, 0);
        }

        private static void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss}\tMain:{msg}");
        }

    }
}
