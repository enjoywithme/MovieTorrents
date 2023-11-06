﻿using MyPageLib;
using MyPageViewer.Dlg;
using mySharedLib;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Timers;
using MyPageLib.PoCo;
using System.Drawing.Printing;

namespace MyPageViewer
{
    public partial class FormMain : Form
    {

        public static FormMain Instance { get; set; }
        private readonly MyPageDocument _startDocument;
        private static System.Threading.Timer _autoIndexTimer;

        public FormMain(MyPageDocument startDocument)
        {
            InitializeComponent();
            _startDocument = startDocument;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            PageIndexer.Instance.IndexStopped += Instance_IndexStopped;
            PageIndexer.Instance.IndexFileChanged += Instance_IndexFileChanged;

            #region 主菜单

            tsmiStartIndex.Click += (_, _) =>
            {
                StartIndex();
            };
            tsmiExit.Click += (_, _) => { Close(); };
            tsmiOptions.Click += (_, _) =>
            {
                var ret = (new DlgOptions()).ShowDialog(this);
                if (ret != DialogResult.OK) return;
                StartAutoIndexTimer();
            };

            //Menu items view
            panelTree.Visible = tsmiViewTree.Checked = MyPageSettings.Instance.ViewTree;

            tsmiViewTree.Click += (_, _) =>
            {
                MyPageSettings.Instance.ViewTree = !MyPageSettings.Instance.ViewTree;
                tsmiViewTree.Checked = MyPageSettings.Instance.ViewTree;
                panelTree.Visible = MyPageSettings.Instance.ViewTree;
            };
            panelPreview.Visible = tsmiViewPreviewPane.Checked = MyPageSettings.Instance.ViewPreview;
            tsmiViewPreviewPane.Click += (_, _) =>
            {
                MyPageSettings.Instance.ViewPreview = !MyPageSettings.Instance.ViewPreview;

                panelPreview.Visible = tsmiViewPreviewPane.Checked = MyPageSettings.Instance.ViewPreview;
            };
            tsmiViewStatus.Click += (_, _) =>
            {
                statusStrip1.Visible = !statusStrip1.Visible;
                tsmiViewStatus.Checked = statusStrip1.Visible;
            };


            #endregion


            //查询
            tbSearch.TextChanged += TbSearch_TextChanged;
            tbSearch.KeyDown += TbSearch_KeyDown;
            listView.MouseDoubleClick += ListView_MouseDoubleClick;

            //Tree
            naviTreeControl1.NodeChanged += NaviTreeControl1_NodeChanged;

            //工具栏
            tsbStartIndex.Click += (_, _) => { StartIndex(); };
            tsbGotoDocFolder.Click += TsbGotoDocFolder_Click;
            tsmiPasteFromClipboard.Click += (_, _) => { PasteFromClipboard(); };
            tsbLast100Items.Click += (_, _) =>
            {
                FillListView(MyPageDb.Instance.FindLastN(100));
                RefreshStatusInfo();
            };
            tsbLastDay1.Click += (_, _) =>
            {
                FillListView(MyPageDb.Instance.FindLastDays(1));
                RefreshStatusInfo();
            };
            tsbLast2Days.Click += (_, _) =>
            {
                FillListView(MyPageDb.Instance.FindLastDays(2));
                RefreshStatusInfo();
            };
            tsbDelete.Click += (_, _) =>
            {
                if(listView.SelectedItems.Count==0) return;
                if(MessageBox.Show("确认删除选择的条目？",Properties.Resources.Text_Hint,MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.No) return;
                var list = (from ListViewItem item in listView.SelectedItems select (PageDocumentPoCo)item.Tag).ToList();
                var ret = MyPageDb.Instance.BatchDelete(list,out var message);
                MessageBox.Show(message, ret ? Properties.Resources.Text_Hint : Properties.Resources.Text_Error,
                    MessageBoxButtons.OK, ret ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            };

            //list view
            listView.SelectedIndexChanged += ListView_SelectedIndexChanged;
            listView.ItemDrag += ListView_ItemDrag;

            Resize += FormMain_Resize;
            notifyIcon1.MouseClick += (_, _) => ShowWindow();
            notifyIcon1.MouseDoubleClick += (_, _) => ShowWindow();

            //处理命令行
            if (_startDocument == null) return;
            (new FormPageViewer(_startDocument)).Show(this);
            Hide();


            //启动Timer
        }


        #region 索引

        private void StartAutoIndexTimer()
        {
            if (!MyPageSettings.Instance.AutoIndex) return;
            _autoIndexTimer ??= new System.Threading.Timer(_autoIndexTimer_Elapsed, null, Timeout.Infinite, Timeout.Infinite);

            _autoIndexTimer.Change(MyPageSettings.Instance.AutoIndexIntervalSeconds, Timeout.Infinite);
        }

        private void _autoIndexTimer_Elapsed(object sender)
        {

            if (PageIndexer.Instance.IsRunning) return;
            _autoIndexTimer.Change(Timeout.Infinite, Timeout.Infinite);

            PageIndexer.Instance.Start();

            Invoke(() =>
            {
                tslbIndexing.Visible = true;
                tsbStartIndex.Checked = true;
            });

        }

        private void TsbGotoDocFolder_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0) return;

            var poco = (PageDocumentPoCo)listView.SelectedItems[0].Tag;
            if (string.IsNullOrEmpty(poco.FolderPath)) return;
            naviTreeControl1.GotoPath(poco.FolderPath);
        }

