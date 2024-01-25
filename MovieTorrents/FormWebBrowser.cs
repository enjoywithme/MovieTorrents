// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using MovieTorrents.Common;
using static System.String;

namespace MovieTorrents
{
    public partial class FormWebBrowser : Form
    {
        private readonly string _startUrl;
        private bool _downloadingPoster;

        public DouBanSubject DouBanSubject { get; private set; }
        public FormWebBrowser(string startUrl)
        {
            _startUrl = startUrl;
            InitializeComponent();
            AttachControlEventHandlers(this.webView2Control);
            HandleResize();
        }

        private void FormWebBrowser_Load(object sender, EventArgs e)
        {
            DouBanSubject = null;
            txtUrl.Text= _startUrl;
            GoUrl(_startUrl);
        }

        private void UpdateTitleWithEvent(string message)
        {
            string currentDocumentTitle = this.webView2Control?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            this.Text = currentDocumentTitle + " (" + message + ")";
        }

        #region Event Handlers
        private void WebView2Control_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            UpdateTitleWithEvent("NavigationStarting");
        }

        private async void WebView2Control_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            UpdateTitleWithEvent("NavigationCompleted");

            if (_downloadingPoster)
            {
                _downloadingPoster = false;

                //if (webView2Control.Source.AbsolutePath.EndsWith(".webp"))
                try
                {
                    var filename = Path.GetFileName(DouBanSubject.img_url);
                    if (IsNullOrEmpty(filename)) return;
                    var downloadedFilePath = MyMtSettings.Instance.CurrentPath + "\\temp\\" + Path.GetFileNameWithoutExtension(filename) + ".jpg";

                    var imageData = await GetImageBytesAsync();
                    File.WriteAllBytes(downloadedFilePath, imageData);
                    DouBanSubject.img_local = downloadedFilePath;
                }
                catch (Exception)
                {
                    // ignored
                }

                DialogResult = DialogResult.OK;
            }
        }

        private void WebView2Control_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            txtUrl.Text = webView2Control.Source.AbsoluteUri;
        }

        private void WebView2Control_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"WebView2 creation failed with exception = {e.InitializationException}");
                UpdateTitleWithEvent("CoreWebView2InitializationCompleted failed");
                return;
            }

            this.webView2Control.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            this.webView2Control.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            this.webView2Control.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            this.webView2Control.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);
            UpdateTitleWithEvent("CoreWebView2InitializationCompleted succeeded");
        }

        void AttachControlEventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control) {
            control.CoreWebView2InitializationCompleted += WebView2Control_CoreWebView2InitializationCompleted;
            control.NavigationStarting += WebView2Control_NavigationStarting;
            control.NavigationCompleted += WebView2Control_NavigationCompleted;
            control.SourceChanged += WebView2Control_SourceChanged;
            control.KeyDown += WebView2Control_KeyDown;
            control.KeyUp += WebView2Control_KeyUp;
        }

        private void WebView2Control_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateTitleWithEvent($"AcceleratorKeyUp key={e.KeyCode}");
            if (!this.acceleratorKeysEnabledToolStripMenuItem.Checked)
                e.Handled = true;
        }

        private void WebView2Control_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateTitleWithEvent($"AcceleratorKeyDown key={e.KeyCode}");
            if (!this.acceleratorKeysEnabledToolStripMenuItem.Checked)
                e.Handled = true;
        }

        private void CoreWebView2_HistoryChanged(object sender, object e)
        {
            // No explicit check for webView2Control initialization because the events can only start
            // firing after the CoreWebView2 and its events exist for us to subscribe.
            btnBack.Enabled = webView2Control.CoreWebView2.CanGoBack;
            btnForward.Enabled = webView2Control.CoreWebView2.CanGoForward;
            UpdateTitleWithEvent("HistoryChanged");
        }

        private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            this.txtUrl.Text = this.webView2Control.Source.AbsoluteUri;
            UpdateTitleWithEvent("SourceChanged");
        }

        private void CoreWebView2_DocumentTitleChanged(object sender, object e)
        {
            this.Text = this.webView2Control.CoreWebView2.DocumentTitle;
            UpdateTitleWithEvent("DocumentTitleChanged");
        }
        #endregion

        #region UI event handlers
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            webView2Control.Reload();
        }

        private void GoUrl(string rawUrl)
        {
            Uri uri;

            if (Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute))
            {
                uri = new Uri(rawUrl);
            }
            else if (!rawUrl.Contains(" ") && rawUrl.Contains("."))
            {
                // An invalid URI contains a dot and no spaces, try tacking http:// on the front.
                uri = new Uri("http://" + rawUrl);
            }
            else
            {
                // Otherwise treat it as a web search.
                uri = new Uri("https://bing.com/search?q=" +
                              Join("+", Uri.EscapeDataString(rawUrl).Split(new[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
            }

            webView2Control.Source = uri;
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            GoUrl(txtUrl.Text);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            webView2Control.GoBack();
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            var html = await webView2Control.CoreWebView2.ExecuteScriptAsync("document.body.outerHTML");
            html = Regex.Unescape(html);
#if DEBUG
            //File.WriteAllText("d:\\temp\\2.txt",html);

#endif
            try
            {
                DouBanSubject = DouBanSubject.InitFromPageHtml(webView2Control.Source.AbsoluteUri, html);
                if(IsNullOrEmpty(DouBanSubject.img_local) && !IsNullOrEmpty(DouBanSubject?.img_url))
                {

                    //从浏览器中下载 https://stackoverflow.com/questions/76647184/how-to-save-image-in-microsoft-webview2-page-to-local-file/77003697#77003697

                    _downloadingPoster = true;
                    webView2Control.Source = new Uri(DouBanSubject.img_url);
                }
                else
                {
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Resource.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        /// <summary>
        /// Get raw data (bytes) about an image in an "img" html element 
        /// where id is indicated by "elementId". 
        /// If "elementId" is null, the first "img" element in the page is used 
        /// </summary>
        async Task<byte[]> GetImageBytesAsync(string elementId = null, bool debug = false)
        {
            var script = @"
function getImageAsBase64(imgElementId)
{
    " + (debug ? "debugger;" : "") + @"
    let img = document.getElementById(imgElementId);
    if (imgElementId == '')
    {
        var results = document.evaluate('//img', document, null, XPathResult.ANY_TYPE, null);
        img = results.iterateNext();
    }
    let canvas = document.createElement('canvas');
    canvas.width = img.naturalWidth;
    canvas.height = img.naturalHeight;

    let ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0, img.naturalWidth, img.naturalHeight);

    let base64String = canvas.toDataURL('image/jpeg');  // or 'image/png'
    return base64String;
};
getImageAsBase64('" + elementId + "')";
            var base64Data = await webView2Control.ExecuteScriptAsync(script);
            base64Data = base64Data.Split(new[] { "base64," }, StringSplitOptions.RemoveEmptyEntries)[1].TrimEnd('"');
            var result = Convert.FromBase64String(base64Data);
            return result;
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            webView2Control.GoForward();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            HandleResize();
        }

        private void xToolStripMenuItem05_Click(object sender, EventArgs e)
        {
            this.webView2Control.ZoomFactor = 0.5;
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webView2Control.ZoomFactor = 1.0;
        }

        private void xToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            webView2Control.ZoomFactor = 2.0;
        }

        private void xToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Zoom factor: {this.webView2Control.ZoomFactor}", "WebView Zoom factor");
        }

        private void backgroundColorMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var backgroundColor = Color.FromName(menuItem.Text);
            webView2Control.DefaultBackgroundColor = backgroundColor;
        }

        private void allowExternalDropMenuItem_Click(object sender, EventArgs e)
        {
            webView2Control.AllowExternalDrop = this.allowExternalDropMenuItem.Checked;
        }
        #endregion

        private void HandleResize()
        {
            // Resize the webview
            webView2Control.Size = ClientSize - new System.Drawing.Size(webView2Control.Location);

            btnOk.Left = ClientSize.Width - btnOk.Size.Width;
            btnGo.Left = btnOk.Left - btnGo.Size.Width;

            // Resize the URL textbox
            txtUrl.Width = btnGo.Left - txtUrl.Left;
        }

        
    }
}
