using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Plex.Client.Win32
{
    public partial class StreamPlayer : Form
    {
        public StreamPlayer()
        {
            InitializeComponent();
        }

        private WaitBox wb = new WaitBox();

        private void StreamPlayer_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
            trackBar1.AutoSize = true;

        }

        public void Play(string url,int offset)
        {

            string vlcOptions = ":fullscreen :http-continuous :input-fast-seek";

            axVLCPlugin21.Toolbar = true;
            axVLCPlugin21.playlist.clear();
            axVLCPlugin21.playlist.add(url, "the thing", vlcOptions);

            axVLCPlugin21.playlist.play();

        }

        private void StreamPlayer_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void StreamPlayer_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void axVLCPlugin21_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                axVLCPlugin21.playlist.togglePause();
                return;
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.VolumeUp)
            {
                if (axVLCPlugin21.Volume == 100)
                    return;

                if (axVLCPlugin21.Volume + 5 > 100)
                {
                    axVLCPlugin21.Volume = 100;
                    return;
                }

                axVLCPlugin21.Volume+=5;
                return;
            }

            if ( e.KeyCode == Keys.Down || e.KeyCode == Keys.VolumeDown )
            {
                axVLCPlugin21.Volume-=5;
                return;
            }

            if (e.KeyCode == Keys.M || e.KeyCode == Keys.VolumeMute)
            {
                axVLCPlugin21.audio.toggleMute();
                return;
            }

            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back)
            {
                this.Close();
                return;
            }
        }

        private void trackBar1_KeyDown(object sender, KeyEventArgs e)
        {
            PreviewKeyDownEventArgs args = new PreviewKeyDownEventArgs(e.KeyData);
            e.Handled = true;
            this.axVLCPlugin21_PreviewKeyDown(sender, args);

            //if (e.KeyCode != Keys.Right && e.KeyCode != Keys.Left)
            //{
            //    PreviewKeyDownEventArgs args = new PreviewKeyDownEventArgs(e.KeyData);
            //    this.axVLCPlugin21_PreviewKeyDown(sender, args );
            //    e.Handled = true;
            //    return;
            //}

            //double val = .01 * axVLCPlugin21.input.Length;

            //if (e.KeyCode == Keys.Right)
            //{
            //    axVLCPlugin21.input.Position += val;
            //    e.Handled = true;
            //    return;
            //}

            //if (e.KeyCode == Keys.Left)
            //{
            //    axVLCPlugin21.input.Position -= val;
            //    e.Handled = true;
            //    return;
            //}

        }

        private void axVLCPlugin21_MediaPlayerPositionChanged(object sender, AxAXVLC.DVLCEvents_MediaPlayerPositionChangedEvent e)
        {
        }

        private void axVLCPlugin21_MediaPlayerOpening(object sender, EventArgs e)
        {
        }

        private void axVLCPlugin21_MediaPlayerTimeChanged(object sender, AxAXVLC.DVLCEvents_MediaPlayerTimeChangedEvent e)
        {
            trackBar1.Value = e.time;
        }

        private void axVLCPlugin21_MediaPlayerPlaying(object sender, EventArgs e)
        {
            double max = axVLCPlugin21.input.Length;

            if (((int)max) != trackBar1.Maximum)
            {
                this.trackBar1.SetRange(0, (int)max);
                trackBar1.LargeChange = (int)(.1 * max);
            }
        }

        private void axVLCPlugin21_MediaPlayerEndReached(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void axVLCPlugin21_MediaPlayerBuffering(object sender, EventArgs e)
        {
        }
    }
}
