using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Getting_Win_Devices
{
    public partial class Form1 : Form
    {
        private readonly DeviceInformationSearcher _searcher = new DeviceInformationSearcher();
        private List<WinDevice> _devices;

        public Form1()
        {
            InitializeComponent();
            UpdateDeviceList();

        }

        private void UpdateDeviceList()
        {
             _devices = _searcher.GetDevices();
            foreach (WinDevice device in _devices)
            {
                DeviceList.Items.Add(device.Name);
            }
        }

        private void DeviceList_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayDeviceInfo(_devices[DeviceList.SelectedItems[0].Index]);
        }

        private void DisplayDeviceInfo(WinDevice device)
        {
            string info = "Guid: " + device.ClassGuid + "\r\nHardwareID: " + device.HardwareId + "\r\nManufacturer: " +
                     device.Manufacturer + "\r\nProvider: " + device.Provider + "\r\nDriver description: " +
                     device.Description + "\r\n.sys path: " + device.SysPath + "\r\nDevice path: " + device.DevicePath;
            DeviceInfo.Text = info;
            EnableB.Enabled = !device.Status;
            DisableB.Enabled = device.Status;
        }

        private void EnableB_Click(object sender, EventArgs e)
        {
            _devices[DeviceList.SelectedItems[0].Index].ChangeConnection("Enable");
            _devices[DeviceList.SelectedItems[0].Index].Status = !_devices[DeviceList.SelectedItems[0].Index].Status;
            EnableB.Enabled = !EnableB.Enabled;
            DisableB.Enabled = !DisableB.Enabled;
        }

        private void DisableB_Click(object sender, EventArgs e)
        {
            _devices[DeviceList.SelectedItems[0].Index].ChangeConnection("Disable");
            _devices[DeviceList.SelectedItems[0].Index].Status = !_devices[DeviceList.SelectedItems[0].Index].Status;
            DisableB.Enabled = !DisableB.Enabled;
            EnableB.Enabled = !EnableB.Enabled;
        }
    }
}
