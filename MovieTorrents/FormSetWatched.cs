using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormSetWatched : Form
    {
        private TorrentFile _torrentFile;
        public FormSetWatched(TorrentFile torrentFile)
        {
            InitializeComponent();
            _torrentFile = torrentFile;
        }

        private void FormSetWatched_Load(object sender, EventArgs e)
        {
            var seeDate = DateTime.Today;
            if (!string.IsNullOrEmpty(_torrentFile.seedate) && DateTime.TryParse(_torrentFile.seedate, out var d))
                seeDate = d;
            dtPicker.Value = seeDate;
            tbComment.Text = _torrentFile.seecomment;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_torrentFile.SetWatched(FormMain.DbConnection, dtPicker.Value, tbComment.Text))
                return;

            DialogResult = DialogResult.OK;
        }
    }
}
