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

            bool haveError = false;
            string errorMessage = "";

            try
            {
                if (host.Text.Length == 0)
                {
                    haveError = true;
                    errorMessage = "You must enter a hostname or IP address";
                }
                else if (int.Parse(port.Text).ToString() != port.Text)
                {
                    haveError = true;
                    errorMessage = "You must enter a valid port number";
                }
                else if (username.Text.Length == 0)
                {
                    haveError = true;
                    errorMessage = "You must enter a valid username";
                }
                else if (password.Password.Length == 0)
                {
                    haveError = true;
                    errorMessage = "You must enter a valid password";
                }

            } catch(Exception ex) {
                haveError = true;
                errorMessage = "You must enter a valid port number";
            }

            if (haveError)
            {
                MessageBox.Show(errorMessage);
            }
            else
            {
                confirmSavehost();
            }

        }
        private void appbarCancelhost_Click(object sender, EventArgs e)
        {

            NavigationService.GoBack();
        }

        private void confirmSavehost()
        {
            App.ViewModel.Hosts.Add(new HostViewModel() { Host = host.Text, Port = int.Parse(port.Text), Username = username.Text, Password = password.Password });
            App.ViewModel.saveHosts();

            NavigationService.GoBack();
        }
    }
}