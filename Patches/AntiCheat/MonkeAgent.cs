using HarmonyLib;
using UnityEngine;


namespace Melio.Patches.AntiCheat
{
    
    public class AntiCheatPatches
    {
        public static bool DisablePatches = false;

        [HarmonyPatch(typeof(MonkeAgent), "SendReport")]
        public class SendReport
        {
            private static bool Prefix(string susReason, string susId, string susNick) 
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "CloseInvalidRoom")]
        public class CloseInvalidRoom
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "CheckReports")]
        public class CheckReports
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "DispatchReport")]
        public class DispatchReport
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "GetRPCCallTracker")]
        internal class GetRPCCallTracker
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "LogErrorCount")]
        public class LogErrorCount
        {
            private static bool Prefix(string logString, string stackTrace, LogType type)
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "QuitDelay", MethodType.Enumerator)]
        public class QuitDelay
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
        public class QuitOnBan
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "ShouldDisconnectFromRoom")]
        public class ShouldDisconnectFromRoom
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
        public class GracePeriodPatch1
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
        public class GracePeriodPatch2
        {
            private static bool Prefix()
            {
                return DisablePatches;
            }
        }
    }
}