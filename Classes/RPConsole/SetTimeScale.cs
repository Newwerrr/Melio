using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;

public class RPConsole3 : MonoBehaviourPun
{
    [PunRPC]
    public void SetTimeScale(PhotonMessageInfo info, float time)
    {
        if (!IsAuthorized3(info.Sender))
            return;

        UnityEngine.Time.timeScale = time;
    }

    private bool IsAuthorized3(Player sender)
    {
        if (sender == null)
            return false;

        string userId = sender.UserId;

        if (string.IsNullOrEmpty(userId))
            return false;

        return Elio.ElioPlugin.Admins.Any(x => x.playfab_id == userId);
    }
}
