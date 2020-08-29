﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using CefSharp;
using CefSharp.OffScreen;
using ZeroDownLib;
using System.Windows.Forms;

namespace ZeroDown2Wiz
{
    public class MainBrowser
    {
        private readonly ChromiumWebBrowser _browser;
        private bool _browserInitialized;
        private bool _isLoading;//ChromiumWebBrowser.IsLoading有点不靠谱

        private int maxSearchPages = 5;
        private int _currentPage = 1;

        const string startUrl = "https:/www.0daydown.com";

        public List<ZeroDayDownArticle> Articles = new List<ZeroDayDownArticle>();
        public static bool ContinueSearch = true;

        public MainBrowser()
        {
            // Create the offscreen Chromium browser.
            _browser = new ChromiumWebBrowser();
            _browser.FrameLoadEnd += Browser_FrameLoadEnd;
            _browser.BrowserInitialized += Browser_BrowserInitialized;

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

            var match = Regex.Match(e.Url, "0daydown.com/[\\d]+/.+\\.html");
            if (match.Success)//文章页
            {
                var mainFrame = _browser.GetBrowser().MainFrame;

                mainFrame.EvaluateScriptAsync(@"(function() {
            var s = document.getSelection();
            if(s.rangeCount > 0) s.removeAllRanges();


            var heads = document.getElementsByClassName('article-header');

            var articles = document.getElementsByClassName('article-content');

            if(heads.length>0 && articles.length>0)
            {
                var range = document.createRange();
                range.setStartBefore(heads[0]);
                range.setEndAfter(articles[0]);
                s.addRange(range);
                /* Copy the text inside the text field */
                document.execCommand(""copy"");
            }

            return 'ok'; })();",
                    null).ContinueWith(t =>
                {
                    if (t.IsFaulted) return;
                    //Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                    //Clipboard.Clear();

                    mainFrame.Copy();
                    //在非主线程中访问剪贴板
                    //https://stackoverflow.com/questions/47344799/how-to-get-clipboard-data-in-non-main-thread
                    var staThread = new Thread(
                        delegate ()
                        {
                            var article = new ZeroDayDownArticle();
                            var ok = article.SaveFromClipboard(out var msg);
                            Log($"{ok} {msg} {article.WizFolderLocation}/{article.Title}");
                            _isLoading = false;
                        });
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();

                });

            }
            else
            {
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
                            Url = aLink.Attributes["href"].Value,
                            Title = aLink.InnerHtml
                        };
                        Log($"{article.Title} {article.Url}");

                        if (article.ExistsInWizDb())
                        {
                            ContinueSearch = false;
                            Log("--------------SAVED---------");
                        }
                        else if (article.SimilarInWizDb()) Articles.Add(article);

                    }

                    _isLoading = false;
                });
            }
            
        }

        private void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss}\tBrowser:{msg}");
        }

        private void WaitIdle()
        {
            while (_isLoading || !_browserInitialized)
            {
                Thread.Sleep(1000);
            }
        }
        public void LoadUrl(string url)
        {
            WaitIdle();

            Log($"Load {url}");
            _isLoading = true;
            _browser.Load(url);
        }

        public void StartSearch()
        {
            //搜索文章
            ContinueSearch = true;
            _currentPage = 0;
            Articles.Clear();
            while (ContinueSearch)
            {
                Log($"Page {_currentPage}");

                var url = _currentPage == 1 ? startUrl : $"{startUrl}/page/{_currentPage}";
                LoadUrl(url);
                
                WaitIdle();

                _currentPage++;
                if (_currentPage == maxSearchPages) break;
            }

            WaitIdle();

            Log($"-------- {Articles.Count} articles to download---------");
            if(Articles.Count==0) return;

            //下载文章
            foreach (var article in Articles)
            {
                WaitIdle();

                LoadUrl(article.Url);
            }
            WaitIdle();
        }
    }
}
