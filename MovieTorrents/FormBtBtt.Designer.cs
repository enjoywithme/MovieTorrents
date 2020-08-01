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
            this.btnQuery = new System.Windows.Forms.Button();
            this.lvResults = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.btnHomePage = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btDownload = new System.Windows.Forms.Button();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbProxy = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDownloadPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(80, 66);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(326, 21);
            this.tbUrl.TabIndex = 0;
            this.tbUrl.Text = "https://www.btbtt.me/index-index-page-4.htm";
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(433, 71);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.button1_Click);
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
            this.lvResults.Location = new System.Drawing.Point(12, 163);
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(834, 313);
            this.lvResults.TabIndex = 3;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = System.Windows.Forms.View.Details;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "网址";
            // 
            // btnHomePage
            // 
            this.btnHomePage.Location = new System.Drawing.Point(80, 22);
            this.btnHomePage.Name = "btnHomePage";
            this.btnHomePage.Size = new System.Drawing.Size(75, 23);
            this.btnHomePage.TabIndex = 2;
            this.btnHomePage.Text = "首页";
            this.btnHomePage.UseVisualStyleBackColor = true;
            this.btnHomePage.Click += new System.EventHandler(this.btnHomePage_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(185, 22);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "下一页>>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(297, 22);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(75, 23);
            this.btnPrev.TabIndex = 5;
            this.btnPrev.Text = "<<上一页";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btDownload
            // 
            this.btDownload.Location = new System.Drawing.Point(433, 115);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(75, 23);
            this.btDownload.TabIndex = 2;
            this.btDownload.Text = "下载";
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
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
            // tbProxy
            // 
            this.tbProxy.Location = new System.Drawing.Point(80, 97);
            this.tbProxy.Name = "tbProxy";
            this.tbProxy.Size = new System.Drawing.Size(326, 21);
            this.tbProxy.TabIndex = 0;
            this.tbProxy.Text = "http://vd-win7-dx:9999";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "代理";
            // 
            // tbDownloadPath
            // 
            this.tbDownloadPath.Location = new System.Drawing.Point(80, 127);
            this.tbDownloadPath.Name = "tbDownloadPath";
            this.tbDownloadPath.Size = new System.Drawing.Size(326, 21);
            this.tbDownloadPath.TabIndex = 0;
            this.tbDownloadPath.Text = "f:\\temp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "下载路径";
            // 
            // FormBtBtt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 488);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvResults);
            this.Controls.Add(this.btnHomePage);
            this.Controls.Add(this.btDownload);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.tbDownloadPath);
            this.Controls.Add(this.tbProxy);
            this.Controls.Add(this.tbUrl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormBtBtt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BtBtt工具";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormBtBtt_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHomePage;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TextBox tbProxy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDownloadPath;
        private System.Windows.Forms.Label label3;
    }
}