using HarmonyLib;
using JetBrains.Annotations;

namespace Melio.Patches.AntiCheat
{
    public class TelemetryPatches
    {
        public static bool enabled = true;

        [HarmonyPatch(typeof(GorillaTelemetry), "EnqueueTelemetryEvent")]
        public class TelemetryPatch
        {
#pragma warning disable CS8625 
            private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null) =>
                !enabled;
#pragma warning restore CS8625 
        }
        
    }
}