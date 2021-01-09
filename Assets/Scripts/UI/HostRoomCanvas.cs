using UnityEngine;

namespace UI
{
    public class HostRoomCanvas : MonoBehaviour
    {
        [SerializeField] private CreateRoomMenu _createRoomMenu;
        private RoomsCanvases _roomsCanvases;   
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
            _createRoomMenu.FirstInitialize(roomsCanvases);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
