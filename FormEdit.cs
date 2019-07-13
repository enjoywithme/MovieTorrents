using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public partial class FormEdit : Form
    {
        private readonly TorrentFile _torrentFile;
        public FormEdit(TorrentFile torrentFile)
        {
            InitializeComponent();
            _torrentFile = torrentFile;
        }

        private void FormRenameTorrent_Load(object sender, EventArgs e)
        {
            tbOldName.Text = tbNewName.Text = _torrentFile.name;
            tbYear.Text = _torrentFile.year;
            tbKeyName.Text = _torrentFile.keyname;
            tbOtherName.Text = _torrentFile.otherName;
            tbGenres.Text = _torrentFile.genres;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            var newName = tbNewName.Text;
            if(string.IsNullOrEmpty(newName) ||
              newName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                MessageBox.Show("无效的文件名！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_torrentFile.EditRecord(FormMain.DbConnectionString, newName,tbYear.Text,tbKeyName.Text,tbOtherName.Text,tbGenres.Text, out var msg))
            {
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

       
    }
}
