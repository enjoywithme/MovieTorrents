using System;
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

        private readonly int _maxSearchPages;
        private int _currentPage = 1;

        const string StartUrl = "https:/www.0daydown.com";

        public List<ZeroDayDownArticle> Articles = new List<ZeroDayDownArticle>();
        public static bool ContinueSearch = true;

        public MainBrowser()
        {
            // Create the offscreen Chromium browser.
            _browser = new ChromiumWebBrowser();
            _browser.FrameLoadEnd += Browser_FrameLoadEnd;
            _browser.BrowserInitialized += Browser_BrowserInitialized;

            _maxSearchPages = mySharedLib.Utility.GetSetting<int>("MaxSearchPage", 10);

            Log("Created!\r\n");

        }

        private void Browser_BrowserInitialized(object sender, EventArgs e)
        {
            Log("initialized!\r\n");

            _browserInitialized = true;
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Log("Load ended!\r\n");

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
                    if (Program.ApplicationExiting)
                    {
                        _isLoading = false;
                        return;
                    }
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
                            if (!ok)
                            {
                                Log($"[Error]", ConsoleColor.Red, ConsoleColor.White);
                                Log($"{msg}\r\n");
                            }
                            else
                            {
                                Log($"[Ok]", ConsoleColor.DarkGreen, ConsoleColor.White);
                                Log($"{msg} {article.WizFolderLocation}/{article.Title}\r\n");

                            }
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
                    try
                    {
                        var parser = new HtmlParser();
                        var document = parser.ParseDocument(htmlSource);
                        var aLinks = document.All.Where(a => a.LocalName == "a" && a.ParentElement.LocalName == "h2").ToArray();
                        foreach (var aLink in aLinks)
                        {
                            if (Program.ApplicationExiting)
                            {
                                _isLoading = false;
                                return;
                            }
                            var article = new ZeroDayDownArticle
                            {
                                Url = aLink.Attributes["href"].Value,
                                Title = aLink.InnerHtml
                            };

                            if (article.ExistsInWizDb())
                            {
                                ContinueSearch = false;
                                Log("[SAVED]", ConsoleColor.DarkRed, ConsoleColor.DarkYellow);
                                Log($"{article.Title}\r\n", printHeader: false);

                            }
                            else if (article.SimilarInWizDb())
                            {
                                Log($"{article.Title}\r\n", ConsoleColor.Green);
                                Articles.Add(article);
                            }
                            else
                            {
                                Log($"{article.Title}\r\n");
                            }
                        }

                    }
                    catch (Exception exception)
                    {
                        Log($"=====Error====={exception.Message}\r\n", ConsoleColor.White, ConsoleColor.DarkRed);
                    }

                    _isLoading = false;
                });
            }

        }

        private void Log(string msg, ConsoleColor foreColor = ConsoleColor.White,
            ConsoleColor bgColor = ConsoleColor.Black, bool printHeader = true)
        {
            if (printHeader)
                Console.Write($"{DateTime.Now:hh:mm:ss}\tBrowser:");
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = bgColor;
            Console.Write(msg);
            Console.ResetColor();
        }

        private bool WaitIdle()
        {
            while (_isLoading || !_browserInitialized)
            {
                Thread.Sleep(1000);
            }

            return !Program.ApplicationExiting;
        }
        public void LoadUrl(string url)
        {
            if(!WaitIdle()) return;

            Log($"Load {url}\r\n", ConsoleColor.Blue);
            _isLoading = true;
            _browser.Load(url);
        }

        public void StartSearch()
        {
            //搜索文章
            ContinueSearch = true;
            _currentPage = 1;
            Articles.Clear();
            while (ContinueSearch)
            {
                Log($"-----Page {_currentPage}-------\r\n", ConsoleColor.Cyan);

                var url = _currentPage == 1 ? StartUrl : $"{StartUrl}/page/{_currentPage}";
                LoadUrl(url);

            if(!WaitIdle()) return;

                _currentPage++;
                if (_currentPage == _maxSearchPages) break;
            }

            if (!WaitIdle()) return;

            Log($"-------- {Articles.Count} articles to download---------\r\n", ConsoleColor.DarkRed);
            if (Articles.Count == 0) return;

            //下载文章
            foreach (var article in Articles)
            {
                if (!WaitIdle()) return;

                LoadUrl(article.Url);
            }

            WaitIdle();

        }
    }
}
