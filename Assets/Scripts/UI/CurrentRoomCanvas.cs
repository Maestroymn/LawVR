using Photon.Pun;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrentRoomCanvas : MonoBehaviour
    {
        private RoomsCanvases _roomsCanvases;
        [SerializeField] private TextMeshProUGUI _roomName;
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
        }

        public void Show(string roomName)
        {
            _roomName.text = roomName;
            gameObject.SetActive(true);
            _roomsCanvases.HostRoomCanvas.Hide();
        }
        
        public void Hide()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LocalPlayer.CustomProperties["Role"] = "none";
            gameObject.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                _roomsCanvases.HostRoomCanvas.Show();
            }
            else
            {
                _roomsCanvases.JoinRoomCanvas.Show();
            }
        }
        public void OnSessionStarted()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //Locking room when the session started, if following bools are set to false, then no one can join after session started.
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel(1);
            }
        }
    }
}
