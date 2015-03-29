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

        public void AddOrUpdateNetwork(Network network)
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
        }

        public void AddOrUpdateEntry(Entry hotspot)
        {
            Network itemForUpdate = null;
            foreach (var network in _networks)
            {
                if (hotspot.Ssid == network.Ssid)
                {
                    itemForUpdate = network;
                }
            }
            if (itemForUpdate == null)
            {
                throw new Exception("No such network. Please check the correct sequence of data updates.");
            }
            itemForUpdate.AddEntry(hotspot);
            if (NetworkUpdated != null)
                NetworkUpdated(itemForUpdate);
        }

    }
}
