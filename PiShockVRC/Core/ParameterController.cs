using System;
using System.Linq;
using System.Reflection;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib.XrefScans;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace PiShockVRC.Core
{
    public static class ParameterController
    {
        private static AvatarPlayableController PlayableController => VRCPlayer.field_Internal_Static_VRCPlayer_0?.field_Private_VRCAvatarManager_0?.field_Private_AvatarPlayableController_0;
        private static MethodInfo SetParameterMethod;

        public static void Init()
        {
            try
            {
                SetParameterMethod = typeof(AvatarPlayableController).GetMethods().First(method => method.Name.StartsWith("Method_Private_Boolean_Int32_Single")
                    && XrefScanner.XrefScan(method).Any(instance => instance.Type == XrefType.Method && instance.TryResolve()?.DeclaringType == typeof(AvatarPlayableController)));
            }
            catch (Exception)
            {
                PiShockVRCMod.Logger.Error("Unable to find the SetParameter method. Parameter support is now disabled.");
            }
#if DEBUG
            PiShockVRCMod.Logger.Msg("SetParameter Method: " + SetParameterMethod.Name);
#endif
        }

        public static bool SetParameter<T>(string parameterName, T value)
        {
            if (RoomManager.field_Internal_Static_ApiWorldInstance_0 == null || PlayableController == null)
                return false;

            int? parameterIndex = GetParameterIndex(parameterName);
            if (parameterIndex == null)
                return false;

            SetParameterMethod?.Invoke(PlayableController, new object[] { parameterIndex, Convert.ChangeType(value, typeof(float)) });
            return true;
        }

        private static int? GetParameterIndex(string parameterName)
        {
            Il2CppReferenceArray<VRCExpressionParameters.Parameter> parameters = PlayableController?.field_Private_VRCAvatarDescriptor_0?.expressionParameters?.parameters;
            if (parameters == null)
                return null;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].name.Equals(parameterName))
                    return i;
            }
            return null;
        }
    }
}