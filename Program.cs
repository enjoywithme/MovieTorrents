using System;
using System.Windows.Forms;
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
           
            try
            {
                Application.Run(new FormMain());

            }
            catch (Exception e)
            {
                ShowError(message);
            }


            SingleInstance.Instance.Stop();

        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
