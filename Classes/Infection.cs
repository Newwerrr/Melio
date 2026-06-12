using Melio.Patches.Menu;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using static Melio.Classes.Methods;
using static Melio.Hooks.Hooks;
using Melio.Extensions;
using Newtonsoft.Json.Linq;
using Extensions = Melio.Extensions.Rig;

public class Infection
{
    private static Vector3 archivedpos;

    public static void TagPlayer(NetPlayer player)
    {
        if (PhotonNetwork.IsMasterClient) // i hate the how master client can just do this lol
        {
            AddInfected(player);
        }
        else
        {           
            VRRig target = Extensions.GetVRRigFromPlayer(player);
            Vector3 cachedpos = VRRig.LocalRig.transform.position;
            VRRig.LocalRig.transform.position = target.transform.position;            
            VRRig.LocalRig.transform.position = cachedpos;


        }
    }
}