using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        print("Connecting to server.");
        PhotonNetwork.GameVersion = GameManager.GameSettings.GameVersion;
        PhotonNetwork.NickName = GameManager.GameSettings.NickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName+" connected to server.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server: "+cause);
    }
}
