using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifiCatcherDesktop.Arduino
{
    public enum CatcherState
    {
        None = 0, Scanning = 1, NetworkFound = 2, NetworkNotFound = 3
    }
}
