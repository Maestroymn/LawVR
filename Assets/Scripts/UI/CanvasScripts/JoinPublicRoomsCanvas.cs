using Data;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace UI
{
    public class JoinPublicRoomsCanvas : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RoomListingsMenu _roomListingsMenu;

        public RoomListingsMenu RoomListingsMenu { get; private set; }

        private RoomsCanvases _roomsCanvases;   
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
            RoomListingsMenu = _roomListingsMenu;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _roomsCanvases.JoinRoomCanvas.Show();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void JoinPublicRoom()
        {
            if (GameManager.GameSettings.PublicSelectedRoomName.Length == 0 || !PhotonNetwork.IsConnected || PhotonNetwork.InRoom) return;
            PhotonNetwork.JoinRoom(GameManager.GameSettings.PublicSelectedRoomName);
        }
        
        #region PhotonCallBacks

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined to "+GameManager.GameSettings.PublicSelectedRoomName);
            gameObject.SetActive(false);
            PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__] = "none";
            _roomsCanvases.JoinRoomCanvas.Hide();
            _roomsCanvases.CurrentRoomCanvas.Show(GameManager.GameSettings.PublicSelectedRoomName,false);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Couldn't joined to "+GameManager.GameSettings.PublicSelectedRoomName+"\n"+message);
        }

        #endregion
    }
}
