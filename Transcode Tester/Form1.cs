using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Transcode_Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayTranscodedWithSegments(textBox1.Text);
        }

        private string FQDN()
        {
            return "http://10.2.0.2:32400";
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
                    return;
                }
            }

            bool b = false;
            int cnt = 0;

            while (b == false && cnt < 4)
            {
                b = TryTranscode(part);
                cnt++;
            }

            if (cnt >= 4)
            {
                MessageBox.Show("Transcoder Error");
                return;
            }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "ffplay.exe";
            psi.Arguments = "-sync video -fs playlist.m3u8";
            psi.UseShellExecute = false;

            Process.Start(psi);
        }

        private bool TryTranscode(string part)
        {
            DateTime jan1 = new DateTime(1970, 1, 1, 0, 0, 0);
            double dTime = (DateTime.Now - jan1).TotalMilliseconds;

            string time = Math.Round(dTime / 1000).ToString();
            string url = "/video/:/transcode/segmented/start.m3u8?identifier=com.plexapp.plugins.library&ratingKey=97007888&offset=0&quality=5&url=http%3A%2F%2Flocalhost%3A32400" + Uri.EscapeDataString(part) + "&3g=0&httpCookies=&userAgent=";
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

            s = s.Substring(s.IndexOf("session")).Replace("\n", "");

            s = FQDN() + "/video/:/transcode/segmented/" + s;


            try
            {
                wc = new WebClient();
                s = wc.DownloadString(s);
            }
            catch
            {
                return false;
            }

            s = s.Replace("/video", FQDN() + "/video");

            StreamWriter sw = File.CreateText("playlist.m3u8");
            sw.Write(s);
            sw.Flush();
            sw.Close();

            return true;
        }

        private void PlayTranscodedWithSegments(string part)
        {
            if (part.IndexOf("http://") != -1)
            {
                if (part.IndexOf(":32400") != -1)
                {
                    part = part.Substring(part.IndexOf(":32400/") + 6);
                }
            }

            string[] segments = TryTranscodeBySegments(part);

            if (segments.Length == 0)
            {
                MessageBox.Show("transcoder failure");
                return;
            }

            string FILENAME = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\media.ts";

            AutoResetEvent ar = new AutoResetEvent(false);
            FileStream media = new FileStream(FILENAME, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            Thread t = new Thread((o) =>
            {
                int ctr = 0;

                Plex.Client.Win32.Buffering bufferBox = new Plex.Client.Win32.Buffering();
                bufferBox.Show();
                Application.DoEvents();

                foreach (string segment in segments)
                {
                    WebClient fetch = new WebClient();
                    byte[] data = fetch.DownloadData(segment);

                    media.Write(data, 0, data.Length);
                    media.Flush();

                    if (ctr == 10)
                    {
                        ar.Set();
                        bufferBox.Close();
                        bufferBox = null;
                    }


                    if (bufferBox != null)
                    {

                        bufferBox.Increment();
                    }

                    ctr++;

                }

            });

            t.Start();

            ar.WaitOne();

            ThreadPool.QueueUserWorkItem((o) =>
            {

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.FileName = "ffplay.exe";
                psi.Arguments = "-fs \"" + FILENAME + "\"";
                psi.UseShellExecute = false;

                Process proc = Process.Start(psi);

                proc.WaitForExit();


                try
                {
                    if (t.IsAlive)
                        t.Abort();
                }
                catch
                {
                }

                try
                {
                    media.Close();
                }
                catch
                {
                }

                File.Delete(FILENAME);

            });

        }

        private string[] TryTranscodeBySegments(string part)
        {
            DateTime jan1 = new DateTime(1970, 1, 1, 0, 0, 0);
            double dTime = (DateTime.Now - jan1).TotalMilliseconds;

            string time = Math.Round(dTime / 1000).ToString();
            string url = "/video/:/transcode/segmented/start.m3u8?identifier=com.plexapp.plugins.library&ratingKey=97007888&offset=0&quality=5&url=http%3A%2F%2Flocalhost%3A32400" + Uri.EscapeDataString(part) + "&3g=0&httpCookies=&userAgent=";
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

            s = s.Substring(s.IndexOf("session")).Replace("\n", "");

            s = FQDN() + "/video/:/transcode/segmented/" + s;

            Clipboard.SetText(s);

            try
            {
                s = wc.DownloadString(s);
            }
            catch
            {
                return new string[] { };
            }

            s = s.Replace("/video", FQDN() + "/video");

            StringReader sr = new StringReader(s);

            string tmp = sr.ReadLine();

            List<string> entries = new List<string>();

            while (tmp != null)
            {
                if (tmp.IndexOf("http://") == 0)
                    entries.Add(tmp);

                tmp = sr.ReadLine();
            }

            return entries.ToArray();
        }

    }
}
