namespace MovieTorrents
{
    partial class FormSetWatched
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
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.btOk = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(76, 12);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(231, 21);
            this.dtPicker.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "观看日期";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "评论";
            // 
            // tbComment
            // 
            this.tbComment.Location = new System.Drawing.Point(76, 49);
            this.tbComment.Multiline = true;
            this.tbComment.Name = "tbComment";
            this.tbComment.Size = new System.Drawing.Size(231, 89);
            this.tbComment.TabIndex = 0;
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(338, 27);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "确定(&O)";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.button1_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(338, 70);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "取消(&C)";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // FormSetWatched
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(436, 160);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.tbComment);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtPicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSetWatched";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "标记为已看";
            this.Load += new System.EventHandler(this.FormSetWatched_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btCancel;
    }
}