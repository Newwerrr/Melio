using HarmonyLib;
using UnityEngine;

namespace Melio.Patches.AntiCheat
{
    public class AntiCheatPatches
    {
        [HarmonyPatch(typeof(MonkeAgent), "SendReport")]
        public class SendReportPatch
        {
            private static bool Prefix(string susReason, string susId, string susNick) => false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "CloseInvalidRoom")]
        public class CloseInvalidRoom
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "CheckReports")]
        public class CheckReports
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "DispatchReport")]
        public class DispatchReport
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "GetRPCCallTracker")]
        internal class GetRPCCallTracker
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "LogErrorCount")]
        public class LogErrorCount
        {
            private static bool Prefix(string logString, string stackTrace, LogType type) =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "QuitDelay", MethodType.Enumerator)]
        public class QuitDelay
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
        public class QuitOnBan
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "ShouldDisconnectFromRoom")]
        public class ShouldDisconnectFromRoom
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
        public class GracePeriodPatch1
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
        public class GracePeriodPatch2
        {
            private static bool Prefix() =>
                false;
        }
    }
}