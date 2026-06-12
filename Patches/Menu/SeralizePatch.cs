using HarmonyLib;
using MelonLoader;
using Melio.Hooks;
using Photon.Pun;
using System;
using System.Reflection;

namespace Melio.Patches.Menu
{
    [HarmonyPatch]
    public class SerializePatch
    {
        public static event Action OnSerialize;
        public static Func<bool> OverrideSerialization;

        static MethodBase TargetMethod()
        {
            return Hooks.SerializePatch.Hooks.RunViewUpdate;
        }

        static bool Prefix()
        {
            if (!PhotonNetwork.InRoom)
                return true;

            try
            {
                OnSerialize?.Invoke();
            }
            catch (Exception e)
            {
                MelonLogger.Error(
                    $"Error in SerializePatch.OnSerialize: {e}"
                );
            }

            if (OverrideSerialization == null)
                return true;

            try
            {
                return OverrideSerialization();
            }
            catch (Exception e)
            {
                MelonLogger.Error(
                    $"Error in SerializePatch.OverrideSerialization: {e}"
                );
                return false;
            }
        }
    }
}