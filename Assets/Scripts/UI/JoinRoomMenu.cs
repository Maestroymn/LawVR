﻿using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace UI
{
    public class JoinRoomMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _roomName, _password;

        private RoomsCanvases _roomsCanvases;   
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
        }
        
        public void OnClickJoinPrivateRoom()
        {
            RoomListing room  = _roomsCanvases.UIManager.RoomListings.FirstOrDefault(r => r.RoomInfo.Name == _roomName.text);
            bool exists = (room != null);
            if (exists && room.RoomInfo.CustomProperties["password"].ToString()==_password.text)
            {
                PhotonNetwork.JoinRoom(_roomName.text);
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined to "+_roomName.text);
            _roomsCanvases.JoinRoomCanvas.Hide();
            _roomsCanvases.CurrentRoomCanvas.Show(_roomName.text,false);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Couldn't joined to "+_roomName.text+"\n"+message);
        }
    }
}
