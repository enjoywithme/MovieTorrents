namespace _0dayrar
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
            this.tbSourceFolder = new System.Windows.Forms.TextBox();
            this.btSelectSourceFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDestFolder = new System.Windows.Forms.TextBox();
            this.btSelectDestFolder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbWinrarPath = new System.Windows.Forms.TextBox();
            this.btSelectWinrarPath = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbCurDestSubPath = new System.Windows.Forms.TextBox();
            this.listProcessed = new System.Windows.Forms.ListBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.nuFreespace = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.nuSingleFileSize = new System.Windows.Forms.NumericUpDown();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.nuFreespace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nuSingleFileSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source folder";
            // 
            // tbSourceFolder
            // 
            this.tbSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSourceFolder.Location = new System.Drawing.Point(131, 19);
            this.tbSourceFolder.Name = "tbSourceFolder";
            this.tbSourceFolder.Size = new System.Drawing.Size(389, 21);
            this.tbSourceFolder.TabIndex = 1;
            this.tbSourceFolder.Text = "D:\\baiduyundownload";
            // 
            // btSelectSourceFolder
            // 
            this.btSelectSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectSourceFolder.Location = new System.Drawing.Point(550, 17);
            this.btSelectSourceFolder.Name = "btSelectSourceFolder";
            this.btSelectSourceFolder.Size = new System.Drawing.Size(34, 23);
            this.btSelectSourceFolder.TabIndex = 2;
            this.btSelectSourceFolder.Text = "...";
            this.btSelectSourceFolder.UseVisualStyleBackColor = true;
            this.btSelectSourceFolder.Click += new System.EventHandler(this.BtnSelectSourceFolderClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Destination folder";
            // 
            // tbDestFolder
            // 
            this.tbDestFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDestFolder.Location = new System.Drawing.Point(131, 46);
            this.tbDestFolder.Name = "tbDestFolder";
            this.tbDestFolder.Size = new System.Drawing.Size(389, 21);
            this.tbDestFolder.TabIndex = 1;
            this.tbDestFolder.Text = "D:\\WeiYunUpload";
            // 
            // btSelectDestFolder
            // 
            this.btSelectDestFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectDestFolder.Location = new System.Drawing.Point(550, 44);
            this.btSelectDestFolder.Name = "btSelectDestFolder";
            this.btSelectDestFolder.Size = new System.Drawing.Size(34, 23);
            this.btSelectDestFolder.TabIndex = 2;
            this.btSelectDestFolder.Text = "...";
            this.btSelectDestFolder.UseVisualStyleBackColor = true;
            this.btSelectDestFolder.Click += new System.EventHandler(this.BtSelectDestFolderClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Winrar path";
            // 
            // tbWinrarPath
            // 
            this.tbWinrarPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWinrarPath.Location = new System.Drawing.Point(131, 75);
            this.tbWinrarPath.Name = "tbWinrarPath";
            this.tbWinrarPath.Size = new System.Drawing.Size(389, 21);
            this.tbWinrarPath.TabIndex = 1;
            this.tbWinrarPath.Text = "C:\\Program Files\\WinRAR\\rar.exe";
            // 
            // btSelectWinrarPath
            // 
            this.btSelectWinrarPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectWinrarPath.Location = new System.Drawing.Point(550, 73);
            this.btSelectWinrarPath.Name = "btSelectWinrarPath";
            this.btSelectWinrarPath.Size = new System.Drawing.Size(34, 23);
            this.btSelectWinrarPath.TabIndex = 2;
            this.btSelectWinrarPath.Text = "...";
            this.btSelectWinrarPath.UseVisualStyleBackColor = true;
            this.btSelectWinrarPath.Click += new System.EventHandler(this.BtSelectWinrarPathClick);
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(30, 238);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(74, 76);
            this.btStart.TabIndex = 3;
            this.btStart.Text = "Start";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.TbnStartClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(129, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(185, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Current destination sub-folder";
            // 
            // tbCurDestSubPath
            // 
            this.tbCurDestSubPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCurDestSubPath.Location = new System.Drawing.Point(131, 128);
            this.tbCurDestSubPath.Name = "tbCurDestSubPath";
            this.tbCurDestSubPath.ReadOnly = true;
            this.tbCurDestSubPath.Size = new System.Drawing.Size(389, 21);
            this.tbCurDestSubPath.TabIndex = 1;
            // 
            // listProcessed
            // 
            this.listProcessed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listProcessed.FormattingEnabled = true;
            this.listProcessed.ItemHeight = 12;
            this.listProcessed.Location = new System.Drawing.Point(3, 21);
            this.listProcessed.Name = "listProcessed";
            this.listProcessed.Size = new System.Drawing.Size(383, 172);
            this.listProcessed.TabIndex = 4;
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Location = new System.Drawing.Point(3, 15);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.Size = new System.Drawing.Size(383, 172);
            this.tbLog.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Processed list";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Processing log";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
            // 
            // nuFreespace
            // 
            this.nuFreespace.Location = new System.Drawing.Point(30, 128);
            this.nuFreespace.Name = "nuFreespace";
            this.nuFreespace.Size = new System.Drawing.Size(74, 21);
            this.nuFreespace.TabIndex = 6;
            this.nuFreespace.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Minimal free space";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(110, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "G";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 176);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "Single file size";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(110, 197);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(11, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "M";
            // 
            // nuSingleFileSize
            // 
            this.nuSingleFileSize.Location = new System.Drawing.Point(30, 194);
            this.nuSingleFileSize.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nuSingleFileSize.Name = "nuSingleFileSize";
            this.nuSingleFileSize.Size = new System.Drawing.Size(74, 21);
            this.nuSingleFileSize.TabIndex = 6;
            this.nuSingleFileSize.Value = new decimal(new int[] {
            900,
            0,
            0,
            0});
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(131, 155);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listProcessed);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbLog);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Size = new System.Drawing.Size(389, 388);
            this.splitContainer1.SplitterDistance = 194;
            this.splitContainer1.TabIndex = 7;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 555);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.nuSingleFileSize);
            this.Controls.Add(this.nuFreespace);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.btSelectWinrarPath);
            this.Controls.Add(this.btSelectDestFolder);
            this.Controls.Add(this.tbCurDestSubPath);
            this.Controls.Add(this.tbWinrarPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btSelectSourceFolder);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbDestFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbSourceFolder);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "0day RAR";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMainFormClosing);
            this.Load += new System.EventHandler(this.FormMainLoad);
            ((System.ComponentModel.ISupportInitialize)(this.nuFreespace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nuSingleFileSize)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSourceFolder;
        private System.Windows.Forms.Button btSelectSourceFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDestFolder;
        private System.Windows.Forms.Button btSelectDestFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbWinrarPath;
        private System.Windows.Forms.Button btSelectWinrarPath;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbCurDestSubPath;
        private System.Windows.Forms.ListBox listProcessed;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.NumericUpDown nuFreespace;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nuSingleFileSize;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

