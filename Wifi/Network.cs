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
            this.Ssid = ssid;
            this.IsFree = isFree;
            _values = new Dictionary<int, int>();
        }

        public void AddValue(int angle, int level)
        {
            _values.Add(angle, level);
        }
    }
}
