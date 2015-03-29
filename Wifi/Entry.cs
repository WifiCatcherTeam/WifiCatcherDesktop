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
        public Dictionary<int, int> Levels { get; private set; }
        public string Mac { get; set; }

        public Entry() { }

        public Entry(string mac)
        {      
            Mac = mac;
            Levels = new Dictionary<int, int>();
        }

        public void AddLevel(int angle, int level)
        {
            Levels[angle] = level;
        }
    }
}
