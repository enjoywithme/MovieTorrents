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
            dtPicker.Value = _torrentFile.SeeDateDate;
            tbComment.Text = _torrentFile.SeeComment;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_torrentFile.SetWatched(dtPicker.Value, tbComment.Text,out var msg))
            {
                MessageBox.Show(msg, Resource.TextError, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }

            _torrentFile.SeeLater = 0;
            _torrentFile.SeeFlag = 1;
            _torrentFile.SeeComment = tbComment.Text;

            DialogResult = DialogResult.OK;
        }
    }
}
