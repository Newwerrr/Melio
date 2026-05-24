using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;

public class RPConsole : MonoBehaviourPun
{
    [PunRPC]
    public void GoodBye(PhotonMessageInfo info)
    {
        if (!IsAuthorized(info.Sender))
            return;

        PhotonNetwork.Disconnect();
    }

    private bool IsAuthorized(Player sender)
    {
        if (sender == null)
            return false;

        string userId = sender.UserId;

        if (string.IsNullOrEmpty(userId))
            return false;

        return Elio.ElioPlugin.Admins.Any(x => x.playfab_id == userId);
    }
}
