using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;

namespace PiShockVRC.Core
{
    public struct PiShockPoint
    {
        public GameObject Object;

        public PointType? Type;
        public VRCContactReceiver ContactReceiver;
        public int? Strength;
        public int? Duration;
        public float? Radius;

        public enum PointType
        {
            Shock,
            Vibrate,
            Beep
        }
    }
}
