using System.Collections.Generic;
using System.Linq;

namespace WifiCatcherDesktop.Wifi
{
    public delegate void NetworkUpdatedEvent(Network network);

    public class Base
    {
        private readonly List<Network> _networks;
        public IEnumerable<Network> Networks
        {
            get { return _networks; }
        }

        public static object locker = new object();

        public event NetworkUpdatedEvent NetworkUpdated;

        public Base()
        {
            _networks = new List<Network>();
        }

        public void AddOrUpdateNetwork(Network network)
        {
            foreach (var item in _networks.Where(item => item.Ssid == network.Ssid))
            {
                UpdateNetwork(item, network);
                return;
            }
            _networks.Add(network);
        }

        private void UpdateNetwork(Network item, Network network)
        {
            foreach (var entry in network.Entries)
            {
                var sameEntry = item.Entries.FirstOrDefault(e => e.Mac == entry.Mac);
                if (sameEntry != null)
                {
                    UpdateEntry(sameEntry, entry);
                    continue;
                }
                item.AddEntry(entry);
            }
        }

        private void UpdateEntry(Entry item, Entry entry)
        {
            lock (locker)
            {
                foreach (var pair in entry.Levels)
                {
                    item.Levels[pair.Key] = pair.Value;
                }
            }
        }

        public void Clear()
        {
            _networks.Clear();
        }
    }
}
