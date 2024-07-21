using MovieTorrents.Common;

namespace MovieTorrents
{
    partial class FormBtBtt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBtBtt));
            tbUrl = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            btnNext = new System.Windows.Forms.Button();
            btnPrev = new System.Windows.Forms.Button();
            btDownload = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            btArchiveTorrent = new System.Windows.Forms.Button();
            cbAutoDownload = new System.Windows.Forms.CheckBox();
            btLog = new System.Windows.Forms.Button();
            btClearLog = new System.Windows.Forms.Button();
            btHomePage = new System.Windows.Forms.Button();
            tbSearch = new TextBoxWithPasteEvent();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            lvResults = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // tbUrl
            // 
            tbUrl.Location = new System.Drawing.Point(93, 60);
            tbUrl.Margin = new System.Windows.Forms.Padding(4);
            tbUrl.Name = "tbUrl";
            tbUrl.Size = new System.Drawing.Size(380, 23);
            tbUrl.TabIndex = 0;
            tbUrl.Text = "https://www.btbtt.me/index-index-page-4.htm";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(20, 65);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(32, 17);
            label1.TabIndex = 4;
            label1.Text = "网址";
            // 
            // btnNext
            // 
            btnNext.Location = new System.Drawing.Point(385, 17);
            btnNext.Margin = new System.Windows.Forms.Padding(4);
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(88, 33);
            btnNext.TabIndex = 5;
            btnNext.Text = "下一页>>";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new System.Drawing.Point(239, 16);
            btnPrev.Margin = new System.Windows.Forms.Padding(4);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new System.Drawing.Size(88, 33);
            btnPrev.TabIndex = 5;
            btnPrev.Text = "<<上一页";
            btnPrev.UseVisualStyleBackColor = true;
            btnPrev.Click += btnPrev_Click;
            // 
            // btDownload
            // 
            btDownload.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btDownload.Location = new System.Drawing.Point(371, 685);
            btDownload.Margin = new System.Windows.Forms.Padding(4);
            btDownload.Name = "btDownload";
            btDownload.Size = new System.Drawing.Size(88, 33);
            btDownload.TabIndex = 2;
            btDownload.Text = "下载";
            btDownload.UseVisualStyleBackColor = true;
            btDownload.Click += btDownload_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(20, 105);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(32, 17);
            label4.TabIndex = 4;
            label4.Text = "搜索";
            // 
            // btArchiveTorrent
            // 
            btArchiveTorrent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btArchiveTorrent.Location = new System.Drawing.Point(926, 65);
            btArchiveTorrent.Margin = new System.Windows.Forms.Padding(4);
            btArchiveTorrent.Name = "btArchiveTorrent";
            btArchiveTorrent.Size = new System.Drawing.Size(88, 33);
            btArchiveTorrent.TabIndex = 2;
            btArchiveTorrent.Text = "归档种子";
            btArchiveTorrent.UseVisualStyleBackColor = true;
            // 
            // cbAutoDownload
            // 
            cbAutoDownload.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cbAutoDownload.AutoSize = true;
            cbAutoDownload.Location = new System.Drawing.Point(926, 21);
            cbAutoDownload.Margin = new System.Windows.Forms.Padding(4);
            cbAutoDownload.Name = "cbAutoDownload";
            cbAutoDownload.Size = new System.Drawing.Size(99, 21);
            cbAutoDownload.TabIndex = 9;
            cbAutoDownload.Text = "启用自动下载";
            cbAutoDownload.UseVisualStyleBackColor = true;
            cbAutoDownload.CheckedChanged += cbAutoDownload_CheckedChanged;
            // 
            // btLog
            // 
            btLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btLog.Location = new System.Drawing.Point(1042, 17);
            btLog.Margin = new System.Windows.Forms.Padding(4);
            btLog.Name = "btLog";
            btLog.Size = new System.Drawing.Size(88, 33);
            btLog.TabIndex = 10;
            btLog.Text = "显示日志";
            btLog.UseVisualStyleBackColor = true;
            // 
            // btClearLog
            // 
            btClearLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btClearLog.Location = new System.Drawing.Point(1042, 65);
            btClearLog.Margin = new System.Windows.Forms.Padding(4);
            btClearLog.Name = "btClearLog";
            btClearLog.Size = new System.Drawing.Size(88, 33);
            btClearLog.TabIndex = 10;
            btClearLog.Text = "清除日志";
            btClearLog.UseVisualStyleBackColor = true;
            // 
            // btHomePage
            // 
            btHomePage.Location = new System.Drawing.Point(93, 16);
            btHomePage.Margin = new System.Windows.Forms.Padding(4);
            btHomePage.Name = "btHomePage";
            btHomePage.Size = new System.Drawing.Size(88, 33);
            btHomePage.TabIndex = 5;
            btHomePage.Text = "首页";
            btHomePage.UseVisualStyleBackColor = true;
            // 
            // tbSearch
            // 
            tbSearch.Location = new System.Drawing.Point(93, 99);
            tbSearch.Margin = new System.Windows.Forms.Padding(4);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new System.Drawing.Size(380, 23);
            tbSearch.TabIndex = 8;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.Location = new System.Drawing.Point(6, 137);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(webView21);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(lvResults);
            splitContainer1.Panel2.Controls.Add(btDownload);
            splitContainer1.Size = new System.Drawing.Size(1134, 722);
            splitContainer1.SplitterDistance = 667;
            splitContainer1.TabIndex = 12;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            webView21.Dock = System.Windows.Forms.DockStyle.Fill;
            webView21.Location = new System.Drawing.Point(0, 0);
            webView21.Name = "webView21";
            webView21.Size = new System.Drawing.Size(667, 722);
            webView21.TabIndex = 12;
            webView21.ZoomFactor = 1D;
            // 
            // lvResults
            // 
            lvResults.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvResults.CheckBoxes = true;
            lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            lvResults.FullRowSelect = true;
            lvResults.Location = new System.Drawing.Point(4, 4);
            lvResults.Margin = new System.Windows.Forms.Padding(4);
            lvResults.Name = "lvResults";
            lvResults.Size = new System.Drawing.Size(459, 662);
            lvResults.TabIndex = 4;
            lvResults.UseCompatibleStateImageBehavior = false;
            lvResults.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "名称";
            columnHeader1.Width = 280;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "发布时间";
            columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "豆瓣评分";
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "分类";
            columnHeader4.Width = 160;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "标签";
            columnHeader5.Width = 160;
            // 
            // FormBtBtt
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1144, 871);
            Controls.Add(splitContainer1);
            Controls.Add(btClearLog);
            Controls.Add(btLog);
            Controls.Add(cbAutoDownload);
            Controls.Add(tbSearch);
            Controls.Add(btHomePage);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(btArchiveTorrent);
            Controls.Add(tbUrl);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            Name = "FormBtBtt";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "BtBtt工具";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            FormClosing += FormBtBtt_FormClosing;
            Load += FormBtBtt_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btDownload;
        private TextBoxWithPasteEvent tbSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btArchiveTorrent;
        private System.Windows.Forms.CheckBox cbAutoDownload;
        private System.Windows.Forms.Button btLog;
        private System.Windows.Forms.Button btClearLog;
        private System.Windows.Forms.Button btHomePage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}