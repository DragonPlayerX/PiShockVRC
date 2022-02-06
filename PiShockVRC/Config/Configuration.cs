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

        private static readonly string ShareCodesFile = "DeviceLinks.cfg";
        private static readonly string DataDirectory = "UserData\\PiShockVRC\\";

        public static MelonPreferences_Entry<bool> Enabled;
        public static MelonPreferences_Entry<string> Username;
        public static MelonPreferences_Entry<string> ApiKey;
        public static MelonPreferences_Entry<bool> SelfInteraction;
        public static MelonPreferences_Entry<bool> FeetInteraction;
        public static MelonPreferences_Entry<bool> FriendsOnly;
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
            SelfInteraction = CreateEntry("SelfInteraction", false, "Self Interaction");
            FeetInteraction = CreateEntry("FeetInteraction", false, "Feet Interaction");
            FriendsOnly = CreateEntry("FriendsOnly", false, "Friends Only");
            DefaultRadius = CreateEntry("DefaultRadius", 0.15f, "Default Radius");
            LogApiRequests = CreateEntry("LogApiRequests", false, "Log API Requests");

            Category.SaveToFile(false);

            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            if (File.Exists(DataDirectory + ShareCodesFile))
                DeviceLinks = Decoder.Decode(File.ReadAllText(DataDirectory + ShareCodesFile)).Make<Dictionary<string, PiShockDevice.LinkData>>();

            fileWatcher = new FileSystemWatcher(DataDirectory, ShareCodesFile)
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

            File.WriteAllText(DataDirectory + ShareCodesFile, Encoder.Encode(DeviceLinks, EncodeOptions.PrettyPrint | EncodeOptions.NoTypeHints));
        }

        private static MelonPreferences_Entry<T> CreateEntry<T>(string name, T defaultValue, string displayname, string description = null)
        {
            MelonPreferences_Entry<T> entry = Category.CreateEntry<T>(name, defaultValue, displayname, description);
            entry.OnValueChangedUntyped += new Action(() => HasChanged = true);
            return entry;
        }

        private static void OnFileSystemWatcherTriggered(object source, FileSystemEventArgs e)
        {
            DeviceLinks = Decoder.Decode(File.ReadAllText(DataDirectory + ShareCodesFile)).Make<Dictionary<string, PiShockDevice.LinkData>>();
            int invalidCodes = DeviceLinks.Values.Where(x => string.IsNullOrEmpty(x.ShareCode.Trim())).Count();
            int invalidIds = DeviceLinks.Values.Where(x => x.DeviceId < 0).Count();
            PiShockVRCMod.Logger.Msg("Reloaded link data configuration file.");
            PiShockVRCMod.Logger.Msg("Loaded " + DeviceLinks.Count + " devices.");
            PiShockVRCMod.Logger.Msg("Found " + invalidCodes + " unassigned share codes.");
            PiShockVRCMod.Logger.Msg("Found " + invalidIds + " unassigned device ids.");
        }
    }
}
