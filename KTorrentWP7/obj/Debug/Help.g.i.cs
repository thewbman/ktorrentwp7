﻿#pragma checksum "C:\Users\wes\Documents\Visual Studio 2010\Projects\KTorrentWP7\KTorrentWP7\Help.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5879DFDE3E575C2557870A614B2D925D"
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


namespace KTorrentWP7 {
    
    
    public partial class Help : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ListBox QuestionListBox;
        
        internal System.Windows.Controls.TextBlock emailTitle;
        
        internal System.Windows.Controls.TextBlock emailContent;
        
        internal System.Windows.Controls.TextBlock twitterTitle;
        
        internal System.Windows.Controls.TextBlock twitterContent;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/KTorrentWP7;component/Help.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.QuestionListBox = ((System.Windows.Controls.ListBox)(this.FindName("QuestionListBox")));
            this.emailTitle = ((System.Windows.Controls.TextBlock)(this.FindName("emailTitle")));
            this.emailContent = ((System.Windows.Controls.TextBlock)(this.FindName("emailContent")));
            this.twitterTitle = ((System.Windows.Controls.TextBlock)(this.FindName("twitterTitle")));
            this.twitterContent = ((System.Windows.Controls.TextBlock)(this.FindName("twitterContent")));
        }
    }
}
