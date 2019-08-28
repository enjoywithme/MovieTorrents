namespace MovieTorrents
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
            this.tsmiShowStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.过滤TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterSeelater = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiFilterWatched = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterNotWatched = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHideSameSubject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiLimit100 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLimit200 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLimit500 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLimit1000 = new System.Windows.Forms.ToolStripMenuItem();
            this.排序OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRatingDesc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRatingAsc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiYearDesc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYearAsc = new System.Windows.Forms.ToolStripMenuItem();
            this.tbSearchText = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsCurrentDir = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsButtonWatch = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiToggleWatch = new System.Windows.Forms.ToolStripMenuItem();
            this.lvContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetWatched = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetSeelater = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSearchDouban = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyDouban = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiShowFileLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyName = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiMove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lvResults = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeelater = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeeDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSeeComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbGenres = new System.Windows.Forms.Label();
            this.lbRating = new System.Windows.Forms.Label();
            this.lbKeyName = new System.Windows.Forms.Label();
            this.lbOtherName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ultraDesktopAlert1 = new Infragistics.Win.Misc.UltraDesktopAlert(this.components);
            this.tsmiCopyPath = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.lvContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDesktopAlert1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem,
            this.过滤TToolStripMenuItem,
            this.排序OToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1273, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScanFile,
            this.tsmiClearRecords,
            this.tsmiShowStatistics,
            this.toolStripMenuItem2,
            this.tsmiExit});
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
            // tsmiShowStatistics
            // 
            this.tsmiShowStatistics.Name = "tsmiShowStatistics";
            this.tsmiShowStatistics.Size = new System.Drawing.Size(164, 22);
            this.tsmiShowStatistics.Text = "统计(&T)";
            this.tsmiShowStatistics.Click += new System.EventHandler(this.tsmiShowStatistics_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(164, 22);
            this.tsmiExit.Text = "退出(X)";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // 过滤TToolStripMenuItem
            // 
            this.过滤TToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFilterRecent,
            this.tsmiFilterSeelater,
            this.toolStripMenuItem1,
            this.tsmiFilterWatched,
            this.tsmiFilterNotWatched,
            this.tsmiHideSameSubject,
            this.toolStripMenuItem6,
            this.tsmiLimit100,
            this.tsmiLimit200,
            this.tsmiLimit500,
            this.tsmiLimit1000});
            this.过滤TToolStripMenuItem.Name = "过滤TToolStripMenuItem";
            this.过滤TToolStripMenuItem.Size = new System.Drawing.Size(59, 21);
            this.过滤TToolStripMenuItem.Text = "过滤(&T)";
            // 
            // tsmiFilterRecent
            // 
            this.tsmiFilterRecent.Name = "tsmiFilterRecent";
            this.tsmiFilterRecent.Size = new System.Drawing.Size(187, 22);
            this.tsmiFilterRecent.Text = "最近添加(&N)";
            this.tsmiFilterRecent.Click += new System.EventHandler(this.tsmiFilterRecent_Click);
            // 
            // tsmiFilterSeelater
            // 
            this.tsmiFilterSeelater.Name = "tsmiFilterSeelater";
            this.tsmiFilterSeelater.Size = new System.Drawing.Size(187, 22);
            this.tsmiFilterSeelater.Text = "稍后看(&L)";
            this.tsmiFilterSeelater.Click += new System.EventHandler(this.tsmiFilterSeelater_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(184, 6);
            // 
            // tsmiFilterWatched
            // 
            this.tsmiFilterWatched.Name = "tsmiFilterWatched";
            this.tsmiFilterWatched.Size = new System.Drawing.Size(187, 22);
            this.tsmiFilterWatched.Text = "看过";
            this.tsmiFilterWatched.Click += new System.EventHandler(this.tsmiFilterWatched_Click);
            // 
            // tsmiFilterNotWatched
            // 
            this.tsmiFilterNotWatched.Name = "tsmiFilterNotWatched";
            this.tsmiFilterNotWatched.Size = new System.Drawing.Size(187, 22);
            this.tsmiFilterNotWatched.Text = "没有看过";
            this.tsmiFilterNotWatched.Click += new System.EventHandler(this.tsmiFilterNotWatched_Click);
            // 
            // tsmiHideSameSubject
            // 
            this.tsmiHideSameSubject.Checked = true;
            this.tsmiHideSameSubject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiHideSameSubject.Enabled = false;
            this.tsmiHideSameSubject.Name = "tsmiHideSameSubject";
            this.tsmiHideSameSubject.Size = new System.Drawing.Size(187, 22);
            this.tsmiHideSameSubject.Text = "隐藏已看同名电影(&S)";
            this.tsmiHideSameSubject.Click += new System.EventHandler(this.tsmiHideSameSubject_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(184, 6);
            // 
            // tsmiLimit100
            // 
            this.tsmiLimit100.Checked = true;
            this.tsmiLimit100.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiLimit100.Name = "tsmiLimit100";
            this.tsmiLimit100.Size = new System.Drawing.Size(187, 22);
            this.tsmiLimit100.Text = "限制100条";
            this.tsmiLimit100.Click += new System.EventHandler(this.tsmiLimit100_Click);
            // 
            // tsmiLimit200
            // 
            this.tsmiLimit200.Name = "tsmiLimit200";
            this.tsmiLimit200.Size = new System.Drawing.Size(187, 22);
            this.tsmiLimit200.Text = "限制200条";
            this.tsmiLimit200.Click += new System.EventHandler(this.tsmiLimit200_Click);
            // 
            // tsmiLimit500
            // 
            this.tsmiLimit500.Name = "tsmiLimit500";
            this.tsmiLimit500.Size = new System.Drawing.Size(187, 22);
            this.tsmiLimit500.Text = "限制500条";
            this.tsmiLimit500.Click += new System.EventHandler(this.tsmiLimit300_Click);
            // 
            // tsmiLimit1000
            // 
            this.tsmiLimit1000.Name = "tsmiLimit1000";
            this.tsmiLimit1000.Size = new System.Drawing.Size(187, 22);
            this.tsmiLimit1000.Text = "限制1000条";
            this.tsmiLimit1000.Click += new System.EventHandler(this.tsmiLimit1000_Click);
            // 
            // 排序OToolStripMenuItem
            // 
            this.排序OToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRatingDesc,
            this.tsmiRatingAsc,
            this.toolStripMenuItem5,
            this.tsmiYearDesc,
            this.tsmiYearAsc});
            this.排序OToolStripMenuItem.Name = "排序OToolStripMenuItem";
            this.排序OToolStripMenuItem.Size = new System.Drawing.Size(62, 21);
            this.排序OToolStripMenuItem.Text = "排序(&O)";
            // 
            // tsmiRatingDesc
            // 
            this.tsmiRatingDesc.Checked = true;
            this.tsmiRatingDesc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiRatingDesc.Name = "tsmiRatingDesc";
            this.tsmiRatingDesc.Size = new System.Drawing.Size(113, 22);
            this.tsmiRatingDesc.Text = "评分 -";
            this.tsmiRatingDesc.Click += new System.EventHandler(this.tsmiRatingDesc_Click);
            // 
            // tsmiRatingAsc
            // 
            this.tsmiRatingAsc.Name = "tsmiRatingAsc";
            this.tsmiRatingAsc.Size = new System.Drawing.Size(113, 22);
            this.tsmiRatingAsc.Text = "评分 +";
            this.tsmiRatingAsc.Click += new System.EventHandler(this.tsmiRatingAsc_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(110, 6);
            // 
            // tsmiYearDesc
            // 
            this.tsmiYearDesc.Name = "tsmiYearDesc";
            this.tsmiYearDesc.Size = new System.Drawing.Size(113, 22);
            this.tsmiYearDesc.Text = "年代 -";
            this.tsmiYearDesc.Click += new System.EventHandler(this.tsmiYearDesc_Click);
            // 
            // tsmiYearAsc
            // 
            this.tsmiYearAsc.Name = "tsmiYearAsc";
            this.tsmiYearAsc.Size = new System.Drawing.Size(113, 22);
            this.tsmiYearAsc.Text = "年代 +";
            this.tsmiYearAsc.Click += new System.EventHandler(this.tsmiYearAsc_Click);
            // 
            // tbSearchText
            // 
            this.tbSearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchText.Location = new System.Drawing.Point(0, 25);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(1273, 21);
            this.tbSearchText.TabIndex = 1;
            this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearchText_TextChanged);
            this.tbSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearchText_KeyDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssState,
            this.tssInfo,
            this.tsCurrentDir,
            this.tsButtonWatch});
            this.statusStrip1.Location = new System.Drawing.Point(0, 557);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1273, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssState
            // 
            this.tssState.AutoSize = false;
            this.tssState.BackColor = System.Drawing.SystemColors.Control;
            this.tssState.Image = global::MovieTorrents.Properties.Resources.InfoGree32;
            this.tssState.Name = "tssState";
            this.tssState.Size = new System.Drawing.Size(17, 21);
            this.tssState.Click += new System.EventHandler(this.tssState_Click);
            // 
            // tssInfo
            // 
            this.tssInfo.AutoSize = false;
            this.tssInfo.Name = "tssInfo";
            this.tssInfo.Size = new System.Drawing.Size(1149, 21);
            this.tssInfo.Spring = true;
            this.tssInfo.Text = "空闲";
            this.tssInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsCurrentDir
            // 
            this.tsCurrentDir.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsCurrentDir.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsCurrentDir.Name = "tsCurrentDir";
            this.tsCurrentDir.Size = new System.Drawing.Size(63, 21);
            this.tsCurrentDir.Text = "种子目录:";
            // 
            // tsButtonWatch
            // 
            this.tsButtonWatch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonWatch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiToggleWatch});
            this.tsButtonWatch.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonWatch.Image")));
            this.tsButtonWatch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonWatch.Name = "tsButtonWatch";
            this.tsButtonWatch.Size = new System.Drawing.Size(29, 24);
            // 
            // tsmiToggleWatch
            // 
            this.tsmiToggleWatch.Name = "tsmiToggleWatch";
            this.tsmiToggleWatch.Size = new System.Drawing.Size(192, 22);
            this.tsmiToggleWatch.Text = "toolStripMenuItem7";
            this.tsmiToggleWatch.Click += new System.EventHandler(this.tsmiToggleWatch_Click);
            // 
            // lvContextMenu
            // 
            this.lvContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetWatched,
            this.tsmiSetSeelater,
            this.tsmiSearchDouban,
            this.tsmiCopyDouban,
            this.toolStripMenuItem3,
            this.tsmiShowFileLocation,
            this.tsmiCopyName,
            this.tsmiCopyPath,
            this.tsmiCopyFile,
            this.toolStripMenuItem4,
            this.tsmiMove,
            this.tsmiDelete});
            this.lvContextMenu.Name = "lvContextMenu";
            this.lvContextMenu.Size = new System.Drawing.Size(213, 258);
            this.lvContextMenu.Text = "设置已看";
            this.lvContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.lvContextMenu_Opening);
            // 
            // tsmiSetWatched
            // 
            this.tsmiSetWatched.Name = "tsmiSetWatched";
            this.tsmiSetWatched.Size = new System.Drawing.Size(212, 22);
            this.tsmiSetWatched.Text = "设置为已观看(&W)";
            this.tsmiSetWatched.Click += new System.EventHandler(this.tsmiSetWatched_Click);
            // 
            // tsmiSetSeelater
            // 
            this.tsmiSetSeelater.Name = "tsmiSetSeelater";
            this.tsmiSetSeelater.Size = new System.Drawing.Size(212, 22);
            this.tsmiSetSeelater.Text = "切换稍后看(&L)";
            this.tsmiSetSeelater.Click += new System.EventHandler(this.tsmiSetSeelater_Click);
            // 
            // tsmiSearchDouban
            // 
            this.tsmiSearchDouban.Image = global::MovieTorrents.Properties.Resources.dou;
            this.tsmiSearchDouban.Name = "tsmiSearchDouban";
            this.tsmiSearchDouban.Size = new System.Drawing.Size(212, 22);
            this.tsmiSearchDouban.Text = "搜索豆瓣信息(&S)";
            this.tsmiSearchDouban.Click += new System.EventHandler(this.tsmiSearchDouban_Click);
            // 
            // tsmiCopyDouban
            // 
            this.tsmiCopyDouban.Name = "tsmiCopyDouban";
            this.tsmiCopyDouban.Size = new System.Drawing.Size(212, 22);
            this.tsmiCopyDouban.Text = "设置豆瓣信息为勾选项(&K)";
            this.tsmiCopyDouban.Click += new System.EventHandler(this.tsmiCopyDouban_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(209, 6);
            // 
            // tsmiShowFileLocation
            // 
            this.tsmiShowFileLocation.Name = "tsmiShowFileLocation";
            this.tsmiShowFileLocation.Size = new System.Drawing.Size(212, 22);
            this.tsmiShowFileLocation.Text = "打开文件所在位置(&L)";
            this.tsmiShowFileLocation.Click += new System.EventHandler(this.tsmiShowFileLocation_Click);
            // 
            // tsmiCopyName
            // 
            this.tsmiCopyName.Name = "tsmiCopyName";
            this.tsmiCopyName.Size = new System.Drawing.Size(212, 22);
            this.tsmiCopyName.Text = "复制名称(&C)";
            this.tsmiCopyName.Click += new System.EventHandler(this.tsmiCopyName_Click);
            // 
            // tsmiCopyFile
            // 
            this.tsmiCopyFile.Name = "tsmiCopyFile";
            this.tsmiCopyFile.Size = new System.Drawing.Size(212, 22);
            this.tsmiCopyFile.Text = "拷贝文件(&F)";
            this.tsmiCopyFile.Click += new System.EventHandler(this.tsmiCopyFile_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(209, 6);
            // 
            // tsmiMove
            // 
            this.tsmiMove.Name = "tsmiMove";
            this.tsmiMove.Size = new System.Drawing.Size(212, 22);
            this.tsmiMove.Text = "移动...(&M)";
            this.tsmiMove.Click += new System.EventHandler(this.tsmiMove_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(212, 22);
            this.tsmiDelete.Text = "删除(&D)";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "双击恢复窗口";
            this.notifyIcon1.BalloonTipTitle = "提示";
            this.notifyIcon1.Text = "Movie torrents";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // lvResults
            // 
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.CheckBoxes = true;
            this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderRating,
            this.columnHeaderYear,
            this.columnHeaderSeelater,
            this.columnHeaderSeen,
            this.columnHeaderSeeDate,
            this.columnHeaderSeeComment});
            this.lvResults.FullRowSelect = true;
            this.lvResults.HideSelection = false;
            this.lvResults.Location = new System.Drawing.Point(132, 52);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(1141, 506);
            this.lvResults.TabIndex = 5;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
            this.lvResults.SelectedIndexChanged += new System.EventHandler(this.lvResults_SelectedIndexChanged);
            this.lvResults.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvResults_MouseClick);
            this.lvResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvResults_MouseDoubleClick);
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
            // lbGenres
            // 
            this.lbGenres.Location = new System.Drawing.Point(12, 472);
            this.lbGenres.Name = "lbGenres";
            this.lbGenres.Size = new System.Drawing.Size(98, 44);
            this.lbGenres.TabIndex = 1;
            this.lbGenres.Text = "Genres";
            this.lbGenres.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbRating
            // 
            this.lbRating.Location = new System.Drawing.Point(12, 219);
            this.lbRating.Name = "lbRating";
            this.lbRating.Size = new System.Drawing.Size(100, 23);
            this.lbRating.TabIndex = 6;
            this.lbRating.Text = "Rating";
            this.lbRating.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbKeyName
            // 
            this.lbKeyName.Location = new System.Drawing.Point(14, 266);
            this.lbKeyName.Name = "lbKeyName";
            this.lbKeyName.Size = new System.Drawing.Size(98, 68);
            this.lbKeyName.TabIndex = 1;
            this.lbKeyName.Text = "keyName";
            this.lbKeyName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbOtherName
            // 
            this.lbOtherName.Location = new System.Drawing.Point(14, 365);
            this.lbOtherName.Name = "lbOtherName";
            this.lbOtherName.Size = new System.Drawing.Size(98, 58);
            this.lbOtherName.TabIndex = 1;
            this.lbOtherName.Text = "otherName";
            this.lbOtherName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 52);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(126, 164);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // ultraDesktopAlert1
            // 
            this.ultraDesktopAlert1.AutoCloseDelay = 2000;
            this.ultraDesktopAlert1.DesktopAlertLinkClicked += new Infragistics.Win.Misc.DesktopAlertLinkClickedHandler(this.ultraDesktopAlert1_DesktopAlertLinkClicked);
            // 
            // tsmiCopyPath
            // 
            this.tsmiCopyPath.Name = "tsmiCopyPath";
            this.tsmiCopyPath.Size = new System.Drawing.Size(212, 22);
            this.tsmiCopyPath.Text = "复制文件完整路径(&P)";
            this.tsmiCopyPath.Click += new System.EventHandler(this.tsmiCopyPath_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1273, 583);
            this.Controls.Add(this.lbRating);
            this.Controls.Add(this.lbOtherName);
            this.Controls.Add(this.lbKeyName);
            this.Controls.Add(this.lbGenres);
            this.Controls.Add(this.lvResults);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tbSearchText);
            this.Controls.Add(this.menuStrip1);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDesktopAlert1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.TextBox tbSearchText;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip lvContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetWatched;
        private System.Windows.Forms.ToolStripMenuItem tsmiScanFile;
        private System.Windows.Forms.ToolStripStatusLabel tssInfo;
        private System.Windows.Forms.ToolStripStatusLabel tssState;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearRecords;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyName;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowFileLocation;
        private System.Windows.Forms.ToolStripMenuItem tsmiSearchDouban;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderRating;
        private System.Windows.Forms.ColumnHeader columnHeaderYear;
        private System.Windows.Forms.ColumnHeader columnHeaderSeen;
        private System.Windows.Forms.ColumnHeader columnHeaderSeeDate;
        private System.Windows.Forms.ColumnHeader columnHeaderSeeComment;
        private System.Windows.Forms.Label lbGenres;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbRating;
        private System.Windows.Forms.ToolStripMenuItem 过滤TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterRecent;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterWatched;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterNotWatched;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyFile;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ColumnHeader columnHeaderSeelater;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetSeelater;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterSeelater;
        private System.Windows.Forms.Label lbKeyName;
        private System.Windows.Forms.Label lbOtherName;
        private System.Windows.Forms.ToolStripMenuItem 排序OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiRatingDesc;
        private System.Windows.Forms.ToolStripMenuItem tsmiRatingAsc;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem tsmiYearDesc;
        private System.Windows.Forms.ToolStripMenuItem tsmiYearAsc;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem tsmiLimit100;
        private System.Windows.Forms.ToolStripMenuItem tsmiLimit200;
        private System.Windows.Forms.ToolStripMenuItem tsmiLimit500;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideSameSubject;
        private System.Windows.Forms.ToolStripMenuItem tsmiLimit1000;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyDouban;
        private System.Windows.Forms.ToolStripStatusLabel tsCurrentDir;
        private System.Windows.Forms.ToolStripDropDownButton tsButtonWatch;
        private System.Windows.Forms.ToolStripMenuItem tsmiToggleWatch;
        private Infragistics.Win.Misc.UltraDesktopAlert ultraDesktopAlert1;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowStatistics;
        private System.Windows.Forms.ToolStripMenuItem tsmiMove;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyPath;
    }
}

