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

        private void StreamPlayer_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        public void Play(string url)
        {
            axVLCPlugin21.Toolbar = true;
            axVLCPlugin21.playlist.clear();
            axVLCPlugin21.playlist.add(url, "the thing", ":fullscreen :http-continuous :ffmpeg-workaround-bugs=40");
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
    }
}
