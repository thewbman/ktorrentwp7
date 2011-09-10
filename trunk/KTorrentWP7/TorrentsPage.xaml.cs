using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace KTorrentWP7.ViewModels
{
    public partial class TorrentsPage : PhoneApplicationPage
    {
        public CookieContainer CookieJar = new CookieContainer();

        private int settingsCalls = 0;

        //private int hostIndex;
        private string _cookie = "";

        private string challengeHash = "";

        String loginRequestString = "http://{0}:{1}{2}login.html";
        String loginChallengeString = "http://{0}:{1}{2}login/challenge.xml";
        //String actualLoginString = "http://{0}:{1}/login?page=interface.html&Login=Sign+in&username={2}&password=&challenge={3}";
        String actualLoginString = "http://{0}:{1}{2}login?page=interface.html&random={3}";
        String actualLoginContent = "username={0}&password=&Login=Sign+in&challenge={1}";
        String refererLoginString = "http://{0}:{1}/";

        String settingsString = "http://{0}:{1}/data/settings.xml?random={2}";
        String globalsString = "http://{0}:{1}/data/global.xml?random={2}";
        String torrentsString = "http://{0}:{1}/data/torrents.xml?random={2}";


        public TorrentsPage()
        {
            InitializeComponent();

            DataContext = App.ViewModel;

            App.ViewModel.addSetting("fakeSetting", "asdf");
            App.ViewModel.addGlobal("fakeGlobal", "asdf");
            //App.ViewModel.addTorrent("fakeTorrent", "asdf", "asdf");

            SettingsListBox.ItemsSource = App.ViewModel.Settings;
            GlobalsListBox.ItemsSource = App.ViewModel.Globals;
            //TorrentsListBox.ItemsSource = App.ViewModel.Torrents;
            //TorrentsLL.ItemsSource = App.ViewModel.Torrents;

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                App.ViewModel.hostIndex = int.Parse(selectedIndex);
                
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host;

                //textBlock1.Text = "Starting to login ...";

                try
                {
                    if(App.ViewModel.Connected == false) CallLoginService();
                }
                catch
                {
                    //textBlock1.Text = "CallLoginService error ...";
                }
            }
        }


        private void CallLoginService()
        {

            PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host+" ...";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(loginRequestString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, App.ViewModel.Slashes)));

            webRequest.CookieContainer = CookieJar;
            webRequest.AllowAutoRedirect = false;
            webRequest.Accept = "text/html";
            webRequest.BeginGetResponse(new AsyncCallback(LoginCallback), webRequest);
        }
        private void LoginCallback(IAsyncResult asynchronousResult)
        {
            string resultString;
            
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            } 
            catch(Exception ex) 
            {
                 Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error connecting to the host.  Check that you have internet connectivity and that the hostname and port are correct.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });

                return;
            }

            GetRedirectURL(request.RequestUri, response);

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock1.Text = resultString;
                //textBlock3.Text = GetRedirectURL(request.RequestUri, response);
            });


            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

            CallChallengeService();
        }

        private void CallChallengeService()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(String.Format(loginChallengeString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, App.ViewModel.Slashes)));

            request.CookieContainer = CookieJar;
            request.AllowAutoRedirect = false;
            //request.Headers["Cookie"] = App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie;

            request.BeginGetResponse(new AsyncCallback(ChallengeCallback), request);
        }
        private void ChallengeCallback(IAsyncResult asynchronousResult)
        {
            string resultString, challenge;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error getting challenge information from host.  Check that you have internet connectivity and that the hostname and port are correct.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock1.Text = resultString;
                //textBlock3.Text = GetRedirectURL(request.RequestUri, response);
            });

            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

            try
            {
                
                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);


                challenge = xdoc.Element("challenge").Value;


                SHA1 sha1 = new SHA1Managed();

                string passwordAndChallenge = challenge + App.ViewModel.Hosts[App.ViewModel.hostIndex].Password;

                UTF8Encoding encoder = new UTF8Encoding();

                byte[] buffer = encoder.GetBytes(passwordAndChallenge);
                //string challengeHash = BitConverter.ToString(sha1.ComputeHash(buffer)).Replace("-", "");
                challengeHash = BitConverter.ToString(sha1.ComputeHash(buffer)).Replace("-", "").ToLower();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //textBlock1.Text = challengeHash;
                    //textBlock2.Text = passwordAndChallenge;
                    //textBlock3.Text = "challengeHashByte " + challengeHashByte.ToString();
                });

                CallActualLoginService();
            }
            catch (System.Xml.XmlException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { 
                    //textBlock1.Text = ex.Message; 
                });
            }

        }

        private void CallActualLoginService()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(actualLoginString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, App.ViewModel.Slashes, randText())));
            
            webRequest.CookieContainer = CookieJar;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            webRequest.Headers["Referer"] = String.Format(refererLoginString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port);
            webRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            webRequest.Headers["Accept-Encoding"] = "gzip, deflate";
            webRequest.AllowAutoRedirect = false;

            webRequest.BeginGetRequestStream(ActualLoginStreamCallback, webRequest);
            
        }
        private void ActualLoginStreamCallback(IAsyncResult asyncResult) 
        {
            HttpWebRequest webRequest = (HttpWebRequest)asyncResult.AsyncState;

            Stream postStream = webRequest.EndGetRequestStream(asyncResult);

            StreamWriter myWriter = new StreamWriter(postStream);
            myWriter.Write(String.Format(actualLoginContent, App.ViewModel.Hosts[App.ViewModel.hostIndex].Username, challengeHash));
            myWriter.Close();

            webRequest.BeginGetResponse(new AsyncCallback(ActualLoginCallback), webRequest);

        }
        private void ActualLoginCallback(IAsyncResult asynchronousResult)
        {
            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                App.ViewModel.Connected = true;
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error logging into the host.  Check that you have internet connectivity and that the hostname, port, username and password are correct.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            if (GetRedirectURL(request.RequestUri, response) == "Moved Permanently")
            {
                //we got a successful login

                App.ViewModel.Connected = true;

                CallSettingsService();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error logging into the host.  Check that you have internet connectivity and that the hostname, port, username and password are correct.  You may need to fully close this app and reopen it if you have already connected to this host.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });
            }


            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock1.Text = resultString;
                //textBlock2.Text = "ActualLoginCallback";
                //textBlock3.Text = GetRedirectURL(request.RequestUri, response);

                //textBlock2.Text = cookie_container.GetCookies(new Uri(String.Format(cookieUriString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port))).ToString();
            });

            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

        }


        private void CallSettingsService()
        {
            settingsCalls++;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + "    ...";
            });

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(settingsString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, randText())));

            webRequest.CookieContainer = CookieJar;
            webRequest.Headers["Cookie"] = App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie;

            if(App.ViewModel.Connected) webRequest.BeginGetResponse(new AsyncCallback(SettingsCallback), webRequest);

        }
        private void SettingsCallback(IAsyncResult asynchronousResult)
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + "       ...";
            }); 
            
            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Unable to get data.  You may have been disconnected from your KTorent system.  You will have to fully close this app before you can reconnect to this system.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock1.Text = resultString;
                //textBlock3.Text = GetRedirectURL(request.RequestUri, response);
            });

            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModel.clearSettings();
                });

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);


                XElement maxDownloads = xdoc.Element("settings").Element("maxDownloads");
                if(maxDownloads != null) Deployment.Current.Dispatcher.BeginInvoke(() => {App.ViewModel.addSetting("Max downloads", maxDownloads.Value); });

                XElement maxSeeds = xdoc.Element("settings").Element("maxSeeds");
                if (maxSeeds != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Max seeds", maxSeeds.Value); });

                XElement maxConnections = xdoc.Element("settings").Element("maxConnections");
                if (maxConnections != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Max connections", maxConnections.Value); });

                XElement maxTotalConnections = xdoc.Element("settings").Element("maxTotalConnections");
                if (maxTotalConnections != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Max total connections", maxTotalConnections.Value); });

                XElement maxUploadRate = xdoc.Element("settings").Element("maxUploadRate");
                if (maxUploadRate != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Max upload rate", maxUploadRate.Value); });

                XElement maxDownloadRate = xdoc.Element("settings").Element("maxDownloadRate");
                if (maxDownloadRate != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Max download rate", maxDownloadRate.Value); });

                XElement useEncryption = xdoc.Element("settings").Element("useEncryption");
                if (useEncryption != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Use encryption", useEncryption.Value); });

                XElement allowUnecryptedConnections = xdoc.Element("settings").Element("allowUnecryptedConnections");
                if (allowUnecryptedConnections != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addSetting("Allow unencrypted connections", allowUnecryptedConnections.Value); });

                CallGlobalsService();
                
            }
            catch (System.Xml.XmlException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { 
                    //textBlock1.Text = ex.Message; 
                });
            }

        }

        private void CallGlobalsService()
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + "          ...";
            }); 
            
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(globalsString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, randText())));

            webRequest.CookieContainer = CookieJar;
            webRequest.Headers["Cookie"] = App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie;

            if (App.ViewModel.Connected) webRequest.BeginGetResponse(new AsyncCallback(GlobalsCallback), webRequest);
        }
        private void GlobalsCallback(IAsyncResult asynchronousResult)
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + "             ...";
            }); 

            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Unable to get data.  You may have been disconnected from your KTorent system.  You will have to fully close this app before you can reconnect to this system.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock1.Text = resultString;
                //textBlock3.Text = GetRedirectURL(request.RequestUri, response);
            });


            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModel.clearGlobals();
                });

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                XElement transferred_down = xdoc.Element("global_data").Element("transferred_down");
                if (transferred_down != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addGlobal("Transferred down", transferred_down.Value); });

                XElement transferred_up = xdoc.Element("global_data").Element("transferred_up");
                if (transferred_up != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addGlobal("Transferred up", transferred_up.Value); });

                XElement speed_down = xdoc.Element("global_data").Element("speed_down");
                if (speed_down != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addGlobal("Speed down", speed_down.Value); });

                XElement speed_up = xdoc.Element("global_data").Element("speed_up");
                if (speed_up != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addGlobal("Speed up", speed_up.Value); });

                XElement dht = xdoc.Element("global_data").Element("dht");
                if (dht != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addGlobal("DHT", dht.Value); });

                XElement encryption = xdoc.Element("global_data").Element("encryption");
                if (encryption != null) Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addGlobal("Encryption", encryption.Value); });

                CallTorrentsService();

            }
            catch (System.Xml.XmlException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { 
                    //textBlock1.Text = ex.Message; 
                });
            }

        }

        private void CallTorrentsService()
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + "                ...";
            });

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(String.Format(torrentsString, App.ViewModel.Hosts[App.ViewModel.hostIndex].Host, App.ViewModel.Hosts[App.ViewModel.hostIndex].Port, randText())));

            webRequest.CookieContainer = CookieJar;
            webRequest.Headers["Cookie"] = App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie;

            if (App.ViewModel.Connected) webRequest.BeginGetResponse(new AsyncCallback(TorrentsCallback), webRequest);
        }
        private void TorrentsCallback(IAsyncResult asynchronousResult)
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + "                   ...";
            }); 
            
            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Unable to get data.  You may have been disconnected from your KTorent system.  You will have to fully close this app before you can reconnect to this system.", "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = false;
                    NavigationService.GoBack();
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock1.Text = resultString;
                //textBlock3.Text = GetRedirectURL(request.RequestUri, response);
            });

            response.GetResponseStream().Close();
            response.Close();
            request.Abort();

            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModel.clearTorrents();
                });

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                List<Torrent> source = new List<Torrent>();

                int torrentId = 0;

                foreach (var item in xdoc.Descendants("torrent"))
                {
                    Torrent p = new Torrent()
                    {
                        id = torrentId,
                        
                        name = item.Element("name").Value,
                        info_hash = item.Element("info_hash").Value,
                        status = item.Element("status").Value,
                        bytes_downloaded = item.Element("bytes_downloaded").Value,
                        bytes_uploaded = item.Element("bytes_uploaded").Value,

                        total_bytes = item.Element("total_bytes").Value,
                        total_bytes_to_download = item.Element("total_bytes_to_download").Value,
                        download_rate = item.Element("download_rate").Value,
                        upload_rate = item.Element("upload_rate").Value,
                        num_peers = item.Element("num_peers").Value,

                        seeders = item.Element("seeders").Value,
                        seeders_total = item.Element("seeders_total").Value,
                        leechers = item.Element("leechers").Value,
                        leechers_total = item.Element("leechers_total").Value,
                        running = item.Element("running").Value,

                        percentage = item.Element("percentage").Value,
                        num_files = item.Element("num_files").Value
                    };

                    Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.addTorrent(p.id, p.name, p.info_hash, p.status, p.bytes_downloaded, p.bytes_uploaded, p.total_bytes, p.total_bytes_to_download, p.download_rate, p.upload_rate, p.num_peers, p.seeders, p.seeders_total, p.leechers, p.leechers_total, p.running, p.percentage, p.num_files); });
                    source.Add(new Torrent() { id = p.id, name = p.name, info_hash = p.info_hash, status = p.status, bytes_downloaded = p.bytes_downloaded, bytes_uploaded = p.bytes_uploaded, total_bytes = p.total_bytes, total_bytes_to_download = p.total_bytes_to_download, download_rate = p.download_rate, upload_rate = p.upload_rate, num_peers = p.num_peers, seeders = p.seeders, seeders_total = p.seeders_total, leechers = p.leechers, leechers_total = p.leechers_total, running = p.running, percentage = p.percentage, num_files = p.num_files });

                    torrentId++;
                }

                var torrentByStatus = from t in source
                                    group t by t.status into c
                                    orderby c.Key
                                    select new Group<Torrent>(c.Key, c);

                Deployment.Current.Dispatcher.BeginInvoke(() => { this.TorrentsLL.ItemsSource = torrentByStatus; });
                
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { PivotTitle.Title = "KTorrentWP7: " + App.ViewModel.Hosts[App.ViewModel.hostIndex].Host + ""; });

                this.Perform(() => CallSettingsService(), 5000);

            }
            catch (System.Xml.XmlException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { 
                    //textBlock1.Text = ex.Message; 
                });
            }

        }



        public virtual string GetRedirectURL(Uri inUri, HttpWebResponse webresponse)
        {
            string uri = "no uri location";

            WebHeaderCollection headers = webresponse.Headers;

            if ((webresponse.StatusCode == HttpStatusCode.Found) ||
              (webresponse.StatusCode == HttpStatusCode.Redirect) ||
              (webresponse.StatusCode == HttpStatusCode.Moved) ||
              (webresponse.StatusCode == HttpStatusCode.MovedPermanently))
            {
                // Get redirected uri

                //uri = headers["Location"];
                //uri = uri.Trim();
            }
            
                //uri += webresponse.StatusCode;
                uri = webresponse.StatusDescription;
            

            //Check for any cookies
            
                if (headers["Set-Cookie"] != null)
                {
                    String tmpCookie = headers["Set-Cookie"].Split(';')[0];

                    if (tmpCookie == "KT_SESSID=666")
                    {
                        //
                    }
                    else
                    {
                        _cookie = tmpCookie;

                        App.ViewModel.Hosts[App.ViewModel.hostIndex].Cookie = tmpCookie;

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            //textBlock4.Text = "_cookie: " + _cookie;
                        });

                        CookieJar.SetCookies(inUri, _cookie);
                    }
                }
                else {

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        //textBlock4.Text += "no cookie";
                    });

                }
             
            //                string StartURI = "http:/";

            //                if (uri.Length > 0 && uri.StartsWith(StartURI)==false)

            //                {

            //                      uri = StartURI + uri;

            //                }

            return uri;
        }

        public class Torrent
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

        public class Group<T> : IEnumerable<T>
        {
            public Group(string name, IEnumerable<T> items)
            {
                this.Title = name;
                this.Items = new List<T>(items);
            }

            public override bool Equals(object obj)
            {
                Group<T> that = obj as Group<T>;

                return (that != null) && (this.Title.Equals(that.Title));
            }

            public string Title
            {
                get;
                set;
            }

            public IList<T> Items
            {
                get;
                set;
            }

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion
        }

        private void Perform(Action myMethod, int delayInMilliseconds)
            {
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += (s, e) => Thread.Sleep(delayInMilliseconds);

                worker.RunWorkerCompleted += (s, e) => myMethod.Invoke();

                worker.RunWorkerAsync();
            }

        private static string randText()
        {
            Random random = new Random();

            return random.Next().ToString();
        }

        private void TorrentsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // If selected index is -1 (no selection) do nothing
            if (TorrentsLL.SelectedItem == null)
                return;

            //LongListSelector item = sender as LongListSelector;

            //TorrentsLL.SelectedItem = item.DataContext;

            //textBlock0.Text = sender.ToString();
            //textBlock1.Text = e.ToString();

            //textBlock0.Text = item.DataContext.ToString();
            
            var selected = (Torrent)TorrentsLL.SelectedItem;

            //textBlock0.Text = TorrentsLL.SelectedItem.GetType().ToString();
            //textBlock1.Text = TorrentsLL.SelectedItem.ToString();
            //textBlock2.Text = selected.name;
            //textBlock3.Text = ""+selected.id;
            

            //KTorrentWP7.ViewModels.TorrentsPage+Torrent

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/TorrentDetailsPage.xaml?selectedId="+selected.id, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            TorrentsLL.SelectedItem = null;
        }

        private void appbarAddtorrent_Click(object sender, EventArgs e)
        {

        }


    }

}