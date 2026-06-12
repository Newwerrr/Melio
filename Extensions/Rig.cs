using Melio.Hooks;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Melio.Extensions
{
    public class Rig
    {
        public static VRRig GetVRRigFromPlayer(NetPlayer player) 
        {
            return GorillaGameManager.StaticFindRigForPlayer(player);
        }
        public static Player NetPlayerToPlayer(NetPlayer p)
        {
            return p.GetPlayerRef();
        }
        public static NetPlayer GetPlayerFromID(string id) 
        {
            return PhotonNetwork.PlayerList.FirstOrDefault(player => player.UserId == id);
        }
        public static NetworkView GetNetworkViewFromVRRig(VRRig rig)
        {
            return (NetworkView)VRRigHooks.GetNetView(rig);
        }

        public static object FindPlayerFromVRRig(VRRig rig)
        {
            if (rig == null)
                return null;


            object serializer = VRRigHooks.GetRigSerializer(rig);

            if (serializer == null)
            {
                MelonLogger.Error("Serializer not found.");
                return null;
            }


            FieldInfo playerField = serializer.GetType().GetField(
                "player",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (playerField != null)
            {
                object player = playerField.GetValue(serializer);


                return player;
            }


            return null;
        }
    } 
}