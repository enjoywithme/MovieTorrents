namespace PostPrin
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.label1 = new System.Windows.Forms.Label();
            this.tbSendName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbSendMobile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbSendPlace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSendAddress = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSendCompany = new System.Windows.Forms.TextBox();
            this.tbSendPhone = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSendZone = new System.Windows.Forms.TextBox();
            this.tbSendCity = new System.Windows.Forms.TextBox();
            this.tbSendProvince = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbRecvMobile = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbRecvPlace = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbRecvAddress = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbRecvCompany = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbRecvPhone = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbRecvName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbRecvZone = new System.Windows.Forms.TextBox();
            this.tbRecvCity = new System.Windows.Forms.TextBox();
            this.tbRecvProvince = new System.Windows.Forms.TextBox();
            this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.pageSetupDialog = new System.Windows.Forms.PageSetupDialog();
            this.label13 = new System.Windows.Forms.Label();
            this.cbPrinter = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cbPostFormats = new System.Windows.Forms.ComboBox();
            this.btPostFormat = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.设置SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiManagePostFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiFont = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btExit = new System.Windows.Forms.Button();
            this.tbPrint = new System.Windows.Forms.Button();
            this.btLoadContact = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btClear = new System.Windows.Forms.Button();
            this.btPreview = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "寄件人姓名";
            // 
            // tbSendName
            // 
            this.tbSendName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendName.Location = new System.Drawing.Point(113, 20);
            this.tbSendName.Name = "tbSendName";
            this.tbSendName.Size = new System.Drawing.Size(85, 14);
            this.tbSendName.TabIndex = 0;
            this.tbSendName.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbSendMobile);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbSendPlace);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbSendAddress);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbSendCompany);
            this.groupBox1.Controls.Add(this.tbSendPhone);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbSendZone);
            this.groupBox1.Controls.Add(this.tbSendCity);
            this.groupBox1.Controls.Add(this.tbSendProvince);
            this.groupBox1.Controls.Add(this.tbSendName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 168);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // tbSendMobile
            // 
            this.tbSendMobile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendMobile.Location = new System.Drawing.Point(288, 140);
            this.tbSendMobile.Name = "tbSendMobile";
            this.tbSendMobile.Size = new System.Drawing.Size(113, 14);
            this.tbSendMobile.TabIndex = 8;
            this.tbSendMobile.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(253, 142);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "手机";
            // 
            // tbSendPlace
            // 
            this.tbSendPlace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendPlace.Location = new System.Drawing.Point(316, 20);
            this.tbSendPlace.Name = "tbSendPlace";
            this.tbSendPlace.Size = new System.Drawing.Size(85, 14);
            this.tbSendPlace.TabIndex = 1;
            this.tbSendPlace.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(253, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "始发地";
            // 
            // tbSendAddress
            // 
            this.tbSendAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendAddress.Location = new System.Drawing.Point(113, 96);
            this.tbSendAddress.Multiline = true;
            this.tbSendAddress.Name = "tbSendAddress";
            this.tbSendAddress.Size = new System.Drawing.Size(288, 34);
            this.tbSendAddress.TabIndex = 6;
            this.tbSendAddress.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(384, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 12);
            this.label17.TabIndex = 0;
            this.label17.Text = "区";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(284, 73);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(17, 12);
            this.label16.TabIndex = 0;
            this.label16.Text = "市";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(187, 73);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 12);
            this.label15.TabIndex = 0;
            this.label15.Text = "省";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "寄件人详细地址";
            // 
            // tbSendCompany
            // 
            this.tbSendCompany.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendCompany.Location = new System.Drawing.Point(113, 47);
            this.tbSendCompany.Name = "tbSendCompany";
            this.tbSendCompany.Size = new System.Drawing.Size(288, 14);
            this.tbSendCompany.TabIndex = 2;
            this.tbSendCompany.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // tbSendPhone
            // 
            this.tbSendPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendPhone.Location = new System.Drawing.Point(113, 140);
            this.tbSendPhone.Name = "tbSendPhone";
            this.tbSendPhone.Size = new System.Drawing.Size(134, 14);
            this.tbSendPhone.TabIndex = 7;
            this.tbSendPhone.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "单位名称";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "联系电话";
            // 
            // tbSendZone
            // 
            this.tbSendZone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendZone.Location = new System.Drawing.Point(311, 72);
            this.tbSendZone.Name = "tbSendZone";
            this.tbSendZone.Size = new System.Drawing.Size(60, 14);
            this.tbSendZone.TabIndex = 5;
            this.tbSendZone.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // tbSendCity
            // 
            this.tbSendCity.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendCity.Location = new System.Drawing.Point(214, 72);
            this.tbSendCity.Name = "tbSendCity";
            this.tbSendCity.Size = new System.Drawing.Size(60, 14);
            this.tbSendCity.TabIndex = 4;
            this.tbSendCity.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // tbSendProvince
            // 
            this.tbSendProvince.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSendProvince.Location = new System.Drawing.Point(113, 72);
            this.tbSendProvince.Name = "tbSendProvince";
            this.tbSendProvince.Size = new System.Drawing.Size(60, 14);
            this.tbSendProvince.TabIndex = 3;
            this.tbSendProvince.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbRecvMobile);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tbRecvPlace);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbRecvAddress);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.tbRecvCompany);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.tbRecvPhone);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.tbRecvName);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tbRecvZone);
            this.groupBox2.Controls.Add(this.tbRecvCity);
            this.groupBox2.Controls.Add(this.tbRecvProvince);
            this.groupBox2.Location = new System.Drawing.Point(13, 294);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(423, 168);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // tbRecvMobile
            // 
            this.tbRecvMobile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvMobile.Location = new System.Drawing.Point(288, 137);
            this.tbRecvMobile.Name = "tbRecvMobile";
            this.tbRecvMobile.Size = new System.Drawing.Size(113, 14);
            this.tbRecvMobile.TabIndex = 8;
            this.tbRecvMobile.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(253, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "手机";
            // 
            // tbRecvPlace
            // 
            this.tbRecvPlace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvPlace.Location = new System.Drawing.Point(316, 20);
            this.tbRecvPlace.Name = "tbRecvPlace";
            this.tbRecvPlace.Size = new System.Drawing.Size(85, 14);
            this.tbRecvPlace.TabIndex = 1;
            this.tbRecvPlace.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(253, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "目的地";
            // 
            // tbRecvAddress
            // 
            this.tbRecvAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvAddress.Location = new System.Drawing.Point(114, 95);
            this.tbRecvAddress.Multiline = true;
            this.tbRecvAddress.Name = "tbRecvAddress";
            this.tbRecvAddress.Size = new System.Drawing.Size(288, 34);
            this.tbRecvAddress.TabIndex = 6;
            this.tbRecvAddress.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(385, 73);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(17, 12);
            this.label20.TabIndex = 0;
            this.label20.Text = "区";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "收件人详细地址";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(285, 73);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 12);
            this.label19.TabIndex = 0;
            this.label19.Text = "市";
            // 
            // tbRecvCompany
            // 
            this.tbRecvCompany.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvCompany.Location = new System.Drawing.Point(113, 47);
            this.tbRecvCompany.Name = "tbRecvCompany";
            this.tbRecvCompany.Size = new System.Drawing.Size(288, 14);
            this.tbRecvCompany.TabIndex = 2;
            this.tbRecvCompany.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(188, 73);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(17, 12);
            this.label18.TabIndex = 0;
            this.label18.Text = "省";
            // 
            // tbRecvPhone
            // 
            this.tbRecvPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvPhone.Location = new System.Drawing.Point(113, 137);
            this.tbRecvPhone.Name = "tbRecvPhone";
            this.tbRecvPhone.Size = new System.Drawing.Size(134, 14);
            this.tbRecvPhone.TabIndex = 7;
            this.tbRecvPhone.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "单位名称";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 139);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "联系电话";
            // 
            // tbRecvName
            // 
            this.tbRecvName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvName.Location = new System.Drawing.Point(113, 20);
            this.tbRecvName.Name = "tbRecvName";
            this.tbRecvName.Size = new System.Drawing.Size(85, 14);
            this.tbRecvName.TabIndex = 0;
            this.tbRecvName.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "收件人姓名";
            // 
            // tbRecvZone
            // 
            this.tbRecvZone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvZone.Location = new System.Drawing.Point(319, 72);
            this.tbRecvZone.Name = "tbRecvZone";
            this.tbRecvZone.Size = new System.Drawing.Size(60, 14);
            this.tbRecvZone.TabIndex = 5;
            this.tbRecvZone.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // tbRecvCity
            // 
            this.tbRecvCity.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvCity.Location = new System.Drawing.Point(215, 72);
            this.tbRecvCity.Name = "tbRecvCity";
            this.tbRecvCity.Size = new System.Drawing.Size(60, 14);
            this.tbRecvCity.TabIndex = 4;
            this.tbRecvCity.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // tbRecvProvince
            // 
            this.tbRecvProvince.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRecvProvince.Location = new System.Drawing.Point(114, 72);
            this.tbRecvProvince.Name = "tbRecvProvince";
            this.tbRecvProvince.Size = new System.Drawing.Size(60, 14);
            this.tbRecvProvince.TabIndex = 3;
            this.tbRecvProvince.Enter += new System.EventHandler(this.textBox_GotFocus);
            // 
            // printPreviewDialog
            // 
            this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog.Document = this.printDocument;
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
            this.printPreviewDialog.Name = "printPreviewDialog";
            this.printPreviewDialog.ShowIcon = false;
            this.printPreviewDialog.Visible = false;
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
            // 
            // pageSetupDialog
            // 
            this.pageSetupDialog.AllowMargins = false;
            this.pageSetupDialog.Document = this.printDocument;
            this.pageSetupDialog.EnableMetric = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 17);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 4;
            this.label13.Text = "选择打印机";
            // 
            // cbPrinter
            // 
            this.cbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrinter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPrinter.FormattingEnabled = true;
            this.cbPrinter.Location = new System.Drawing.Point(113, 14);
            this.cbPrinter.Name = "cbPrinter";
            this.cbPrinter.Size = new System.Drawing.Size(288, 20);
            this.cbPrinter.TabIndex = 0;
            this.cbPrinter.SelectedIndexChanged += new System.EventHandler(this.cbPrinter_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(18, 47);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 4;
            this.label14.Text = "选择快递面单";
            // 
            // cbPostFormats
            // 
            this.cbPostFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPostFormats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPostFormats.FormattingEnabled = true;
            this.cbPostFormats.Location = new System.Drawing.Point(113, 44);
            this.cbPostFormats.Name = "cbPostFormats";
            this.cbPostFormats.Size = new System.Drawing.Size(252, 20);
            this.cbPostFormats.TabIndex = 1;
            this.cbPostFormats.SelectedIndexChanged += new System.EventHandler(this.cbPostFormat_SelectedIndexChanged);
            // 
            // btPostFormat
            // 
            this.btPostFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btPostFormat.Location = new System.Drawing.Point(371, 44);
            this.btPostFormat.Name = "btPostFormat";
            this.btPostFormat.Size = new System.Drawing.Size(30, 23);
            this.btPostFormat.TabIndex = 2;
            this.btPostFormat.Text = "..";
            this.toolTip.SetToolTip(this.btPostFormat, "管理快递面单格式");
            this.btPostFormat.UseVisualStyleBackColor = true;
            this.btPostFormat.Click += new System.EventHandler(this.btPostFormat_Click);
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置SToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.ShowItemToolTips = true;
            this.mainMenuStrip.Size = new System.Drawing.Size(563, 24);
            this.mainMenuStrip.TabIndex = 7;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // 设置SToolStripMenuItem
            // 
            this.设置SToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiManagePostFormat,
            this.tsiFont});
            this.设置SToolStripMenuItem.Name = "设置SToolStripMenuItem";
            this.设置SToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.设置SToolStripMenuItem.Text = "设置(&S)";
            // 
            // tsiManagePostFormat
            // 
            this.tsiManagePostFormat.Name = "tsiManagePostFormat";
            this.tsiManagePostFormat.Size = new System.Drawing.Size(136, 22);
            this.tsiManagePostFormat.Text = "面单格式(&P)";
            this.tsiManagePostFormat.Click += new System.EventHandler(this.tsiManagePostFormat_Click);
            // 
            // tsiFont
            // 
            this.tsiFont.Name = "tsiFont";
            this.tsiFont.Size = new System.Drawing.Size(136, 22);
            this.tsiFont.Text = "字体(&F)";
            this.tsiFont.Click += new System.EventHandler(this.tsiFont_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.btPostFormat);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.cbPostFormats);
            this.groupBox3.Controls.Add(this.cbPrinter);
            this.groupBox3.Location = new System.Drawing.Point(13, 31);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(423, 73);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            // 
            // btExit
            // 
            this.btExit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btExit.Image = global::PostPrin.Properties.Resources.exit;
            this.btExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btExit.Location = new System.Drawing.Point(456, 348);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(91, 50);
            this.btExit.TabIndex = 7;
            this.btExit.Text = "退出(&E)";
            this.btExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btExit, "退出程序");
            this.btExit.UseVisualStyleBackColor = true;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // tbPrint
            // 
            this.tbPrint.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tbPrint.Image = global::PostPrin.Properties.Resources.printer;
            this.tbPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tbPrint.Location = new System.Drawing.Point(456, 288);
            this.tbPrint.Name = "tbPrint";
            this.tbPrint.Size = new System.Drawing.Size(91, 50);
            this.tbPrint.TabIndex = 7;
            this.tbPrint.Text = "打印(&P)";
            this.tbPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.tbPrint, "打印面单");
            this.tbPrint.UseVisualStyleBackColor = true;
            this.tbPrint.Click += new System.EventHandler(this.tbPrint_Click);
            // 
            // btLoadContact
            // 
            this.btLoadContact.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btLoadContact.Image = global::PostPrin.Properties.Resources.upload;
            this.btLoadContact.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btLoadContact.Location = new System.Drawing.Point(456, 40);
            this.btLoadContact.Name = "btLoadContact";
            this.btLoadContact.Size = new System.Drawing.Size(91, 50);
            this.btLoadContact.TabIndex = 3;
            this.btLoadContact.Text = "读入(&C)";
            this.btLoadContact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btLoadContact, "读入联系人信息");
            this.btLoadContact.UseVisualStyleBackColor = true;
            this.btLoadContact.Click += new System.EventHandler(this.btLoadContact_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Image = global::PostPrin.Properties.Resources.save;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(456, 164);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 50);
            this.button1.TabIndex = 5;
            this.button1.Text = "保存(&C)";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.button1, "保存联系人信息");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btClear
            // 
            this.btClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btClear.Image = global::PostPrin.Properties.Resources.clear;
            this.btClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btClear.Location = new System.Drawing.Point(456, 102);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(91, 50);
            this.btClear.TabIndex = 4;
            this.btClear.Text = "清空(&C)";
            this.btClear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btClear, "清空联系人信息");
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btPreview
            // 
            this.btPreview.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btPreview.Image = global::PostPrin.Properties.Resources.Preview;
            this.btPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btPreview.Location = new System.Drawing.Point(456, 226);
            this.btPreview.Name = "btPreview";
            this.btPreview.Size = new System.Drawing.Size(91, 50);
            this.btPreview.TabIndex = 6;
            this.btPreview.Text = "预览(&V)";
            this.btPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btPreview, "预览面单");
            this.btPreview.UseVisualStyleBackColor = true;
            this.btPreview.Click += new System.EventHandler(this.btPreview_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 485);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.tbPrint);
            this.Controls.Add(this.btLoadContact);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.btPreview);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mainMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "快递面单打印";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSendName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbSendMobile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbSendPlace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSendAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSendCompany;
        private System.Windows.Forms.TextBox tbSendPhone;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbRecvMobile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbRecvPlace;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRecvAddress;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbRecvCompany;
        private System.Windows.Forms.TextBox tbRecvPhone;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbRecvName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btPreview;
        private System.Windows.Forms.Button tbPrint;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbPrinter;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cbPostFormats;
        private System.Windows.Forms.Button btPostFormat;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 设置SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsiManagePostFormat;
        private System.Windows.Forms.ToolStripMenuItem tsiFont;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbSendZone;
        private System.Windows.Forms.TextBox tbSendCity;
        private System.Windows.Forms.TextBox tbSendProvince;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbRecvZone;
        private System.Windows.Forms.TextBox tbRecvCity;
        private System.Windows.Forms.TextBox tbRecvProvince;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btLoadContact;
        private System.Windows.Forms.Button btExit;
    }
}

