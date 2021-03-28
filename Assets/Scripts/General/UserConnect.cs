using Managers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace General
{
    public class UserConnect : MonoBehaviourPunCallbacks
    {
        public ExitGames.Client.Photon.Hashtable PlayerProperties = new ExitGames.Client.Photon.Hashtable();

        private void Start()
        {
            if(!PhotonNetwork.IsConnected)
            {
                print("Connecting to server.");
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = GameManager.GameSettings.GameVersion;
                PhotonNetwork.NickName = GameManager.GameSettings.NickName;
                PlayerProperties["Role"] = "none";
                PhotonNetwork.SetPlayerCustomProperties(PlayerProperties);
                PhotonNetwork.ConnectUsingSettings();
                if (PhotonNetwork.InRoom)
                    PhotonNetwork.LeaveRoom();
            }
        }

        #region PhotonCallbacks

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

        #endregion

    }
}
