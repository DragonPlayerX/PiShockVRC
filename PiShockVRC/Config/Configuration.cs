using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
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
        public static MelonPreferences_Entry<float> DefaultRadius;
        public static MelonPreferences_Entry<bool> LogApiRequests;

        public static Dictionary<string, PiShockDevice.LinkData> DeviceLinks = new Dictionary<string, PiShockDevice.LinkData>();

        private static FileSystemWatcher fileWatcher;

        public static bool HasChanged;

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
            DefaultRadius = CreateEntry("DefaultRadius", 0.15f, "Default Radius");
            LogApiRequests = CreateEntry("LogApiRequests", false, "Log API Requests");

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

        private static MelonPreferences_Entry<T> CreateEntry<T>(string name, T defaultValue, string displayname, string description = null)
        {
            MelonPreferences_Entry<T> entry = Category.CreateEntry<T>(name, defaultValue, displayname, description);
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
    }
}
