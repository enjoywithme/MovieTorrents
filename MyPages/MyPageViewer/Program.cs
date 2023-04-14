﻿using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using MyPageViewer.Model;
using mySharedLib;

namespace MyPageViewer
{
    internal static partial class Program
    {
        public const string InstanceGuid = "AC9B6BF8-A4A6-4FAD-AC57-856CB01280C}";


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //Associate .piz extension
            FileAssociations.EnsureAssociationsSet();

            if (!SingleInstance.InitInstance(out var message, InstanceGuid))
            {
                ShowError(message);
                return;
            }

            //Single instance
            var myPageDoc = MyPageDocument.NewFromArgs(Environment.GetCommandLineArgs());
            if (!SingleInstance.Instance.Start())
            {
                SingleInstance.Instance.ShowFirstInstance(myPageDoc?.FilePath);
                return;
            }

            //run main form
            FormMain.Instance = new FormMain(myPageDoc);
            Application.Run(FormMain.Instance);

            //Task.Delay(1000).Wait();

            //Save settings
            MyPageSettings.Instance.Save();

            SingleInstance.Instance.Stop();
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, Properties.Resources.Text_Hint, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        public static void ShowError(string message)
        {
            MessageBox.Show(message, Properties.Resources.Text_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


    }
}