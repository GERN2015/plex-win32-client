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
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label1.Text += ".";
        }

        private void WaitBox_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                this.timer1.Enabled = false;
                this.label1.Text = "Getting data";
            }
            else
            {
                this.timer1.Enabled = true;
            }
        }
    }
}
