using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PostPrin
{
    public partial class FormContacts : Form
    {
        public string ContactName = "";

        private string[] contacts;
        private Boolean bLoad = false;
        
        public FormContacts()
        {
            InitializeComponent();
        }

        public FormContacts(Boolean load)
        {
            InitializeComponent();
            bLoad = load;
        }

        private void FormContacts_Load(object sender, EventArgs e)
        {
            cbContacts.Items.Clear();
            contacts = Contact.ContactsFromIniFile();
            foreach (string con in contacts)
            {
                if(con.Trim()!="") cbContacts.Items.Add(con);
            }

            if(bLoad)
            {
                cbContacts.DropDownStyle = ComboBoxStyle.DropDownList;
                btDelete.Visible = cbContacts.Items.Count>0;
                if (cbContacts.Items.Count > 0) cbContacts.SelectedIndex = 0;
            }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            ContactName = cbContacts.Text.Trim();
            if (ContactName == "")
            {
                errorProvider.SetError(cbContacts, "你必须给定一个名称");
                cbContacts.Focus();
                return;
            }

            if (!bLoad)
            {
                

                for (int i = 0; i < contacts.Length; i++)
                {
                    if (contacts[i] == ContactName)
                    {
                        if (
                            MessageBox.Show(string.Format("已经保存了名为\"{0}\"的联系人，确定要覆盖它吗?", ContactName), "确认",
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        {
                            cbContacts.Focus();
                            cbContacts.SelectAll();
                            return;
                        }
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void cbContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            btDelete.Enabled = (cbContacts.SelectedIndex >= 0);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if(cbContacts.SelectedIndex>=0)
            {
                if(MessageBox.Show(string.Format("确实要删除联系人\"{0}\"吗",cbContacts.Text),"确认",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)==DialogResult.Cancel)
                    return;
                Contact.DeleteContactFromIniFile(cbContacts.Text);
                cbContacts.Items.RemoveAt(cbContacts.SelectedIndex);
                if (cbContacts.Items.Count > 0) cbContacts.SelectedIndex = 0;
                btDelete.Enabled = cbContacts.Items.Count > 0;
            }
        }



        private void FormContacts_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                this.DialogResult = DialogResult.Cancel;

            }
            else if(e.KeyChar == '\r')
            {
                btOk_Click(null,null);
            }
        }
    }
}
