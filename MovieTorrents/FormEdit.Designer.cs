﻿namespace MovieTorrents
{
    partial class FormEdit
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
            this.tbOldName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNewName = new System.Windows.Forms.TextBox();
            this.btOk = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbYear = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbKeyName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbOtherName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbGenres = new System.Windows.Forms.TextBox();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.cbWatched = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbZone = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbDoubanId = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbRating = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbDirectors = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbCasts = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbposterpath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "原名称";
            // 
            // tbOldName
            // 
            this.tbOldName.Location = new System.Drawing.Point(66, 35);
            this.tbOldName.Name = "tbOldName";
            this.tbOldName.ReadOnly = true;
            this.tbOldName.Size = new System.Drawing.Size(381, 21);
            this.tbOldName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "新名称";
            // 
            // tbNewName
            // 
            this.tbNewName.Location = new System.Drawing.Point(66, 70);
            this.tbNewName.Name = "tbNewName";
            this.tbNewName.Size = new System.Drawing.Size(381, 21);
            this.tbNewName.TabIndex = 0;
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(476, 34);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 8;
            this.btOk.Text = "确定(&O)";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(476, 75);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 9;
            this.btCancel.Text = "取消(&C)";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "年份";
            // 
            // tbYear
            // 
            this.tbYear.Location = new System.Drawing.Point(66, 105);
            this.tbYear.Name = "tbYear";
            this.tbYear.Size = new System.Drawing.Size(100, 21);
            this.tbYear.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "关键字";
            // 
            // tbKeyName
            // 
            this.tbKeyName.Location = new System.Drawing.Point(66, 245);
            this.tbKeyName.Multiline = true;
            this.tbKeyName.Name = "tbKeyName";
            this.tbKeyName.Size = new System.Drawing.Size(381, 37);
            this.tbKeyName.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 307);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "其他名称";
            // 
            // tbOtherName
            // 
            this.tbOtherName.Location = new System.Drawing.Point(66, 296);
            this.tbOtherName.Multiline = true;
            this.tbOtherName.Name = "tbOtherName";
            this.tbOtherName.Size = new System.Drawing.Size(381, 34);
            this.tbOtherName.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 348);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "类型";
            // 
            // tbGenres
            // 
            this.tbGenres.Location = new System.Drawing.Point(66, 344);
            this.tbGenres.Name = "tbGenres";
            this.tbGenres.Size = new System.Drawing.Size(381, 21);
            this.tbGenres.TabIndex = 4;
            // 
            // tbComment
            // 
            this.tbComment.Location = new System.Drawing.Point(66, 414);
            this.tbComment.Multiline = true;
            this.tbComment.Name = "tbComment";
            this.tbComment.Size = new System.Drawing.Size(381, 73);
            this.tbComment.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 444);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "评论";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 383);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "观看日期";
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(66, 379);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(200, 21);
            this.dtPicker.TabIndex = 5;
            // 
            // cbWatched
            // 
            this.cbWatched.AutoSize = true;
            this.cbWatched.Location = new System.Drawing.Point(369, 381);
            this.cbWatched.Name = "cbWatched";
            this.cbWatched.Size = new System.Drawing.Size(48, 16);
            this.cbWatched.TabIndex = 7;
            this.cbWatched.Text = "已看";
            this.cbWatched.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(176, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "制片地区";
            // 
            // tbZone
            // 
            this.tbZone.Location = new System.Drawing.Point(235, 105);
            this.tbZone.Name = "tbZone";
            this.tbZone.Size = new System.Drawing.Size(212, 21);
            this.tbZone.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 144);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "豆瓣ID";
            // 
            // tbDoubanId
            // 
            this.tbDoubanId.Location = new System.Drawing.Point(66, 140);
            this.tbDoubanId.Name = "tbDoubanId";
            this.tbDoubanId.Size = new System.Drawing.Size(100, 21);
            this.tbDoubanId.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(200, 144);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "评分";
            // 
            // tbRating
            // 
            this.tbRating.Location = new System.Drawing.Point(235, 140);
            this.tbRating.Name = "tbRating";
            this.tbRating.Size = new System.Drawing.Size(61, 21);
            this.tbRating.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(28, 179);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "导演";
            // 
            // tbDirectors
            // 
            this.tbDirectors.Location = new System.Drawing.Point(66, 175);
            this.tbDirectors.Name = "tbDirectors";
            this.tbDirectors.Size = new System.Drawing.Size(100, 21);
            this.tbDirectors.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(200, 179);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "主演";
            // 
            // tbCasts
            // 
            this.tbCasts.Location = new System.Drawing.Point(235, 175);
            this.tbCasts.Name = "tbCasts";
            this.tbCasts.Size = new System.Drawing.Size(212, 21);
            this.tbCasts.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(28, 214);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(29, 12);
            this.label14.TabIndex = 0;
            this.label14.Text = "海报";
            // 
            // tbposterpath
            // 
            this.tbposterpath.Location = new System.Drawing.Point(66, 210);
            this.tbposterpath.Name = "tbposterpath";
            this.tbposterpath.Size = new System.Drawing.Size(381, 21);
            this.tbposterpath.TabIndex = 1;
            // 
            // FormEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(579, 518);
            this.Controls.Add(this.cbWatched);
            this.Controls.Add(this.tbComment);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.tbOtherName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbKeyName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbGenres);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbZone);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbRating);
            this.Controls.Add(this.tbposterpath);
            this.Controls.Add(this.tbCasts);
            this.Controls.Add(this.tbDirectors);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tbDoubanId);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.tbYear);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbNewName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbOldName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEdit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改记录";
            this.Load += new System.EventHandler(this.FormRenameTorrent_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbOldName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNewName;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbKeyName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbOtherName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbGenres;
        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.CheckBox cbWatched;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbZone;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbDoubanId;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbRating;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbDirectors;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbCasts;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbposterpath;
    }
}