using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PostPrin
{
    public partial class FormPostFormat : Form
    {
        private string initialPostFormat = "";
        private Boolean postFormatChanged = false;
        private Boolean postFormatSizeChanged = false;
        private Boolean dataLoading = true;
        private PostFormat postFormat = null;

        public FormPostFormat()
        {
            InitializeComponent();
        }

        public FormPostFormat(string selectPostFormat)
        {
            InitializeComponent();
            initialPostFormat = selectPostFormat;
        }

        private void FormPostFormat_Load(object sender, EventArgs e)
        {
            foreach (PostFormat postFormat in PostFormatCollection.PostFormats.Values)
            {
                int i = lbPostFormats.Items.Add(postFormat.Name);
                if (!string.IsNullOrEmpty(initialPostFormat) && postFormat.Name == initialPostFormat)
                {
                    lbPostFormats.SelectedIndex = i;
                }
            }

        }
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) //激活回车键
        {
            int WM_KEYDOWN = 256;
            int WM_SYSKEYDOWN = 260;

            if (msg.Msg == WM_KEYDOWN | msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        this.Close();//csc关闭窗体
                        break;
                }

            }
            return false;
        }

        private void lbPostFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPostFormats.SelectedItems.Count == 0)
            {
                gbPostFormat.Enabled = false;
                btPostFormatDelete.Enabled = false;
                btPostFormatEdit.Enabled = false;
                postFormat = null;
            }
            else
            {
                //检查是否要保存更改
                CheckUpdatePostFormat(true);

                //
                postFormat =
                   (PostFormat)PostFormatCollection.PostFormats[lbPostFormats.SelectedItem];
                if (postFormat != null)
                {
                    dataLoading = true;

                    udPageWidth.Value = (int)postFormat.Values["Width"];
                    udPageHeight.Value = (int)postFormat.Values["Height"];
                    udPageXOffset.Value = (int)postFormat.Values["OffsetX"];
                    udPageYOffset.Value = (int)postFormat.Values["OffsetY"];

                    udSendNameX.Value = (int)postFormat.Values["SendNameX"];
                    udSendNameY.Value = (int)postFormat.Values["SendNameY"];
                    udSendPlaceX.Value = (int)postFormat.Values["SendPlaceX"];
                    udSendPlaceY.Value = (int)postFormat.Values["SendPlaceY"];
                    udSendCompanyX.Value = (int)postFormat.Values["SendCompanyX"];
                    udSendCompanyY.Value = (int)postFormat.Values["SendCompanyY"];
                    udSendProvinceX.Value = (int)postFormat.Values["SendProvinceX"];
                    udSendProvinceY.Value = (int)postFormat.Values["SendProvinceY"];
                    udSendProvinceLen.Value = (int)postFormat.Values["SendProvinceLen"];
                    udSendCityX.Value = (int)postFormat.Values["SendCityX"];
                    udSendCityY.Value = (int)postFormat.Values["SendCityY"];
                    udSendCityLen.Value = (int)postFormat.Values["SendCityLen"];
                    udSendZoneX.Value = (int)postFormat.Values["SendZoneX"];
                    udSendZoneY.Value = (int)postFormat.Values["SendZoneY"];
                    udSendZoneLen.Value = (int)postFormat.Values["SendZoneLen"];
                    udSendAddress1X.Value = (int)postFormat.Values["SendAddress1X"];
                    udSendAddress1Y.Value = (int)postFormat.Values["SendAddress1Y"];
                    udSendAddress1Len.Value = (int)postFormat.Values["SendAddress1Len"];
                    udSendAddress2X.Value = (int)postFormat.Values["SendAddress2X"];
                    udSendAddress2Y.Value = (int)postFormat.Values["SendAddress2Y"];
                    udSendAddress2Len.Value = (int)postFormat.Values["SendAddress2Len"];
                    udSendPhoneX.Value = (int)postFormat.Values["SendPhoneX"];
                    udSendPhoneY.Value = (int)postFormat.Values["SendPhoneY"];
                    udSendMobileX.Value = (int)postFormat.Values["SendMobileX"];
                    udSendMobileY.Value = (int)postFormat.Values["SendmobileY"];

                    udRecvNameX.Value = (int)postFormat.Values["RecvNameX"];
                    udRecvNameY.Value = (int)postFormat.Values["RecvNameY"];
                    udRecvPlaceX.Value = (int)postFormat.Values["RecvPlaceX"];
                    udRecvPlaceY.Value = (int)postFormat.Values["RecvPlaceY"];
                    udRecvCompanyX.Value = (int)postFormat.Values["RecvCompanyX"];
                    udRecvCompanyY.Value = (int)postFormat.Values["RecvCompanyY"];
                    udRecvProvinceX.Value = (int)postFormat.Values["RecvProvinceX"];
                    udRecvProvinceY.Value = (int)postFormat.Values["RecvProvinceY"];
                    udRecvProvinceLen.Value = (int)postFormat.Values["RecvProvinceLen"];
                    udRecvCityX.Value = (int)postFormat.Values["RecvCityX"];
                    udRecvCityY.Value = (int)postFormat.Values["RecvCityY"];
                    udRecvCityLen.Value = (int)postFormat.Values["RecvCityLen"];
                    udRecvZoneX.Value = (int)postFormat.Values["RecvZoneX"];
                    udRecvZoneY.Value = (int)postFormat.Values["RecvZoneY"];
                    udRecvZoneLen.Value = (int)postFormat.Values["RecvZoneLen"];
                    udRecvAddress1X.Value = (int)postFormat.Values["RecvAddress1X"];
                    udRecvAddress1Y.Value = (int)postFormat.Values["RecvAddress1Y"];
                    udRecvAddress1Len.Value = (int)postFormat.Values["RecvAddress1Len"];
                    udRecvAddress2X.Value = (int)postFormat.Values["RecvAddress2X"];
                    udRecvAddress2Y.Value = (int)postFormat.Values["RecvAddress2Y"];
                    udRecvAddress2Len.Value = (int)postFormat.Values["RecvAddress2Len"];
                    udRecvPhoneX.Value = (int)postFormat.Values["RecvPhoneX"];
                    udRecvPhoneY.Value = (int)postFormat.Values["RecvPhoneY"];
                    udRecvMobileX.Value = (int)postFormat.Values["RecvMobileX"];
                    udRecvMobileY.Value = (int)postFormat.Values["RecvmobileY"];

                    btPostFormatDelete.Enabled = !postFormat.IsDefault;

                    dataLoading = false;
                }
                gbPostFormat.Enabled = true;
            }

        }

        private void btPostFormatAdd_Click(object sender, EventArgs e)
        {
            CheckUpdatePostFormat(true);

            FormPostFormAdd formPostFormAdd = new FormPostFormAdd();
            if (formPostFormAdd.ShowDialog() == DialogResult.OK)
            {
                PostFormat postFormat = new PostFormat(formPostFormAdd.PostFormName);
                PostFormatCollection.AddPostFormat(postFormat);
                int i = lbPostFormats.Items.Add(postFormat.Name);
                lbPostFormats.SelectedIndex = i;
            }
        }

        private void btPostFormatDelete_Click(object sender, EventArgs e)
        {
            if (lbPostFormats.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(string.Format("确实要删除面单\"{0}\"吗？", (string)lbPostFormats.SelectedItem), "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                PostFormatCollection.DeletePostFormat((string)lbPostFormats.SelectedItem);
                lbPostFormats.Items.RemoveAt(lbPostFormats.SelectedIndex);
            }
        }

        private void udValueChanged(object sender, EventArgs e)
        {
            if (!dataLoading)
            {
                postFormatChanged = true;
                btPostFormatEdit.Enabled = true;
            }
        }

        private void CheckUpdatePostFormat(Boolean confirm)
        {
            if (postFormatChanged && postFormat != null)
            {
                postFormatChanged = false;

                if (confirm)
                {
                    DialogResult dialogResult = MessageBox.Show("面单设置已经改变，要保存更改吗", "确认", MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Question);
                    btPostFormatEdit.Enabled = false;

                    if (dialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        postFormatSizeChanged = false;
                        return;
                    }
                }
                //保存更改
                postFormat.Values["Width"]= (int)udPageWidth.Value;
                postFormat.Values["Height"]= (int)udPageHeight.Value;
                postFormat.Values["OffsetX"]= (int)udPageXOffset.Value;
                postFormat.Values["OffsetY"]= (int)udPageYOffset.Value;

                postFormat.Values["SendNameX"]= (int)udSendNameX.Value;
                postFormat.Values["SendNameY"]= (int)udSendNameY.Value;
                postFormat.Values["SendPlaceX"]= (int)udSendPlaceX.Value;
                postFormat.Values["SendPlaceY"]= (int)udSendPlaceY.Value;
                postFormat.Values["SendCompanyX"]= (int)udSendCompanyX.Value;
                postFormat.Values["SendCompanyY"]= (int)udSendCompanyY.Value;
                postFormat.Values["SendProvinceX"] = (int)udSendProvinceX.Value;
                postFormat.Values["SendProvinceY"] = (int)udSendProvinceY.Value;
                postFormat.Values["SendProvinceLen"] = (int)udSendProvinceLen.Value;
                postFormat.Values["SendCityX"] = (int)udSendCityX.Value;
                postFormat.Values["SendCityY"] = (int)udSendCityY.Value;
                postFormat.Values["SendCityLen"] = (int)udSendCityLen.Value;
                postFormat.Values["SendZoneX"] = (int)udSendZoneX.Value;
                postFormat.Values["SendZoneY"] = (int)udSendZoneY.Value;
                postFormat.Values["SendZoneLen"] = (int)udSendZoneLen.Value;
                postFormat.Values["SendAddress1X"]= (int)udSendAddress1X.Value;
                postFormat.Values["SendAddress1Y"]= (int)udSendAddress1Y.Value;
                postFormat.Values["SendAddress1Len"]= (int)udSendAddress1Len.Value;
                postFormat.Values["SendAddress2X"]= (int)udSendAddress2X.Value;
                postFormat.Values["SendAddress2Y"]= (int)udSendAddress2Y.Value;
                postFormat.Values["SendAddress2Len"] = (int)udSendAddress2Len.Value;
                postFormat.Values["SendPhoneX"]= (int)udSendPhoneX.Value;
                postFormat.Values["SendPhoneY"]= (int)udSendPhoneY.Value;
                postFormat.Values["SendMobileX"]= (int)udSendMobileX.Value;
                postFormat.Values["SendmobileY"]= (int)udSendMobileY.Value;

                postFormat.Values["RecvNameX"]= (int)udRecvNameX.Value;
                postFormat.Values["RecvNameY"]= (int)udRecvNameY.Value;
                postFormat.Values["RecvPlaceX"]= (int)udRecvPlaceX.Value;
                postFormat.Values["RecvPlaceY"]= (int)udRecvPlaceY.Value;
                postFormat.Values["RecvCompanyX"]= (int)udRecvCompanyX.Value;
                postFormat.Values["RecvCompanyY"]= (int)udRecvCompanyY.Value;
                postFormat.Values["RecvProvinceX"] = (int)udRecvProvinceX.Value;
                postFormat.Values["RecvProvinceY"] = (int)udRecvProvinceY.Value;
                postFormat.Values["RecvProvinceLen"] = (int)udRecvProvinceLen.Value;
                postFormat.Values["RecvCityX"] = (int)udRecvCityX.Value;
                postFormat.Values["RecvCityY"] = (int)udRecvCityY.Value;
                postFormat.Values["RecvCityLen"] = (int)udRecvCityLen.Value;
                postFormat.Values["RecvZoneX"] = (int)udRecvZoneX.Value;
                postFormat.Values["RecvZoneY"] = (int)udRecvZoneY.Value;
                postFormat.Values["RecvZoneLen"] = (int)udRecvZoneLen.Value;
                postFormat.Values["RecvAddress1X"]= (int)udRecvAddress1X.Value;
                postFormat.Values["RecvAddress1Y"]= (int)udRecvAddress1Y.Value;
                postFormat.Values["RecvAddress1Len"]= (int)udRecvAddress1Len.Value;
                postFormat.Values["RecvAddress2X"]= (int)udRecvAddress2X.Value;
                postFormat.Values["RecvAddress2Y"]= (int)udRecvAddress2Y.Value;
                postFormat.Values["RecvAddress2Len"] = (int)udRecvAddress2Len.Value;
                postFormat.Values["RecvPhoneX"]= (int)udRecvPhoneX.Value;
                postFormat.Values["RecvPhoneY"]= (int)udRecvPhoneY.Value;
                postFormat.Values["RecvMobileX"]= (int)udRecvMobileX.Value;
                postFormat.Values["RecvmobileY"]= (int)udRecvMobileY.Value;

                if(postFormatSizeChanged) postFormat.RestPaperSize();
                postFormat.WriteToIniFile();

                btPostFormatEdit.Enabled = false;
                postFormatSizeChanged = false;
            }

        }

        private void btPostFormatEdit_Click(object sender, EventArgs e)
        {
            CheckUpdatePostFormat(false);
        }

        private void FormPostFormat_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckUpdatePostFormat(true);
        }

        private void PaperSizeChanged(object sender, EventArgs e)
        {
            if (!dataLoading)
            {
                postFormatChanged = true;
                postFormatSizeChanged = true;
                btPostFormatEdit.Enabled = true;

            }
        }


    }
}
