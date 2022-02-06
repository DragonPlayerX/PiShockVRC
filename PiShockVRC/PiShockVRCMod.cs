using System.Collections;
using MelonLoader;

using PiShockVRC;
using PiShockVRC.Core;
using PiShockVRC.Config;

[assembly: MelonInfo(typeof(PiShockVRCMod), "PiShockVRC", "1.0.0", "DragonPlayer", "https://github.com/DragonPlayerX/PiShockVRC")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace PiShockVRC
{
    public class PiShockVRCMod : MelonMod
    {
        public static readonly string Version = "1.0.0";

        public static PiShockVRCMod Instance;
        public static MelonLogger.Instance Logger => Instance.LoggerInstance;

        public override void OnApplicationStart()
        {
            Instance = this;
            MelonLogger.Msg("Initializing PiShockVRC " + Version + "...");

            Configuration.Init();

            MelonCoroutines.Start(Init());
        }

        public override void OnUpdate()
        {
            AvatarManager.Update();
        }

        private IEnumerator Init()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return null;

            NetworkEvents.Init();
            AvatarManager.Init();

            MelonLogger.Msg("Running version " + Version + " of PiShockVRC.");
        }
    }
}
