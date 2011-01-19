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
    public partial class Buffering : Form
    {
        public Buffering()
        {
            InitializeComponent();
        }

        public void Increment()
        {
            if (progressBar1.Value < progressBar1.Maximum)
                progressBar1.Value++;
        }

        private void WaitBox_VisibleChanged(object sender, EventArgs e)
        {
        }

    }
}
