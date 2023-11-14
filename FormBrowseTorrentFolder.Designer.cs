
namespace MovieTorrents
{
    partial class FormBrowseTorrentFolder
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
            this.btSelectPath = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.tbSelectedPath = new System.Windows.Forms.ComboBox();
            this.cbRenameMoveFolder = new System.Windows.Forms.CheckBox();
            this.cbCreateFolder = new System.Windows.Forms.CheckBox();
            this.bt1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "目录";
            // 
            // btSelectPath
            // 
            this.btSelectPath.Location = new System.Drawing.Point(436, 139);
            this.btSelectPath.Name = "btSelectPath";
            this.btSelectPath.Size = new System.Drawing.Size(75, 23);
            this.btSelectPath.TabIndex = 2;
            this.btSelectPath.Text = "..";
            this.btSelectPath.UseVisualStyleBackColor = true;
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(336, 208);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "确定(&O)";
            this.btOk.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(436, 208);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消(&C)";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // tbSelectedPath
            // 
            this.tbSelectedPath.FormattingEnabled = true;
            this.tbSelectedPath.Location = new System.Drawing.Point(47, 142);
            this.tbSelectedPath.Name = "tbSelectedPath";
            this.tbSelectedPath.Size = new System.Drawing.Size(364, 20);
            this.tbSelectedPath.TabIndex = 3;
            // 
            // cbRenameMoveFolder
            // 
            this.cbRenameMoveFolder.AutoSize = true;
            this.cbRenameMoveFolder.Location = new System.Drawing.Point(47, 168);
            this.cbRenameMoveFolder.Name = "cbRenameMoveFolder";
            this.cbRenameMoveFolder.Size = new System.Drawing.Size(132, 16);
            this.cbRenameMoveFolder.TabIndex = 4;
            this.cbRenameMoveFolder.Text = "重命名移动的文件夹";
            this.cbRenameMoveFolder.UseVisualStyleBackColor = true;
            // 
            // cbCreateFolder
            // 
            this.cbCreateFolder.AutoSize = true;
            this.cbCreateFolder.Location = new System.Drawing.Point(47, 190);
            this.cbCreateFolder.Name = "cbCreateFolder";
            this.cbCreateFolder.Size = new System.Drawing.Size(84, 16);
            this.cbCreateFolder.TabIndex = 4;
            this.cbCreateFolder.Text = "创建文件夹";
            this.cbCreateFolder.UseVisualStyleBackColor = true;
            // 
            // bt1
            // 
            this.bt1.Location = new System.Drawing.Point(47, 33);
            this.bt1.Name = "bt1";
            this.bt1.Size = new System.Drawing.Size(75, 30);
            this.bt1.TabIndex = 5;
            this.bt1.Tag = "[剧集]\\[英剧]\\";
            this.bt1.Text = "英剧";
            this.bt1.UseVisualStyleBackColor = true;
            this.bt1.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(155, 33);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 30);
            this.button2.TabIndex = 5;
            this.button2.Tag = "[剧集]\\[中剧]\\";
            this.button2.Text = "中剧";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(255, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 5;
            this.button1.Tag = "[剧集]\\[日剧]\\";
            this.button1.Text = "日剧";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(358, 33);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 5;
            this.button3.Tag = "[剧集]\\[韩剧]\\";
            this.button3.Text = "韩剧";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(47, 79);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 5;
            this.button4.Tag = "[番剧]\\";
            this.button4.Text = "番剧";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(155, 79);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 30);
            this.button5.TabIndex = 5;
            this.button5.Tag = "[卡通]\\[美漫]\\";
            this.button5.Text = "美漫";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(255, 79);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 30);
            this.button6.TabIndex = 5;
            this.button6.Tag = "[卡通]\\[国漫]\\";
            this.button6.Text = "国漫";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // FormBrowseTorrentFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 252);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bt1);
            this.Controls.Add(this.cbCreateFolder);
            this.Controls.Add(this.cbRenameMoveFolder);
            this.Controls.Add(this.tbSelectedPath);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.btSelectPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBrowseTorrentFolder";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择目录";
            this.Load += new System.EventHandler(this.FormBrowseTorrentFolder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSelectPath;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ComboBox tbSelectedPath;
        private System.Windows.Forms.CheckBox cbRenameMoveFolder;
        private System.Windows.Forms.CheckBox cbCreateFolder;
        private System.Windows.Forms.Button bt1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}