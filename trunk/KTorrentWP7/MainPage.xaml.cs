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
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //appbarAddhost.IconUri = new Uri("/Images/appbar.add.rest.png", UriKind.Relative);
            //appbarHelp.IconUri = new Uri("/Images/appbar.questionmark.rest.png", UriKind.Relative);

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Handle selection changed on ListBox
        private void HostsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (HostsListBox.SelectedIndex == -1)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/TorrentsPage.xaml?selectedItem=" + HostsListBox.SelectedIndex, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            HostsListBox.SelectedIndex = -1;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData(); 
                
                if (App.ViewModel.Hosts.Count < 1) MessageBox.Show("Welcome to KTorrentWP7.  This app is for controlling a KTorrent program running a desktop computer.  The torrents are only downloaded on the computer running KTorrent, not to this device.  If you do not already have KTorrent running on a computer this app is not for you.", "KTorrentWP7", MessageBoxButton.OK);

            }

            
            //HostsListBox.DataContext = App.ViewModel.Hosts;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            //
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void appbarAddhost_Click(object sender, EventArgs e)
        {


            NavigationService.Navigate(new Uri("/Addhost.xaml", UriKind.Relative));
        }

        private void appbarHelp_Click(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }
    }
}