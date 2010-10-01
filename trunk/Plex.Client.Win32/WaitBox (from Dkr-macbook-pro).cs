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
    public partial class WaitBox : Form
    {
        public WaitBox()
        {
            InitializeComponent();

            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible == false)
                this.Show();

            this.label1.Text = "." + label1.Text + ".";
        }

        private void WaitBox_VisibleChanged(object sender, EventArgs e)
        {
        }

        public void Start()
        {
            this.timer1.Enabled = true;
        }

        public void Stop()
        {
            this.timer1.Enabled = false;
            this.label1.Text = "Getting data";
            this.Hide();
        }
    }
}
