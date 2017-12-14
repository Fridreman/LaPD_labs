using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;
using SimpleWifi;
using SimpleWifi.Win32.Interop;

namespace Getting_WIFI_Networks
{
    class NetworkSearcher
    {
        private readonly Wifi _wifi;
        private WlanClient _wlanClient;

        public NetworkSearcher()
        {
            _wifi = new Wifi();
            _wlanClient = new WlanClient();
        }

        public List<WiFiNetwork> GetNetworks()
        {
            List<WiFiNetwork> networks = new List<WiFiNetwork>();
            List<AccessPoint> accessPoints = _wifi.GetAccessPoints();
            foreach (AccessPoint accessPoint in accessPoints)
            {
                networks.Add(new WiFiNetwork(accessPoint.Name, accessPoint.SignalStrength.ToString() + "%", accessPoint.ToString(), GetBssId(accessPoint), accessPoint.IsSecure, accessPoint.IsConnected));
            }
            return networks;
        }

        private List<string> GetBssId(AccessPoint accessPoint)
        {
            var wlanInterface = _wlanClient.Interfaces.FirstOrDefault();
            return wlanInterface?.GetNetworkBssList()
                .Where(x => Encoding.ASCII.GetString(x.dot11Ssid.SSID, 0, (int)x.dot11Ssid.SSIDLength).Equals(accessPoint.Name))
                .Select(y => Dot11BSSTostring(y)).ToList();
        }

        private string Dot11BSSTostring(Wlan.WlanBssEntry entry)
        {
            StringBuilder bssIdBuilder = new StringBuilder();
            foreach (byte bssByte in entry.dot11Bssid)
            {
                bssIdBuilder.Append(bssByte.ToString("X"));
                bssIdBuilder.Append("-");
            }
            bssIdBuilder.Remove(bssIdBuilder.Length - 1, 1);
            return bssIdBuilder.ToString();
        }
    }
}
