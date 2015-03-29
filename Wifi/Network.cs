using System.Collections.Generic;

namespace WifiCatcherDesktop.Wifi
{
    public class Network
    {
        public string Ssid { get; set; }
        public bool IsFree { get; set; }
        public List<Entry> Entries { get; private set; }

        public Network() { }

        public Network(string ssid, bool isFree)
        {
            Ssid = ssid;
            IsFree = isFree;
            Entries = new List<Entry>();
        }

        public void AddEntry(Entry entry)
        {
            Entries.Add(entry);
        }
    }
}
