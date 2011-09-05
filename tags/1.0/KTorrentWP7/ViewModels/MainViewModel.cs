using System;
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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;


namespace KTorrentWP7
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Hosts = new ObservableCollection<HostViewModel>();
            this.Settings = new ObservableCollection<NameContentViewModel>();
            this.Globals = new ObservableCollection<NameContentViewModel>();
            this.Torrents = new ObservableCollection<TorrentsViewModel>();

            this.SelectedTorrent = new TorrentsViewModel();
            this.hostIndex = 0;
            
            appSettings = new AppSettings();

            this.Connected = false;

            this.prefs = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// A collection of objects.
        /// </summary>
        public ObservableCollection<HostViewModel> Hosts { get; private set; }
        public ObservableCollection<NameContentViewModel> Settings { get; private set; }
        public ObservableCollection<NameContentViewModel> Globals { get; private set; }
        public ObservableCollection<TorrentsViewModel> Torrents { get; private set; }

        public TorrentsViewModel SelectedTorrent { get; set; }
        public int hostIndex { get; set; }
        public int torrentIndex { get; set; }
        
        public AppSettings appSettings;

        public bool IsDataLoaded { get; private set; }

        public bool Connected { get; set; }

        private IsolatedStorageSettings prefs;

        /// <summary>
        /// Creates and adds a few objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            //load hosts
            var savedHostsList = StorageLoad<List<HostViewModel>>("Hosts");

            if (savedHostsList.Count < 1)
            {
                //this.Hosts.Add(new HostViewModel() { Host = "192.168.1.105", Port = 8052, Username = "ktorrent", Password = "ktorrent" });
            }
            else
            {
                foreach (var e in savedHostsList) this.Hosts.Add(e);
                
            }

            //save hosts
            this.saveHosts();

            //load prefs
            var theme = appSettings.ThemeSetting;
            var analytics = appSettings.AnaltyicsSetting;
            appSettings.Save();

            this.IsDataLoaded = true;
        }

        public void clearSettings()
        {
            this.Settings.Clear();
        }

        public void addSetting(string settingName, string settingValue)
        {
            this.Settings.Add(new NameContentViewModel() { Name = settingName, Content = settingValue });
        }


        public void clearGlobals()
        {
            this.Globals.Clear();
        }

        public void addGlobal(string globalName, string globalValue)
        {
            this.Globals.Add(new NameContentViewModel() { Name = globalName, Content = globalValue });
        }


        public void clearTorrents()
        {
            this.Torrents.Clear();
        }

        public void addTorrent(int _id, string _name, string _info_hash, string _status, string _bytes_downloaded, string _bytes_uploaded, string _total_bytes, string _total_bytes_to_download, string _download_rate, string _upload_rate, string _num_peers, string _seeders, string _seeders_total, string _leechers, string _leechers_total, string _running, string _percentage, string _num_files)
        {
            this.Torrents.Add(new TorrentsViewModel() { id = _id, name = _name, info_hash = _info_hash, status = _status, bytes_downloaded = _bytes_downloaded, bytes_uploaded = _bytes_uploaded, total_bytes = _total_bytes, total_bytes_to_download = _total_bytes_to_download, download_rate = _download_rate, upload_rate = _upload_rate, num_peers = _num_peers, seeders = _seeders, seeders_total = _seeders_total, leechers = _leechers, leechers_total = _leechers_total, running = _running, percentage = _percentage, num_files = _num_files });

        }

        public void savePrefs()
        {
            appSettings.Save();
        }

        public void saveHosts()
        {
            List<HostViewModel> hostsList = new List<HostViewModel>(this.Hosts);
            StorageSave<List<HostViewModel>>("Hosts", hostsList);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
        public static T StorageLoad<T>(string name) where T : class, new()
        {
            T loadedObject = null;
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(name, System.IO.FileMode.OpenOrCreate, storageFile))
            {
                if (storageFileStream.Length > 0)
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    loadedObject = serializer.ReadObject(storageFileStream) as T;
                }
                if (loadedObject == null)
                {
                    loadedObject = new T();
                }
            }
 
            return loadedObject;
        }
        public static void StorageSave<T>(string name, T objectToSave)
        {
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(name, System.IO.FileMode.Create, storageFile))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(storageFileStream, objectToSave);
            }
        }
        public static void StorageDelete(string name)
        {
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                storageFile.Remove();
            }
        }




    }
}