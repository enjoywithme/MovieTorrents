using System;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormSetWatched : Form
    {
        private readonly TorrentFile _torrentFile;
        public FormSetWatched(TorrentFile torrentFile)
        {
            InitializeComponent();
            _torrentFile = torrentFile;
        }

        private void FormSetWatched_Load(object sender, EventArgs e)
        {
            dtPicker.Value = _torrentFile.SeeDate;
            tbComment.Text = _torrentFile.seecomment;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_torrentFile.SetWatched(FormMain.DbConnectionString, dtPicker.Value, tbComment.Text,out var msg))
            {
                MessageBox.Show(msg, Properties.Resources.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
