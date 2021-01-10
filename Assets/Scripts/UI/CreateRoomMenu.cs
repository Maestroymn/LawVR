using System.Collections.Generic;
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
            RoomOptions _roomOptions = new RoomOptions();
            //TypedLobby typedLobby = new TypedLobby("competitive",);
            _roomOptions.MaxPlayers = 16;
            _roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            if (_password.text.Length != 0)
            {
                _roomOptions.CustomRoomProperties.Add("password",_password.text);
            }
            PhotonNetwork.CreateRoom(_roomName.text, _roomOptions, TypedLobby.Default);
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log("Room is created its name is "+_roomName.text+" and its password: "+_password.text);
            _roomsCanvases.CurrentRoomCanvas.Show(_roomName.text);
            PhotonNetwork.JoinLobby();
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room couldn't be created ",_roomName);
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomList.ForEach(room =>
            {
                // Removed from rooms list.
                if (room.RemovedFromList)
                {
                    int index = RoomListingsMenu.RoomListings.FindIndex(x => x.RoomInfo.Name == room.Name);
                    if (index != -1)
                    {
                        Destroy(RoomListingsMenu.RoomListings[index].gameObject);
                        RoomListingsMenu.RoomListings.RemoveAt(index);
                    }
                }
                // Added to rooms list.
                else
                {
                    RoomListingsMenu.AddRoomListing(room);
                }
            });
        }
    }
}
