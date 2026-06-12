using ExitGames.Client.Photon;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTagScripts;
using Melio.Hooks;
using Melio.Patches.Menu;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace Melio.Classes
{
    public class Methods
    {
        private static MethodInfo OnSerializeWriteMethod;
        private static FieldInfo BlockedSendingGroupsField;
        private static FieldInfo CurrentLevelPrefixField;
        private static FieldInfo PhotonViewListField;
        private static FieldInfo SerializeRaiseEvOptionsField;
        private static FieldInfo SerializeViewBatchesField;
        private static PropertyInfo MixedModeIsReliableProperty;
        public static System.Single RopeDelay = 0f;


        static Methods()
        {
            Type punType = typeof(PhotonNetwork);
            OnSerializeWriteMethod = punType.GetMethod("OnSerializeWrite", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            BlockedSendingGroupsField = punType.GetField("blockedSendingGroups", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            CurrentLevelPrefixField = punType.GetField("currentLevelPrefix", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            PhotonViewListField = punType.GetField("photonViewList", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            SerializeRaiseEvOptionsField = punType.GetField("serializeRaiseEvOptions", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            SerializeViewBatchesField = punType.GetField("serializeViewBatches", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            MixedModeIsReliableProperty = typeof(PhotonView).GetProperty("mixedModeIsReliable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

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
        public static void BetaStrongerLagTarget(object target, float delay, int amount)
        {
            if (!PhotonNetwork.InRoom) return;
            if (Time.time < lagDebounce) return;

            if (target is VRRig rig) target = rig.Creator;
            if (target is NetPlayer legacyNetPlayer) target = legacyNetPlayer.GetPlayerRef();

            lagDebounce = (float)(Time.time + delay);

            byte eventIndex = 186;
            object data = new object[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity, null, null, null, int.MaxValue, int.MinValue, float.NaN, float.PositiveInfinity, float.NegativeInfinity, null, null, null, int.MaxValue, int.MinValue };
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


        public static void LocalFlingAllRopes()
        {
            foreach (var rope in Hooks.RopeSwingHooks.GetAllRopes())
            {
                RopeSwingManager.instance.SetVelocity(
                    rope.ropeId,
                    -1,
                    new Vector3(0, 100000f, 0),
                    true,
                    default(PhotonMessageInfo)
                );
            }
        }
        public static void LocalFlingRopeByRope(GorillaRopeSwing RopeObject)
        {
            RopeSwingManager.instance.SetVelocity(
                               RopeObject.ropeId,
                               -1,
                               new Vector3(0, 100000f, 0),
                               true,
                               default(PhotonMessageInfo)
                           );
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
                PhotonNetwork.QuickResends = int.MaxValue;
                

                PhotonNetwork.SendAllOutgoingCommands();
            }
            catch { MelonLogger.Warning("RPC protection failed, are you in a lobby?"); }
        }

        public static Player NetPlayerToPlayer(NetPlayer p)
        {
            return p.GetPlayerRef();
        }
        

        public static void InvisForU(Photon.Pun.RpcTarget target)
        {
            SerializePatch.OverrideSerialization = () =>
            {
                MassSerialize(true, new[] { VRRig.LocalRig.gameObject.GetPhotonView() });

                Vector3 positionArchive = VRRig.LocalRig.transform.position;
                int[] invisibleTargets = new int[0];
                int[] visibleTargets = new int[0];

                switch (target)
                {
                    case RpcTarget.MasterClient:
                        invisibleTargets = new[] { PhotonNetwork.MasterClient.ActorNumber };
                        visibleTargets = PhotonNetwork.PlayerList
                            .Where(plr => plr.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
                            .Select(plr => plr.ActorNumber)
                            .ToArray();
                        break;

                    case RpcTarget.Others:
                        invisibleTargets = PhotonNetwork.PlayerList
                            .Where(plr => !plr.IsLocal)
                            .Select(plr => plr.ActorNumber)
                            .ToArray();
                        visibleTargets = new int[0];
                        break;

                    default:
                        return true;
                }

                if (visibleTargets.Length > 0)
                {
                    SendSerialize(VRRig.LocalRig.gameObject.GetPhotonView(), new RaiseEventOptions { TargetActors = visibleTargets });
                }

                VRRig.LocalRig.transform.position = new Vector3(UnityEngine.Random.Range(-99999f, 99999f), 99999f, UnityEngine.Random.Range(-99999f, 99999f));

                if (invisibleTargets.Length > 0)
                {
                    SendSerialize(VRRig.LocalRig.gameObject.GetPhotonView(), new RaiseEventOptions { TargetActors = invisibleTargets });
                }

                RPCProtection();
                VRRig.LocalRig.transform.position = positionArchive;

                return false;
            };
        }

        public static void ResetSerialization()
        {
            SerializePatch.OverrideSerialization = null;
        }
        

        public static void MassSerialize(bool exclude = false, PhotonView[] viewFilter = null, int timeOffset = 0, float delay = 0f)
        {
            if (!PhotonNetwork.InRoom)
                return;

            viewFilter ??= Array.Empty<PhotonView>();

            var photonViewList = PhotonViewListField?.GetValue(null) as IDictionary;
            if (photonViewList == null) return;

            var blockedSendingGroups = BlockedSendingGroupsField?.GetValue(null) as HashSet<byte>;

            List<PhotonView> viewsToSerialize = new List<PhotonView>();
            List<int> filteredViewIDs = viewFilter.Select(view => view.ViewID).ToList();

            foreach (var value in photonViewList.Values)
            {
                PhotonView photonView = value as PhotonView;
                if (photonView == null) continue;

                if (!photonView.IsMine || photonView.Synchronization == ViewSynchronization.Off || !photonView.isActiveAndEnabled || (blockedSendingGroups != null && blockedSendingGroups.Contains(photonView.Group)))
                    continue;

                if (exclude)
                {
                    if (!filteredViewIDs.Contains(photonView.ViewID))
                        viewsToSerialize.Add(photonView);
                }
                else
                {
                    if (filteredViewIDs.Contains(photonView.ViewID))
                        viewsToSerialize.Add(photonView);
                }
            }

            foreach (PhotonView view in viewsToSerialize)
                SendSerialize(view, null, timeOffset, delay);
        }

        public static void SendSerialize(PhotonView pv, RaiseEventOptions options = null, int timeOffset = 0, float delay = 0f)
        {
            if (!PhotonNetwork.InRoom || pv == null)
                return;

            List<object> serializedData = OnSerializeWriteMethod?.Invoke(null, new object[] { pv }) as List<object>;
            if (serializedData == null) return;

            Type batchType = typeof(PhotonNetwork).GetNestedType("RaiseEventBatch", BindingFlags.NonPublic | BindingFlags.Public);
            object raiseEventBatch = Activator.CreateInstance(batchType);

            bool mixedReliable = false;
            if (MixedModeIsReliableProperty != null)
                mixedReliable = (bool)MixedModeIsReliableProperty.GetValue(pv);

            bool reliable = pv.Synchronization == ViewSynchronization.ReliableDeltaCompressed || mixedReliable;
            batchType.GetField("Reliable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(raiseEventBatch, reliable);
            batchType.GetField("Group", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(raiseEventBatch, pv.Group);

            IDictionary dictionary = SerializeViewBatchesField?.GetValue(null) as IDictionary;
            if (dictionary == null) return;

            Type viewBatchType = typeof(PhotonNetwork).GetNestedType("SerializeViewBatch", BindingFlags.NonPublic | BindingFlags.Public);
            object serializeViewBatch = Activator.CreateInstance(viewBatchType, new object[] { raiseEventBatch, 2 });

            if (!dictionary.Contains(raiseEventBatch))
                dictionary[raiseEventBatch] = serializeViewBatch;

            viewBatchType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(serializeViewBatch, new object[] { serializedData });

            RaiseEventOptions sendOptions = SerializeRaiseEvOptionsField?.GetValue(null) as RaiseEventOptions;
            RaiseEventOptions finalOptions = options != null ? new RaiseEventOptions
            {
                CachingOption = sendOptions != null ? sendOptions.CachingOption : EventCaching.DoNotCache,
                Flags = sendOptions != null ? sendOptions.Flags : WebFlags.Default,
                InterestGroup = sendOptions != null ? sendOptions.InterestGroup : (byte)0,
                TargetActors = options.TargetActors,
                Receivers = options.Receivers
            } : sendOptions;

            List<object> objectUpdate = viewBatchType.GetField("ObjectUpdates", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(serializeViewBatch) as List<object>;
            if (objectUpdate == null) return;

            byte currentLevelPrefix = 0;
            if (CurrentLevelPrefixField != null)
                currentLevelPrefix = (byte)CurrentLevelPrefixField.GetValue(null);

            objectUpdate[0] = PhotonNetwork.ServerTimestamp + timeOffset;
            objectUpdate[1] = currentLevelPrefix != 0 ? (object)currentLevelPrefix : null;

            if (delay <= 0f)
            {
                PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206 : 201), objectUpdate, finalOptions,
                    reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
            }
            else
            {
                objectUpdate = new List<object>(objectUpdate);
                CoroutineManager.instance.StartCoroutine(SerializationDelay(() =>
                    PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206 : 201), objectUpdate, finalOptions,
                        reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable), delay));
            }

            viewBatchType.GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(serializeViewBatch, null);
        }

        public static IEnumerator SerializationDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }

    public class CoroutineManager : MonoBehaviour
    {
        public static CoroutineManager instance;

        private void Awake() => instance = this;

        [System.Obsolete]
        public static Coroutine RunCoroutine(IEnumerator enumerator) => instance.StartCoroutine(enumerator);

        [System.Obsolete]
        public static void EndCoroutine(Coroutine enumerator) => instance.StopCoroutine(enumerator);
    }
}