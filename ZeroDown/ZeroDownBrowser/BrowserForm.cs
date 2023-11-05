// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using mySharedLib;
using ZeroDownBrowser.Controls;
using ZeroDownLib;

namespace ZeroDownBrowser
{
    public partial class BrowserForm : Form
    {
        private ChromiumWebBrowser _mainBrowser;
        private OffScreenBrowser _offScreenBrowser;
        private bool _is0DayDown;

        private System.Threading.Timer _offDownloadTimer;
        private int _searchInterval;
        private int _offDownloadRunning = 0;
        public static bool ApplicationExiting;

        private NotifyIcon _notifyIcon;
        private MenuItem _hideMenu;
        private MenuItem _restoreMenu;

        #region Form主要函数

        public BrowserForm()
        {
            InitializeComponent();
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {
            Text = "0DayDown browser";
            Initialize();
            WindowState = FormWindowState.Maximized;

            FormClosing += BrowserForm_FormClosing;
            FormClosed += BrowserForm_FormClosed;
            Resize += BrowserForm_Resize;

            _offDownloadTimer = new System.Threading.Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            _searchInterval = Utility.GetSetting("SearchInterval", 15) * 60 * 1000;

            //图标区图标
            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = Application.ProductName,
                Visible = true
            };
            _notifyIcon.MouseClick += notifyIcon_MouseClick;

            var menu = new ContextMenu();
            _hideMenu = new MenuItem("Hide", Minimize_Click);
            _restoreMenu = new MenuItem("Restore", Maximize_Click);

            menu.MenuItems.Add(_restoreMenu);
            menu.MenuItems.Add(_hideMenu);
            menu.MenuItems.Add(new MenuItem("Exit", CleanExit));
            _notifyIcon.ContextMenu = menu;

            startAutoSaveToWizNoteToolStripMenuItem.Click += startAutoSaveToWizNoteToolStripMenuItem_Click;
            startAutoSaveToWizNoteToolStripMenuItem.Enabled = true;

            tsbAutoDownloadNow.Click += TsbAutoDownloadNow_Click;
            tsbAutoDownloadNow.Enabled = true;

        }

        private void TsbAutoDownloadNow_Click(object sender, EventArgs e)
        {
            if (0 != Interlocked.Exchange(ref _offDownloadRunning, 1))
            {
                MessageBox.Show("自动下载正在运行，稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            Task.Run(DownloadArticles);
        }

        private void BrowserForm_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized) return;
            ShowWindow(false);
        }

        private void BrowserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _notifyIcon.Visible = false;
            _mainBrowser.Dispose();
            _offScreenBrowser.Dispose();
            Cef.Shutdown();
        }

        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationExiting = true;

            while (0 != Interlocked.Exchange(ref _offDownloadRunning, 1))
            {
                Thread.Sleep(2000);
            }
        }
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                ShowWindow();
            }
            base.WndProc(ref message);
        }
        #endregion


        //初始化
        private void Initialize()
        {
            ZeroDayDownArticle.WizDbPath = Utility.GetSetting("IndexDbPath", "");
            ZeroDayDownArticle.WizDefaultFolder = Utility.GetSetting("DefaultFolder", "");
            ZeroDayDownArticle.MyPageTempPath = Utility.GetSetting("MyPageTempPath", "");
            ZeroDayDownArticle.SaveFormat = Utility.GetSetting("SaveFormat", "");

            var settings = new CefSettings
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CEF")
            };
            settings.CefCommandLineArgs.Add("proxy-server", Utility.GetSetting("proxy-server", ""));
            Cef.Initialize(settings);

            var startUrl = Program.ZeroDownHomeUrl;
#if DEBUG
            //startUrl = "https://www.0daydown.com/08/1394649.html";
