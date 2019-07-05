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
        public DateTime WatchDate { get; set; }
        public string Comment { get; set; }
        public FormSetWatched(DateTime watchDate,string comment)
        {
            InitializeComponent();
            WatchDate = watchDate;
            Comment = comment;
        }

        private void FormSetWatched_Load(object sender, EventArgs e)
        {
            dtPicker.Value = WatchDate;
            tbComment.Text = Comment;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WatchDate = dtPicker.Value;
            Comment = tbComment.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
