using System;
using Photon.Pun;
using UnityEngine;

namespace UI
{
    public class RoomsCanvases : MonoBehaviour
    {
        [SerializeField] private HostRoomCanvas _hostRoomCanvas;
        public HostRoomCanvas HostRoomCanvas => _hostRoomCanvas;
        
        [SerializeField] private CurrentRoomCanvas _currentRoomCanvas;
        public CurrentRoomCanvas CurrentRoomCanvas => _currentRoomCanvas;

        private void Awake()
        {
            FirstInitialize();
        }

        private void FirstInitialize()
        {
            HostRoomCanvas.FirstInitialize(this);
            CurrentRoomCanvas.FirstInitialize(this);
        }
        
        private void Update()
        {
            print("Total Rooms: "+PhotonNetwork.CountOfRooms);
        }
        
    }
}
