using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using NativeWifi;

namespace WifiMap
{
    public class WifiNetworks
    {
        public class Network
        {
            public string Ssid { get; private set; }
            public bool SecurityEnabled { get; private set; }
            public int SignalLevel { get; private set; }
            public int Angle { get; private set; }

            public Network(string ssid, bool securityEnabled, int signalLevel, int angle)
            {
                this.Ssid = ssid;
                this.SecurityEnabled = securityEnabled;
                this.SignalLevel = signalLevel;
                this.Angle = angle;
            }
        }

        private List<Network> _networks;

        public List<Network> Networks
        {
            //rewrite it
            get { return _networks; }
        } 

        //there are errors
        public void Search()
        {
            int angle = minangle;
            //to start angle
            List<Network> temp;
            do
            {
                RotatePlatform(angle);
                temp = GetEnabledNetworks(angle);
                UpdateData(temp);
                angle += 10;
            }while (angle <= maxangle);
        }

        private void UpdateData(List<Network> newData)
        {
            foreach (var item in newData)
            {
                bool isExists = false;
                for ( int i = 0; i < _networks.Count; ++i )
                {
                    if (_networks[i].Ssid == item.Ssid)
                    {
                        isExists = true;
                        if (item.SignalLevel > _networks[i].SignalLevel)
                        {
                            _networks[i] = item;
                        }
                    }
                }
                if (!isExists)
                {
                    _networks.Add(item);
                }
            }
        }

        private string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private List<Network> GetEnabledNetworks(int angle)
        {
            var result = new List<Network>();
            WlanClient client = new WlanClient();
            //change for our device
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    string ssid = GetStringForSSID(network.dot11Ssid);
                    bool security = network.securityEnabled;
                    int sLevel = (int)network.wlanSignalQuality;
                    result.Add(new Network(ssid, security, sLevel, angle));
                }
            }
            return result;
        }
    }
}
