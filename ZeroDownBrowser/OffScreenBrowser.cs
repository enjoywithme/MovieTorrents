using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Html.Parser;
using CefSharp;
using mySharedLib;
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
        private readonly string _clipperJs;


        public List<ZeroDayDownArticle> Articles = new List<ZeroDayDownArticle>();
        public static bool ContinueSearch = true;

        public OffScreenBrowser()
        {
            // Create the offscreen Chromium browser.
            _browser = new CefSharp.OffScreen.ChromiumWebBrowser();
            _browser.FrameLoadEnd += Browser_FrameLoadEnd;
            _browser.BrowserInitialized += Browser_BrowserInitialized;

            _maxSearchPages = Utility.GetSetting<int>("MaxSearchPage", 10);
            _clipperJs = File.ReadAllText(Path.Combine(Utility.ExecutingAssemblyPath(), "myClipper.js"));

            //Log("Created!");

        }

        private void Browser_BrowserInitialized(object sender, EventArgs e)
        {
            //Log("initialized!");

            _browserInitialized = true;
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //Log("Load ended!");

            var match = Regex.Match(e.Url, "0daydown.com/[\\d]+/.+\\.html");
            if (match.Success)//文章页
            {
                var mainFrame = _browser.GetBrowser().MainFrame;
                var url = mainFrame.Url;

                mainFrame.EvaluateScriptAsync(_clipperJs, null).ContinueWith(t =>
                {
                    if (t.IsFaulted) return;
                    if (BrowserForm.ApplicationExiting)
                    {
                        _isLoading = false;
                        return;
                    }

                    if (t.Result?.Result == null) return;
                    var html = Regex.Replace((string)t.Result.Result, "&quot;", "'");
                    var article = new ZeroDayDownArticle(html, url);
                    try
                    {
                        article.SaveToWiz();
                        Log($"[OK] 保存成功！{article.WizFolderLocation} {article.Title}");

                    }
                    catch (Exception exception)
                    {
                        Log($"[Error] 保存{article.Title} 失败：{exception.Message}");

                    }
                    _isLoading = false;


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
