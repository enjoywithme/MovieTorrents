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
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.lvResults = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btDownload = new System.Windows.Forms.Button();
            this.lvTorrents = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeelater = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeenowant = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeeDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeeComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btArchiveTorrent = new System.Windows.Forms.Button();
            this.cbAutoDownload = new System.Windows.Forms.CheckBox();
            this.btLog = new System.Windows.Forms.Button();
            this.btClearLog = new System.Windows.Forms.Button();
            this.btHomePage = new System.Windows.Forms.Button();
            this.tbSearch = new MovieTorrents.TextBoxWithPasteEvent();
            this.SuspendLayout();
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(80, 42);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(326, 21);
            this.tbUrl.TabIndex = 0;
            this.tbUrl.Text = "https://www.btbtt.me/index-index-page-4.htm";
            // 
            // lvResults
            // 
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.CheckBoxes = true;
            this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvResults.FullRowSelect = true;
            this.lvResults.HideSelection = false;
            this.lvResults.Location = new System.Drawing.Point(12, 115);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(834, 178);
            this.lvResults.TabIndex = 3;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.SelectedIndexChanged += new System.EventHandler(this.lvResults_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名称";
            this.columnHeader1.Width = 480;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "发布时间";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "豆瓣评分";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "分类";
            this.columnHeader4.Width = 160;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "标签";
            this.columnHeader5.Width = 160;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "网址";
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(330, 12);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "下一页>>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(205, 11);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(75, 23);
            this.btnPrev.TabIndex = 5;
            this.btnPrev.Text = "<<上一页";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btDownload
            // 
            this.btDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btDownload.Location = new System.Drawing.Point(537, 297);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(75, 23);
            this.btDownload.TabIndex = 2;
            this.btDownload.Text = "下载";
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // lvTorrents
            // 
            this.lvTorrents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTorrents.CheckBoxes = true;
            this.lvTorrents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderRating,
            this.columnHeaderYear,
            this.columnHeaderSeelater,
            this.columnHeaderSeenowant,
            this.columnHeaderSeen,
            this.columnHeaderSeeDate,
            this.columnHeaderSeeComment});
            this.lvTorrents.FullRowSelect = true;
            this.lvTorrents.HideSelection = false;
            this.lvTorrents.Location = new System.Drawing.Point(12, 327);
            this.lvTorrents.Name = "lvTorrents";
            this.lvTorrents.Size = new System.Drawing.Size(834, 154);
            this.lvTorrents.TabIndex = 6;
            this.lvTorrents.UseCompatibleStateImageBehavior = false;
            this.lvTorrents.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "名称";
            this.columnHeaderName.Width = 500;
            // 
            // columnHeaderRating
            // 
            this.columnHeaderRating.Text = "评分";
            // 
            // columnHeaderYear
            // 
            this.columnHeaderYear.Text = "年代";
            this.columnHeaderYear.Width = 100;
            // 
            // columnHeaderSeelater
            // 
            this.columnHeaderSeelater.Text = "稍后看";
            // 
            // columnHeaderSeenowant
            // 
            this.columnHeaderSeenowant.Text = "不想看";
            // 
            // columnHeaderSeen
            // 
            this.columnHeaderSeen.Text = "看过";
            // 
            // columnHeaderSeeDate
            // 
            this.columnHeaderSeeDate.Text = "观看日期";
            this.columnHeaderSeeDate.Width = 120;
            // 
            // columnHeaderSeeComment
            // 
            this.columnHeaderSeeComment.Text = "观看评论";
            this.columnHeaderSeeComment.Width = 190;
            // 
            // tbTitle
            // 
            this.tbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbTitle.Location = new System.Drawing.Point(12, 299);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(496, 21);
            this.tbTitle.TabIndex = 7;
            this.tbTitle.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "搜索";
            // 
            // btArchiveTorrent
            // 
            this.btArchiveTorrent.Location = new System.Drawing.Point(433, 74);
            this.btArchiveTorrent.Name = "btArchiveTorrent";
            this.btArchiveTorrent.Size = new System.Drawing.Size(75, 23);
            this.btArchiveTorrent.TabIndex = 2;
            this.btArchiveTorrent.Text = "归档种子";
            this.btArchiveTorrent.UseVisualStyleBackColor = true;
            // 
            // cbAutoDownload
            // 
            this.cbAutoDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAutoDownload.AutoSize = true;
            this.cbAutoDownload.Location = new System.Drawing.Point(660, 15);
            this.cbAutoDownload.Name = "cbAutoDownload";
            this.cbAutoDownload.Size = new System.Drawing.Size(96, 16);
            this.cbAutoDownload.TabIndex = 9;
            this.cbAutoDownload.Text = "启用自动下载";
            this.cbAutoDownload.UseVisualStyleBackColor = true;
            this.cbAutoDownload.CheckedChanged += new System.EventHandler(this.cbAutoDownload_CheckedChanged);
            // 
            // btLog
            // 
            this.btLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btLog.Location = new System.Drawing.Point(771, 12);
            this.btLog.Name = "btLog";
            this.btLog.Size = new System.Drawing.Size(75, 23);
            this.btLog.TabIndex = 10;
            this.btLog.Text = "显示日志";
            this.btLog.UseVisualStyleBackColor = true;
            // 
            // btClearLog
            // 
            this.btClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClearLog.Location = new System.Drawing.Point(771, 46);
            this.btClearLog.Name = "btClearLog";
            this.btClearLog.Size = new System.Drawing.Size(75, 23);
            this.btClearLog.TabIndex = 10;
            this.btClearLog.Text = "清除日志";
            this.btClearLog.UseVisualStyleBackColor = true;
            // 
            // btHomePage
            // 
            this.btHomePage.Location = new System.Drawing.Point(80, 11);
            this.btHomePage.Name = "btHomePage";
            this.btHomePage.Size = new System.Drawing.Size(75, 23);
            this.btHomePage.TabIndex = 5;
            this.btHomePage.Text = "首页";
            this.btHomePage.UseVisualStyleBackColor = true;
            this.btHomePage.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // tbSearch
            // 
            this.tbSearch.Location = new System.Drawing.Point(80, 70);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(326, 21);
            this.tbSearch.TabIndex = 8;
            // 
            // FormBtBtt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 488);
            this.Controls.Add(this.btClearLog);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.cbAutoDownload);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.lvTorrents);
            this.Controls.Add(this.btHomePage);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvResults);
            this.Controls.Add(this.btArchiveTorrent);
            this.Controls.Add(this.btDownload);
            this.Controls.Add(this.tbUrl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormBtBtt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BtBtt工具";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBtBtt_FormClosing);
            this.Load += new System.EventHandler(this.FormBtBtt_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ListView lvTorrents;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderRating;
        private System.Windows.Forms.ColumnHeader columnHeaderYear;
        private System.Windows.Forms.ColumnHeader columnHeaderSeelater;
        private System.Windows.Forms.ColumnHeader columnHeaderSeenowant;
        private System.Windows.Forms.ColumnHeader columnHeaderSeen;
        private System.Windows.Forms.ColumnHeader columnHeaderSeeDate;
        private System.Windows.Forms.ColumnHeader columnHeaderSeeComment;
        private System.Windows.Forms.TextBox tbTitle;
        private MovieTorrents.TextBoxWithPasteEvent tbSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btArchiveTorrent;
        private System.Windows.Forms.CheckBox cbAutoDownload;
        private System.Windows.Forms.Button btLog;
        private System.Windows.Forms.Button btClearLog;
        private System.Windows.Forms.Button btHomePage;
    }
}