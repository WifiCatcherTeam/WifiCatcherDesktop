using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using NativeWifi;
using WifiCatcherDesktop.Arduino;

namespace WifiCatcherDesktop.Wifi
{
    public delegate void ScanningStartedEvent();
    public delegate void ScanningStoppedEvent();
    public delegate void ScanningMakeAngleEvent(int angle, List<Network> networks);

    public class Scanner
    {
#if DEBUG
        private const int StepsCount = 3;
#else
        private const int StepsCount = 12;
#endif
        private static readonly Guid AdapterGuid = new Guid("{7ae830fe-f14c-486d-836a-d6fb96da9854}");

        private readonly ArduinoController _controller;

        private readonly Base _wifiBase;
        public Base WifiBase
        {
            get { return _wifiBase; }
        }

        public event ScanningStartedEvent ScanningStarted;
        public event ScanningMakeAngleEvent ScanningMakeAngle;
        public event ScanningStoppedEvent ScanningStopped;

        public bool IsScanning = false;

        private CancellationTokenSource _scanningTokenSource;

        public Scanner(ArduinoController controller, Base wifiBase)
        {
            _controller = controller;
            _wifiBase = wifiBase;
        }

        public void StartScanning()
        {
            _scanningTokenSource = new CancellationTokenSource();
            Task.Run(() => DoScanning(), _scanningTokenSource.Token);
        }

        public void StopScanning()
        {
            if (!IsScanning) return;
            _scanningTokenSource.Cancel();
        }

        private void DoScanning()
        {
            _wifiBase.Clear();
            NotifyScanningStarted();

            var angleStep = (ArduinoController.HighestServoAngle - ArduinoController.LowestServoAngle + 1) / StepsCount;
            for (var angle = ArduinoController.LowestServoAngle; angle <= ArduinoController.HighestServoAngle; angle += angleStep)
            {
                if (_scanningTokenSource.IsCancellationRequested)
                {
                    NotifyScanningStopped();
                    return;
                }

                _controller.MakeAngle(angle);

                var networks = GetEnabledNetworksWithEntries(angle);
                foreach (var network in networks)
                    _wifiBase.AddOrUpdateNetwork(network);

                if (ScanningMakeAngle != null)
                    ScanningMakeAngle(angle, networks);
            }

            _controller.MakeAngle(ArduinoController.InitServoAngle);

            NotifyScanningStopped();
        }

        private void NotifyScanningStarted()
        {
            if (ScanningStarted != null)
                ScanningStarted();
            IsScanning = true;

            _controller.MakeState(CatcherState.Scanning);
        }

        private void NotifyScanningStopped()
        {
            if (ScanningStopped != null)
                ScanningStopped();
            IsScanning = false;

            _controller.MakeState(CatcherState.None);
        }

        private List<Network> GetEnabledNetworksWithEntries(int angle)
        {
            var adapterIface = GetAdapterWlanInterface();
            adapterIface.Scan();

            var networks = GetEnabledNetworks(adapterIface);
            GetEnabledEntriesAndAddToNetworks(networks, adapterIface, angle);
            return networks;
        }

        private WlanClient.WlanInterface GetAdapterWlanInterface()
        {
            var client = new WlanClient();
            return client.Interfaces.FirstOrDefault(wlanIface => wlanIface.InterfaceGuid == AdapterGuid);
        }

        private List<Network> GetEnabledNetworks(WlanClient.WlanInterface wlanInterface)
        {
            var networks = new List<Network>();
            var apiNetworks = wlanInterface.GetAvailableNetworkList(0);
            foreach (var apiNetwork in apiNetworks)
            {
                var ssid = GetStringForSsid(apiNetwork.dot11Ssid);
                var isFree = !apiNetwork.securityEnabled;
                
                var network = new Network(ssid, isFree);
                networks.Add(network);
            }
            return networks;
        }

        private void GetEnabledEntriesAndAddToNetworks(List<Network> networks, WlanClient.WlanInterface wlanInterface, int angle)
        {
            var wlanBssEntries = wlanInterface.GetNetworkBssList();
            foreach (var wlanBssEntry in wlanBssEntries)
            {
                var ssid = GetStringForSsid(wlanBssEntry.dot11Ssid);
                var macAddr = GetStringForMacAddr(wlanBssEntry.dot11Bssid); 
                var level = (int)wlanBssEntry.linkQuality;

                var entry = new Entry(macAddr);
                entry.AddLevel(angle, level);

                foreach (var network in networks.Where(network => network.Ssid == ssid))
                    network.AddEntry(entry);
            }
        }

        private static string GetStringForSsid(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private static string GetStringForMacAddr(byte[] bytes)
        {
            var macAddrLen = (uint)bytes.Length;
            var str = new string[(int)macAddrLen];
            for (var i = 0; i < macAddrLen; i++)
            {
                str[i] = bytes[i].ToString("x2");
            }
            var mac = string.Join("", str);
            return mac;
        }
    }
}
