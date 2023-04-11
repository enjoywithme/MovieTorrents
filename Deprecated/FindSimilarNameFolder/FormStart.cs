using System;
using System.Windows.Forms;
using FindSimilarNameFolder.Properties;

namespace FindSimilarNameFolder
{
    public partial class FormStart : Form
    {
        public string StartPath { get { return textBox1.Text; } }
        public float MinimumSimilarity { get { return (float)numericUpDown1.Value; } }
        readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();

        public FormStart()
        {
            InitializeComponent();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show(Resources.MsgSelectFolder, Resources.TextWarning, MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
                textBox1.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void FormStartLoad(object sender, EventArgs e)
        {
#if DEBUG
            textBox1.Text =_folderBrowser.SelectedPath = "F:\\__TMEP__\\0day";
#endif
        }

        private void BtnSelFolderClick(object sender, EventArgs e)
        {
            if(_folderBrowser.ShowDialog()==DialogResult.Cancel) return;
            textBox1.Text = _folderBrowser.SelectedPath;
        }
    }
}
