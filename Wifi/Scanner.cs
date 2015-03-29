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
    public delegate void ScanningMakeAngleEvent(int angle);

    public class Scanner
    {
#if DEBUG
        private const int StepsCount = 3;
#else
        private const int StepsCount = 12;
#endif

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
                if (ScanningMakeAngle != null)
                    ScanningMakeAngle(angle);

                var networks = GetEnabledNetworks(angle);
                foreach (var network in networks)
                    _wifiBase.AddOrUpdateNetwork(network);
            }

            NotifyScanningStopped();
        }

        private void NotifyScanningStarted()
        {
            if (ScanningStarted != null)
                ScanningStarted();
            IsScanning = true;

            //_controller.MakeState(CatcherState.Scanning);
        }

        private void NotifyScanningStopped()
        {
            if (ScanningStopped != null)
                ScanningStopped();
            IsScanning = false;

            _controller.MakeState(CatcherState.None);
        }

        private List<Network> GetEnabledNetworks(int angle)
        {
            var result = new List<Network>();
            var hotspots = new List<Entry>();
            var client = new WlanClient();
           
            foreach (var wlanIface in client.Interfaces)
            {
                if (wlanIface.InterfaceGuid != new Guid("{7ae830fe-f14c-486d-836a-d6fb96da9854}"))
                    continue;

                wlanIface.Scan();

                Wlan.WlanBssEntry[] wlanBssEntries = wlanIface.GetNetworkBssList();
                foreach (Wlan.WlanBssEntry wlanBssEntry in wlanBssEntries)
                {
                    string ssid = GetStringForSsid(wlanBssEntry.dot11Ssid);
                    byte[] macAddr = wlanBssEntry.dot11Bssid;
                    var macAddrLen = (uint)macAddr.Length;
                    var str = new string[(int)macAddrLen];
                    for (int i = 0; i < macAddrLen; i++)
                    {
                        str[i] = macAddr[i].ToString("x2");
                    }
                    string mac = string.Join("", str);
                    int quality = (int) wlanBssEntry.linkQuality;
                    Entry temp = new Entry(ssid, mac);
                    temp.AddQualityValues(angle, quality);
                    hotspots.Add(temp);
                }

                var networks = wlanIface.GetAvailableNetworkList(0);
                foreach (var network in networks)
                {
                    var ssid = GetStringForSsid(network.dot11Ssid);
                    var security = network.securityEnabled;
                    //var sLevel = (int)network.wlanSignalQuality;
                    var temp = new Network(ssid, security);
                    foreach (var entry in hotspots)
                    {
                        if (temp.Ssid == entry.Ssid)
                        {
                            temp.AddEntry(entry);
                        }
                    }
                    result.Add(temp);
                }
            }
            return result;
        }

        private static string GetStringForSsid(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }
    }
}
