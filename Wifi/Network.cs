using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WifiCatcherDesktop.Wifi
{
    public class Network
    {
        private Dictionary<int, int> _values;

        public string Ssid { get; private set; }
        public bool IsFree { get; private set; }

        public Dictionary<int, int> Values
        {
            get { return _values; }
        }

        public Network(string ssid, bool isFree)
        {
            Ssid = ssid;
            IsFree = isFree;
            _values = new Dictionary<int, int>();
        }

        public Network(string ssid, bool isFree, int angle, int level) : this(ssid, isFree)
        {
            AddValue(angle, level);
        }

        public void AddValue(int angle, int level)
        {
            _values[angle] = level;
        }
    }
}
