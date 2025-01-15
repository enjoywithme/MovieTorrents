using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using MovieTorrents.Common;
using mySharedLib;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace MovieTorrents
{
    public partial class FormBtBtt : Form
    {
        private bool _isWorking;

        private IList<string> _threads;
        private int _threadIndex;
        private IList<BtBtItem> _btItems;
        /// <summary>
        /// 准备要下载的附件URL
        /// </summary>
        private readonly List<DownloadItem> _itemsToDownload = [];
        CoreWebView2Environment _environment;
        private string _currentPageUrl;

        private string _webViewUserDataFolder;

        public FormBtBtt()
        {
            InitializeComponent();
        }

        private async void FormBtBtt_Load(object sender, EventArgs e)
        {
            tbUrl.Text = MyMtSettings.Instance.BtBtHomeUrl;
            _webViewUserDataFolder = Path.Combine(Program.AssemblyDirectory, "webViewCache\\btbt\\");


            if (!string.IsNullOrEmpty(MyMtSettings.Instance.WebProxy))
            {
                var options = new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = $"--proxy-server={MyMtSettings.Instance.WebProxy}"
                };
                _environment =
                    await CoreWebView2Environment.CreateAsync(null, _webViewUserDataFolder, options);

            }
            else
            {
                _environment = await CoreWebView2Environment.CreateAsync(null,_webViewUserDataFolder);

            }


            await webView21.EnsureCoreWebView2Async(_environment);
            webView21.CoreWebView2.Profile.DefaultDownloadFolderPath = MyMtSettings.Instance.DownLoadRootPath;

            webView21.CoreWebView2.AddWebResourceRequestedFilter("http*", CoreWebView2WebResourceContext.Image);
            webView21.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webView21.NavigationCompleted += WebView21_NavigationCompleted;
            webView21.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;

            btArchiveTorrent.Click += BtArchiveTorrent_Click;
#if DEBUG
            tbSearch.Text = "网络寻凶";
#endif
            Resize += FormBtBtt_Resize;
            tbSearch.KeyDown += TbSearch_KeyDown;
            tbSearch.Pasted += TbSearch_Pasted;
            tbUrl.KeyDown += TbUrl_KeyDown;
            btLog.Click += BtLog_Click;
            btClearLog.Click += BtClearLog_Click;
            btHomePage.Click += BtHomePage_Click;
        }

        #region 动作事件

        private void TbSearch_Pasted(object sender, ClipboardEventArgs e)
        {
            tbSearch.Text = e.ClipboardText;
            DoSearch();
        }

        private void FormBtBtt_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void TbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                QueryIndexPage(tbUrl.Text.Trim());
        }

        private void BtLog_Click(object sender, EventArgs e)
        {
            if (!MyLog.OpenLog(out var msg))
                MessageBox.Show(this, msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void TbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return) return;
            DoSearch();
        }

        private void DoSearch()
        {
            if(_isWorking)
                return;

            _isWorking = true;
            _isAutoDownloading = false;

            if (string.IsNullOrEmpty(tbSearch.Text.Trim())) return;
            tbUrl.Text = BtBtItem.SearPageUrl(tbSearch.Text.Trim());
            QueryIndexPage(tbUrl.Text.Trim());
        }

        #endregion




        #region WebView actions
        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            e.Response = _environment.CreateWebResourceResponse(null, 404, "Not found", "");
        }

        /// <summary>
        /// 页面加载结束回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
   

            if (!_isWorking) 
                return;

            var html = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            html = html.Remove(html.Length - 1, 1);


            var url = webView21.CoreWebView2.Source;
            if (url.StartsWith($"{MyMtSettings.Instance.BtBtHomeUrl}index-",
                    StringComparison.InvariantCultureIgnoreCase)
                || string.Compare(url, MyMtSettings.Instance.BtBtHomeUrl,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                //We are query list page
                ProcessIndexPage(html);

            }
            else if (url.StartsWith($"{MyMtSettings.Instance.BtBtHomeUrl}search-"))
            {
                //Search page
                ProcessSearchPage(html);
            }
            else if (url.StartsWith($"{MyMtSettings.Instance.BtBtHomeUrl}thread-",
                         StringComparison.InvariantCultureIgnoreCase))
            {
                //Parse thread page
                ProcessThreadPage(html,url);
            }
            //https://www.1lou.me/attach-download-2096344.htm
            else if (url.StartsWith($"{MyMtSettings.Instance.BtBtHomeUrl}attach-download-",
                         StringComparison.InvariantCultureIgnoreCase))
            {
                CheckDownloadAttachment();
            }

        }

        private void ProcessIndexPage(string html)
        {
            _threads = BtBtItem.ParseIndexPage(html, out var msg);
            if (!string.IsNullOrEmpty(msg))
            {
                if(!_isAutoDownloading)
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (_threads.Count == 0)
            {
                if (_isAutoDownloading)
                {
                    AutoDownloadNextPage();
                } 
                return;
            }


            _threadIndex = 0;
            _btItems = new List<BtBtItem>();
            _currentPageUrl = webView21.CoreWebView2.Source;
            webView21.CoreWebView2?.Navigate($"{MyMtSettings.Instance.BtBtHomeUrl}{_threads[_threadIndex]}");
        }

        private void ProcessSearchPage(string html)
        {
            _threads = BtBtItem.ParseSearchPage(html, out var msg);
            if (!string.IsNullOrEmpty(msg))
            {
                if (!_isAutoDownloading)
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (_threads.Count == 0)
            {
                _isWorking = false;
                return;
            }


            _threadIndex = 0;
            _btItems = new List<BtBtItem>();
            _currentPageUrl = webView21.CoreWebView2.Source;
            webView21.CoreWebView2?.Navigate($"{MyMtSettings.Instance.BtBtHomeUrl}{_threads[_threadIndex]}");
        }

        /// <summary>
        /// 解析帖子页面
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        private void ProcessThreadPage(string html,string url)
        {
            var btItem = BtBtItem.ParseThreadPage(html, url,out var msg);
            if (btItem != null)
            {
                if (_isAutoDownloading)
                {
                    _autoDownloadItems.Add(btItem);
                }
                else
                {
                    _btItems.Add(btItem);

                }
            }

            //next thread
            _threadIndex++;
            if (_threadIndex == _threads.Count)
            {

                if (_isAutoDownloading)
                    AutoDownloadNextPage();
                else
                {

                    AddBtItemsToList();

                    if (!string.IsNullOrEmpty(_currentPageUrl))
                        webView21.CoreWebView2?.Navigate(_currentPageUrl);//get back to index page
                }

                
            }
            else
            {
                webView21.CoreWebView2?.Navigate($"{MyMtSettings.Instance.BtBtHomeUrl}{_threads[_threadIndex]}");

            }
        }

        #endregion



        private void AddBtItemsToList()
        {
            lvResults.Items.Clear();
            if (_btItems == null || _btItems.Count ==0 )
            {
                return;
            }

            foreach (var btItem in _btItems)
            {
                string[] row =
                {
                    btItem.Title,
                    btItem.PublishTime,
                    btItem.DouBanRating,
                    btItem.Gene,
                    btItem.Tag
                };
                lvResults.Items.Add(new ListViewItem(row)
                {
                    Tag = btItem,
                    Checked = btItem.Checked

                });
            }
        }

        /// <summary>
        /// Query index page
        /// </summary>
        /// <param name="url"></param>
        private void QueryIndexPage(string url)
        {
            if (webView21.CoreWebView2 == null)
            {
                _isWorking = false;
                return;
            }
            webView21.CoreWebView2?.Navigate(url);

        }


        #region Button actions

        private void BtHomePage_Click(object sender, EventArgs e)
        {
            tbUrl.Text = MyMtSettings.Instance.BtBtHomeUrl;
            QueryIndexPage(tbUrl.Text.Trim());
        }

        private void BtClearLog_Click(object sender, EventArgs e)
        {
            MyLog.ClearLog();
        }

        //下一页
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (IsWorking()) return;

            tbUrl.Text = BtBtItem.NextPageUrl(tbUrl.Text);
            QueryIndexPage(tbUrl.Text.Trim());
        }

        //上一页
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (IsWorking()) return;

            tbUrl.Text = BtBtItem.PrevPageUrl(tbUrl.Text);
            QueryIndexPage(tbUrl.Text.Trim());

        }

        //下载勾选的种子文件
        private void btDownload_Click(object sender, EventArgs e)
        {
            if (IsWorking()) return;

            if (lvResults.CheckedItems.Count == 0) return;
            //var c = Cursor;
            //Cursor = Cursors.WaitCursor;
            //var message = "";
            //var i = 0;
            var itemsToDownload = new List<BtBtItem>();
            foreach (ListViewItem checkedItem in lvResults.CheckedItems)
            {
                var btItem = (BtBtItem)checkedItem.Tag;

                itemsToDownload.Add(btItem);

                //i += btItem.DownLoadAttachments(out var msg);
                
                //if (!string.IsNullOrEmpty(msg))
                //    message += $"{msg}\r\n";

            }
            //Cursor = c;

            DownloadItemsWithBrowser(itemsToDownload);

            //message = $"下载了{i}个文件。{message}";
            //MessageBox.Show(message, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //Interlocked.Exchange(ref BtBtItem.AutoDownloadRunning, 0);

        }

        #endregion





        private void FormBtBtt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = true;
            Hide();
        }

       
        //检查是否正在自动下载
        private bool IsWorking()
        {
            if (!_isWorking) 
                return false;

            MessageBox.Show(Resource.BTIsWorking, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            return true;


        }

        //将目录下的种子文件转移到收藏目录
        private void BtArchiveTorrent_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FolderWatch.Instance?.Suspend();

            var sb =new StringBuilder();
            sb.AppendLine(BtBtItem.ExtractZipFiles());
            sb.Append(BtBtItem.RenameSpecialFiles());
            sb.Append(BtBtItem.ArchiveTorrentFiles());

            FolderWatch.Instance?.Resume();

            Cursor.Current = Cursors.Default;

            (new DlgMessageBox(sb.ToString(), Resource.TextHint)).ShowDialog(this);
            //MessageBox.Show(sb.ToString(), Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);


        }



        #region 自动定时下载

        private static System.Windows.Forms.Timer _autoDownloadTimer;
        private bool _isAutoDownloading;
        private int _autoDownloadPages;
        private IList<BtBtItem> _autoDownloadItems;

        /// <summary>
        /// 切换自动下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAutoDownload_CheckedChanged(object sender, EventArgs e)
        {
            if (MyMtSettings.Instance.IsCurrentMonitor())
            {
                EnableAutoDownload(cbAutoDownload.Checked);
            }
            else
            {
                if (!cbAutoDownload.Checked) return;
                MessageBox.Show(Resource.BtCurrentPcIsNotMonitor, Resource.TextHint, MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbAutoDownload.Checked = false;

            }
        }

        //启动自动下载
        private void EnableAutoDownload(bool bEnable)
        {
            if (bEnable)
            {
                if (_autoDownloadTimer == null)
                {
                    _autoDownloadTimer = new System.Windows.Forms.Timer();
                    _autoDownloadTimer.Interval = MyMtSettings.Instance.AutoDownloadInterval * 60 *  1000;
                    _autoDownloadTimer.Tick += _autoDownloadTimer_Tick; 
                }

                _autoDownloadTimer.Start();
                _autoDownloadTimer_Tick(null,null);
            }
            else
            {
                if (_autoDownloadTimer == null) 
                    return;

                if (_isWorking && _isAutoDownloading)
                {
                    _isWorking = false;
                    _isAutoDownloading = false;
                }

                _autoDownloadTimer.Stop();


            }
        }

        private void _autoDownloadTimer_Tick(object sender, EventArgs e)
        {
            if (_isWorking)
                return;

            MyLog.Log("=====自动下载开始运行======");
            _isWorking = true;
            _isAutoDownloading = true;
            _autoDownloadPages = 0;
            _autoDownloadItems = new List<BtBtItem>();
            var pageUrl = MyMtSettings.Instance.BtBtHomeUrl;
            QueryIndexPage(pageUrl);
        }



        private bool CheckAutoDownloadStop()
        {
            if (_autoDownloadPages > MyMtSettings.Instance.AutoDownloadSearchPages)
                return true;

            if (_autoDownloadItems == null) return false;


            //如果上次已经有查询，退出
            if (MyMtSettings.Instance.AutoDownloadLastTid != 0
                && MyMtSettings.Instance.AutoDownloadLastPostDateTime != default(DateTime)
                && _autoDownloadItems.Any(x =>
                    x.tid == MyMtSettings.Instance.AutoDownloadLastTid && x.PostDateTime == MyMtSettings.Instance.AutoDownloadLastPostDateTime))
            {

                MyLog.Log("=====已抵达上次搜索文章，退出======");
                return true;
            }

            //如果超过24小时的贴，退出
            var now = DateTime.Now;
            if (_autoDownloadItems.Any(x =>
                    x.PostDateTime != null && (now - x.PostDateTime.Value).TotalHours > MyMtSettings.Instance.AutoDownloadSearchHours))
            {
                MyLog.Log($"====已搜索{MyMtSettings.Instance.AutoDownloadSearchHours}小时的文章，退出======");
                return true;
            }


            return false;

        
        }

        private void AutoDownloadNextPage()
        {
            _autoDownloadPages++;

            if (CheckAutoDownloadStop())
            {

                if (_autoDownloadItems is { Count: > 0 })
                {
                    //下载附件

                    //DownloadAttachmentsWithWebClient();

                    DownloadItemsWithBrowser(_autoDownloadItems);

                    
                }
                else
                {
                    _isAutoDownloading = false;
                    _isWorking = false;
                }

                
            }
            else
            {
                var pageUrl = BtBtItem.NextPageUrl(_currentPageUrl);
                MyLog.Log($"===搜索第{_autoDownloadPages}页====={pageUrl}");
                QueryIndexPage(pageUrl);
            }
        }

        /// <summary>
        /// 使用webclient单独下载种子附件 - 2025/1/14日开始下载不了了
        /// </summary>
        private void DownloadAttachmentsWithWebClient()
        {
            //下载附件
            var checkedItems = _autoDownloadItems.Where(x => x.Checked
                                                             && (x.tid == 0 || MyMtSettings.Instance.AutoDownloadLastTid == 0
                                                                            || x.tid > MyMtSettings.Instance.AutoDownloadLastTid)
            ).ToList();
            var i = 0;
            foreach (var btItem in checkedItems)
            {
                i += btItem.DownLoadAttachments(out var msg);
            }
            MyLog.Log($"下载了 {i} 个文件");

            //记录最新查询的
            var maxTid = _autoDownloadItems.Max(x => x.tid);
            var latestItem = _autoDownloadItems.FirstOrDefault(x => x.tid != 0 && x.tid == maxTid);
            if (latestItem?.PostDateTime != null)
            {
                MyMtSettings.Instance.AutoDownloadLastPostDateTime = latestItem.PostDateTime.Value;
                MyMtSettings.Instance.AutoDownloadLastTid = latestItem.tid;
                //Utility.SaveSetting(nameof(AutoDownloadLastPostDateTime), AutoDownloadLastPostDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                //Utility.SaveSetting(nameof(AutoDownloadLastTid), AutoDownloadLastTid.ToString());
                MyMtSettings.Instance.Save();
                MyLog.Log($"===Last item===={latestItem.Title}=={latestItem.tid}=={latestItem.PostDateTime}");
            }
        }

        /// <summary>
        /// 使用网页来下载附件
        /// </summary>
        /// <param name="items"></param>
        private void DownloadItemsWithBrowser(IList<BtBtItem> items)
        {
            foreach (var item in items)
            {
                _itemsToDownload.AddRange(item.GetDownloadItems());
            }

            if(_itemsToDownload.Count>0)
               webView21.CoreWebView2.Navigate($"{MyMtSettings.Instance.BtBtHomeUrl}{_itemsToDownload[0].Url}");

        }


        #endregion


        #region 附件下载

        CoreWebView2DownloadOperation _downloadOperation;

        private void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            var state = e.DownloadOperation.State;
            switch (state)
            {
                case CoreWebView2DownloadState.InProgress:
                    _downloadOperation = e.DownloadOperation;
                    var subPath = _itemsToDownload[0].SubPath;
                    var fileName = Path.GetFileName(e.ResultFilePath);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        e.Cancel = true;
                    }
                    else
                        e.ResultFilePath = Path.Combine(subPath, fileName);

                    //如果已经存在不重复下载
                    if (File.Exists(e.ResultFilePath))
                    {
                        e.Cancel = true;
                    }

                    _downloadOperation.StateChanged += DownloadOperation_StateChanged;

                    break;
                case CoreWebView2DownloadState.Completed:
                case CoreWebView2DownloadState.Interrupted:
                    CheckDownloadAttachment();
                    break;
            }

        }

        private void DownloadOperation_StateChanged(object sender, object e)
        {
            CheckDownloadAttachment();
        }

        private void CheckDownloadAttachment()
        {
            if(_itemsToDownload.Count>0)
                _itemsToDownload.RemoveAt(0);
            //下载下一个附件
            if (_itemsToDownload.Count > 0)
                webView21.CoreWebView2.Navigate($"{MyMtSettings.Instance.BtBtHomeUrl}{_itemsToDownload[0].Url}");
            else
            {
                var profile = webView21.CoreWebView2.Profile;
                profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DownloadHistory);

                if(!_isWorking)
                    return;

                if (!_isAutoDownloading)
                {
                    _isWorking = false;
                    MessageBox.Show("下载完毕");
                }
                else
                {
                    //记录最新查询的
                    var maxTid = _autoDownloadItems.Max(x => x.tid);
                    var latestItem = _autoDownloadItems.FirstOrDefault(x => x.tid != 0 && x.tid == maxTid);
                    if (latestItem?.PostDateTime != null)
                    {
                        MyMtSettings.Instance.AutoDownloadLastPostDateTime = latestItem.PostDateTime.Value;
                        MyMtSettings.Instance.AutoDownloadLastTid = latestItem.tid;
                        //Utility.SaveSetting(nameof(AutoDownloadLastPostDateTime), AutoDownloadLastPostDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        //Utility.SaveSetting(nameof(AutoDownloadLastTid), AutoDownloadLastTid.ToString());
                        MyMtSettings.Instance.Save();
                        MyLog.Log($"===Last item===={latestItem.Title}=={latestItem.tid}=={latestItem.PostDateTime}");
                    }

                    _isWorking = false;
                    _isAutoDownloading = false;
                }
            }
        }

        #endregion
    }
}
