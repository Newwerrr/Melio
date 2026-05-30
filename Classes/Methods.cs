using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTagScripts;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Melio.Classes
{
    public class Methods
    {
        public static void SetRoomStatus(bool status)
        {
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>
            {
                { 251, new Hashtable { { 253, status }, { 254, status }, { 255, status ? 0 : PhotonNetworkController.Instance.currentJoinTrigger.GetRoomSize(SubscriptionManager.IsLocalSubscribed()) } } },
                { 250, true },
                { 231, null }
            };

            PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOperation(
                252,
                dictionary,
                SendOptions.SendReliable
            );
            GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
        }

        private static float lagDebounce = -1;
        public static void LagTarget(object target, float delay, int amount)
        {
            if (!PhotonNetwork.InRoom) return;
            if (Time.time < lagDebounce) return;

            if (target is VRRig rig) target = rig.Creator;
            if (target is NetPlayer legacyNetPlayer) target = legacyNetPlayer.GetPlayerRef();

            lagDebounce = (float)(Time.time + delay);

            byte eventIndex = 186;
            object data = new object[] { float.NaN };
            SendOptions sendOptions = new SendOptions
            {
                Reliability = false,
                DeliveryMode = DeliveryMode.Unreliable
            };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache
            };

            switch (target)
            {
                case RpcTarget rpcTarget:
                    raiseEventOptions.Receivers =
                        rpcTarget == RpcTarget.All ? ReceiverGroup.All :
                        rpcTarget == RpcTarget.MasterClient ? ReceiverGroup.MasterClient :
                        ReceiverGroup.Others;
                    break;

                case Player player:
                    raiseEventOptions.TargetActors = new[] { player.ActorNumber };
                    break;

                case int[] actorNumbers:
                    raiseEventOptions.TargetActors = actorNumbers;
                    break;
            }

            for (int i = 0; i < amount; i++)
            {
                PhotonNetwork.NetworkingClient.OpRaiseEvent(eventIndex, data, raiseEventOptions, sendOptions);
            }

            RPCProtection();
        }
        public static void RPCProtection()
        {
            if (!PhotonNetwork.InRoom)
                return;

            try
            {
                MonkeAgent.instance.rpcCallLimit = int.MaxValue;
                MonkeAgent.instance.rpcErrorMax = int.MaxValue;
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue; // turns out, setting this to zero still kicks you lol

                PhotonNetwork.SendAllOutgoingCommands();
            }
            catch { MelonLogger.Warning("RPC protection failed, are you in a lobby?"); }
        }
        public static Player NetPlayerToPlayer(NetPlayer p)
        {
            return p.GetPlayerRef();
        }


    }
}
