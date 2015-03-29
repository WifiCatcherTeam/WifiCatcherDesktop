using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;
using WifiCatcherDesktop.Arduino;

namespace WifiCatcherDesktop.Wifi
{
    public class Connector
    {
        private ArduinoController _controller;
        private static readonly Guid AdapterGuid = new Guid("{7ae830fe-f14c-486d-836a-d6fb96da9854}");

        private WlanClient.WlanInterface GetAdapterWlanInterface()
        {
            var client = new WlanClient();
            return client.Interfaces.FirstOrDefault(wlanIface => wlanIface.InterfaceGuid == AdapterGuid);
        }

        public Connector(ArduinoController controller)
        {
            _controller = controller;
        }

        private int FindBestAngle(Network network)
        {
            int bestAngle = 0;
            int bestQuality = -10000;
            foreach (var entry in network.Entries)
            {
                foreach (var item in entry.Levels)
                {
                    if (item.Value > bestQuality)
                    {
                        bestAngle = item.Key;
                        bestQuality = item.Value;
                    }
                }
            }
            return bestAngle;
        }

        public void Connect(Network network, ArduinoController controller)
        {
            int bestAngle = FindBestAngle(network);
            _controller.MakeAngle(bestAngle);

            WlanClient.WlanInterface wlanIface = GetAdapterWlanInterface();
            if (network.IsFree)
            {
                string profileName = network.Ssid;
                string profileXml = string.Format("<?xml version=\"1.0\" encoding=\"US-ASCII\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>", profileName);
                wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
            }
        }
    }
}
