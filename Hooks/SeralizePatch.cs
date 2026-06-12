using HarmonyLib;
using Photon.Pun;
using System.Reflection;

namespace Melio.Hooks.SerializePatch
{
    public static class Hooks
    {
        public static MethodInfo RunViewUpdate =
            AccessTools.Method(
                typeof(PhotonNetwork),
                "RunViewUpdate"
            );
    }
}