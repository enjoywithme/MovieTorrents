using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PostPrin
{
    public partial class FormPostFormAdd : Form
    {
        private string postFormName = "";
        public string PostFormName { get { return postFormName; } }
        public FormPostFormAdd()
        {
            InitializeComponent();
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            postFormName = tbPostFormatName.Text.Trim();
            if(postFormName=="")
            {
                errorProvider.SetError(tbPostFormatName,"面单名称不能为空");
                tbPostFormatName.Focus();
                return;
            }
            if(PostFormatCollection.PostFormats.ContainsKey(postFormName))
            {
                errorProvider.SetError(tbPostFormatName,"面单已经存在，请输入另外的名称");
                tbPostFormatName.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void tbPostFormatName_TextChanged(object sender, EventArgs e)
        {
            errorProvider.Clear();
        }
    }
}
