namespace FindSimilarNameFolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tsbSelectFolder = new System.Windows.Forms.ToolStrip();
            this.tsbFolder = new System.Windows.Forms.ToolStripButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tsbOpenResult = new System.Windows.Forms.ToolStripButton();
            this.tsbSelectFolder.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsbSelectFolder
            // 
            this.tsbSelectFolder.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsbSelectFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFolder,
            this.tsbOpenResult});
            this.tsbSelectFolder.Location = new System.Drawing.Point(0, 0);
            this.tsbSelectFolder.Name = "tsbSelectFolder";
            this.tsbSelectFolder.Size = new System.Drawing.Size(480, 43);
            this.tsbSelectFolder.TabIndex = 0;
            this.tsbSelectFolder.Text = "toolStrip1";
            // 
            // tsbFolder
            // 
            this.tsbFolder.AutoSize = false;
            this.tsbFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFolder.Name = "tsbFolder";
            this.tsbFolder.Size = new System.Drawing.Size(114, 40);
            this.tsbFolder.Text = "Start";
            this.tsbFolder.Click += new System.EventHandler(this.TsbFolderClick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1RunWorkerCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 319);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(480, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.SizeChanged += new System.EventHandler(this.StatusStrip1SizeChanged);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(100, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Step = 1;
            this.toolStripProgressBar1.Visible = false;
            this.toolStripProgressBar1.VisibleChanged += new System.EventHandler(this.ToolStripProgressBar1VisibleChanged);
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(0, 43);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.Size = new System.Drawing.Size(480, 276);
            this.tbLog.TabIndex = 3;
            // 
            // tsbOpenResult
            // 
            this.tsbOpenResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbOpenResult.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpenResult.Image")));
            this.tsbOpenResult.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenResult.Name = "tsbOpenResult";
            this.tsbOpenResult.Size = new System.Drawing.Size(101, 40);
            this.tsbOpenResult.Text = "Open result file";
            this.tsbOpenResult.Click += new System.EventHandler(this.tsbOpenResult_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 341);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tsbSelectFolder);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find folder with similar name";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMainFormClosing);
            this.Load += new System.EventHandler(this.FormMainLoad);
            this.tsbSelectFolder.ResumeLayout(false);
            this.tsbSelectFolder.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsbSelectFolder;
        private System.Windows.Forms.ToolStripButton tsbFolder;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.ToolStripButton tsbOpenResult;
    }
}

