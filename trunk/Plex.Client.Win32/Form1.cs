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

namespace Plex.Client.Win32
{
    public partial class Form1 : Form
    {
        private WebClient wc = null;

        private string _oldArt = "", _containerArt = "";
        private Stack<string> _history = new Stack<string>();
        private Stack<int> _selectionHistory = new Stack<int>();

        private Dictionary<string, byte[]> _imageCache = new Dictionary<string, byte[]>();
        private int _lastSelectedIndex = 0;

        public Form1()
        {
            InitializeComponent();

            ServerSelector selector = new ServerSelector();
            selector.ShowDialog();

            wc = new WebClient();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string baseuri = FQDN() + "/library/sections/";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

//            string baseuri = FQDN() + "/";

            FillListFromUrl(baseuri, 0);

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected == false)
                return;

            XmlNode node = (XmlNode) e.Item.Tag;

            bool isThumb = false;
            Image img = null;
            Image old = null;

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

                if (node.Attributes["summary"] != null)
                    if (node.Attributes["summary"].Value != null)
                        label1.Text = HttpUtility.HtmlDecode(node.Attributes["summary"].Value);

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

            label1.Text = "";

            if (node.Attributes["summary"] != null)
                if (node.Attributes["summary"].Value != null)
                    label1.Text = HttpUtility.HtmlDecode(node.Attributes["summary"].Value);
        }

        private void LoadGenericArt()
        {
            Image old = pictureBox1.Image;

            SuspendLayout();
            pictureBox1.Image = Properties.Resources.plex_iTunesArtwork;
            ResumeLayout();
 

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
                        
        }

        private void PlayHTTP(string url)
        {
            Clipboard.SetText(url);

            if (HandleIfPlexWebkit(url) )
                return;

            string path = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            if (path == null || path.Length == 0)
                path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            path += "\\VideoLan\\Vlc\\vlc.exe";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("\"" + path + "\"", "\"" + HttpUtility.UrlDecode(url) + "\" :fullscreen");
            System.Diagnostics.Process.Start(psi);
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

        private void PlayStream(string url)
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

            }
            else
            {
                if (url.ToLower().Substring(0, 6).CompareTo("/video") == 0)
                {
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

                            PlayHTTP(newURL);

                            break;
                        }

                        if (s.ToLower().IndexOf("plex://") != -1)
                        {
                            newURL = s.Substring(s.IndexOf("plex://"));

                            r.Close();
                            w.Close();
                            ns.Close();
                            client.Close();

                            PlayStream(newURL);
                            break;
                        }

                        s = r.ReadLine();
                    }

                    return;
                }

                PlayHTTP(url);
            }

        }

        private static void OpenWebPage(string url)
        {
            ProcessStartInfo psi = new ProcessStartInfo("chrome.exe", "-kiosk \"" + url + "\"");
            Process.Start(psi);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            _lastSelectedIndex = listView1.SelectedIndices[0];

            XmlNode node = (XmlNode) listView1.SelectedItems[0].Tag;

            if (node.Attributes["key"] != null && node.Attributes["key"].Value.CompareTo("Quit") == 0)
            {
                this.Close();
                return;
            }

            if (node.Name.CompareTo("Video") == 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(node.OuterXml);

                XmlNode mediaPart = doc.SelectSingleNode("//Media/Part");

                string playuri = "";

                if (mediaPart != null)
                {
                    playuri = FQDN() + mediaPart.Attributes["key"].Value;
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

                PlayStream(playuri);

                return;
            }

            string url = _history.Peek() + node.Attributes["key"].Value + "/";

            if (node.Attributes["key"].Value[0] == '/')
                url = FQDN() + node.Attributes["key"].Value;

            bool isThumb = false;
            _oldArt = GetArtForNode(node, ref isThumb);

            _selectionHistory.Push(listView1.SelectedIndices[0]);

            FillListFromUrl(url, listView1.SelectedIndices[0]);
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

        private string FQDN()
        {
            return "http://" + Properties.Settings.Default.Server + ":32400";
        }

        private void FillListFromUrl(string url,int selection)
        {
            SuspendLayout();

            _history.Push(url);

            string xml = wc.DownloadString(url);

            listView1.Clear();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlAttribute conatinerArt = doc.SelectSingleNode("//MediaContainer").Attributes["art"];
            XmlAttribute summary = doc.SelectSingleNode("//MediaContainer").Attributes["summary"];

            if (summary != null)
                if (summary.Value != null )
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
                listView1.Items[0].Selected = true;
                listView1.FocusedItem = listView1.Items[0];
            }

            ResumeLayout();
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
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SuspendLayout();

            pictureBox1.Width = this.ClientRectangle.Width - listView1.Width;
            pictureBox1.Location = new Point(this.ClientRectangle.X + listView1.Width, 0);
            pictureBox1.Height = (int)(ClientRectangle.Height * .75);

            label1.Location = new Point(this.ClientRectangle.X + listView1.Width, pictureBox1.Bottom);
            label1.Height = ClientRectangle.Height - pictureBox1.Height;
            label1.Width = pictureBox1.Width;

            ResumeLayout();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Escape)
            {
                if (_history.Count == 1)
                    return;

                _history.Pop();

                string url = _history.Pop();
                int index = _selectionHistory.Pop();

                FillListFromUrl(url, index);
                listView1.Items[index].Selected = true;
                listView1.FocusedItem = listView1.Items[index];

                return;
            }

            if (e.KeyCode == Keys.F11  )
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
            }

            if (e.KeyCode == Keys.Up && listView1.SelectedIndices[0] == 0)
            {
                e.Handled = true;
                int index = listView1.Items.Count - 1;

                listView1.Items[index].Selected = true;
                listView1.FocusedItem = listView1.Items[index];
                SendKeys.Send("{END}");

            }

            if (e.KeyCode == Keys.Down && listView1.SelectedIndices[0] == (listView1.Items.Count - 1))
            {
                e.Handled = true;

                listView1.Items[0].Selected = true;
                listView1.FocusedItem = listView1.Items[0];
                SendKeys.Send("{HOME}");
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            Form1_KeyDown(sender, e);
        }
    }
}
