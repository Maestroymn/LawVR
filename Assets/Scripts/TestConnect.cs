using System;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    public ExitGames.Client.Photon.Hashtable PlayerProperties = new ExitGames.Client.Photon.Hashtable();
    private void Start()
    {
        print("Connecting to server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = GameManager.GameSettings.GameVersion;
        PhotonNetwork.NickName = GameManager.GameSettings.NickName;
        PlayerProperties["Role"] = "none";
        PhotonNetwork.SetPlayerCustomProperties(PlayerProperties);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName+" connected to server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server: "+cause);
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log("lobi katıldım (MAIN)");
    }

    
}
