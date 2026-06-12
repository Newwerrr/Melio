using ExitGames.Client.Photon.StructWrapping;
using HarmonyLib;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace Melio.Patches.AntiCheat
{
    public class IncrementRPCPatches
    {
        private static readonly HashSet<string> ignoredRPCs = new HashSet<string>
        {
            "OnHandTapRPCShared",
            "PlayHandTapShared",
            "RPC_UpdateCosmeticsWithTryon",
            "RequestCosmetics"
        };

        private static readonly Dictionary<int, string> nameCache = new Dictionary<int, string>();

        private static bool TryResolvePlayerName(PhotonMessageInfo info, out string name)
        {
            name = null;

            if (info.Sender == null)
                return false;

            int id = info.Sender.ActorNumber;

            if (nameCache.TryGetValue(id, out name))
                return true;

            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == id)
                {
                    name = p.NickName;
                    nameCache[id] = name;
                    return true;
                }
            }

            return false;
        }

        [HarmonyPatch(typeof(MonkeAgent), "IncrementRPCCall", typeof(PhotonMessageInfo), typeof(string))]
        public class IncrementRPCCall
        {
            private static bool Prefix(PhotonMessageInfo info, string callingMethod = "")
            {
                if (!MelioMod.LogRPCs)
                    return false;

                if (ignoredRPCs.Contains(callingMethod))
                    return false;

                if (TryResolvePlayerName(info, out var name))
                {
                    MelonLogger.Msg($"-RPCLogger- {name} sent RPC: {callingMethod}");
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(VRRig), "IncrementRPC", typeof(PhotonMessageInfoWrapped), typeof(string))]
        public class IncrementRPC
        {
            private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall)
            {
                if (!MelioMod.LogRPCs)
                    return false;

                if (ignoredRPCs.Contains(sourceCall))
                    return false;

                try
                {
                    PhotonMessageInfo raw = StructWrapperUtility.Unwrap<PhotonMessageInfo>(info);

                    if (TryResolvePlayerName(raw, out var name))
                    {
                        MelonLogger.Msg($"-RPCLogger- {name} sent RPC: {sourceCall}");
                    }
                }
                catch { }

                return false;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "IncrementRPCCallLocal")]
        public class IncrementRPCCallLocal
        {
            private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
            {
                if (!MelioMod.LogRPCs)
                    return false;

                if (ignoredRPCs.Contains(rpcFunction))
                    return false;

                try
                {
                    PhotonMessageInfo raw = StructWrapperUtility.Unwrap<PhotonMessageInfo>(infoWrapped);

                    if (TryResolvePlayerName(raw, out var name))
                    {
                        MelonLogger.Msg($"-RPCLogger- {name} sent RPC: {rpcFunction}");
                    }
                }
                catch { }

                return false;
            }
        }
    }
}