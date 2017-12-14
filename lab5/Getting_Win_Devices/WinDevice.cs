using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Win_Devices
{
    class WinDevice
    {
        public string Name { get; set; }
        public string ClassGuid { get; set; }
        public string HardwareId { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string Provider { get; set; }
        public string SysPath { get; set; }
        public string DevicePath { get; set; }
        public bool Status { get; set; }

        public WinDevice(string name,string classGuid, string hardwareId, string manufacturer, string description, string provider,
            string sysPath, string devicePath, bool status)
        {
            Name = name;
            ClassGuid = classGuid;
            HardwareId = hardwareId;
            Manufacturer = manufacturer;
            Description = description;
            Provider = provider;
            SysPath = sysPath;
            DevicePath = devicePath;
            Status = status;
        }

        public void ChangeConnection(string method)
        {
            var device = new ManagementObjectSearcher("SELECT * FROM Win32_PNPEntity").Get().OfType<ManagementObject>()
                .FirstOrDefault(x => x["DeviceID"].ToString().Equals(DevicePath));
            device.InvokeMethod(method, new object[]{ false });
        }

        
    }
}
