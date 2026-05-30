using HarmonyLib;
using Photon.Pun;
using MelonLoader;

namespace Melio.Patches.AntiCheat
{
    [HarmonyPatch(typeof(PhotonNetwork), nameof(PhotonNetwork.SetMasterClient))]
    public static class SetMasterPatch
    {
        private static bool Prefix()
        {
            
            MelonLogger.Warning(
                "-[PhotonPatch]- You made an attempt to change master client, this has been blocked for your safety." // it sends twice, idk why.
            );
            return false;
        }
    }
}