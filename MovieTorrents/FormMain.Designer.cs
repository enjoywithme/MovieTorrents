﻿namespace MovieTorrents
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearRecords = new System.Windows.Forms.ToolStripMenuItem();
            this.退出XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbSearchText = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.lvResults = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeeDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeeComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetWatched = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyName = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeaderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tsmiShowFileLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.lvContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1071, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScanFile,
            this.tsmiClearRecords,
            this.退出XToolStripMenuItem});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.文件FToolStripMenuItem.Text = "文件(&F)";
            // 
            // tsmiScanFile
            // 
            this.tsmiScanFile.Name = "tsmiScanFile";
            this.tsmiScanFile.Size = new System.Drawing.Size(164, 22);
            this.tsmiScanFile.Text = "扫描种子文件(&S)";
            this.tsmiScanFile.Click += new System.EventHandler(this.tsmiScanFile_Click);
            // 
            // tsmiClearRecords
            // 
            this.tsmiClearRecords.Name = "tsmiClearRecords";
            this.tsmiClearRecords.Size = new System.Drawing.Size(164, 22);
            this.tsmiClearRecords.Text = "清除无效记录(&C)";
            this.tsmiClearRecords.Click += new System.EventHandler(this.tsmiClearRecords_Click);
            // 
            // 退出XToolStripMenuItem
            // 
            this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
            this.退出XToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.退出XToolStripMenuItem.Text = "退出(X)";
            // 
            // tbSearchText
            // 
            this.tbSearchText.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbSearchText.Location = new System.Drawing.Point(0, 25);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(1071, 21);
            this.tbSearchText.TabIndex = 1;
            this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearchText_TextChanged);
            this.tbSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearchText_KeyDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssState,
            this.tssInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 506);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1071, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssState
            // 
            this.tssState.AutoSize = false;
            this.tssState.BackColor = System.Drawing.Color.LimeGreen;
            this.tssState.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssState.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tssState.Name = "tssState";
            this.tssState.Size = new System.Drawing.Size(17, 17);
            this.tssState.Click += new System.EventHandler(this.tssState_Click);
            // 
            // tssInfo
            // 
            this.tssInfo.AutoSize = false;
            this.tssInfo.Name = "tssInfo";
            this.tssInfo.Size = new System.Drawing.Size(1039, 17);
            this.tssInfo.Spring = true;
            this.tssInfo.Text = "空闲";
            this.tssInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lvResults
            // 
            this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderRating,
            this.columnHeaderYear,
            this.columnHeaderSeen,
            this.columnHeaderSeeDate,
            this.columnHeaderSeeComment,
            this.columnHeaderPath});
            this.lvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvResults.FullRowSelect = true;
            this.lvResults.Location = new System.Drawing.Point(0, 46);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(1071, 460);
            this.lvResults.TabIndex = 3;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvResults_MouseClick);
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
            // lvContextMenu
            // 
            this.lvContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetWatched,
            this.tsmiCopyName,
            this.tsmiShowFileLocation});
            this.lvContextMenu.Name = "lvContextMenu";
            this.lvContextMenu.Size = new System.Drawing.Size(187, 70);
            this.lvContextMenu.Text = "设置已看";
            this.lvContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.lvContextMenu_Opening);
            // 
            // tsmiSetWatched
            // 
            this.tsmiSetWatched.Name = "tsmiSetWatched";
            this.tsmiSetWatched.Size = new System.Drawing.Size(186, 22);
            this.tsmiSetWatched.Text = "设置为已观看(&W)";
            this.tsmiSetWatched.Click += new System.EventHandler(this.tsmiSetWatched_Click);
            // 
            // tsmiCopyName
            // 
            this.tsmiCopyName.Name = "tsmiCopyName";
            this.tsmiCopyName.Size = new System.Drawing.Size(186, 22);
            this.tsmiCopyName.Text = "拷贝名称(&C)";
            this.tsmiCopyName.Click += new System.EventHandler(this.tsmiCopyName_Click);
            // 
            // columnHeaderPath
            // 
            this.columnHeaderPath.Text = "路径";
            this.columnHeaderPath.Width = 160;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "双击恢复窗口";
            this.notifyIcon1.BalloonTipTitle = "Movie torrents";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Movie torrents";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // tsmiShowFileLocation
            // 
            this.tsmiShowFileLocation.Name = "tsmiShowFileLocation";
            this.tsmiShowFileLocation.Size = new System.Drawing.Size(186, 22);
            this.tsmiShowFileLocation.Text = "打开文件所在位置(&L)";
            this.tsmiShowFileLocation.Click += new System.EventHandler(this.tsmiShowFileLocation_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 528);
            this.Controls.Add(this.lvResults);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tbSearchText);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Movie torrents";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.lvContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出XToolStripMenuItem;
        private System.Windows.Forms.TextBox tbSearchText;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderRating;
        private System.Windows.Forms.ColumnHeader columnHeaderYear;
        private System.Windows.Forms.ColumnHeader columnHeaderSeen;
        private System.Windows.Forms.ColumnHeader columnHeaderSeeDate;
        private System.Windows.Forms.ContextMenuStrip lvContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetWatched;
        private System.Windows.Forms.ToolStripMenuItem tsmiScanFile;
        private System.Windows.Forms.ToolStripStatusLabel tssInfo;
        private System.Windows.Forms.ToolStripStatusLabel tssState;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearRecords;
        private System.Windows.Forms.ColumnHeader columnHeaderSeeComment;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyName;
        private System.Windows.Forms.ColumnHeader columnHeaderPath;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowFileLocation;
    }
}
