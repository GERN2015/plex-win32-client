using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace Plex.Client.Win32
{
    public partial class Form1 : Form
    {
        #region Vars

        private WebClient wc = null;

        private string _oldArt = "", _containerArt = "";
        private Stack<string> _history = new Stack<string>();
        private Stack<int> _selectionHistory = new Stack<int>();

        private Dictionary<string, byte[]> _imageCache = new Dictionary<string, byte[]>();

        private WaitBox _wb = new WaitBox();

        private System.Threading.ManualResetEvent _mre = new System.Threading.ManualResetEvent(false);

        private int _selection = 0;

        private bool _firstConnect = true;

        private string FQDN()
        {
            if (Properties.Settings.Default.Server.Contains(@"http://"))
            {
                return Properties.Settings.Default.Server + ":32400";
            }
            else
            {
                return "http://" + Properties.Settings.Default.Server + ":32400";
            }
        }
        
        #endregion

        #region UI
        
        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectToServer();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            SuspendLayout();

            listView1.Top = panel1.Top;
            listView1.Height = panel1.Height - 2;
            listView1.Left = panel1.Left + 1;

            pictureBox1.Width = panel1.Width - listView1.Width - 2;
            pictureBox1.Location = new Point(panel1.Left + listView1.Width, panel1.Top);
            pictureBox1.Height = (int)(panel1.Height * .75);

            label1.Location = new Point(panel1.Left + listView1.Width + 2, pictureBox1.Bottom + 1);
            label1.Height = panel1.Height - pictureBox1.Height - 4;
            label1.Width = pictureBox1.Width - 2;

            ResumeLayout();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected == false)
                return;

            foreach (ListViewItem itm in listView1.Items)
            {
                if (itm.Index != e.ItemIndex)
                {
                    itm.ImageIndex = -1;
                }
                else
                {
                    itm.ImageIndex = 0;
                }
            }

            XmlNode node = (XmlNode) e.Item.Tag;

            bool isThumb = false;
            Image img = null;
            Image old = null;

            string text = "";

            if (node.Attributes["originallyAvailableAt"] != null)
            {
                text = "Released: " + node.Attributes["originallyAvailableAt"].Value + "     ";
            }

            if (node.Attributes["duration"] != null)
            {
                int minutes = Int32.Parse(node.Attributes["duration"].Value) / 1000 / 60;

                text += "Runtime: " + minutes.ToString() + " minutes\r\n\r\n";
            }
            else
            {
                if (text.Trim().Length > 0)
                    text += "\r\n\r\n";
            }

            if (node.Attributes["summary"] != null)
                if (node.Attributes["summary"].Value != null)
                {
                    string val = node.Attributes["summary"].Value;
                    text += Encoding.UTF8.GetString(Encoding.Default.GetBytes(val));
                }

            label1.Text = text;

            if (text.Trim().Length > 0)
            {
                label1.BackColor = Color.Black;
            }
            else
            {
                label1.BackColor = Color.Transparent;
            }

            string art = GetArtForNode(node, ref isThumb);

            if (art.CompareTo("Quit") == 0)
            {
                img = Properties.Resources.Quit;
                old = this.BackgroundImage;
                this.BackgroundImage = img;

                Image pold = pictureBox1.Image;
                pictureBox1.Image = null;

                if (pold != null)
                    pold.Dispose();

                if (old != null)
                    old.Dispose();

                label1.Text = "";

                return;
            }

            if (art.StartsWith("http://") ) {

                byte[] data = null;

                if (_imageCache.ContainsKey(art) == true)
                {
                    data = _imageCache[art];
                    MemoryStream ms = new MemoryStream(data);
                    img = Image.FromStream(ms);
                }
                else
                {
                    WebClient wcArt = new WebClient();

                    try
                    {
                        data = wcArt.DownloadData(art);
                        MemoryStream ms = new MemoryStream(data);
                        img = Image.FromStream(ms);

                        _imageCache.Add(art, data);
                    }
                    catch
                    {
                        img = Properties.Resources.plex_iTunesArtwork;
                    }
                }



                if (isThumb == true)
                {
                    Image bold = this.BackgroundImage;

                    this.BackgroundImage = null;

                    if (bold != null)
                        bold.Dispose();

                    old = pictureBox1.Image;

                    pictureBox1.Image = img;
                }
                else
                {
                    old = this.BackgroundImage;
                    this.BackgroundImage = img;

                    Image pold = pictureBox1.Image;
                    pictureBox1.Image = null;

                    if (pold != null)
                        pold.Dispose();
                }

                if (old != null)
                    old.Dispose();
            }else{
                
                if ( art.CompareTo("Quit") != 0 )
                    LoadGenericArt();
            }

        }
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            _selection = 0;

            XmlNode node = (XmlNode) listView1.SelectedItems[0].Tag;

            if (node.Attributes["key"] != null && node.Attributes["key"].Value.CompareTo("Quit") == 0)
            {
                this.Close();
                return;
            }

            if (node.Attributes["key"] != null && node.Attributes["key"].Value.CompareTo("ConnectTo") == 0)
            {

                System.Threading.ThreadPool.QueueUserWorkItem((arg) =>
                {
                    System.Threading.Thread.Sleep(250);

                    Invoke( new EventHandler((o, parms) =>
                    {
                        ConnectToServer();
                    }));
                });

                return;
            }

            if (node.Name.CompareTo("Video") == 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(node.OuterXml);

                XmlNode mediaPart = doc.SelectSingleNode("//Media/Part");

                string playuri = "";
                int offset = 0;

                if (node.Attributes["viewOffset"] != null)
                {
                    offset = int.Parse(node.Attributes["viewOffset"].Value);
                }

                if (mediaPart != null)
                {
                    string file = mediaPart.Attributes["file"].Value;

                    playuri = FQDN() + mediaPart.Attributes["key"].Value;

                    if (file.EndsWith(".strm"))
                    {
                        Uri uri = new Uri(playuri);

                        TcpClient tcp = new TcpClient(uri.Host, uri.Port);
                        NetworkStream ns = tcp.GetStream();

                        StreamWriter sw = new System.IO.StreamWriter(ns);
                        StreamReader sr = new System.IO.StreamReader(ns);

                        sw.Write("GET " + uri.AbsolutePath + " HTTP/1.0\r\n\r\n");
                        sw.Flush();

                        sr.ReadLine();

                        string newURL = sr.ReadLine().Substring(10);

                        sw.Close();
                        sr.Close();
                        tcp.Close();

                        if (newURL.IndexOf("webkit") != -1)
                        {
                            newURL = newURL.Substring(newURL.IndexOf("url=") + 4);
                            newURL = Uri.UnescapeDataString(newURL);

                            MessageBox.Show(newURL);

                            OpenWebPage(newURL);
                            return;
                        }

                        PlayStream(newURL, 0);

                        return;
                    }

                }
                else
                {
                    XmlNode video = doc.SelectSingleNode("//Video");

                    if (video != null)
                    {
                        XmlAttribute key = video.Attributes["key"];

                        if (key == null)
                        {
                            MessageBox.Show("This is no URL listed for this item... Sorry...", "Can't Fetch This!");
                            return;
                        }
                        else
                        {
                            playuri = key.Value;
                        }
                    
                    }
                    else
                    {
                        playuri = "";
                    }
                }

                PlayStream(playuri, offset);
//                PlayTranscoded(playuri);

                    return;
            }

            string url = _history.Peek() + node.Attributes["key"].Value + "/";

            if (node.Attributes["key"].Value[0] == '/')
                url = FQDN() + node.Attributes["key"].Value;

            bool isThumb = false;
            _oldArt = GetArtForNode(node, ref isThumb);

            _selectionHistory.Push(listView1.SelectedIndices[0]);

            FillListFromUrl(url, 0);
        }
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKey(e);
        }
        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

            e.DrawDefault = false;

            Image img = imageList1.Images[0];

            if (e.Item.ImageIndex == 0)
                e.Graphics.DrawImage(img, new Point(e.Bounds.Left, e.Bounds.Top + 1));

            int newTextY = e.Bounds.Y;

            if (e.Item.Text.Contains('\n') == false)
            {
                newTextY += ((e.Bounds.Height / 2) - (e.Item.Font.Height / 2));
            }
            else
            {
                newTextY += ((e.Bounds.Height / 2) - ((int) (e.Graphics.MeasureString(e.Item.Text, e.Item.Font).Height / 2)));
            }

            Point newLoc = new Point(e.Bounds.Left + img.Width + 1, newTextY);
            Size newSize = new Size(e.Bounds.Width - img.Width - 1, e.Bounds.Height);

            Rectangle textBounds = new Rectangle( newLoc, newSize );

            e.DrawFocusRectangle();

            e.Graphics.DrawString(e.Item.Text, e.Item.Font, Brushes.White, textBounds, StringFormat.GenericTypographic);
        }
        
        #endregion

        #region Methods

        private void ConnectToServer()
        {

            string[] args = System.Environment.GetCommandLineArgs();

            if (Properties.Settings.Default.Server == null || Properties.Settings.Default.Server.Length == 0 || args.Contains("-last-server") == false || _firstConnect == false)
            {
                _firstConnect = false;

                int i = 0;

                for (i = 0; i < 100; i++)
                    Application.DoEvents();

                ServerSelector selector = new ServerSelector();
                selector.Parent = null;
                selector.TopMost = true;

                DialogResult dr = selector.ShowDialog();

                if (dr != DialogResult.OK)
                    return;
            }

            _firstConnect = false;

            if (Properties.Settings.Default.Server == null || Properties.Settings.Default.Server.Length == 0)
            {
                MessageBox.Show("Hmmm....  We got an empty server selection... That won't due. Please report to the devs..");
                return;
            }

            string baseuri = FQDN() + "/library/sections/";

            FillListFromUrl(baseuri, 0);
        }
        private void FillListFromUrl(string url, int selection)
        {
            _wb.Start();

            _selection = selection;

            _history.Push(url);

            try
            {
                wc.CancelAsync();
            }
            catch
            {
            }
            wc.DownloadStringAsync(new Uri(url), url);

        }
        
        private delegate void FillListBoxDelegate(string url, string xml);
        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _wb.Stop();
                MessageBox.Show(e.Error.ToString(), "An Error Occured During the Call");
                return;
            }

            if (e.Cancelled == true)
            {
                _wb.Stop();
                MessageBox.Show("The request was cancelled.");
                return;
            }

            if (this.InvokeRequired == true)
            {
                this.Invoke(new FillListBoxDelegate(FillListBox), new object[] { e.UserState.ToString(), e.Result });
            }
            else
            {
                FillListBox(e.UserState.ToString(), e.Result);
            }

        }
        private void FillListBox(string url, string xml)
        {
            SuspendLayout();

            listView1.Clear();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlAttribute conatinerArt = doc.SelectSingleNode("//MediaContainer").Attributes["art"];
            XmlAttribute summary = doc.SelectSingleNode("//MediaContainer").Attributes["summary"];

            if (summary != null)
                if (summary.Value != null)
                    label1.Text = summary.Value;

            if (conatinerArt != null)
                _containerArt = conatinerArt.Value;

            XmlNodeList entries = doc.SelectNodes("//MediaContainer/Video");
            if (entries.Count != 0)
                ParseEntries(url, entries);

            entries = doc.SelectNodes("//MediaContainer/Directory");
            ParseEntries(url, entries);

            if (listView1.Items.Count > 0)
            {
                listView1.Items[_selection].Focused = true;
                listView1.Items[_selection].Selected = true;
            }

            ResumeLayout();

            _wb.Stop();
        }
        private void ParseEntries(string url, XmlNodeList entries)
        {
            Uri uri = new Uri(url);

            foreach (XmlNode entry in entries)
            {
                string text = "";

                XmlAttribute title = entry.Attributes["title"];

                if (title != null)
                    text = title.Value;

                if (title == null)
                    title = entry.Attributes["name"];

                if (title != null)
                    text = title.Value;

                if (title == null)
                    text = "<Unknown>";

                text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));

                XmlAttribute vtype = entry.Attributes["type"];

                if (vtype != null && vtype.Value.CompareTo("episode") == 0)
                {

                    XmlAttribute gpt = entry.Attributes["grandparentTitle"];

                    if (gpt != null)
                    {
                        if (url.Contains("/allLeaves") == false)
                        {
                            text = gpt.Value + "\r\nSeason " + entry.Attributes["parentIndex"].Value + " / Episode " + entry.Attributes["index"].Value + "\r\n" + text;
                        }
                        else
                        {
                            text = "Season " + entry.Attributes["parentIndex"].Value + " / Episode " + entry.Attributes["index"].Value + "\r\n" + text;

                        }

                    }
                    else
                    {
                        text = entry.Attributes["index"].Value + " - " + text;
                    }
                }

                ListViewItem item = listView1.Items.Add(text);

                item.Tag = entry;
                item.ImageIndex = 0;
            }

            if (uri.AbsolutePath.CompareTo("/library/sections/") == 0)
            {
                XmlDocument doc = new XmlDocument();

                XmlNode videos = doc.CreateNode(XmlNodeType.Element, "Directory", "");
                XmlAttribute attr = videos.Attributes.Append(doc.CreateAttribute("title"));
                attr.Value = "Video Plugins";
                attr = videos.Attributes.Append(doc.CreateAttribute("key"));
                attr.Value = "/video/";
                attr = videos.Attributes.Append(doc.CreateAttribute("art"));
                attr.Value = "/:/resources/movie-fanart.jpg";

                ListViewItem itm = listView1.Items.Add("Video Plugins");

                itm.Tag = videos;
                itm.ImageIndex = 0;

                XmlNode connectTo = doc.CreateNode(XmlNodeType.Element, "Directory", "");
                attr = connectTo.Attributes.Append(doc.CreateAttribute("title"));
                attr.Value = "Connect to...";
                attr = connectTo.Attributes.Append(doc.CreateAttribute("key"));
                attr.Value = "ConnectTo";
                attr = connectTo.Attributes.Append(doc.CreateAttribute("art"));
                attr.Value = "Quit";

                itm = listView1.Items.Add("Connect to...");

                itm.Tag = connectTo;
                itm.ImageIndex = 0;

                XmlNode quit = doc.CreateNode(XmlNodeType.Element, "Directory", "");
                attr = quit.Attributes.Append(doc.CreateAttribute("title"));
                attr.Value = "Quit";
                attr = quit.Attributes.Append(doc.CreateAttribute("key"));
                attr.Value = "Quit";
                attr = quit.Attributes.Append(doc.CreateAttribute("art"));
                attr.Value = "Quit";

                itm = listView1.Items.Add("Quit");

                itm.Tag = quit;
                itm.ImageIndex = 0;
            }
            else
            {
                listView1.Sort();
            }
        }
        
        private string GetArtForNode(XmlNode node, ref bool isThumb)
        {
            string result = "";

            XmlAttribute art = node.Attributes["thumb"];

            if (art != null)
            {
                isThumb = true;

                if (art.Value.Length > 7 && art.Value.Substring(0, 7).ToLower().CompareTo("http://") == 0)
                {
                    return art.Value;
                }
                else
                {
                    return FQDN() + art.Value;
                }
            }

            isThumb = false;

            art = node.Attributes["art"];

            if (art != null && art.Value.ToLower().CompareTo("quit") == 0)
            {
                return "Quit";
            }

            if (art != null)
            {
                return FQDN() + art.Value;
            }

            if (_containerArt.Trim().Length != 0)
                return FQDN() + _containerArt;

            return result;
        }
        private void LoadGenericArt()
        {
            Image old = pictureBox1.Image;

            SuspendLayout();
            pictureBox1.Image = Properties.Resources.plex_iTunesArtwork;
            ResumeLayout();
        }

        private void PlayTranscoded(string part)
        {
            if (part.IndexOf("http://") != -1)
            {
                if (part.IndexOf(":32400") != -1)
                {
                    part = part.Substring(part.IndexOf(":32400/") + 6);
                }
                else
                {
                    PlayHttpWithDirectShow(part);
                    return;
                }
            }

            DateTime jan1 = new DateTime(1970, 1, 1, 0, 0, 0);
            double dTime = (DateTime.Now - jan1).TotalMilliseconds;

            string time = Math.Round(dTime / 1000).ToString();
            string url = "/video/:/transcode/segmented/start.m3u8?identifier=com.plexapp.plugins.library&ratingKey=97007888&offset=5&quality=6&url=http%3A%2F%2Flocalhost%3A32400" + Uri.EscapeDataString(part) + "&3g=0&httpCookies=&userAgent=";
            string msg = url + "@" + time;
            string publicKey = "KQMIY6GATPC63AIMC4R2";
            byte[] privateKey = Convert.FromBase64String("k3U6GLkZOoNIoSgjDshPErvqMIFdE0xMTx8kgsrhnC0=");

            HMACSHA256 hmac = new HMACSHA256(privateKey);
            hmac.ComputeHash(UTF8Encoding.UTF8.GetBytes(msg));

            string token = Convert.ToBase64String(hmac.Hash);

            System.Net.WebClient wc = new System.Net.WebClient();

            wc.Headers.Add("X-Plex-Access-Key", publicKey);
            wc.Headers.Add("X-Plex-Access-Time", time);
            wc.Headers.Add("X-Plex-Access-Code", token);

            string s = wc.DownloadString(FQDN() + url);

            s = s.Substring(s.IndexOf("session")).Replace("\n","");

            s = FQDN() + "/video/:/transcode/segmented/" + s;

            int retries = 0;

            while (retries <= 4)
            {
                try
                {
                    s = wc.DownloadString(s);
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(3000);

                    retries++;
                }
            }

            if (retries >= 4 && s.IndexOf('#') == -1)
            {
                MessageBox.Show("Transcoder failure..");
                return;
            }

            s = s.Replace("/video", FQDN() + "/video");

            StreamWriter sw = File.CreateText("playlist.m3u8");
            sw.Write(s);
            sw.Flush();
            sw.Close();

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "ffplay.exe";
            psi.Arguments = "-rtbufsize 100000000 -sync audio -fs playlist.m3u8";
            psi.UseShellExecute = false;

            Process.Start(psi);
        }

        private void PlayStream(string url, int offset)
        {
            if (url.Substring(0, 7).ToLower().CompareTo("plex://") == 0)
            {
                Uri uri = new Uri(url);

                url = uri.Query.Split(new char[] { '&' })[0];
                url = url.Substring(5);
                url = HttpUtility.UrlDecode(url);

                //
                // special case for Netflix plugin
                //

                if (url.IndexOf("netflix") != -1)
                {
                    string[] parts = url.Split(new char[] { '&' });

                    string tmp = parts[0];

                    if (url.IndexOf("#resume") != -1)
                    {
                        tmp += "#resume";
                    }
                    else
                    {
                        tmp += "#restart";
                    }

                    url = tmp;

                    //
                    // end case
                    //
                }

                Clipboard.SetText(url);

                OpenWebPage(url);

                return;

            }
            else
            {
                if (url.IndexOf("/video") != -1 && url.IndexOf(":32400") != -1)
                {
                    url = url.Substring(url.IndexOf("/video"));

                    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                    client.Connect(Properties.Settings.Default.Server, 32400);

                    System.Net.Sockets.NetworkStream ns = client.GetStream();
                    StreamWriter w = new StreamWriter(ns);
                    StreamReader r = new StreamReader(ns);

                    w.WriteLine("GET " + url + " HTTP/1.0");
                    w.WriteLine("");
                    w.Flush();

                    string s = r.ReadLine();

                    string newURL = "";

                    while (s != null)
                    {
                        if (s.ToLower().IndexOf("http://") != -1)
                        {
                            newURL = s.Substring(s.IndexOf("http://"));

                            r.Close();
                            w.Close();
                            ns.Close();
                            client.Close();

//                            PlayTranscoded(url);
                            PlayHttpWithDirectShow(newURL);

                            break;
                        }

                        if (s.ToLower().IndexOf("plex://") != -1)
                        {
                            newURL = s.Substring(s.IndexOf("plex://"));

                            r.Close();
                            w.Close();
                            ns.Close();
                            client.Close();

                            PlayStream(newURL, offset);
                            break;
                        }

                        s = r.ReadLine();
                    }

                    return;
                }

//                PlayHTTP(url, offset);
                PlayTranscoded(url);
            }

        }

        private static void PlayHttpWithDirectShow(string newURL)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "ffplay.exe";
            psi.Arguments = "-fs " + newURL;
            psi.UseShellExecute = false;

            Process.Start(psi);
        }
        private void PlayHTTP(string url, int offset)
        {
            Clipboard.SetText(url);

            if (HandleIfPlexWebkit(url) )
                return;

            TimeSpan ts = TimeSpan.FromMilliseconds(offset);

            //DialogResult dr = MessageBox.Show("Resume from position " + ts.ToString() + " ?", "Resume", MessageBoxButtons.YesNoCancel);

            //if (dr == DialogResult.Cancel)
            //    return;

            //if (dr == DialogResult.No)
            //    offset = 0;

            StreamPlayer sp = new StreamPlayer();
            sp.Show();
            sp.Play(url, offset);
            sp.BringToFront();
            sp.TopMost = true;

        }
        private static void OpenWebPage(string url)
        {

            WebPlayer wp = new WebPlayer();
            wp.Show();
            wp.Play(url);
            wp.BringToFront();
            wp.TopMost = true;
        }     
        private bool HandleIfPlexWebkit(string url)
        {
            bool rval = false;

            Uri uri = new Uri(url);

            TcpClient c = new TcpClient();
            c.Connect(uri.Host, uri.Port);
            StreamReader sr = new StreamReader(c.GetStream());
            StreamWriter sw = new StreamWriter(c.GetStream());

            sw.Write("GET " + uri.PathAndQuery + " HTTP/1.0\r\n\r\n");
            sw.Flush();


            string s = sr.ReadLine();

            int status = Int32.Parse(s.Split(new char[] { ' ' })[1]);

            while (s.Trim().Length > 0)
            {
                if (s.Substring(0, 9).CompareTo("Location:") == 0)
                {
                    string newPath = s.Substring(9);

                    if (newPath.IndexOf("webkit") != -1)
                    {
                        newPath = newPath.Substring(newPath.IndexOf("url=") + 4);
                        newPath = HttpUtility.UrlDecode(newPath);

                        rval = true;
                        OpenWebPage(newPath);

                        break;
                    }
                }

                s = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
            c.Close();

            return rval;
        }    

        private void HandleKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Escape)
            {
                if (_history.Count == 1)
                    return;

                _history.Pop();

                string url = _history.Pop();
                int index = _selectionHistory.Pop();

                FillListFromUrl(url, index);

                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.F11)
            {
                if (this.FormBorderStyle == FormBorderStyle.None)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                }

                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Up && listView1.SelectedIndices[0] == 0)
            {
                e.Handled = true;
                int index = listView1.Items.Count - 1;

                listView1.Items[index].Selected = true;
                listView1.FocusedItem = listView1.Items[index];
                SendKeys.Send("{END}");

                return;
            }

            if (e.KeyCode == Keys.Down && listView1.SelectedIndices[0] == (listView1.Items.Count - 1))
            {
                e.Handled = true;

                listView1.Items[0].Selected = true;
                listView1.FocusedItem = listView1.Items[0];
                SendKeys.Send("{HOME}");

                return;
            }

        }

        #endregion
        
    }

}
