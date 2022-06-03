using System;
using System.Collections;
using MelonLoader;
using UnityEngine;

using PiShockVRC;
using PiShockVRC.Core;
using PiShockVRC.Config;

[assembly: MelonInfo(typeof(PiShockVRCMod), "PiShockVRC", "1.2.0", "DragonPlayer", "https://github.com/DragonPlayerX/PiShockVRC")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonOptionalDependencies("UIExpansionKit")]

namespace PiShockVRC
{
    public class PiShockVRCMod : MelonMod
    {
        public static readonly string Version = "1.2.0";

        public static PiShockVRCMod Instance;
        public static MelonLogger.Instance Logger => Instance.LoggerInstance;

        public override void OnApplicationStart()
        {
            Instance = this;
            MelonLogger.Msg("Initializing PiShockVRC " + Version + "...");

            Configuration.Init();

            MelonCoroutines.Start(Init());
        }

        public override void OnUpdate() => AvatarManager.Update();

        private IEnumerator Init()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return null;

            NetworkEvents.Init();
            ParameterController.Init();
            AvatarManager.Init();

            MelonLogger.Msg("Running version " + Version + " of PiShockVRC.");
        }

        public static void Run(Action action, float? delay = null) => MelonCoroutines.Start(RunAction(action, delay));

        public static IEnumerator RunAction(Action action, float? delay)
        {
            if (delay != null)
                yield return new WaitForSeconds((float)delay);

            action?.Invoke();
        }
    }
}
