using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Html.Parser;
using CefSharp;
using ZeroDownLib;

namespace ZeroDownBrowser
{
    public class OffScreenBrowser:IDisposable
    {
        private readonly CefSharp.OffScreen.ChromiumWebBrowser _browser;
        private bool _browserInitialized;
        private bool _isLoading;//ChromiumWebBrowser.IsLoading有点不靠谱

        private readonly int _maxSearchPages;
        private int _currentPage = 1;

        public List<ZeroDayDownArticle> Articles = new List<ZeroDayDownArticle>();
        public static bool ContinueSearch = true;

        public OffScreenBrowser()
        {
            // Create the offscreen Chromium browser.
            _browser = new CefSharp.OffScreen.ChromiumWebBrowser();
            _browser.FrameLoadEnd += Browser_FrameLoadEnd;
            _browser.BrowserInitialized += Browser_BrowserInitialized;

            _maxSearchPages = mySharedLib.Utility.GetSetting<int>("MaxSearchPage", 10);

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
                    if (BrowserForm.ApplicationExiting)
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
                                Log($"[Error] {msg}");
                            }
                            else
                            {
                                Log($"[Ok] {msg} {article.WizFolderLocation}/{article.Title}");

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
                            if (BrowserForm.ApplicationExiting)
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
                                Log($"[SAVED] {article.Title}");

                            }
                            else if (article.SimilarInWizDb())
                            {
                                Log($"{article.Title}");
                                Articles.Add(article);
                            }
                            else
                            {
                                Log($"{article.Title}");
                            }
                        }

                    }
                    catch (Exception exception)
                    {
                        Log($"=====Error====={exception.Message}");
                    }

                    _isLoading = false;
                });
            }

        }

        private void Log(string msg)
        {
            mySharedLib.MyLog.Log(msg);
        }

        private bool WaitIdle()
        {
            while (_isLoading || !_browserInitialized)
            {
                Thread.Sleep(1000);
            }

            return !BrowserForm.ApplicationExiting;
        }
        public void LoadUrl(string url)
        {
            if (!WaitIdle()) return;

            Log($"Load {url}");
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
                Log($"-----Page {_currentPage}-------");

                var url = _currentPage == 1 ? Program.ZeroDownHomeUrl : $"{Program.ZeroDownHomeUrl}/page/{_currentPage}";
                LoadUrl(url);

                if (!WaitIdle()) return;

                _currentPage++;
                if (_currentPage == _maxSearchPages) break;
            }

            if (!WaitIdle()) return;

            Log($"-------- {Articles.Count} articles to download---------");
            if (Articles.Count == 0) return;

            //下载文章
            foreach (var article in Articles)
            {
                if (!WaitIdle()) return;

                LoadUrl(article.Url);
            }

            WaitIdle();

        }

        public void Dispose()
        {
            _browser?.Dispose();
        }
    }
}
