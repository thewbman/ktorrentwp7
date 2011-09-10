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
    public class HostViewModel : INotifyPropertyChanged
    {
        private string _host;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                if (value != _host)
                {
                    _host = value;
                    NotifyPropertyChanged("Host");
                }
            }
        }

        private int _port;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (value != _port)
                {
                    _port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }

        private string _username;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (value != _username)
                {
                    _username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }

        private string _password;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        private string _cookie;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Cookie
        {
            get
            {
                return _cookie;
            }
            set
            {
                if (value != _cookie)
                {
                    _cookie = value;
                    NotifyPropertyChanged("Cookie");
                }
            }
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
    }
}