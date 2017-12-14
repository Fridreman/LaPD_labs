using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Getting_WIFI_Networks
{
    public partial class Form1 : Form
    {
        private readonly NetworkSearcher _searcher = new NetworkSearcher();
        private List<WiFiNetwork> networks;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateNetworkList();
            ConnectionB.Enabled = false;
            PasswordF.Enabled = false;
            timer1.Enabled = true;
        }

        private void UpdateNetworkList()
        {
            NetworkList.Items.Clear();
            networks = _searcher.GetNetworks();
            foreach (WiFiNetwork network in networks)
            {
                ListViewItem item = new ListViewItem(network.Name);
                item.SubItems.Add(network.SignalStrength);
                NetworkList.Items.Add(item);
            }
        }

        private void NetworkList_MouseClick(object sender, MouseEventArgs e)
        {
            ShowInfo(networks[NetworkList.SelectedItems[0].Index]);
        }

        private void ShowInfo(WiFiNetwork network)
        {
            NetworkInformation.Text = network.Description + network.GetBssIds();
            if (network.IsConnected)
            {
                ConnectionStatusL.Text = "Connected";
                PasswordF.Enabled = false;
                ConnectionB.Enabled = false;
            }
            else
            {
                ConnectionStatusL.Text = "Available";
                PasswordF.Enabled = network.IsSecured;
                ConnectionB.Enabled = true;
            }
        }

        private void ConnectionB_Click(object sender, EventArgs e)
        {
            if (PasswordF.Text.Length > 0)
            {
                if (networks[NetworkList.SelectedItems[0].Index].Connect(PasswordF.Text))
                {
                    ConnectionStatusL.Text = "Connected";
                    PasswordF.Enabled = false;
                    ConnectionB.Enabled = false;
                    NetworkList.SelectedItems[0].Selected = false;
                }
                else
                {
                    ConnectionStatusL.Text = "Error";
                }
            }
        }

        private void PingB_Click(object sender, EventArgs e)
        {
            if (UrlF.Text.Length > 0)
            {
                NetworkPing ping = new NetworkPing();
                PingAnswer.Text = ping.PingToAddress(UrlF.Text);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (NetworkList.SelectedItems.Count == 0)
            {
                UpdateNetworkList();
            }
        }
    }
}
