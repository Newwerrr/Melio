using HarmonyLib;
using MelonLoader;
using Photon.Pun;
using System;

namespace Melio.Patches.Menu
{
    [HarmonyPatch(typeof(PhotonNetwork), "RunViewUpdate")]
    public class SerializePatch
    {

        public static event Action OnSerialize;

        public static Func<bool> OverrideSerialization;

        public static bool Prefix()
        {
            if (!PhotonNetwork.InRoom)
                return true;

            try
            {
                OnSerialize?.Invoke();
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error in SerializePatch.OnSerialize: {e}");
            }

            if (OverrideSerialization == null)
                return true;

            try
            {
                return OverrideSerialization();
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error in SerializePatch.OverrideSerialization: {e}");
                return true;
            }
        }
    }
}