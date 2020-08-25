// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using CefBrowser.Controls;
using CefSharp;
using CefSharp.WinForms;

namespace CefBrowser
{
    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser _browser;
        private string ZerodayDownUrl = "www.0daydown.com";
        private bool _is0DayDown;

        public BrowserForm()
        {
            InitializeComponent();

            Text = "CefSharp";
            WindowState = FormWindowState.Maximized;
            
            var settings = new CefSettings
            {
                CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF"
            };
            Cef.Initialize(settings);

            _browser = new ChromiumWebBrowser(ZerodayDownUrl)
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

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version =
                $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}, Environment: {bitness}";
            DisplayOutput(version);
        }

        private void _browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

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
                _is0DayDown =
                    args.Address.IndexOf(ZerodayDownUrl, StringComparison.InvariantCultureIgnoreCase) >= 0;
                
            });
        }

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

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            _browser.Dispose();
            Cef.Shutdown();
            Close();
        }

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

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                _browser.Load(url);
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            _browser.ShowDevTools();
        }

        private void tsbCapture0DayDown_Click(object sender, EventArgs e)
        {
            var script = @"
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
    
    
    var e = new Event(""keydown"");
    e.key=""s"";    // just enter the char you want to send 
    e.keyCode=e.key.charCodeAt(0);
    e.which=e.keyCode;
    e.altKey=false;
    e.ctrlKey=true;
    e.shiftKey=true;
    e.metaKey=false;
    e.bubbles=true;
    document.dispatchEvent(e);
}
";
            _browser.ExecuteScriptAsync(script);

            SendKeys.Send("+^S");
        }
    }
}