        private void Instance_IndexFileChanged(object sender, string e)
        {
            Invoke(() =>
            {
                tslbIndexing.Text = e;
            });
        }

        private void StartIndex()
        {
            if (!PageIndexer.Instance.IsRunning)
            {
                PageIndexer.Instance.Start();
                tslbIndexing.Visible = true;
                tsbStartIndex.Checked = true;

                return;
            }

            PageIndexer.Instance.Stop();
        }

        private void Instance_IndexStopped(object sender, EventArgs e)
        {
            Invoke(() =>
            {
                tslbIndexing.Visible = false;
                tsbStartIndex.Checked = false;


            });

            StartAutoIndexTimer();
        }
        #endregion


        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshStatusInfo();
        }

        private void RefreshStatusInfo()
        {
            if (listView.SelectedIndices.Count == 0)
            {
                tsbDelete.Enabled = false;
                tsslInfo.Text = $"共 {listView.Items.Count} 个对象。";
                return;
            }

            tsslInfo.Text = ((PageDocumentPoCo)listView.SelectedItems[0].Tag).Description;
            tsbDelete.Enabled = true;
        }

        private void NaviTreeControl1_NodeChanged(object sender, string e)
        {
            listView.Items.Clear();

            if (!Directory.Exists(e)) return;

            var pocos = MyPageDb.Instance.FindFolderPath(e);
            FillListView(pocos);
            RefreshStatusInfo();
        }

        private void OpenDocumentFromFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;
            var doc = new MyPageDocument(filePath);
            var form = new FormPageViewer(doc);
            form.Show();
            WinApi.ShowToFront(form.Handle);

            form.WindowState = FormWindowState.Maximized;

        }

        #region List view动作
        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var info = ((ListView)sender).HitTest(e.X, e.Y);
            if (info.Item == null) return;

            var poCo = (PageDocumentPoCo)info.Item.Tag;
            OpenDocumentFromFilePath(poCo.FilePath);
        }
        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (listView.SelectedIndices.Count == 0) return;

            listView.DoDragDrop(new DataObject(DataFormats.FileDrop,
                (from ListViewItem item in listView.SelectedItems where item.Tag != null select ((PageDocumentPoCo)item.Tag).FilePath).ToArray()),
                DragDropEffects.Move);
        }
        #endregion

        #region 搜索
        private void TbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                DoSearch();
        }

        private async void DoSearch()
        {
            _searchCancellationTokenSource?.Cancel();

            _searchCancellationTokenSource = new CancellationTokenSource();
            var items = await MyPageDb.Instance.Search(tbSearch.Text, _searchCancellationTokenSource.Token);
            FillListView(items);
            RefreshStatusInfo();
        }

        private void FillListView(IList<PageDocumentPoCo> poCos)
        {
            if (poCos == null || poCos.Count == 0)
            {
                listView.Items.Clear();
                return;
            }

            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (var poCo in poCos)
            {
                var listViewItem = new ListViewItem(poCo.Title, 0)
                {
                    Tag = poCo
                };
                listViewItem.SubItems.Add(poCo.Name);
                listViewItem.SubItems.Add(poCo.FileSize.FormatFileSize());
                listViewItem.SubItems.Add(poCo.DtModified.FormatModifiedDateTime());
                listViewItem.SubItems.Add(poCo.FilePath);

                listView.Items.Add(listViewItem);
            }


            listView.EndUpdate();
        }


        private CancellationTokenSource _searchCancellationTokenSource;
        private async void TbSearch_TextChanged(object sender, EventArgs e)
        {

            async Task<bool> UserKeepsTyping()
            {
                var txt = tbSearch.Text;   // remember text
                await Task.Delay(500);        // wait some
                return txt != tbSearch.Text;  // return that text changed or not
            }
            if (await UserKeepsTyping()) return;

            DoSearch();
        }
        #endregion

        #region 操作

        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsText(TextDataFormat.Html)) return;
            var dlg = new DlgSaveClipboard();
            dlg.ShowDialog(this);
        }

        #endregion

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                if (message.LParam != IntPtr.Zero)
                {
                    try
                    {
                        var mmf = MemoryMappedFile.CreateOrOpen(SingleInstance.Instance.MmfName, SingleInstance.MmfLength, MemoryMappedFileAccess.ReadWrite);
                        var n = (int)message.LParam;
                        var accessor = mmf.CreateViewAccessor(0, SingleInstance.MmfLength);
                        var bytes = new byte[n];
                        var c = accessor.ReadArray(0, bytes, 0, n);
                        var s = System.Text.Encoding.Default.GetString(bytes, 0, c);

                        OpenDocumentFromFilePath(s);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                else
                    ShowWindow();


            }

            base.WndProc(ref message);
        }

        #region 窗口控制

        public void ShowWindow()
        {
            // Insert code here to make your form show itself.
            WinApi.ShowToFront(this.Handle);
        }

        void MinimizeToTray()
        {
            notifyIcon1.Visible = true;
            //WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        #endregion


    }
}