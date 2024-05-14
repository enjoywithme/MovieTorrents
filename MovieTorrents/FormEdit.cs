using System;
using System.IO;
using System.Windows.Forms;
using MovieTorrents.Common;

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
            tbDoubanTitle.Text = _torrentFile.DoubanTitle;
            tbDoubanSubTitle.Text = _torrentFile.DoubanSubTitle;
            tbCasts.Text = _torrentFile.Casts;
            tbDirectors.Text = _torrentFile.Directors;
            tbRating.Text = _torrentFile.Rating.ToString("F");
            tbposterpath.Text = _torrentFile.PosterPath;
            //tbKeyName.Text = _torrentFile.KeyName;
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

            var editTorrentFile = new TorrentFile();
            editTorrentFile.Name = newName;
            editTorrentFile.Year = tbYear.Text;
            editTorrentFile.Zone = tbZone.Text;
            editTorrentFile.OtherName = tbOtherName.Text;
            editTorrentFile.Genres = tbGenres.Text;
            editTorrentFile.SeeFlag = cbWatched.Checked ? 1 : 0;
            editTorrentFile.SeeDate = cbWatched.Checked ? "" : dtPicker.Value.ToString("yyyy-MM-dd");
            editTorrentFile.SeeComment = tbComment.Text;
            editTorrentFile.DoubanId = tbDoubanId.Text;
            editTorrentFile.DoubanTitle = tbDoubanTitle.Text;
            editTorrentFile.DoubanSubTitle = tbDoubanSubTitle.Text;
            editTorrentFile.Rating = newRating;
            editTorrentFile.PosterPath = tbposterpath.Text;
            editTorrentFile.Directors = tbDirectors.Text;
            editTorrentFile.Casts = tbCasts.Text;

            if (!_torrentFile.EditRecord(editTorrentFile, out var msg))
            {
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

       
    }
}
