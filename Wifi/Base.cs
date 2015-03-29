using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WifiCatcherDesktop.Wifi
{
    public delegate void NetworkUpdatedEvent(Network network);

    public class Base
    {
        private readonly List<Network> _networks;

        public event NetworkUpdatedEvent NetworkUpdated;

        public List<Network> Networks
        {
            get { return _networks; }
        }

        public Base()
        {
            _networks = new List<Network>();
        }

        public void InsertOrUpdate(Network network)
        {
            Network itemForUpdate = null;
            foreach (var item in _networks)
            {
                if (item.Ssid == network.Ssid)
                {
                    itemForUpdate = item;
                }
            }
            if (itemForUpdate == null)
            {
                _networks.Add(network);
                if (NetworkUpdated != null)
                    NetworkUpdated(network);
            }
            else
            {
                foreach (var value in network.Values)
                {
                    itemForUpdate.AddValue(value.Key, value.Value);
                }
                if (NetworkUpdated != null)
                    NetworkUpdated(itemForUpdate);
            }
        }

    }
}
