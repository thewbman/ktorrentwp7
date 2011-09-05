using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KTorrentWP7
{
    public class TorrentsViewModel
    {
        public int id { get; set; }
        
        public string name { get; set; }
        public string info_hash { get; set; }
        public string status { get; set; }
        public string bytes_downloaded { get; set; }
        public string bytes_uploaded { get; set; }

        public string total_bytes { get; set; }
        public string total_bytes_to_download { get; set; }
        public string download_rate { get; set; }
        public string upload_rate { get; set; }
        public string num_peers { get; set; }

        public string seeders { get; set; }
        public string seeders_total { get; set; }
        public string leechers { get; set; }
        public string leechers_total { get; set; }
        public string running { get; set; }

        public string percentage { get; set; }
        public string num_files { get; set; }

    }
}