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
            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

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
                SingleInstance.Stop();
            }
        }
    }
}
