﻿#pragma checksum "C:\Users\wes\documents\visual studio 2010\Projects\KTorrentWP7\KTorrentWP7\TorrentsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4E917A5C781F32A6DDFD0D5F1E719158"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace KTorrentWP7.ViewModels {
    
    
    public partial class TorrentsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot PivotTitle;
        
        internal Microsoft.Phone.Controls.LongListSelector TorrentsLL;
        
        internal System.Windows.Controls.ListBox GlobalsListBox;
        
        internal System.Windows.Controls.ListBox SettingsListBox;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/KTorrentWP7;component/TorrentsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.PivotTitle = ((Microsoft.Phone.Controls.Pivot)(this.FindName("PivotTitle")));
            this.TorrentsLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("TorrentsLL")));
            this.GlobalsListBox = ((System.Windows.Controls.ListBox)(this.FindName("GlobalsListBox")));
            this.SettingsListBox = ((System.Windows.Controls.ListBox)(this.FindName("SettingsListBox")));
        }
    }
}
