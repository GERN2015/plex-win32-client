using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZeroconfService;
using System.Net;

namespace Plex.Client.Win32
{
    public partial class ServerSelector : Form
    {
        NetServiceBrowser _browser = new NetServiceBrowser();

        public ServerSelector()
        {
            InitializeComponent();

            _browser.DidFindDomain += new NetServiceBrowser.DomainFound(_browser_DidFindDomain);
            _browser.DidFindService += new NetServiceBrowser.ServiceFound(_browser_DidFindService);

            _browser.SearchForBrowseableDomains();
        }

        void _browser_DidFindService(NetServiceBrowser browser, NetService service, bool moreComing)
        {
            service.DidResolveService += new NetService.ServiceResolved(service_DidResolveService);
            service.ResolveWithTimeout(60);
        }

        void service_DidResolveService(NetService service)
        {
            IPEndPoint addr = (IPEndPoint)service.Addresses[0];

            int idx = comboBox1.Items.Add(service.HostName + "(" + addr.Address.ToString() + ")" );
        }

        void _browser_DidFindDomain(NetServiceBrowser browser, string domainName, bool moreComing)
        {
            _browser.SearchForService("_plexmediasvr._tcp", domainName);
        }

        private void ServerSelector_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.Add(Properties.Settings.Default.Server);
        }

        private void ItemPicked(string s)
        {
            string ip = "";

            if (s.IndexOf("(") != -1)
            {
                ip = s.Split(new char[] { '(' })[1];
                ip = ip.Replace(")", "").Trim();
            }
            else
            {
                ip = s;
            }

            Properties.Settings.Default.Server = ip;
            Properties.Settings.Default.Save();

            this.Close();
        }

        private void comboBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ItemPicked(comboBox1.SelectedText);
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ItemPicked(comboBox1.Text);
        }

    }
}
