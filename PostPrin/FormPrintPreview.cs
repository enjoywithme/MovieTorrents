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
    public partial class FormPrintPreview : Form
    {
        //private PrintDocument printDocument = null;
        
        public FormPrintPreview()
        {
            InitializeComponent();
        }

        public FormPrintPreview(PrintDocument pDocument)
        {
            InitializeComponent();
            if(pDocument!=null) printPreviewControl.Document = pDocument;
        }

        private void tsbPrint_Click(object sender, EventArgs e)
        {
            printPreviewControl.Document.Print();
        }

        private void setZoom()
        {

            string zoomText = this.tscmbZoom.Text.Trim();
            double zoom = 1;
            if (zoomText == "适合窗口大小")
            {
                printPreviewControl.AutoZoom = true;
            }
            else if ((zoomText != "") && (zoomText != "."))
            {
                printPreviewControl.AutoZoom = false;
                if (zoomText.EndsWith("%"))
                {
                    zoomText = zoomText.Substring(0, zoomText.Length - 1);
                }
                zoom = Convert.ToDouble(zoomText) / 100;
                printPreviewControl.Zoom = zoom;
            }

        }

        private void tscmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            setZoom();
        }

        private void tscmbZoom_Leave(object sender, EventArgs e)
        {
            string zoomText = this.tscmbZoom.Text;
            if ((zoomText.ToLower() != "适合窗口大小") && (!zoomText.EndsWith("%")))
            {
                this.tscmbZoom.Text += "%";
            }
        }

        private void tscmbZoom_TextUpdate(object sender, EventArgs e)
        {
            setZoom();
        }

        private void number_KeyPress(object sender, KeyPressEventArgs e)
        {
            string key = new string(e.KeyChar, 1);
            bool validKey = (e.KeyChar >= 48 && e.KeyChar <= 57)
                || (e.KeyChar == 8);
            if (!validKey)
            {
                e.Handled = true;
            }
        }

        private void tsbExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
    }
}
