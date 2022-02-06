using System.Collections.Generic;

namespace PiShockVRC.Core
{
    public class PiShockDevice
    {
        public string Name;
        public LinkData Link;
        public List<PiShockPoint> Points;
        public float NextValidShock;

        public PiShockDevice(string name, LinkData link)
        {
            Name = name;
            Link = link;
            Points = new List<PiShockPoint>();
        }

        public struct LinkData
        {
            public string ShareCode;
            public int DeviceId;
        }
    }
}
