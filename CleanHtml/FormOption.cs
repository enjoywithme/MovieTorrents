using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanHtml
{
    public partial class FormOption : Form
    {
        public enum ActionType
        {
            CleanImageLazyLoad,
            ExtractEmbeddedImage
        }

        public ActionType Action { get; private set; }
        public bool ConvertWebp { get; private set; }

        public FormOption()
        {
            InitializeComponent();
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                Action = ActionType.CleanImageLazyLoad;
            else if (radioButton2.Checked)
                Action = ActionType.ExtractEmbeddedImage;

            ConvertWebp = cbConvertWebp.Checked;

            DialogResult = DialogResult.OK;
        }
    }
}
