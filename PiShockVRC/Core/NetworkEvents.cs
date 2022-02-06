using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using VRC.Core;

namespace PiShockVRC.Core
{
    public static class NetworkEvents
    {
        public static event Action<VRCAvatarManager, ApiAvatar, GameObject> OnAvatarInstantiated;

        private static void OnAvatarInstantiate(VRCAvatarManager manager, ApiAvatar apiAvatar, GameObject avatar)
        {
            if (manager == null || apiAvatar == null || avatar == null)
                return;

            try
            {
                OnAvatarInstantiated?.Invoke(manager, apiAvatar, avatar);
            }
            catch (Exception e)
            {
                PiShockVRCMod.Logger.Error("Error while invoking OnAvatarInstantiated: " + e.ToString());
            }
        }

        public static void Init()
        {
            PiShockVRCMod.Instance.HarmonyInstance.Patch(typeof(VRCPlayer).GetMethods().First(method => method.Name.StartsWith("Awake")), postfix: new HarmonyMethod(typeof(NetworkEvents).GetMethod(nameof(OnPlayerAwake), BindingFlags.NonPublic | BindingFlags.Static)));
        }

        private static void OnPlayerAwake(VRCPlayer __instance)
        {
            __instance.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() => OnAvatarInstantiate(__instance.prop_VRCAvatarManager_0, __instance.field_Private_ApiAvatar_0, __instance.field_Internal_GameObject_0)));
        }
    }
}
