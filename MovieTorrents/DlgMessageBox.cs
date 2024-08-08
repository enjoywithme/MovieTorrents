using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MovieTorrents
{
    public partial class DlgMessageBox : Form
    {
        private readonly string _message;
        private readonly string _caption;

        public DlgMessageBox(string message, string caption)
        {
            _message = message;
            _caption = caption;
            InitializeComponent();
        }

        private void DlgMessageBox_Load(object sender, EventArgs e)
        {
            Text = _caption;
            tbMessage.Text = _message;
        }

        private void DlgMessageBox_Layout(object sender, LayoutEventArgs e)
        {
            //button1.Top = tbMessage.Bottom + 20;
            //button1.Left = (ClientSize.Width - button1.Width) / 2;
        }
    }
}
