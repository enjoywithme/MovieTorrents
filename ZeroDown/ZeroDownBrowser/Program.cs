using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using mySharedLib;

namespace ZeroDownBrowser
{
    static class Program
    {
        public static string ZeroDownHomeUrl;
        public static string ZeroDownPageUrlPattern;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!SingleInstance.InitInstance(out var message))
            {
                return;
            }

            if (!SingleInstance.Instance.Start())
            {
                SingleInstance.Instance.ShowFirstInstance();
                return;
            }

            //https://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash
            var currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            System.Windows.Forms.Application.ThreadException += GlobalThreadExceptionHandler;

            ZeroDownHomeUrl = Utility.GetSetting("ZeroDownHomeUrl", "www.0daydown.com");
            ZeroDownPageUrlPattern =
                Utility.GetSetting("ZeroDownPageUrlPattern", "0daydown.com/[\\d]+/.+\\.html");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new BrowserForm());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                SingleInstance.Instance.Stop();
            }
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            MyLog.Log(ex.Message + "\n" + ex.StackTrace);
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            MyLog.Log(ex.Message + "\n" + ex.StackTrace);
        }
    }
}