#endif
            _mainBrowser = new ChromiumWebBrowser(startUrl)
            {
                Dock = DockStyle.Fill,
            };
            toolStripContainer.ContentPanel.Controls.Add(_mainBrowser);
            _mainBrowser.IsBrowserInitializedChanged += OnIsMainBrowserInitializedChanged;
            _mainBrowser.LoadingStateChanged += OnLoadingStateChanged;
            _mainBrowser.ConsoleMessage += OnMainBrowserConsoleMessage;
            _mainBrowser.StatusMessage += OnMainBrowserStatusMessage;
            _mainBrowser.TitleChanged += OnMainBrowserTitleChanged;
            _mainBrowser.AddressChanged += OnMainBrowserAddressChanged;
            _mainBrowser.FrameLoadEnd += MainBrowserFrameLoadEnd;

            var platform = Environment.Is64BitProcess ? "x64" : "x86";
            var version =
                $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}, Environment: {platform}";
            DisplayOutput(version);

            _offScreenBrowser = new OffScreenBrowser();
        }


        private void TimerCallback(object o)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked?view=netcore-3.1
            //0 indicates that the method is in use.
            if (0 != Interlocked.Exchange(ref _offDownloadRunning, 1))
                return;
            DownloadArticles();
        }

        private void DownloadArticles()
        {
            try
            {
                mySharedLib.MyLog.Log("------------Starting search-------------");
                _offScreenBrowser.StartSearch();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                Interlocked.Exchange(ref _offDownloadRunning, 0);
            }
        }


        #region 浏览器事件
        private void MainBrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var frame = _mainBrowser.GetMainFrame();
            frame.GetSourceAsync().ContinueWith(t =>
            {
                var htmlSource = t.Result;
                //File.WriteAllText("d:\\temp\\3.text",htmlSource,Encoding.UTF8);
                try
                {
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(htmlSource);
                    var aLinks = document.DocumentNode.SelectNodes("//a").Where(a => a.ParentNode.Name == "h2").ToArray();
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
        private void OnIsMainBrowserInitializedChanged(object sender, EventArgs e)
        {

            var b = ((ChromiumWebBrowser)sender);

            this.InvokeOnUiThreadIfRequired(() => b.Focus());
        }

        private void OnMainBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput($"Line: {args.Line}, Source: {args.Source}, Message: {args.Message}");
        }

        private void OnMainBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnMainBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnMainBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                urlTextBox.Text = args.Address;
                var match = Regex.Match(args.Address, Program.ZeroDownPageUrlPattern);
                _is0DayDown = match.Success;

            });
        }
        #endregion

        #region 辅助函数

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                _mainBrowser.Load(url);
            }
        }

        private void ToggleWindowVisible()
        {
            ShowWindow(!Visible);
        }
        private void ShowWindow(bool bShow = true)
        {
            if (bShow)
            {
                _restoreMenu.Enabled = false;
                _hideMenu.Enabled = true;
                Show();
                if (WindowState == FormWindowState.Minimized)
                    WindowState = FormWindowState.Maximized;
            }
            else
            {
                _restoreMenu.Enabled = true;
                _hideMenu.Enabled = false;
                Hide();
            }
        }

        private void ShowLog()
        {
            if (!MyLog.OpenLog(out var msg))
                MessageBox.Show(this, msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region 菜单项


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
        private void startAutoSaveToWizNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (startAutoSaveToWizNoteToolStripMenuItem.Checked)
                _offDownloadTimer.Change(Timeout.Infinite, Timeout.Infinite);
            else
                _offDownloadTimer.Change(5000, _searchInterval);
            startAutoSaveToWizNoteToolStripMenuItem.Checked = !startAutoSaveToWizNoteToolStripMenuItem.Checked;
        }
        private void ExitMenuItemClick(object sender, EventArgs e)
        {

            Close();
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            _mainBrowser.ShowDevTools();
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyLog.ClearLog();
        }

        //图标区
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            ToggleWindowVisible();
        }
        private void CleanExit(object sender, EventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            ShowWindow(false);
        }


        private void Maximize_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }
        #endregion

        #region 工具栏
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
        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            _mainBrowser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            _mainBrowser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }
        //保存到为知笔记
        private async void tsbCapture0DayDown_Click(object sender, EventArgs e)
        {

            var mainFrame = _mainBrowser.GetBrowser().MainFrame;
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

                    MessageBox.Show(this,$"{msg}\r\n{article.Title}\r\n{article.PizFileName}", ok ? "提示" : "错误", MessageBoxButtons.OK,
                        ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                });

            }, TaskScheduler.FromCurrentSynchronizationContext());

            
        }

        private void tsbShowLog_Click(object sender, EventArgs e)
        {
            ShowLog();
        }
        #endregion




    }
}
