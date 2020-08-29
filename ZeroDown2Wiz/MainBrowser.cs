using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using CefSharp;
using CefSharp.OffScreen;
using ZeroDownLib;

namespace ZeroDown2Wiz
{
    public class MainBrowser
    {
        private readonly ChromiumWebBrowser browser;
        private bool _browserInitialized;
        private bool _isLoading;//ChromiumWebBrowser.IsLoading有点不靠谱

        private int currentPage = 1;

        const string startUrl = "https:/www.0daydown.com/";

        public List<ZeroDayDownArticle> Articles = new List<ZeroDayDownArticle>();
        public static bool ContinueSearch = true;

        public MainBrowser()
        {
            // Create the offscreen Chromium browser.
            browser = new ChromiumWebBrowser();
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.BrowserInitialized += Browser_BrowserInitialized;
            // An event that is fired when the first page is finished loading.
            // This returns to us from another thread.
            //browser.LoadingStateChanged += BrowserLoadingStateChanged;

           Log("Created!");

        }

        private void Browser_BrowserInitialized(object sender, EventArgs e)
        {
            Log("initialized!");

            _browserInitialized = true;
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Log("Load ended!");

            e.Frame.GetSourceAsync().ContinueWith(t =>
            {
                var htmlSource = t.Result;
                //File.WriteAllText("d:\\temp\\3.text",htmlSource,Encoding.UTF8);

                var parser = new HtmlParser();
                var document = parser.ParseDocument(htmlSource);
                var aLinks = document.All.Where(a => a.LocalName == "a" && a.ParentElement.LocalName == "h2").ToArray();
                foreach (var aLink in aLinks)
                {
                    var article = new ZeroDayDownArticle
                    {
                        Url = aLink.Attributes["href"].Value, Title = aLink.InnerHtml
                    };
                    Log($"{article.Title} {article.Url}");

                    if (article.ExistsInWizDb())
                    {
                        ContinueSearch = false;
                        Log("--------------SAVED---------");
                    }
                    else if(article.SimilarInWizDb()) Articles.Add(article);

                }

                _isLoading = false;
            });
        }

        private void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss}\tMain browser:{msg}");
        }
        public void LoadUrl(string url)
        {
            while (_isLoading || !_browserInitialized)
            {

                Thread.Sleep(1000);
            }
            Log($"Load {url}");
            _isLoading = true;
            browser.Load(url);
        }

        public void StartSearch()
        {
            ContinueSearch = true;
            Articles.Clear();
            while (ContinueSearch)
            {
                var url = currentPage == 1 ? startUrl : $"{startUrl}/page/{currentPage}";
                LoadUrl(url);
                Log($"Page {currentPage}");

                currentPage++;
                if (currentPage == 10) break;
            }
            while (_isLoading || !_browserInitialized)
            {

                Thread.Sleep(1000);
            }
        }
    }
}
