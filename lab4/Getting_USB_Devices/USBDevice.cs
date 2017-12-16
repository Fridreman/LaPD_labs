using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbEject;

namespace Getting_USB_Devices
{
    class USBDevice
    {
        public string Name { get; set; }
        public string TotalFreeSpace { get; set; }
        public string OccupiedSpace { get; set; }
        public string TotalSize { get; set; }
        public bool IsMtp { get; set; }

        public USBDevice(string name, string totalFreeSpace, string occupiedSpace, string totalSize, bool isMtp)
        {
            Name = name;
            TotalFreeSpace = totalFreeSpace;
            OccupiedSpace = occupiedSpace;
            TotalSize = totalSize;
            IsMtp = isMtp;
        }

        public bool Eject()
        {
            var ejectedDevice = new VolumeDeviceClass().SingleOrDefault(v => v.LogicalDrive == this.Name.Remove(2));
            ejectedDevice.Eject(false);
            ejectedDevice = new VolumeDeviceClass().SingleOrDefault(v => v.LogicalDrive == this.Name.Remove(2));
            return ejectedDevice == null;
        }
    }
}
