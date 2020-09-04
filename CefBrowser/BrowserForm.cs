// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using AngleSharp.Html.Parser;
using CefBrowser.Controls;
using CefSharp;
using CefSharp.WinForms;
using ZeroDownLib;

namespace CefBrowser
{
    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser _browser;
        private string _zerodayDownUrl = "www.0daydown.com";
        private const string ZeroDownPageUrlPattern = "0daydown.com/[\\d]+/.+\\.html";
        private bool _is0DayDown;

        public BrowserForm()
        {
            InitializeComponent();

            Text = "CefSharp";
            WindowState = FormWindowState.Maximized;
            
            var settings = new CefSettings
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CEF")
            };
            Cef.Initialize(settings);
            var startUrl = "https://0daydown.com";
#if DEBUG
            startUrl = "https://www.0daydown.com/08/1394649.html";
#endif
            _browser = new ChromiumWebBrowser(startUrl)
            {
                Dock = DockStyle.Fill,
            };
            toolStripContainer.ContentPanel.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.LoadingStateChanged += OnLoadingStateChanged;
            _browser.ConsoleMessage += OnBrowserConsoleMessage;
            _browser.StatusMessage += OnBrowserStatusMessage;
            _browser.TitleChanged += OnBrowserTitleChanged;
            _browser.AddressChanged += OnBrowserAddressChanged;
            _browser.FrameLoadEnd += _browser_FrameLoadEnd;

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version =
                $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}, Environment: {bitness}";
            DisplayOutput(version);
        }

        private void _browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var frame = _browser.GetMainFrame();
            frame.GetSourceAsync().ContinueWith(t =>
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
                        
                        var article = new ZeroDayDownArticle
                        {
                            Url = aLink.Attributes["href"].Value,
                            Title = aLink.InnerHtml
                        };

                        if (article.ExistsInWizDb())
                        {
                            var script = @"$('a').each(function(index,el) 
{if($(el).html()==('" + article.Title + @"')) 
$(el).css({'background-color':'green','color':'blue'});});";
                            frame.ExecuteJavaScriptAsync(script);
                        }
                        else if (article.SimilarInWizDb())
                        {
                            var script = @"$('a').each(function(index,el) 
{if($(el).html()==('" + article.Title + @"')) 
$(el).css({'background-color':'yellow','color':'red'});});";
                            frame.ExecuteJavaScriptAsync(script);
                        }
                        else
                        {
                        }
                    }

                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"=====Error====={exception.Message}\r\n");
                }
            });
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {
            ZeroDayDownArticle.WizDbPath = System.Configuration.ConfigurationManager.AppSettings["IndexDbPath"];
            ZeroDayDownArticle.WizDefaultFolder = System.Configuration.ConfigurationManager.AppSettings["DefaultFolder"];


        }

        #region 浏览器事件
        private void OnIsBrowserInitializedChanged(object sender, EventArgs e)
        {
            
                var b = ((ChromiumWebBrowser)sender);

                this.InvokeOnUiThreadIfRequired(() => b.Focus());
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput($"Line: {args.Line}, Source: {args.Source}, Message: {args.Message}");
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                urlTextBox.Text = args.Address;
                var match = Regex.Match(args.Address, ZeroDownPageUrlPattern);
                _is0DayDown =match.Success;
                
            });
        }
        #endregion

        #region 工具栏按钮/菜单项
        //设置工具栏按钮状态
        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                Properties.Resources.nav_plain_red :
                Properties.Resources.nav_plain_green;
            tsbCapture0DayDown.Enabled = !isLoading && _is0DayDown;

            HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }
        //菜单
        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            _browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            _browser.ShowDevTools();
        }

        //工具栏
        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            _browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            _browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }        

        #endregion

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                _browser.Load(url);
            }
        }



        private async void tsbCapture0DayDown_Click(object sender, EventArgs e)
        {

            var mainFrame = _browser.GetBrowser().MainFrame;
            //https://stackoverflow.com/questions/35890355/get-html-source-code-from-cefsharp-web-browser
            //var htmlSource = await mainFrame.GetSourceAsync();
            //Debug.WriteLine(htmlSource);
            //File.WriteAllText("d:\\temp\\2.txt", htmlSource, Encoding.UTF8);

            //var parser = new HtmlParser();
            //var document = parser.ParseDocument(htmlSource);

            //https://stackoverflow.com/questions/50688705/how-to-access-elements-with-cefsharp
            //string EvaluateJavaScriptResult;

            var task = mainFrame.EvaluateScriptAsync(@"(function() {
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
                null);

            await task.ContinueWith(t =>
            {
                if (t.IsFaulted) return;
                //var response = t.Result;
                //EvaluateJavaScriptResult = response.Success ? (response.Result.ToString() ?? "") : response.Message;
                Clipboard.Clear();

                //使用拷贝是因为拷贝的HTML会自动展开Stylesheet为内嵌
                mainFrame.Copy();

                //等待500ms执行，否则剪贴板可能没有数据
                //不能使用Thread.SpinWait()，这个空转没有用
                //部门使用Thread.Sleep()，会导致访问剪贴板死锁
                mySharedLib.Utility.DelayAction(500, () =>
                {
                    var article = new ZeroDayDownArticle();
                    var ok = article.SaveFromClipboard(out var msg);

                    MessageBox.Show($"{msg}\r\n{article.WizFolderLocation}\r\n{article.Title}", ok ? "提示" : "错误", MessageBoxButtons.OK,
                        ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                });

            }, TaskScheduler.FromCurrentSynchronizationContext());

            
        }

        
    }
}
