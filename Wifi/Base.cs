using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WifiCatcherDesktop.Wifi
{
    public class Base
    {
        private List<Network> _networks; 

        public List<Network> Networks
        {
            get { return _networks; }
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
            }
            else
            {
                foreach (var value in network.Values)
                {
                    itemForUpdate.AddValue(value.Key, value.Value);
                }
            }
        }


    }
}
