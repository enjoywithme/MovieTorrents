namespace MovieTorrents
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
            label1 = new System.Windows.Forms.Label();
            tbOldName = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            tbNewName = new System.Windows.Forms.TextBox();
            btOk = new System.Windows.Forms.Button();
            btCancel = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            tbYear = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            tbKeyName = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            tbOtherName = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            tbGenres = new System.Windows.Forms.TextBox();
            tbComment = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            dtPicker = new System.Windows.Forms.DateTimePicker();
            cbWatched = new System.Windows.Forms.CheckBox();
            label9 = new System.Windows.Forms.Label();
            tbZone = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            tbDoubanId = new System.Windows.Forms.TextBox();
            label11 = new System.Windows.Forms.Label();
            tbRating = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            tbDirectors = new System.Windows.Forms.TextBox();
            label13 = new System.Windows.Forms.Label();
            tbCasts = new System.Windows.Forms.TextBox();
            label14 = new System.Windows.Forms.Label();
            tbposterpath = new System.Windows.Forms.TextBox();
            label15 = new System.Windows.Forms.Label();
            tbDoubanTitle = new System.Windows.Forms.TextBox();
            tbDoubanSubTitle = new System.Windows.Forms.TextBox();
            label16 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(19, 55);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(44, 17);
            label1.TabIndex = 0;
            label1.Text = "原名称";
            // 
            // tbOldName
            // 
            tbOldName.Location = new System.Drawing.Point(77, 50);
            tbOldName.Margin = new System.Windows.Forms.Padding(4);
            tbOldName.Name = "tbOldName";
            tbOldName.ReadOnly = true;
            tbOldName.Size = new System.Drawing.Size(444, 23);
            tbOldName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(19, 105);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 17);
            label2.TabIndex = 0;
            label2.Text = "新名称";
            // 
            // tbNewName
            // 
            tbNewName.Location = new System.Drawing.Point(77, 99);
            tbNewName.Margin = new System.Windows.Forms.Padding(4);
            tbNewName.Name = "tbNewName";
            tbNewName.Size = new System.Drawing.Size(444, 23);
            tbNewName.TabIndex = 0;
            // 
            // btOk
            // 
            btOk.Location = new System.Drawing.Point(555, 48);
            btOk.Margin = new System.Windows.Forms.Padding(4);
            btOk.Name = "btOk";
            btOk.Size = new System.Drawing.Size(88, 33);
            btOk.TabIndex = 8;
            btOk.Text = "确定(&O)";
            btOk.UseVisualStyleBackColor = true;
            btOk.Click += btOk_Click;
            // 
            // btCancel
            // 
            btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btCancel.Location = new System.Drawing.Point(555, 106);
            btCancel.Margin = new System.Windows.Forms.Padding(4);
            btCancel.Name = "btCancel";
            btCancel.Size = new System.Drawing.Size(88, 33);
            btCancel.TabIndex = 9;
            btCancel.Text = "取消(&C)";
            btCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(33, 237);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(32, 17);
            label3.TabIndex = 0;
            label3.Text = "年份";
            // 
            // tbYear
            // 
            tbYear.Location = new System.Drawing.Point(77, 232);
            tbYear.Margin = new System.Windows.Forms.Padding(4);
            tbYear.Name = "tbYear";
            tbYear.Size = new System.Drawing.Size(116, 23);
            tbYear.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(19, 157);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(44, 17);
            label4.TabIndex = 0;
            label4.Text = "关键字";
            // 
            // tbKeyName
            // 
            tbKeyName.Location = new System.Drawing.Point(77, 140);
            tbKeyName.Margin = new System.Windows.Forms.Padding(4);
            tbKeyName.Multiline = true;
            tbKeyName.Name = "tbKeyName";
            tbKeyName.Size = new System.Drawing.Size(444, 51);
            tbKeyName.TabIndex = 2;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(5, 368);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(56, 17);
            label5.TabIndex = 0;
            label5.Text = "其他名称";
            // 
            // tbOtherName
            // 
            tbOtherName.Location = new System.Drawing.Point(77, 352);
            tbOtherName.Margin = new System.Windows.Forms.Padding(4);
            tbOtherName.Multiline = true;
            tbOtherName.Name = "tbOtherName";
            tbOtherName.Size = new System.Drawing.Size(444, 46);
            tbOtherName.TabIndex = 3;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(33, 501);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(32, 17);
            label6.TabIndex = 0;
            label6.Text = "类型";
            // 
            // tbGenres
            // 
            tbGenres.Location = new System.Drawing.Point(77, 495);
            tbGenres.Margin = new System.Windows.Forms.Padding(4);
            tbGenres.Name = "tbGenres";
            tbGenres.Size = new System.Drawing.Size(444, 23);
            tbGenres.TabIndex = 4;
            // 
            // tbComment
            // 
            tbComment.Location = new System.Drawing.Point(77, 593);
            tbComment.Margin = new System.Windows.Forms.Padding(4);
            tbComment.Multiline = true;
            tbComment.Name = "tbComment";
            tbComment.Size = new System.Drawing.Size(444, 102);
            tbComment.TabIndex = 6;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(33, 636);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(32, 17);
            label7.TabIndex = 4;
            label7.Text = "评论";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(5, 550);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(56, 17);
            label8.TabIndex = 5;
            label8.Text = "观看日期";
            // 
            // dtPicker
            // 
            dtPicker.Location = new System.Drawing.Point(77, 544);
            dtPicker.Margin = new System.Windows.Forms.Padding(4);
            dtPicker.Name = "dtPicker";
            dtPicker.Size = new System.Drawing.Size(233, 23);
            dtPicker.TabIndex = 5;
            // 
            // cbWatched
            // 
            cbWatched.AutoSize = true;
            cbWatched.Location = new System.Drawing.Point(430, 547);
            cbWatched.Margin = new System.Windows.Forms.Padding(4);
            cbWatched.Name = "cbWatched";
            cbWatched.Size = new System.Drawing.Size(51, 21);
            cbWatched.TabIndex = 7;
            cbWatched.Text = "已看";
            cbWatched.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(205, 237);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(56, 17);
            label9.TabIndex = 0;
            label9.Text = "制片地区";
            // 
            // tbZone
            // 
            tbZone.Location = new System.Drawing.Point(274, 232);
            tbZone.Margin = new System.Windows.Forms.Padding(4);
            tbZone.Name = "tbZone";
            tbZone.Size = new System.Drawing.Size(247, 23);
            tbZone.TabIndex = 1;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(19, 287);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(45, 17);
            label10.TabIndex = 0;
            label10.Text = "豆瓣ID";
            // 
            // tbDoubanId
            // 
            tbDoubanId.Location = new System.Drawing.Point(77, 281);
            tbDoubanId.Margin = new System.Windows.Forms.Padding(4);
            tbDoubanId.Name = "tbDoubanId";
            tbDoubanId.Size = new System.Drawing.Size(116, 23);
            tbDoubanId.TabIndex = 1;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(233, 287);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(32, 17);
            label11.TabIndex = 0;
            label11.Text = "评分";
            // 
            // tbRating
            // 
            tbRating.Location = new System.Drawing.Point(274, 281);
            tbRating.Margin = new System.Windows.Forms.Padding(4);
            tbRating.Name = "tbRating";
            tbRating.Size = new System.Drawing.Size(247, 23);
            tbRating.TabIndex = 1;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(33, 420);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(32, 17);
            label12.TabIndex = 0;
            label12.Text = "导演";
            // 
            // tbDirectors
            // 
            tbDirectors.Location = new System.Drawing.Point(77, 414);
            tbDirectors.Margin = new System.Windows.Forms.Padding(4);
            tbDirectors.Name = "tbDirectors";
            tbDirectors.Size = new System.Drawing.Size(116, 23);
            tbDirectors.TabIndex = 1;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(233, 420);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(32, 17);
            label13.TabIndex = 0;
            label13.Text = "主演";
            // 
            // tbCasts
            // 
            tbCasts.Location = new System.Drawing.Point(274, 414);
            tbCasts.Margin = new System.Windows.Forms.Padding(4);
            tbCasts.Name = "tbCasts";
            tbCasts.Size = new System.Drawing.Size(247, 23);
            tbCasts.TabIndex = 1;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(33, 469);
            label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(32, 17);
            label14.TabIndex = 0;
            label14.Text = "海报";
            // 
            // tbposterpath
            // 
            tbposterpath.Location = new System.Drawing.Point(77, 464);
            tbposterpath.Margin = new System.Windows.Forms.Padding(4);
            tbposterpath.Name = "tbposterpath";
            tbposterpath.Size = new System.Drawing.Size(444, 23);
            tbposterpath.TabIndex = 1;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(233, 327);
            label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(44, 17);
            label15.TabIndex = 0;
            label15.Text = "副标题";
            // 
            // tbDoubanTitle
            // 
            tbDoubanTitle.Location = new System.Drawing.Point(77, 321);
            tbDoubanTitle.Margin = new System.Windows.Forms.Padding(4);
            tbDoubanTitle.Name = "tbDoubanTitle";
            tbDoubanTitle.Size = new System.Drawing.Size(116, 23);
            tbDoubanTitle.TabIndex = 1;
            // 
            // tbDoubanSubTitle
            // 
            tbDoubanSubTitle.Location = new System.Drawing.Point(274, 321);
            tbDoubanSubTitle.Margin = new System.Windows.Forms.Padding(4);
            tbDoubanSubTitle.Name = "tbDoubanSubTitle";
            tbDoubanSubTitle.Size = new System.Drawing.Size(247, 23);
            tbDoubanSubTitle.TabIndex = 1;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(13, 327);
            label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(56, 17);
            label16.TabIndex = 0;
            label16.Text = "豆瓣标题";
            // 
            // FormEdit
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btCancel;
            ClientSize = new System.Drawing.Size(681, 729);
            Controls.Add(cbWatched);
            Controls.Add(tbComment);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(dtPicker);
            Controls.Add(btCancel);
            Controls.Add(btOk);
            Controls.Add(tbOtherName);
            Controls.Add(label5);
            Controls.Add(tbKeyName);
            Controls.Add(label4);
            Controls.Add(tbGenres);
            Controls.Add(label6);
            Controls.Add(tbZone);
            Controls.Add(label9);
            Controls.Add(tbDoubanSubTitle);
            Controls.Add(tbRating);
            Controls.Add(tbposterpath);
            Controls.Add(tbCasts);
            Controls.Add(tbDirectors);
            Controls.Add(tbDoubanTitle);
            Controls.Add(label14);
            Controls.Add(tbDoubanId);
            Controls.Add(label15);
            Controls.Add(label13);
            Controls.Add(label11);
            Controls.Add(label12);
            Controls.Add(tbYear);
            Controls.Add(label16);
            Controls.Add(label10);
            Controls.Add(label3);
            Controls.Add(tbNewName);
            Controls.Add(label2);
            Controls.Add(tbOldName);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormEdit";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "修改记录";
            Load += FormRenameTorrent_Load;
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbDoubanTitle;
        private System.Windows.Forms.TextBox tbDoubanSubTitle;
        private System.Windows.Forms.Label label16;
    }
}