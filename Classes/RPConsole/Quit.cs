using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;

public class RPConsole1 : MonoBehaviourPun
{
    [PunRPC]
    public void Quit(PhotonMessageInfo info)
    {
        if (!IsAuthorized1(info.Sender))
            return;

        Application.Quit();
    }

    private bool IsAuthorized1(Player sender)
    {
        if (sender == null)
            return false;

        string userId = sender.UserId;

        if (string.IsNullOrEmpty(userId))
            return false;

        return Elio.ElioPlugin.Admins.Any(x => x.playfab_id == userId);
    }
}
