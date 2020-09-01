using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using ZeroDownLib;

namespace ZeroDown2Wiz
{
    class Program
    {
        private static MainBrowser _mainBrowser;
        private static int _runningState=0;


        #region DLLImport

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        #endregion


        static void Main(string[] args)
        {
            //禁用关闭按钮 https://stackoverflow.com/questions/6052992/how-can-i-disable-close-button-of-console-window-in-a-visual-studio-console-appl 
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("This application will save 0daydown articles to WizNote.");
            Console.WriteLine("You may see Chromium debugging output, please wait...");
            Console.WriteLine("Press <Q> to exit... ");
            Console.WriteLine();
            Console.ResetColor();

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
            var interval = mySharedLib.Utility.GetSetting("SearchInterval", 15);//分钟
            var t = new Timer(TimerCallback, null, 10 * 1000,  interval * 60 * 1000);

            // https://docs.microsoft.com/en-us/dotnet/api/system.console.readkey?view=netcore-3.1
            do
            {
                var cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Q)
                {
                    Log("Exiting...",ConsoleColor.DarkRed);
                    break;
                }
                Log("Press <Q> to exit... ");

            } while (true);

            //等待搜索结束
            while(0 != Interlocked.Exchange(ref _runningState, 1))
            {
                Log("Browser running,Waiting it end......",ConsoleColor.DarkRed);
                Thread.Sleep(2000);
            }

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
            Log("------------Starting search-------------",ConsoleColor.Yellow);
            _mainBrowser.StartSearch();
            Interlocked.Exchange(ref _runningState, 0);
        }

        private static void Log(string msg,ConsoleColor color = ConsoleColor.White)
        {
            Console.Write($"{DateTime.Now:hh:mm:ss}\tMain:");
            Console.ForegroundColor = color;
            Console.WriteLine($"{msg}");
            Console.ResetColor();
        }

    }
}
