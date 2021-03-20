using Managers;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

namespace UI
{
    public class CreateRoomMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _roomName, _password;
        [SerializeField] private RoomListingsMenu _roomListingsMenu;
        public RoomListingsMenu RoomListingsMenu { get; private set; }
        private RoomsCanvases _roomsCanvases;
        private bool _joinable;
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
            RoomListingsMenu = _roomListingsMenu;
        }
        public void OnClickCreateRoom()
        {
            if (!PhotonNetwork.IsConnected)
                return;
            RoomOptions _roomOptions = new RoomOptions();
            //TypedLobby typedLobby = new TypedLobby("competitive",);
            _roomOptions.MaxPlayers = 16;
            _roomOptions.IsVisible = true;
            _roomOptions.BroadcastPropsChangeToAll = true;
            _roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            if (_password.text.Length != 0)
            {
                _roomOptions.CustomRoomProperties.Add("password",_password.text);
            }
            PhotonNetwork.CreateRoom(_roomName.text, _roomOptions, TypedLobby.Default);
            //PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("katıldım");
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("lobi katıldım");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room is created its name is "+_roomName.text+" and its password: "+_password.text);
           // PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby,null);
           _roomsCanvases.CurrentRoomCanvas.Show(_roomName.text,true);
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room couldn't be created ",_roomName);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            if (PhotonNetwork.IsMasterClient)
            {
                _roomListingsMenu.RemoveRoomListing(PhotonNetwork.CurrentRoom);
            }
        }
    }
}
