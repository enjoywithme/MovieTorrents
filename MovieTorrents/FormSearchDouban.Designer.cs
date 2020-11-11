namespace MovieTorrents
{
    partial class FormSearchDouban
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
            this.tbSearchText = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTitle1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btSearchName = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btSave = new System.Windows.Forms.Button();
            this.tbOrigTitle = new System.Windows.Forms.TextBox();
            this.btSearchId = new System.Windows.Forms.Button();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.btnSearchBrowser = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbSearchText
            // 
            this.tbSearchText.Location = new System.Drawing.Point(24, 13);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(560, 21);
            this.tbSearchText.TabIndex = 0;
            this.tbSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearchText_KeyDown);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderTitle1,
            this.columnHeaderYear,
            this.columnHeaderType});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(179, 79);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(570, 300);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "标题";
            this.columnHeaderTitle.Width = 200;
            // 
            // columnHeaderTitle1
            // 
            this.columnHeaderTitle1.Text = "副标题";
            this.columnHeaderTitle1.Width = 200;
            // 
            // columnHeaderYear
            // 
            this.columnHeaderYear.Text = "年份";
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "类型";
            // 
            // btSearchName
            // 
            this.btSearchName.Location = new System.Drawing.Point(590, 13);
            this.btSearchName.Name = "btSearchName";
            this.btSearchName.Size = new System.Drawing.Size(88, 23);
            this.btSearchName.TabIndex = 2;
            this.btSearchName.Text = "搜素名称(&F)";
            this.btSearchName.UseVisualStyleBackColor = true;
            this.btSearchName.Click += new System.EventHandler(this.btSearch_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(24, 79);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(137, 193);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(43, 297);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(89, 23);
            this.btSave.TabIndex = 4;
            this.btSave.Text = "保存选取(&S)";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // tbOrigTitle
            // 
            this.tbOrigTitle.Location = new System.Drawing.Point(24, 44);
            this.tbOrigTitle.Name = "tbOrigTitle";
            this.tbOrigTitle.ReadOnly = true;
            this.tbOrigTitle.Size = new System.Drawing.Size(560, 21);
            this.tbOrigTitle.TabIndex = 5;
            // 
            // btSearchId
            // 
            this.btSearchId.Location = new System.Drawing.Point(590, 42);
            this.btSearchId.Name = "btSearchId";
            this.btSearchId.Size = new System.Drawing.Size(88, 23);
            this.btSearchId.TabIndex = 2;
            this.btSearchId.Text = "ID查询(&S)";
            this.btSearchId.UseVisualStyleBackColor = true;
            this.btSearchId.Click += new System.EventHandler(this.btSearchId_Click);
            // 
            // tbInfo
            // 
            this.tbInfo.BackColor = System.Drawing.SystemColors.Info;
            this.tbInfo.Location = new System.Drawing.Point(24, 394);
            this.tbInfo.Multiline = true;
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            this.tbInfo.Size = new System.Drawing.Size(725, 51);
            this.tbInfo.TabIndex = 6;
            // 
            // btnSearchBrowser
            // 
            this.btnSearchBrowser.Location = new System.Drawing.Point(684, 13);
            this.btnSearchBrowser.Name = "btnSearchBrowser";
            this.btnSearchBrowser.Size = new System.Drawing.Size(65, 52);
            this.btnSearchBrowser.TabIndex = 7;
            this.btnSearchBrowser.Text = "浏览器搜索（&B)";
            this.btnSearchBrowser.UseVisualStyleBackColor = true;
            this.btnSearchBrowser.Click += new System.EventHandler(this.btnSearchBrowser_Click);
            // 
            // FormSearchDouban
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 465);
            this.Controls.Add(this.btnSearchBrowser);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.tbOrigTitle);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btSearchId);
            this.Controls.Add(this.btSearchName);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.tbSearchText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSearchDouban";
            this.ShowInTaskbar = false;
            this.Text = "搜索豆瓣信息";
            this.Load += new System.EventHandler(this.FormSearchDouban_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSearchDouban_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSearchText;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle1;
        private System.Windows.Forms.ColumnHeader columnHeaderYear;
        private System.Windows.Forms.Button btSearchName;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.TextBox tbOrigTitle;
        private System.Windows.Forms.Button btSearchId;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.Button btnSearchBrowser;
    }
}