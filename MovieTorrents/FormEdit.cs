﻿using System;
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
            tbOldName.Text = tbNewName.Text = _torrentFile.name;
            tbYear.Text = _torrentFile.year;
            tbZone.Text = _torrentFile.zone;
            tbDoubanId.Text = _torrentFile.doubanid;
            tbCasts.Text = _torrentFile.casts;
            tbDirectors.Text = _torrentFile.directors;
            tbRating.Text = _torrentFile.rating.ToString("F");
            tbposterpath.Text = _torrentFile.posterpath;
            tbKeyName.Text = _torrentFile.keyname;
            tbOtherName.Text = _torrentFile.otherName;
            tbGenres.Text = _torrentFile.genres;
            cbWatched.Checked = _torrentFile.seeflag == 1;
            dtPicker.Value = _torrentFile.SeeDate;
            tbComment.Text = _torrentFile.seecomment;

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
                newRating,tbposterpath.Text,tbCasts.Text,tbDirectors.Text,
                out var msg))
            {
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

       
    }
}
