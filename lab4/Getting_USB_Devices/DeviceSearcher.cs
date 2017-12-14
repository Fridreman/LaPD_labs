using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MediaDevices;

namespace Getting_USB_Devices
{
    class DeviceSearcher
    {
        public List<USBDevice> GetDevices()
        {
            List<USBDevice> devices = new List<USBDevice>();
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Removable).ToList();
            var mtpDevices = MediaDevice.GetDevices().ToList();
            foreach (MediaDevice device in mtpDevices)
            {
                device.Connect();
                if (device.DeviceType != DeviceType.Generic)
                {
                    devices.Add(new USBDevice(device.FriendlyName, null, null, null, true));
                }
            }
            foreach (DriveInfo drive in drives)
            {
                devices.Add(new USBDevice(drive.Name, BytesToMegaBytesString(drive.TotalFreeSpace),
                    BytesToMegaBytesString(drive.TotalSize - drive.TotalFreeSpace),
                    BytesToMegaBytesString(drive.TotalSize), false));
            }
            return devices;
        }

        private string BytesToMegaBytesString(long value)
        {
            double megaBytes = (value / 1024) / 1024;
            return megaBytes.ToString() + " MB";
        }
    }
}
