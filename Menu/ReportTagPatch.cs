using HarmonyLib;
using System.Collections.Generic;

namespace Melio.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaTagManager), nameof(GorillaTagManager.ReportTag))]
    public class ReportTagPatch
    {
        public static readonly List<NetPlayer> blacklistedPlayers = new List<NetPlayer>();
        public static readonly List<NetPlayer> invinciblePlayers = new List<NetPlayer>();

        public static bool Prefix(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
        {
            return !blacklistedPlayers.Contains(taggingPlayer) && !invinciblePlayers.Contains(taggedPlayer);
        }
    }
}   