using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Getting_USB_Devices
{
    public partial class Form1 : Form
    {
        private const int WM_DEVICECHANGE = 0X219;
        private static readonly DeviceSearcher Searcher = new DeviceSearcher();
        private List<USBDevice> _devices;
        private readonly DataTable _table = new DataTable();

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_DEVICECHANGE)
            {
                UpdateGrid();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _devices = new List<USBDevice>();
            _table.Columns.Add("Name", typeof(string));
            _table.Columns.Add("Total free space", typeof(string));
            _table.Columns.Add("Occupied space", typeof(string));
            _table.Columns.Add("Total size", typeof(string));
            UpdateGrid();
            OutputGrid.DataSource = _table;
            RemoveB.Enabled = false;
            timer1.Enabled = true;
        }

        private void UpdateGrid()
        {
            int currentPosition = 0;
            if (OutputGrid.CurrentRow != null)
            {
                currentPosition = OutputGrid.CurrentRow.Index;
            }
            _table.Clear();
            _devices = Searcher.GetDevices();
            foreach(USBDevice device in _devices)
            {
                _table.Rows.Add(device.Name, device.TotalFreeSpace, device.OccupiedSpace, device.TotalSize);
            }
            if (OutputGrid.RowCount - 1 > currentPosition)
            {
                OutputGrid.Rows[currentPosition].Selected = true;
            }
        }

        private void OutputGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OutputGrid.CurrentRow != null)
            {
                if (OutputGrid.CurrentRow.Index >= 0 && OutputGrid.CurrentRow.Index < _devices.Count)
                {
                    RemoveB.Enabled = !_devices[OutputGrid.CurrentRow.Index].IsMtp;
                }
                else
                {
                    RemoveB.Enabled = false;
                }
            }
        }

        private void RemoveB_Click(object sender, EventArgs e)
        {
            if (OutputGrid.CurrentRow != null)
            {
                if (!_devices[OutputGrid.CurrentRow.Index].Eject())
                {
                    Message.Text = "Busy";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateGrid();
        }
    }
}
