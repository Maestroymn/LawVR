using UnityEngine;

namespace UI
{
    public class JoinRoomCanvas : MonoBehaviour
    {
        [SerializeField] private JoinRoomMenu _joinRoomMenu;

        public JoinRoomMenu JoinRoomMenu { get; private set; }

        private RoomsCanvases _roomsCanvases;   
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
            JoinRoomMenu = _joinRoomMenu;
            _joinRoomMenu.FirstInitialize(roomsCanvases);
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
