using System;
using System.Net;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Runtime.Serialization;
using Microsoft.Phone;
using Microsoft.Phone.Controls;

namespace KTorrentWP7
{
    public partial class TorrentDetailsPage : PhoneApplicationPage
    {
        public ObservableCollection<NameContentViewModel> TorrentDetails { get; private set; }
        public ObservableCollection<TorrentFile> TorrentFiles { get; private set; }

        public CookieContainer CookieJar = new CookieContainer();

        String filesString = "http://{0}:{1}/data/torrent/files.xml?torrent={2}&random={3}";
        String actionString = "http://{0}:{1}/action?{2}={3}";
       

        public TorrentDetailsPage()
        {
            InitializeComponent();

            this.TorrentDetails = new ObservableCollection<NameContentViewModel>();
            this.TorrentFiles = new ObservableCollection<TorrentFile>();

            DetailsListBox.DataContext = TorrentDetails;
            DetailsListBox.ItemsSource = TorrentDetails;

            //appbarStarttorrent.IsEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedId", out selectedIndex))
            {
                TorrentDetails.Clear();
                App.ViewModel.torrentIndex = int.Parse(selectedIndex);

                               
                TorrentDetails.Add(new NameContentViewModel() { Name = "name", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].name });
                TorrentDetails.Add(new NameContentViewModel() { Name = "status", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].status });
                TorrentDetails.Add(new NameContentViewModel() { Name = "total_bytes", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].total_bytes });
                TorrentDetails.Add(new NameContentViewModel() { Name = "bytes_downloaded", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].bytes_downloaded });
                TorrentDetails.Add(new NameContentViewModel() { Name = "bytes_uploaded", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].bytes_uploaded });

                TorrentDetails.Add(new NameContentViewModel() { Name = "percentage", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].percentage+"%" });
                TorrentDetails.Add(new NameContentViewModel() { Name = "download_rate", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].download_rate });
                TorrentDetails.Add(new NameContentViewModel() { Name = "upload_rate", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].upload_rate });
                
                TorrentDetails.Add(new NameContentViewModel() { Name = "seeders", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].seeders });
                TorrentDetails.Add(new NameContentViewModel() { Name = "leechers", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].leechers });
                TorrentDetails.Add(new NameContentViewModel() { Name = "running", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].running });
                
                TorrentDetails.Add(new NameContentViewModel() { Name = "num_files", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].num_files });

                TorrentDetails.Add(new NameContentViewModel() { Name = "total_bytes_to_download", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].total_bytes_to_download });
                TorrentDetails.Add(new NameContentViewModel() { Name = "num_peers", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].num_peers });
                TorrentDetails.Add(new NameContentViewModel() { Name = "seeders_total", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].seeders_total });
                TorrentDetails.Add(new NameContentViewModel() { Name = "leechers_total", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].leechers_total });
                TorrentDetails.Add(new NameContentViewModel() { Name = "id", Content = "" + App.ViewModel.Torrents[App.ViewModel.torrentIndex].id });
                TorrentDetails.Add(new NameContentViewModel() { Name = "info_hash", Content = App.ViewModel.Torrents[App.ViewModel.torrentIndex].info_hash });

                DetailsListBox.ItemsSource = TorrentDetails;
                
                if (int.Parse(App.ViewModel.Torrents[App.ViewModel.torrentIndex].num_files) > 0)
                {
                    GetFilesService(App.ViewModel.Torrents[App.ViewModel.torrentIndex].id);
                }
                else
                {
                    TorrentFiles.Add(new TorrentFile()
                    {
                        id = 0,

                        path = App.ViewModel.Torrents[App.ViewModel.torrentIndex].name,
                        priority = "50",
                        percentage = App.ViewModel.Torrents[App.ViewModel.torrentIndex].percentage + "%",
                        size = App.ViewModel.Torrents[App.ViewModel.torrentIndex].total_bytes
                    });

                    FilesListBox.ItemsSource = TorrentFiles;
                }

            }
        }
        private void GetFilesService(int inTorrent)
        {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(filesString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, inTorrent, randText())));

            webRequest.CookieContainer = CookieJar;
            webRequest.Headers["Cookie"] = App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie;

            webRequest.BeginGetResponse(new AsyncCallback(FilesCallback), webRequest);
        }
        private void FilesCallback(IAsyncResult asynchronousResult)
        {
            
            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }
            
            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

            try
            {

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                TorrentFiles.Clear();

                int fileId = 0;

                foreach (var item in xdoc.Descendants("file"))
                {
                    TorrentFiles.Add(new TorrentFile()
                    {
                        id = fileId,

                        path = item.Element("path").Value,
                        priority = item.Element("priority").Value,
                        percentage = item.Element("percentage").Value + "%",
                        size = item.Element("size").Value
                    });

                    
                    fileId++;
                }


                Deployment.Current.Dispatcher.BeginInvoke(() => { FilesListBox.ItemsSource = TorrentFiles; });

            }
            catch (System.Xml.XmlException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { 
                    //textBlock1.Text = ex.Message; 
                });
            }
        }



        public class TorrentFile
        {
            public int id { get; set; }

            public string path { get; set; }
            public string priority { get; set; }
            public string percentage { get; set; }
            public string size { get; set; }

        }

        private static string randText()
        {
            Random random = new Random();

            return random.Next().ToString();
        }

        private void appbarStoptorrent_Click(object sender, EventArgs e)
        {
            CallActionService("stop", App.ViewModel.Torrents[App.ViewModel.torrentIndex].id);
        }
        private void appbarStarttorrent_Click(object sender, EventArgs e)
        {
            CallActionService("start", App.ViewModel.Torrents[App.ViewModel.torrentIndex].id);
        }
        private void appbarRemovetorrent_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this torrent?  This action cannot be undone.", "Remove torrent", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CallActionService("remove", App.ViewModel.Torrents[App.ViewModel.torrentIndex].id);
            }
        }

        private void CallActionService(string inAction, int inTorrent)
        {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(actionString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, inAction, inTorrent)));

           // Deployment.Current.Dispatcher.BeginInvoke(() => { textBlock0.Text = String.Format(actionString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, inAction, inTorrent); });

            webRequest.CookieContainer = CookieJar;
            webRequest.Headers["Cookie"] = App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie;

            webRequest.BeginGetResponse(new AsyncCallback(ActionCallback), webRequest);
        }
        private void ActionCallback(IAsyncResult asynchronousResult)
        {

            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            response.GetResponseStream().Close();
            response.Close();
            request.Abort();
            
            //Deployment.Current.Dispatcher.BeginInvoke(() => { textBlock1.Text = resultString; });

            Deployment.Current.Dispatcher.BeginInvoke(() => { NavigationService.GoBack(); });

        }
    }
}