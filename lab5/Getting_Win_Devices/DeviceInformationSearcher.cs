using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Win_Devices
{
    class DeviceInformationSearcher
    {
        private ManagementObjectSearcher _searcher;
        private static readonly SelectQuery _query = new SelectQuery("SELECT * FROM Win32_PnPEntity");
        private static readonly ManagementScope _scope = new ManagementScope("\\\\.\\root\\cimv2");

        public DeviceInformationSearcher()
        {
            _searcher = new ManagementObjectSearcher(_scope, _query);
        }

        public List<WinDevice> GetDevices()
        {
            List<WinDevice> devices = new List<WinDevice>();
            foreach (ManagementObject device in _searcher.Get())
            {
                string[] driverInfo = GetDriverInfo(device);
                devices.Add(new WinDevice( device["Name"]?.ToString(),
                    device["ClassGuid"]?.ToString(),
                    device["HardwareID"] == null ? "" : String.Join("\n", (string[])device["HardwareID"]),
                    device["Manufacturer"]?.ToString(),
                    driverInfo[0],
                    device["Caption"]?.ToString(),
                    driverInfo[1],
                    device["DeviceID"]?.ToString(),
                    device["Status"].ToString().Equals("OK")
                    ));
            }
            return devices;
        }

        private string[] GetDriverInfo(ManagementObject device)
        {
            string[] driverInfo = new string[2];
            foreach (var driverParameter in device.GetRelated("Win32_SystemDriver"))
            {
                driverInfo[0] += driverParameter["Description"] == null ? "" : (driverParameter["Description"].ToString() + "\n");
                driverInfo[1] += driverParameter["PathName"] == null ? "" : (driverParameter["PathName"].ToString() + "\n");
            }
            return driverInfo;
        }
    }
}
