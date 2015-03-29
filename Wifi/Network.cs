using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace WifiCatcherDesktop.Wifi
{
    public class Network
    {
        private Dictionary<string, Entry> _values;

        public string Ssid { get; private set; }
        public bool IsFree { get; private set; }

        public Dictionary<string, Entry> Values
        {
            get { return _values; }
        }

        public Network(string ssid, bool isFree)
        {
            Ssid = ssid;
            IsFree = isFree;
            _values = new Dictionary<string, Entry>();
        }

        //??
        public Network(string ssid, bool isFree, Entry hotspot) : this(ssid, isFree)
        {
            _values.Add(hotspot.Mac, hotspot);
        }

        public void AddEntry(string mac, int angle, int level)
        {
            if (_values.ContainsKey(mac))
            {
                Entry temp;
                _values.TryGetValue(mac,out temp);
                temp.AddQualityValues(angle, level);
            }
            else
            {
                Entry temp = new Entry(Ssid, mac);
                temp.AddQualityValues(angle, level);
                _values.Add(mac, temp);
            }
        }

        public void AddEntry(Entry hotspot)
        {
            foreach (var item in hotspot.Quality)
            {
                this.AddEntry(hotspot.Mac, item.Key, item.Value);
            }
        }
    }
}
