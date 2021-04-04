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
        
        private void Awake()
        {
            if(!PhotonNetwork.IsConnected)
            {
                print("Connecting to server.");
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = GameManager.GameSettings.GameVersion;
                PhotonNetwork.NickName = "";
                PlayerProperties["Role"] = "none";
                PhotonNetwork.SetPlayerCustomProperties(PlayerProperties);
                PhotonNetwork.ConnectUsingSettings();
                if (PhotonNetwork.InRoom)
                    PhotonNetwork.LeaveRoom();
            }
            UserConnect[] objs = FindObjectsOfType<UserConnect>(); 
            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(gameObject);
            GameManager.UserConnect = this;
        }

        #region PhotonCallbacks

        public override void OnConnectedToMaster()
        {
            print(PhotonNetwork.LocalPlayer.NickName+" connected to server.");
            if(_loginUIManager)
                _loginUIManager.OnConnected();
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
