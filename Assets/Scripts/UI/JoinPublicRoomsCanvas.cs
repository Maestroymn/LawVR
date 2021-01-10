using UnityEngine;

namespace UI
{
    public class JoinPublicRoomsCanvas : MonoBehaviour
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
    }
}
