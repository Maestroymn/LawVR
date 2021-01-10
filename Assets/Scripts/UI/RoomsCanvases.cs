using System;
using Photon.Pun;
using UnityEngine;

namespace UI
{
    public class RoomsCanvases : MonoBehaviour
    {
        [SerializeField] private HostRoomCanvas _hostRoomCanvas;
        public HostRoomCanvas HostRoomCanvas => _hostRoomCanvas;
        [SerializeField] private JoinRoomCanvas _joinRoomCanvas;
        public JoinRoomCanvas JoinRoomCanvas => _joinRoomCanvas;
        [SerializeField] private JoinPublicRoomsCanvas _joinPublicRoomsCanvas;
        public JoinPublicRoomsCanvas JoinPublicRoomsCanvas => _joinPublicRoomsCanvas;
        [SerializeField] private CurrentRoomCanvas _currentRoomCanvas;
        public CurrentRoomCanvas CurrentRoomCanvas => _currentRoomCanvas;

        public void FirstInitialize()
        {
            HostRoomCanvas.FirstInitialize(this);
            CurrentRoomCanvas.FirstInitialize(this);
            JoinRoomCanvas.FirstInitialize(this);
            JoinPublicRoomsCanvas.FirstInitialize(this);
        }

    }
}
