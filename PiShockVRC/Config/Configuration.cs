using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MelonLoader;
using MelonLoader.Preferences;
using MelonLoader.TinyJSON;

using PiShockVRC.Core;

namespace PiShockVRC.Config
{
    public static class Configuration
    {
        private static readonly MelonPreferences_Category Category = MelonPreferences.CreateCategory("PiShockVRC", "PiShockVRC");

        private static readonly string DeviceLinksFile = "DeviceLinks.cfg";
        private static readonly string DataDirectory = "UserData\\PiShockVRC\\";

        public static MelonPreferences_Entry<bool> Enabled;
        public static MelonPreferences_Entry<string> Username;
        public static MelonPreferences_Entry<string> ApiKey;
        public static MelonPreferences_Entry<bool> UseLocalServer;
        public static MelonPreferences_Entry<string> LocalAddress;
        public static MelonPreferences_Entry<int> LocalPiShockId;
        public static MelonPreferences_Entry<bool> SelfInteraction;
        public static MelonPreferences_Entry<bool> FeetInteraction;
        public static MelonPreferences_Entry<bool> FriendsOnly;
        public static MelonPreferences_Entry<bool> UseAvatarParameters;
        public static MelonPreferences_Entry<bool> UseAvatarDynamics;
        public static MelonPreferences_Entry<string> DefaultType;
        public static MelonPreferences_Entry<int> DefaultStrength;
        public static MelonPreferences_Entry<int> DefaultDuration;
        public static MelonPreferences_Entry<float> DefaultRadius;
        public static MelonPreferences_Entry<bool> LogApiRequests;
        public static MelonPreferences_Entry<int> UpdateRate;

        public static Dictionary<string, PiShockDevice.LinkData> DeviceLinks = new Dictionary<string, PiShockDevice.LinkData>();
        public static PiShockPoint.PointType ParsedDefaultType;
        public static bool HasChanged;

        private static FileSystemWatcher fileWatcher;

        public static void Init()
        {
            Enabled = CreateEntry("Enabled", true, "Enabled");
            Username = CreateEntry("Username", "name", "Username");
            ApiKey = CreateEntry("ApiKey", "key", "ApiKey");
            UseLocalServer = CreateEntry("UseLocalServer", false, "Use Local Server");
            LocalAddress = CreateEntry("LocalAddress", "127.0.0.1", "Local Address");
            LocalPiShockId = CreateEntry("LocalPiShockId", -1, "Local PiShock ID");
            SelfInteraction = CreateEntry("SelfInteraction", false, "Self Interaction");
            FeetInteraction = CreateEntry("FeetInteraction", false, "Feet Interaction");
            FriendsOnly = CreateEntry("FriendsOnly", false, "Friends Only");
            UseAvatarParameters = CreateEntry("UseAvatarParameters", false, "Use Avatar Parameters");
            UseAvatarDynamics = CreateEntry("UseAvatarDynamics", false, "Use Avatar Dynamics");
            DefaultType = CreateEntry("DefaultType", nameof(PiShockPoint.PointType.Shock), "Default Type");
            DefaultStrength = CreateEntry("DefaultStrength", 25, "Default Strength");
            DefaultDuration = CreateEntry("DefaultDuration", 1, "Default Duration");
            DefaultRadius = CreateEntry("DefaultRadius", 0.25f, "Default Radius");
            LogApiRequests = CreateEntry("LogApiRequests", false, "Log API Requests");
            UpdateRate = CreateEntry("UpdateRate", 10, "Update Rate (per second)", valueValidator: new IntegerValidator(10, 1, 60));

            Action<string> parseTypeAction = new Action<string>(value =>
            {
                if (Enum.TryParse(value, true, out PiShockPoint.PointType alignment))
                    ParsedDefaultType = alignment;
            });

            DefaultType.OnValueChanged += new Action<string, string>((oldValue, newValue) => parseTypeAction.Invoke(newValue));
            parseTypeAction.Invoke(DefaultType.Value);

            if (MelonHandler.Mods.Any(mod => mod.Info.Name.Equals("UI Expansion Kit")))
                UIXIntegration.InitUIX();

            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            if (File.Exists(DataDirectory + DeviceLinksFile))
                DeviceLinks = Decoder.Decode(File.ReadAllText(DataDirectory + DeviceLinksFile)).Make<Dictionary<string, PiShockDevice.LinkData>>();

            fileWatcher = new FileSystemWatcher(DataDirectory, DeviceLinksFile)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            fileWatcher.Created += OnFileSystemWatcherTriggered;
            fileWatcher.Changed += OnFileSystemWatcherTriggered;
            fileWatcher.BeginInit();
        }

        public static void Save()
        {
            if (RoomManager.field_Internal_Static_ApiWorldInstance_0 == null) return;
            if (HasChanged)
            {
                MelonPreferences.Save();
                HasChanged = false;
            }

            File.WriteAllText(DataDirectory + DeviceLinksFile, Encoder.Encode(DeviceLinks, EncodeOptions.PrettyPrint | EncodeOptions.NoTypeHints));
        }

        private static MelonPreferences_Entry<T> CreateEntry<T>(string name, T defaultValue, string displayname, string description = null, ValueValidator valueValidator = null)
        {
            MelonPreferences_Entry<T> entry = Category.CreateEntry<T>(name, defaultValue, displayname, description, validator: valueValidator);
            entry.OnValueChangedUntyped += new Action(() => HasChanged = true);
            return entry;
        }

        private static void OnFileSystemWatcherTriggered(object source, FileSystemEventArgs e)
        {
            DeviceLinks = Decoder.Decode(File.ReadAllText(DataDirectory + DeviceLinksFile)).Make<Dictionary<string, PiShockDevice.LinkData>>();
            int invalidCodes = DeviceLinks.Values.Where(x => string.IsNullOrEmpty(x.ShareCode.Trim())).Count();
            int invalidIds = DeviceLinks.Values.Where(x => x.DeviceId < 0).Count();
            PiShockVRCMod.Logger.Msg("Reloaded link data configuration file.");
            PiShockVRCMod.Logger.Msg("Loaded " + DeviceLinks.Count + " devices.");
            PiShockVRCMod.Logger.Msg("Found " + invalidCodes + " unassigned share codes.");
            PiShockVRCMod.Logger.Msg("Found " + invalidIds + " unassigned device ids.");
        }

        private class IntegerValidator : ValueValidator
        {
            public int DefaultValue;
            public int MinValue;
            public int MaxValue;

            public IntegerValidator(int defaultValue, int minValue, int maxValue)
            {
                DefaultValue = defaultValue;
                MinValue = minValue;
                MaxValue = maxValue;
            }

            public override object EnsureValid(object value)
            {
                if (IsValid(value))
                    return value;
                else
                    return DefaultValue;
            }

            public override bool IsValid(object value)
            {
                int v = Convert.ToInt32(value);
                return v >= MinValue && v <= MaxValue;
            }
        }

        internal class UIXIntegration
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void InitUIX()
            {
                UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum(Category.Identifier, DefaultType.Identifier, new List<(string, string)>()
                {
                    (nameof(PiShockPoint.PointType.Shock), "Shock"),
                    (nameof(PiShockPoint.PointType.Vibrate), "Vibrate"),
                    (nameof(PiShockPoint.PointType.Beep), "Beep")
                });
            }
        }
    }
}
