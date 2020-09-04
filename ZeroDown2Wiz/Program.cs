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
using mySharedLib;

namespace ZeroDown2Wiz
{
    class Program
    {
        private static MainBrowser _mainBrowser;
        private static int _runningState=0;
        public static bool ApplicationExiting;

        static NotifyIcon _notifyIcon;
        static IntPtr _consoleWindowHandle;
        static MenuItem _hideMenu;
        static MenuItem _restoreMenu;

        #region DLLImport

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
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
        private const uint MF_GRAYED = 0x00000001;
        
        #endregion

        

        static void Main(string[] args)
        {
            _consoleWindowHandle = GetConsoleWindow();
           
            //禁用桌面启动栏图标 https://stackoverflow.com/questions/39296364/how-to-hide-the-windows-10-taskbar-close-button-using-winapi
            SetWindowLong(_consoleWindowHandle, -20, 0x00000080);
            //禁用关闭按钮 https://stackoverflow.com/questions/6052992/how-can-i-disable-close-button-of-console-window-in-a-visual-studio-console-appl 
            EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, (uint)(MF_BYCOMMAND | MF_DISABLED | MF_GRAYED));

            Console.ForegroundColor = ConsoleColor.Green;

            //判断单进程
            if (!SingleInstance.Start())
            {
                Console.WriteLine("The program is already running....");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
                return;
            }

            //图标区图标
            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = Application.ProductName,
                Visible = true
            };
            _notifyIcon.MouseClick += _notifyIcon_MouseClick;

            var menu = new ContextMenu();
            _hideMenu = new MenuItem("Hide", Minimize_Click);
            _restoreMenu = new MenuItem("Restore", Maximize_Click);

            menu.MenuItems.Add(_restoreMenu);
            menu.MenuItems.Add(_hideMenu);
            menu.MenuItems.Add(new MenuItem("Exit", CleanExit));
            _notifyIcon.ContextMenu = menu;

            //隐藏主窗口
            ShowConsoleWindow(false);

            Console.WriteLine("This application will save 0daydown articles to WizNote.");
            Console.WriteLine("You may see Chromium debugging output, please wait...");
            //Console.WriteLine("Press <Q> to exit... ");
            Console.WriteLine();
            Console.ResetColor();

            //配置wizdb
            ZeroDayDownArticle.WizDbPath = System.Configuration.ConfigurationManager.AppSettings["IndexDbPath"];
            ZeroDayDownArticle.WizDefaultFolder = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];

            //配置cef
            var settings = new CefSettings
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CEF")
            };
            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);


            // 初始化浏览器
            _mainBrowser = new MainBrowser();
            var interval = mySharedLib.Utility.GetSetting("SearchInterval", 15);//分钟
            var t = new Timer(TimerCallback, null, 10 * 1000, interval * 60 * 1000);


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

            SingleInstance.Stop();
        }

        private static void _notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            ShowConsoleWindow(_restoreMenu.Enabled);
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
            _notifyIcon.Visible = false;
            Application.Exit();
        }


        static void Minimize_Click(object sender, EventArgs e)
        {
            ShowConsoleWindow(false);
        }


        static void Maximize_Click(object sender, EventArgs e)
        {
            ShowConsoleWindow();
        }

        static void ShowConsoleWindow(bool bShow = true)
        {
            if (bShow)
            {
                _restoreMenu.Enabled = false;
                _hideMenu.Enabled = true;
                ShowWindow(_consoleWindowHandle, 1);
            }
            else
            {
                _restoreMenu.Enabled = true;
                _hideMenu.Enabled = false;
                ShowWindow(_consoleWindowHandle, 0);
            }
        }
    }
}
