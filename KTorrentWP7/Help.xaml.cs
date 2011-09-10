using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace KTorrentWP7
{
    public partial class Help : PhoneApplicationPage
    {
        public ObservableCollection<NameContentViewModel> Questions { get; private set; }
            
        public Help()
        {
            InitializeComponent();

            this.Questions = new ObservableCollection<NameContentViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Questions.Clear();
            
            this.Questions.Add(new NameContentViewModel() { Name = "What is KTorrent?", Content = "KTorrent is a Linux program for downloading and seeding torrent files." });
            this.Questions.Add(new NameContentViewModel() { Name = "What does this KTorrentWP7 app do?", Content = "KTorrentWP7 is an app for monitoring and controlling a KTorrent program laready running on a desktop.  The torrent files and data are not downloaded to this device." });
            this.Questions.Add(new NameContentViewModel() { Name = "How do I setup KTorrent to use this app?", Content = "You must turn enable the 'Web Interface' plugin inside the KTorrent program.  When you enable you will be able to set the port, username and password needed." });
            this.Questions.Add(new NameContentViewModel() { Name = "Can I start new torrent downloads from the app?", Content = "Not currently, but that feature is planned for a future version." });
            this.Questions.Add(new NameContentViewModel() { Name = "Why does the app say I have to fully the app it before reconncting to a host?", Content = "There is a limitation in how WindowsPhone7 ccommunicates with web services that won't allow the app to reconnect to the same host twice without closing the app." });
            this.Questions.Add(new NameContentViewModel() { Name = "What I have trouble getting this app to work?", Content = "Try emailing the developer.  The contact information is available to right." });

            QuestionListBox.ItemsSource = this.Questions;
        }

        private void email_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "ktorrentwp7.help@gmail.com";
            emailcomposer.Subject = "KTorrentWP7 Help";
            emailcomposer.Show();

        }

        private void twitter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webopen = new WebBrowserTask();

            webopen.Uri = new Uri("http://twitter.com/webmyth_dev");
            webopen.Show();
        }
        
    }
}