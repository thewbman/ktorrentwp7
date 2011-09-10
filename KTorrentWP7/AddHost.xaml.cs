using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace KTorrentWP7
{
    public partial class AddHost : PhoneApplicationPage
    {
        public AddHost()
        {
            InitializeComponent();
        }

        private void appbarSavehost_Click(object sender, EventArgs e)
        {
            App.ViewModel.Hosts.Add(new HostViewModel() { Host = host.Text, Port = int.Parse(port.Text), Username = username.Text, Password = password.Password });
            App.ViewModel.saveHosts();

            NavigationService.GoBack();
        }
        private void appbarCancelhost_Click(object sender, EventArgs e)
        {

            NavigationService.GoBack();
        }
    }
}