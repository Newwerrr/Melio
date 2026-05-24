using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;

public class RPConsole2 : MonoBehaviourPun
{
    [PunRPC]
    public void BreakPhoton(PhotonMessageInfo info)
    {
        if (!IsAuthorized2(info.Sender))
            return;

        PhotonNetwork.IsMessageQueueRunning = false;
    }

    private bool IsAuthorized2(Player sender)
    {
        if (sender == null)
            return false;

        string userId = sender.UserId;

        if (string.IsNullOrEmpty(userId))
            return false;

        return Elio.ElioPlugin.Admins.Any(x => x.playfab_id == userId);
    }
}
