using System;
using System.IO;
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
            tbOldName.Text = tbNewName.Text = _torrentFile.Name;
            tbYear.Text = _torrentFile.Year;
            tbZone.Text = _torrentFile.Zone;
            tbDoubanId.Text = _torrentFile.DoubanId;
            tbCasts.Text = _torrentFile.Casts;
            tbDirectors.Text = _torrentFile.Directors;
            tbRating.Text = _torrentFile.Rating.ToString("F");
            tbposterpath.Text = _torrentFile.PosterPath;
            tbKeyName.Text = _torrentFile.KeyName;
            tbOtherName.Text = _torrentFile.OtherName;
            tbGenres.Text = _torrentFile.Genres;
            cbWatched.Checked = _torrentFile.SeeFlag == 1;
            dtPicker.Value = _torrentFile.SeeDateDate;
            tbComment.Text = _torrentFile.SeeComment;

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

            if (!double.TryParse(tbRating.Text, out var newRating))
            {
                MessageBox.Show("无效的评分", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_torrentFile.EditRecord(newName,tbYear.Text,tbZone.Text,
                tbKeyName.Text,tbOtherName.Text,tbGenres.Text,
                cbWatched.Checked,dtPicker.Value,tbComment.Text,
                tbDoubanId.Text,newRating,tbposterpath.Text,tbCasts.Text,tbDirectors.Text,
                out var msg))
            {
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

       
    }
}
