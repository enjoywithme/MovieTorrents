using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Unicode;
using System.Windows.Forms;
using MovieTorrents.Common;
using mySharedLib;

namespace MovieTorrents
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!SingleInstance.InitInstance(out var message))
            {
                ShowError(message);
                return;
            }

            if (!SingleInstance.Instance.Start())
            {
                SingleInstance.Instance.ShowFirstInstance();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //初始化设置
            var dataPath = System.Configuration.ConfigurationManager.AppSettings["DataPath"];

            if (!MyMtSettings.InitInstance(dataPath, out message))
            {
                MessageBox.Show(message, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!TorrentFile.CheckTorrentPath(out message, dataPath))
            {
                MessageBox.Show(message, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try
            {
                Application.Run(new FormMain());

            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }

            MyMtSettings.Instance?.UnRegisterMonitor();
            MyMtSettings.Instance?.Save();
            SingleInstance.Instance.Stop();

        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().Location;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
