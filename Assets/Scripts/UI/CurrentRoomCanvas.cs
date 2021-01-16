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
            gameObject.SetActive(false);
            _roomsCanvases.JoinRoomCanvas.Show();
        }
    }
}
