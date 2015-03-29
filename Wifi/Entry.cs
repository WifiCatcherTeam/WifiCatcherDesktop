using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace WifiCatcherDesktop.Wifi
{
    public class Entry
    {
        private Dictionary<int, int> _quality; 

        public string Ssid { get; private set; }
        public string Mac { get; private set; }

        public Dictionary<int, int> Quality
        {
            get { return _quality; }
        }

        public Entry(string ssid, string mac)
        {
            Ssid = ssid;
            Mac = mac;
            _quality = new Dictionary<int, int>();
        }

        public void AddQualityValues(int angle, int level)
        {
            _quality[angle] = level;
        }
    }
}
