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
            PlayTranscoded(textBox1.Text);
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

            string cookie = wc.ResponseHeaders[HttpResponseHeader.SetCookie];

            wc = new WebClient();

            try
            {
                if (cookie != null && cookie.Length > 0)
                    wc.Headers[HttpRequestHeader.Cookie] = cookie;

                s = wc.DownloadString(s);
            }
            catch(Exception x)
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


    }
}
