using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleWifi;

namespace Getting_WIFI_Networks
{
    class WiFiNetwork
    {
        public string Name { get; set; }
        public string SignalStrength { get; set; }
        public string Description { get; set; }
        public List<string> BssId { get; set; }
        public bool IsSecured { get; set; }
        public bool IsConnected { get; set; }

        public WiFiNetwork(string name, string signalStrength, string description, List<string> bssId, bool isSecured, bool isConnected)
        {
            Name = name;
            SignalStrength = signalStrength;
            Description = description;
            BssId = bssId;
            IsSecured = isSecured;
            IsConnected = isConnected;
        }

        public bool Connect(string password)
        {
            Wifi wifi = new Wifi();
            AccessPoint accessPoint = wifi.GetAccessPoints().FirstOrDefault(x => x.Name.Equals(Name));
            if (accessPoint != null)
            {
                AuthRequest authRequest = new AuthRequest(accessPoint);
                authRequest.Password = password;
                return accessPoint.Connect(authRequest);
            }
            return false;
        }

        public string GetBssIds()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("BssIds: ");
            foreach (var bssId in BssId)
            {
                builder.Append(bssId + "\r\n");
            }
            return builder.ToString();
        }
    }
}
