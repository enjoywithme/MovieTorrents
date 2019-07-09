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
    public partial class FormRenameTorrent : Form
    {
        private TorrentFile _torrentFile;
        public FormRenameTorrent(TorrentFile torrentFile)
        {
            InitializeComponent();
            _torrentFile = torrentFile;
        }

        private void FormRenameTorrent_Load(object sender, EventArgs e)
        {
            tbOldName.Text = tbNewName.Text = _torrentFile.name;
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

            if (!_torrentFile.Rename(FormMain.DbConnectionString, newName, out var msg))
            {
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

       
    }
}
