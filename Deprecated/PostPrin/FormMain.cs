using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace PostPrin
{
    public partial class FormMain : Form
    {
        private Font defautFont = new Font("宋体", 11);

        private PostFormat postFormat = PostFormatCollection.DefaultPostFormat;
        private Boolean loadingPostFormat = true;

        // SystemMenu object
        private SystemMenu m_SystemMenu = null;
        // ID constants
        private const int m_AboutID = 0x100;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //得到打印机列表
            int index;
            string defaultPrinterName = PrinterHelper.GetDefaultPrinter();
            foreach (string strPrinter in PrinterSettings.InstalledPrinters)
            {
                index = cbPrinter.Items.Add(strPrinter);
                if (strPrinter == defaultPrinterName)
                {
                    cbPrinter.SelectedIndex = index;
                }
            }

            //设置默认打印属性
            printDocument.DocumentName = "快递面单打印";


            printDocument.DefaultPageSettings.Margins.Top = 0;
            printDocument.DefaultPageSettings.Margins.Left = 0;
            printDocument.DefaultPageSettings.Margins.Right = 0;
            printDocument.DefaultPageSettings.Margins.Bottom = 0;

            PostFormatCollection.LoadPostFormatsFromIniFile();
            LoadPostFormats("");

            try
            {
                m_SystemMenu = SystemMenu.FromForm(this);
                // Add a separator ...
                m_SystemMenu.AppendSeparator();
                // ... and an "About" entry
                m_SystemMenu.AppendMenu(m_AboutID, "关于本程序(&A)");

            }
            catch (NoSystemMenuException /* err */ )
            {
                // Do some error handling
            }
        }

        private void LoadPostFormats(string selPostFormat)
        {
            loadingPostFormat = true;

            cbPostFormats.Items.Clear();

            //初始化面单列表
            int i = -1;
            int j = -1;
            foreach (PostFormat postFormat in PostFormatCollection.PostFormats.Values)
            {
                int index = cbPostFormats.Items.Add(postFormat.Name);
                if (postFormat.IsDefault) j = index;
                if (selPostFormat != "" && postFormat.Name == selPostFormat) i = index;
            }
            loadingPostFormat = false;

            if (i >= 0) cbPostFormats.SelectedIndex = i;
            else cbPostFormats.SelectedIndex = j;
        }

        //设置当前打印机
        private void ChangePrintSetting()
        {
            printDocument.PrinterSettings.PrinterName = cbPrinter.Text;
            //postFormat.AddPaperFormat(printDocument.PrinterSettings.PrinterName);
            //foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
            //{
            //    if (paperSize.PaperName == postFormat.PaperName)
            //    {
            //        printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
            //        break;
            //    }
            //}

            printDocument.DefaultPageSettings.PaperSize = postFormat.PaperSize;
            printDocument.DefaultPageSettings.Landscape = postFormat.Landscape;
        }

        private void cbPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangePrintSetting();
        }

        private void btPreview_Click(object sender, EventArgs e)
        {
            try
            {
                FormPrintPreview formpreview = new FormPrintPreview(printDocument);
                formpreview.ShowDialog();
                //if (pageSetupDialog.ShowDialog(this) == DialogResult.OK)
                //{
                //    //System.Diagnostics.Debug.WriteLine(printDocument.DefaultPageSettings.PaperSize.PaperName);
                //    printPreviewDialog.ShowDialog(this);
                //}
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("{0}\n请选择的其他的打印机重试。", exception.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void tbPrint_Click(object sender, EventArgs e)
        {
            //if (pageSetupDialog.ShowDialog(this) == DialogResult.OK)
            printDocument.Print();
        }

        private string FilterAddress(string text)
        {
            string s = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\n' && text[i] != '\r') s += text[i];
                else s += ' ';
            }
            return s;
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;
            e.Graphics.PageScale = 1.0f;
            float HardMarginX =
                (float)PrinterUnitConvert.Convert(e.PageSettings.PrintableArea.Left * 10, PrinterUnit.ThousandthsOfAnInch,
                                           PrinterUnit.TenthsOfAMillimeter) / 10.0f;
            float HardMarginY =
                (float)PrinterUnitConvert.Convert(e.PageSettings.PrintableArea.Top * 10, PrinterUnit.ThousandthsOfAnInch,
                                           PrinterUnit.TenthsOfAMillimeter) / 10.0f;

            e.Graphics.TranslateTransform(-HardMarginX + (int)postFormat.Values["OffsetX"], -HardMarginY + (int)postFormat.Values["OffsetY"]);
            //打印发件人信息
            if (!string.IsNullOrEmpty(tbSendName.Text))
                e.Graphics.DrawString(tbSendName.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendNameX"], (int)postFormat.Values["SendNameY"]);
            if (!string.IsNullOrEmpty(tbSendPlace.Text))
                e.Graphics.DrawString(tbSendPlace.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendPlaceX"], (int)postFormat.Values["SendPlaceY"]);
            if (!string.IsNullOrEmpty(tbSendCompany.Text))
                e.Graphics.DrawString(tbSendCompany.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendCompanyX"], (int)postFormat.Values["SendCompanyY"]);
            string fullSendAddress = "";
            if(!string.IsNullOrEmpty(tbSendProvince.Text)&&(int)postFormat.Values["SendProvinceLen"]>0)
            {
                e.Graphics.DrawString(tbSendProvince.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendProvinceX"], (int)postFormat.Values["SendProvinceY"]);
            }
            else if(!string.IsNullOrEmpty(tbSendProvince.Text))
            {
                fullSendAddress += tbSendProvince.Text + "省";
            }
            if (!string.IsNullOrEmpty(tbSendCity.Text) && (int)postFormat.Values["SendCityLen"] > 0)
            {
                e.Graphics.DrawString(tbSendCity.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendCityX"], (int)postFormat.Values["SendCityY"]);
            }
            else if (!string.IsNullOrEmpty(tbSendCity.Text))
            {
                fullSendAddress += tbSendCity.Text + "市";
            }
            if (!string.IsNullOrEmpty(tbSendZone.Text) && (int)postFormat.Values["SendZoneLen"] > 0)
            {
                e.Graphics.DrawString(tbSendZone.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendZoneX"], (int)postFormat.Values["SendZoneY"]);
            }
            else if (!string.IsNullOrEmpty(tbSendZone.Text))
            {
                fullSendAddress += tbSendZone.Text + "区(镇)";
            }
            fullSendAddress += FilterAddress(tbSendAddress.Text);
            if (!string.IsNullOrEmpty(fullSendAddress))
            {
                SizeF sf = e.Graphics.MeasureString(fullSendAddress, defautFont);
                if (sf.Width <= (int)postFormat.Values["RecvAddress1Len"]||(int)postFormat.Values["SendAddress2Len"]==0)
                {
                    e.Graphics.DrawString(fullSendAddress, defautFont, Brushes.Black, (int)postFormat.Values["SendAddress1X"], (int)postFormat.Values["SendAddress1Y"]);
                }
                else
                {
                    int line1Max = (int)((float)(int)postFormat.Values["RecvAddress1Len"] / sf.Width * fullSendAddress.Length) - 1;
                    e.Graphics.DrawString(fullSendAddress.Substring(0, line1Max), defautFont, Brushes.Black, (int)postFormat.Values["SendAddress1X"], (int)postFormat.Values["SendAddress1Y"]);
                    e.Graphics.DrawString(fullSendAddress.Substring(line1Max, fullSendAddress.Length - line1Max), defautFont, Brushes.Black, (int)postFormat.Values["SendAddress2X"], (int)postFormat.Values["SendAddress2Y"]);
                }
            }
            if (!string.IsNullOrEmpty(tbSendPhone.Text))
                e.Graphics.DrawString(tbSendPhone.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendPhoneX"], (int)postFormat.Values["SendPhoneY"]);
            if (!string.IsNullOrEmpty(tbSendMobile.Text))
                e.Graphics.DrawString(tbSendMobile.Text, defautFont, Brushes.Black, (int)postFormat.Values["SendMobileX"], (int)postFormat.Values["SendmobileY"]);
            //打印收件人信息
            if (!string.IsNullOrEmpty(tbRecvName.Text))
                e.Graphics.DrawString(tbRecvName.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvNameX"], (int)postFormat.Values["RecvNameY"]);
            if (!string.IsNullOrEmpty(tbRecvPlace.Text))
                e.Graphics.DrawString(tbRecvPlace.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvPlaceX"], (int)postFormat.Values["RecvPlaceY"]);
            if (!string.IsNullOrEmpty(tbRecvCompany.Text))
                e.Graphics.DrawString(tbRecvCompany.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvCompanyX"], (int)postFormat.Values["RecvCompanyY"]);
            string fullRecvAddress = "";
            if (!string.IsNullOrEmpty(tbRecvProvince.Text) && (int)postFormat.Values["RecvProvinceLen"] > 0)
            {
                e.Graphics.DrawString(tbRecvProvince.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvProvinceX"], (int)postFormat.Values["RecvProvinceY"]);
            }
            else if (!string.IsNullOrEmpty(tbRecvProvince.Text))
            {
                fullRecvAddress += tbRecvProvince.Text + "省";
            }
            if (!string.IsNullOrEmpty(tbRecvCity.Text) && (int)postFormat.Values["RecvCityLen"] > 0)
            {
                e.Graphics.DrawString(tbRecvCity.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvCityX"], (int)postFormat.Values["RecvCityY"]);
            }
            else if (!string.IsNullOrEmpty(tbRecvCity.Text))
            {
                fullRecvAddress += tbRecvCity.Text + "市";
            }
            if (!string.IsNullOrEmpty(tbRecvZone.Text) && (int)postFormat.Values["RecvZoneLen"] > 0)
            {
                e.Graphics.DrawString(tbRecvZone.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvZoneX"], (int)postFormat.Values["RecvZoneY"]);
            }
            else if (!string.IsNullOrEmpty(tbRecvZone.Text))
            {
                fullRecvAddress += tbRecvZone.Text + "区(镇)";
            }
            fullRecvAddress += FilterAddress(tbRecvAddress.Text);
            if (!string.IsNullOrEmpty(fullRecvAddress))
            {
                SizeF sf = e.Graphics.MeasureString(fullRecvAddress, defautFont);
                if (sf.Width <= (int)postFormat.Values["RecvAddress1Len"] || (int)postFormat.Values["RecvAddress2Len"] == 0)
                {
                    e.Graphics.DrawString(fullRecvAddress, defautFont, Brushes.Black, (int)postFormat.Values["RecvAddress1X"], (int)postFormat.Values["RecvAddress1Y"]);
                }
                else
                {
                    int line1Max = (int)((float)(int)postFormat.Values["RecvAddress1Len"] / sf.Width * fullRecvAddress.Length) - 1;
                    e.Graphics.DrawString(fullRecvAddress.Substring(0, line1Max), defautFont, Brushes.Black, (int)postFormat.Values["RecvAddress1X"], (int)postFormat.Values["RecvAddress1Y"]);
                    e.Graphics.DrawString(fullRecvAddress.Substring(line1Max, fullRecvAddress.Length - line1Max), defautFont, Brushes.Black, (int)postFormat.Values["RecvAddress2X"], (int)postFormat.Values["RecvAddress2Y"]);
                }
            }
            if (!string.IsNullOrEmpty(tbRecvPhone.Text))
                e.Graphics.DrawString(tbRecvPhone.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvPhoneX"], (int)postFormat.Values["RecvPhoneY"]);
            if (!string.IsNullOrEmpty(tbRecvMobile.Text))
                e.Graphics.DrawString(tbRecvMobile.Text, defautFont, Brushes.Black, (int)postFormat.Values["RecvMobileX"], (int)postFormat.Values["RecvmobileY"]);
        }

        private void ManagePostFormat(string postFormatName)
        {
            string selPostFormat = cbPostFormats.Text;
            FormPostFormat formPostFormat = new FormPostFormat(postFormatName);
            formPostFormat.ShowDialog();
            LoadPostFormats(selPostFormat);
            ChangePrintSetting();
        }

        private void btPostFormat_Click(object sender, EventArgs e)
        {
            ManagePostFormat(cbPostFormats.Text);
        }

        private void tsiManagePostFormat_Click(object sender, EventArgs e)
        {
            ManagePostFormat("");
        }

        private void tsiFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = defautFont;
            if (fontDialog.ShowDialog() == DialogResult.OK) defautFont = fontDialog.Font;
        }

        private delegate void SelectAllDelegate(TextBox textBox);

        private IAsyncResult _selectAllar = null;
        void textBox_GotFocus(object sender, EventArgs e)
        {
            //We could have gotten here many ways (including mouse click)
            //so there could be other messages queued up already that might change the selection.
            //Don't call SelectAll here, since it might get undone by things such as positioning the cursor.
            //Instead use BeginInvoke on the form to queue up a message
            //to select all the text after everything caused by the current event is processed.
            this._selectAllar = this.BeginInvoke(new SelectAllDelegate(this._SelectAll), sender as TextBox);
        }
        private void _SelectAll(TextBox textBox)
        {
            //Clean-up the BeginInvoke
            if (this._selectAllar != null)
            {
                this.EndInvoke(this._selectAllar);
            }
            //Now select everything.
            textBox.SelectAll();
        }

        private void cbPostFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadingPostFormat && cbPostFormats.Text != postFormat.Name)
            {
                postFormat = (PostFormat)PostFormatCollection.PostFormats[cbPostFormats.Text];
                ChangePrintSetting();
            }
        }



        private void btClear_Click(object sender, EventArgs e)
        {
            tbSendName.Text = "";
            tbSendPlace.Text = "";
            tbSendCompany.Text = "";
            tbSendAddress.Text = "";
            tbSendPhone.Text = "";
            tbSendMobile.Text = "";

            tbRecvName.Text = "";
            tbRecvPlace.Text = "";
            tbRecvCompany.Text = "";
            tbRecvAddress.Text = "";
            tbRecvPhone.Text = "";
            tbRecvMobile.Text = "";

        }

        protected override void WndProc(ref Message msg)
        {
            // Now we should catch the WM_SYSCOMMAND message and process it.
            // We override the WndProc to catch the WM_SYSCOMMAND message.
            // You don't have to look this message up in the MSDN; it is
            // defined in WindowMessages enumeration.
            // The WParam of the message contains the ID that was pressed.
            // It is the same value you have passed through InsertMenu()
            // or AppendMenu() member functions of my class.
            // Just check for them and do the proper action.
            //
            if (msg.Msg == (int)WindowMessages.wmSysCommand)
            {
                switch (msg.WParam.ToInt32())
                {

                    case m_AboutID:
                        { // Our about id
                            MessageBox.Show(this, "本程序由DuanXin编写.\n邮件:shuiliuduan@163.com", "关于");
                        } break;

                    // TODO: Add more handles, for more menu items

                }
            }
            // Call base class function
            base.WndProc(ref msg);
        }

        private void btLoadContact_Click(object sender, EventArgs e)
        {
            FormContacts formContacts = new FormContacts(true);
            if (formContacts.ShowDialog() == DialogResult.OK)
            {
                Contact contact = new Contact(formContacts.ContactName);
                contact.ReadFromIniFile();

                tbSendName.Text = (string)contact.Values["SendName"];
                tbSendPlace.Text = (string)contact.Values["SendPlace"];
                tbSendCompany.Text = (string)contact.Values["SendCompany"];
                tbSendAddress.Text = (string)contact.Values["SendAddress"];
                tbSendProvince.Text = (string)contact.Values["SendProvince"];
                tbSendCity.Text = (string)contact.Values["SendCity"];
                tbSendZone.Text = (string)contact.Values["SendZone"];
                tbSendPhone.Text = (string)contact.Values["SendPhone"];
                tbSendMobile.Text = (string)contact.Values["SendMobile"];

                tbRecvName.Text = (string)contact.Values["RecvName"];
                tbRecvPlace.Text = (string)contact.Values["RecvPlace"];
                tbRecvCompany.Text = (string)contact.Values["RecvCompany"];
                tbRecvAddress.Text = (string)contact.Values["RecvAddress"];
                tbRecvProvince.Text = (string)contact.Values["RecvProvince"];
                tbRecvCity.Text = (string)contact.Values["RecvCity"];
                tbRecvZone.Text = (string)contact.Values["RecvZone"];
                tbRecvPhone.Text = (string)contact.Values["RecvPhone"];
                tbRecvMobile.Text = (string)contact.Values["RecvMobile"];

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Contact contact = new Contact();
            contact.Values["SendName"] = tbSendName.Text;
            contact.Values["SendPlace"] = tbSendPlace.Text;
            contact.Values["SendCompany"] = tbSendCompany.Text;
            contact.Values["SendAddress"] = FilterAddress(tbSendAddress.Text);
            contact.Values["SendProvince"] = tbSendProvince.Text;
            contact.Values["SendCity"] = tbSendCity.Text;
            contact.Values["SendZone"] = tbSendZone.Text;
            contact.Values["SendPhone"] = tbSendPhone.Text;
            contact.Values["SendMobile"] = tbSendMobile.Text;

            contact.Values["RecvName"] = tbRecvName.Text;
            contact.Values["RecvPlace"] = tbRecvPlace.Text;
            contact.Values["RecvCompany"] = tbRecvCompany.Text;
            contact.Values["RecvAddress"] = FilterAddress(tbRecvAddress.Text);
            contact.Values["RecvProvince"] =tbRecvProvince.Text;
            contact.Values["RecvCity"] =tbRecvCity.Text ;
            contact.Values["RecvZone"]= tbRecvZone.Text ;
            contact.Values["RecvPhone"] = tbRecvPhone.Text;
            contact.Values["RecvMobile"] = tbRecvMobile.Text;

            FormContacts formContacts = new FormContacts();
            if (formContacts.ShowDialog() == DialogResult.OK)
            {
                contact.Name = formContacts.ContactName;
                contact.WriteToIniFile();
            }
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
