using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTorrents
{
    //The code below is from https://www.codeproject.com/Articles/32908/C-Single-Instance-App-With-the-Ability-To-Restore
    public static class SingleInstance
    {
        public static readonly int WM_SHOWFIRSTINSTANCE = WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ProgramInfo.AssemblyGuid);
        static Mutex _mutex;
        public static bool Start()
        {
            var mutexName = $"Local\\{ProgramInfo.AssemblyGuid}";

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);

            _mutex = new Mutex(true, mutexName, out var onlyInstance);
            return onlyInstance;
        }
        public static void ShowFirstInstance()
        {
            WinApi.PostMessage(
                (IntPtr)WinApi.HWND_BROADCAST,
                WM_SHOWFIRSTINSTANCE,
                IntPtr.Zero,
                IntPtr.Zero);
        }
        public static void Stop()
        {
            _mutex.ReleaseMutex();
        }
    }

    public static class WinApi
    {
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        public static int RegisterWindowMessage(string format, params object[] args)
        {
            var message = string.Format(format, args);
            return RegisterWindowMessage(message);
        }

        public const int HWND_BROADCAST = 0xffff;
        public const int SW_SHOWNORMAL = 1;
        public const int ASFW_ANY = -1;
        public const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
        public const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
        public const uint SPIF_SENDWININICHANGE = 0x02;
        public const uint SPIF_UPDATEINIFILE = 0x01;

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImportAttribute("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);
        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo,bool fAttach);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfoGet(uint action, uint param, ref uint vparam, uint init);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfoSet(uint action, uint param, uint vparam, uint init);
        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int dwProcessId);

        public static void ShowToFront(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_SHOWNORMAL);
            SetForegroundWindow(hWnd);
            SwitchToThisWindow(hWnd, true);
        }

        //https://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo
        //因为SetForegroudWindow 的使用限制，如果进程不是当前前景进程，上面的showToFront 只会让程序在任务栏上闪烁
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow
        public static void ForceShowToFront(IntPtr hWnd)
        {
            //ShowWindow(hWnd, SW_SHOWNORMAL);

            //relation time of SetForegroundWindow lock
            uint lockTimeOut = 0;
            var hCurrWnd = GetForegroundWindow();
            uint dwThisTid = GetCurrentThreadId(),
            dwCurrTid = GetWindowThreadProcessId(hCurrWnd, IntPtr.Zero);

            //we need to bypass some limitations from Microsoft :)
            if (dwThisTid != dwCurrTid)
            {

                AttachThreadInput(dwCurrTid, dwThisTid,true);
                SystemParametersInfoGet(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, ref lockTimeOut, 0);
                SystemParametersInfoSet(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, 0, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE);

                AllowSetForegroundWindow(ASFW_ANY);
            }


            SetForegroundWindow(hWnd);

            if (dwThisTid != dwCurrTid)
            {

                SystemParametersInfoSet(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, lockTimeOut, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE);

                AttachThreadInput( dwCurrTid, dwThisTid, false);
            }

            ShowWindow(hWnd, SW_SHOWNORMAL);
        }




        public static bool IsActive(IntPtr handle)
        {
            var activeHandle = GetForegroundWindow();
            return (activeHandle == handle);
        }
    }

    public static class ProgramInfo
    {
        public static string AssemblyGuid
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
            }
        }
        public static string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }

    }
}