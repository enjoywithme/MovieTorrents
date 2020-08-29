using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using CefSharp;
using CefSharp.OffScreen;
using ZeroDownLib;

namespace ZeroDown2Wiz
{
    class Program
    {
        private static MainBrowser mainBrowser;
        

        static void Main(string[] args)
        {

            Console.WriteLine("This application will save 0daydown articles to WizNote.");
            Console.WriteLine("You may see Chromium debugging output, please wait...");
            Console.WriteLine();

            //配置wizdb
            ZeroDayDownArticle.WizDbPath = System.Configuration.ConfigurationManager.AppSettings["IndexDbPath"];
            ZeroDayDownArticle.WizDefaultFolder = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];



            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            // Load main browser to start page
            mainBrowser = new MainBrowser();
            mainBrowser.StartSearch();
            Console.WriteLine($"Found {mainBrowser.Articles.Count} needed to save!");

            // We have to wait for something, otherwise the process will exit too soon.
            Console.ReadKey();

            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();

        }

        

        

    }
}
