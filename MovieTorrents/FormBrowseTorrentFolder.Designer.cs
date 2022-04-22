
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "目录";
            // 
            // btSelectPath
            // 
            this.btSelectPath.Location = new System.Drawing.Point(436, 44);
            this.btSelectPath.Name = "btSelectPath";
            this.btSelectPath.Size = new System.Drawing.Size(75, 23);
            this.btSelectPath.TabIndex = 2;
            this.btSelectPath.Text = "..";
            this.btSelectPath.UseVisualStyleBackColor = true;
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(336, 113);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "确定(&O)";
            this.btOk.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(436, 113);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消(&C)";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // tbSelectedPath
            // 
            this.tbSelectedPath.FormattingEnabled = true;
            this.tbSelectedPath.Location = new System.Drawing.Point(47, 47);
            this.tbSelectedPath.Name = "tbSelectedPath";
            this.tbSelectedPath.Size = new System.Drawing.Size(364, 20);
            this.tbSelectedPath.TabIndex = 3;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(47, 73);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "重命名文件夹";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // FormBrowseTorrentFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 173);
            this.Controls.Add(this.checkBox1);
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
        private System.Windows.Forms.CheckBox checkBox1;
    }
}