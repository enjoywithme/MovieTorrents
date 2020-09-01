using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.OffScreen;
using ZeroDownLib;
using Timer = System.Threading.Timer;
using System.Drawing;

namespace ZeroDown2Wiz
{
    class Program
    {
        private static MainBrowser _mainBrowser;
        private static int _runningState=0;
        public static bool ApplicationExiting;

        static NotifyIcon notifyIcon;
        static IntPtr processHandle;
        static IntPtr WinShell;
        static IntPtr WinDesktop;
        static MenuItem HideMenu;
        static MenuItem RestoreMenu;

        #region DLLImport

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);
        [DllImport("user32.dll")]
        static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        private const int MF_BYCOMMAND = 0x00000000;

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_ENABLED = 0x00000000;
        private const uint MF_DISABLED = 0x00000002;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventHandler handler, bool add);

        private delegate bool ConsoleEventHandler(CtrlType sig);
        static ConsoleEventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        #endregion

        private static bool Handler(CtrlType sig)
        {
            
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    ApplicationExiting = true;
                    Application.Exit();
                    return true;//不管什么返回值，程序5秒后退出
            }
        }

        static void Main(string[] args)
        {
            //图标区图标
            notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = Application.ProductName,
                Visible = true
            };

            var menu = new ContextMenu();
            HideMenu = new MenuItem("Hide", Minimize_Click);
            RestoreMenu = new MenuItem("Restore", Maximize_Click);

            menu.MenuItems.Add(RestoreMenu);
            menu.MenuItems.Add(HideMenu);
            menu.MenuItems.Add(new MenuItem("Exit", CleanExit));

            notifyIcon.ContextMenu = menu;

            // Some biolerplate to react to close window event https://stackoverflow.com/questions/474679/capture-console-exit-c-sharp
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);

            //禁用关闭按钮 https://stackoverflow.com/questions/6052992/how-can-i-disable-close-button-of-console-window-in-a-visual-studio-console-appl 
            //DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, (uint)(MF_ENABLED | MF_DISABLED));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("This application will save 0daydown articles to WizNote.");
            Console.WriteLine("You may see Chromium debugging output, please wait...");
            //Console.WriteLine("Press <Q> to exit... ");
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

            //隐藏主窗口
            processHandle = GetConsoleWindow();
            //ShowWindow(hWndConsole, SW_MINIMIZE);
            //processHandle = Process.GetCurrentProcess().MainWindowHandle;
            //WinShell = GetShellWindow();
            //WinDesktop = GetDesktopWindow();

            //Hide the Window
            ResizeWindow(false);
            Application.Run();

            //等待搜索结束
            ApplicationExiting = true;
            while(0 != Interlocked.Exchange(ref _runningState, 1))
            {
                Log("Browser running,Waiting it end......",ConsoleColor.DarkRed);
                Thread.Sleep(2000);
            }

            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();

            Debug.WriteLine("-----------final exit---------------");
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

        private static void CleanExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }


        static void Minimize_Click(object sender, EventArgs e)
        {
            ResizeWindow(false);
        }


        static void Maximize_Click(object sender, EventArgs e)
        {
            ResizeWindow();
        }

        static void ResizeWindow(bool Restore = true)
        {
            if (Restore)
            {
                RestoreMenu.Enabled = false;
                HideMenu.Enabled = true;
                //SetParent(processHandle, WinDesktop);
                ShowWindow(processHandle, 1);
            }
            else
            {
                RestoreMenu.Enabled = true;
                HideMenu.Enabled = false;
                ShowWindow(processHandle, 0);
            }
        }
    }
}
