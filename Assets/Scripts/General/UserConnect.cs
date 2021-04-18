using DatabaseScripts;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using UI.LoginScripts;
using UnityEngine;

namespace General
{
    public class UserConnect : MonoBehaviourPunCallbacks
    {
        [SerializeField] private LoginUIManager _loginUIManager;
        public ExitGames.Client.Photon.Hashtable PlayerProperties = new ExitGames.Client.Photon.Hashtable();
        
        public void ConnectPhoton()
        {
            if(!PhotonNetwork.IsConnected)
            {
                print("Connecting to server.");
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.AuthValues = new AuthenticationValues(GameManager.GameSettings.UserID);
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
            _loginUIManager=FindObjectOfType<LoginUIManager>();
            if(_loginUIManager)
                _loginUIManager.OnConnected();
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            print("Disconnected from server: "+cause);
            DatabaseConnection.SetUserOffline(GameManager.GameSettings.NickName);
        }
    
        public override void OnJoinedLobby()
        {
            Debug.Log("lobi katıldım (MAIN)");
        }

        #endregion

    }
}
