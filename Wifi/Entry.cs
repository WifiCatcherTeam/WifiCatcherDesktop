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
            lock (Base.locker)
            {
                Levels[angle] = level;
            }
        }

        public int GetLevel()
        {
            int n = 0;
            int sum = 0;
            lock (Base.locker)
            {
                foreach (var level in Levels.Values)
                {
                    n++;
                    sum += level;
                }   
            }
            return (int) Math.Round((double) sum/n);
        }

        public int GetBestAngle()
        {
            int level = -1;
            int angle = 0;

            lock (Base.locker)
            {
                foreach (var pair in Levels)
                {
                    if (level < pair.Value)
                    {
                        level = pair.Value;
                        angle = pair.Key;
                    }
                }
            }
            return angle;
        }

        public int GetBestLevel()
        {
            int level = -1;
            int angle = 0;

            lock (Base.locker)
            {
                foreach (var pair in Levels)
                {
                    if (level < pair.Value)
                    {
                        level = pair.Value;
                        angle = pair.Key;
                    }
                }
            }
            return level;
        }
    }
}
