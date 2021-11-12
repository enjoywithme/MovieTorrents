namespace _0dayUnrar
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
            this.label1 = new System.Windows.Forms.Label();
            this.btSelectSourceFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDestFolder = new System.Windows.Forms.TextBox();
            this.btSelectDestFolder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbWinrarPath = new System.Windows.Forms.TextBox();
            this.btSelectWinrarPath = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btClear = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cbDeleteFileAfterExtracting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "源路径";
            // 
            // btSelectSourceFolder
            // 
            this.btSelectSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectSourceFolder.Location = new System.Drawing.Point(671, 22);
            this.btSelectSourceFolder.Name = "btSelectSourceFolder";
            this.btSelectSourceFolder.Size = new System.Drawing.Size(34, 23);
            this.btSelectSourceFolder.TabIndex = 0;
            this.btSelectSourceFolder.Text = "...";
            this.btSelectSourceFolder.UseVisualStyleBackColor = true;
            this.btSelectSourceFolder.Click += new System.EventHandler(this.BtnSelectSourceFolderClick);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 367);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "解压目的路径";
            // 
            // tbDestFolder
            // 
            this.tbDestFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDestFolder.Location = new System.Drawing.Point(12, 382);
            this.tbDestFolder.Name = "tbDestFolder";
            this.tbDestFolder.Size = new System.Drawing.Size(623, 21);
            this.tbDestFolder.TabIndex = 3;
            this.tbDestFolder.Text = "F:\\temp\\x\\";
            // 
            // btSelectDestFolder
            // 
            this.btSelectDestFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectDestFolder.Location = new System.Drawing.Point(671, 376);
            this.btSelectDestFolder.Name = "btSelectDestFolder";
            this.btSelectDestFolder.Size = new System.Drawing.Size(34, 23);
            this.btSelectDestFolder.TabIndex = 5;
            this.btSelectDestFolder.Text = "...";
            this.btSelectDestFolder.UseVisualStyleBackColor = true;
            this.btSelectDestFolder.Click += new System.EventHandler(this.BtSelectDestFolderClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 409);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Winrar路径";
            // 
            // tbWinrarPath
            // 
            this.tbWinrarPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWinrarPath.Location = new System.Drawing.Point(12, 426);
            this.tbWinrarPath.Name = "tbWinrarPath";
            this.tbWinrarPath.Size = new System.Drawing.Size(623, 21);
            this.tbWinrarPath.TabIndex = 4;
            this.tbWinrarPath.Tag = "";
            this.tbWinrarPath.Text = "C:\\Program Files\\WinRAR\\WinRAR.exe";
            // 
            // btSelectWinrarPath
            // 
            this.btSelectWinrarPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectWinrarPath.Location = new System.Drawing.Point(671, 420);
            this.btSelectWinrarPath.Name = "btSelectWinrarPath";
            this.btSelectWinrarPath.Size = new System.Drawing.Size(34, 23);
            this.btSelectWinrarPath.TabIndex = 6;
            this.btSelectWinrarPath.Text = "...";
            this.btSelectWinrarPath.UseVisualStyleBackColor = true;
            this.btSelectWinrarPath.Click += new System.EventHandler(this.BtSelectWinrarPathClick);
            // 
            // btStart
            // 
            this.btStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btStart.Location = new System.Drawing.Point(654, 164);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(69, 140);
            this.btStart.TabIndex = 2;
            this.btStart.Text = "开始";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.TbnStartClick);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Location = new System.Drawing.Point(12, 500);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.Size = new System.Drawing.Size(623, 47);
            this.tbLog.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 450);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "处理进度";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 22);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(623, 339);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "路径";
            this.columnHeader1.Width = 360;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "状态";
            // 
            // btClear
            // 
            this.btClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClear.Location = new System.Drawing.Point(671, 51);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(34, 26);
            this.btClear.TabIndex = 1;
            this.btClear.Text = "X";
            this.btClear.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 471);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(623, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // cbDeleteFileAfterExtracting
            // 
            this.cbDeleteFileAfterExtracting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDeleteFileAfterExtracting.AutoSize = true;
            this.cbDeleteFileAfterExtracting.Location = new System.Drawing.Point(503, 366);
            this.cbDeleteFileAfterExtracting.Name = "cbDeleteFileAfterExtracting";
            this.cbDeleteFileAfterExtracting.Size = new System.Drawing.Size(132, 16);
            this.cbDeleteFileAfterExtracting.TabIndex = 10;
            this.cbDeleteFileAfterExtracting.Text = "解压缩后删除原文件";
            this.cbDeleteFileAfterExtracting.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 559);
            this.Controls.Add(this.cbDeleteFileAfterExtracting);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.btSelectWinrarPath);
            this.Controls.Add(this.btSelectDestFolder);
            this.Controls.Add(this.tbWinrarPath);
            this.Controls.Add(this.btSelectSourceFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbDestFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "0day unRAR";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMainFormClosing);
            this.Load += new System.EventHandler(this.FormMainLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSelectSourceFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDestFolder;
        private System.Windows.Forms.Button btSelectDestFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbWinrarPath;
        private System.Windows.Forms.Button btSelectWinrarPath;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label6;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cbDeleteFileAfterExtracting;
    }
}

