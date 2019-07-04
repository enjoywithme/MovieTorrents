using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTorrents
{
    public class TorrentFile
    {
        public long fid { get; set;}
        public string name { get; set; }
        public double rating { get; set; }
        public string year { get; set; }
        public long seeflag { get; set; }
        public string seedate { get; set; }
    }
}
