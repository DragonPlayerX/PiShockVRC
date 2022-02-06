using UnityEngine;

namespace PiShockVRC.Core
{
    public struct PiShockPoint
    {
        public GameObject Object;
        public PointType Type;
        public int Strength;
        public int Duration;
        public float Radius;

        public enum PointType
        {
            Shock,
            Vibrate,
            Beep
        }
    }
}
