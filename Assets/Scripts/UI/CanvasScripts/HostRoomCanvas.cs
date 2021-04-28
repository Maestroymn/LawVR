using UI.CanvasScripts;
using UnityEngine;

namespace UI
{
    public class HostRoomCanvas : MonoBehaviour
    {
        [SerializeField] private CreateRoomMenu _createRoomMenu;
        public CaseListCanvas CaseListCanvas;
        public CreateRoomMenu CreateRoomMenu { get; private set; }

        private RoomsCanvases _roomsCanvases;   
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
            CreateRoomMenu = _createRoomMenu;
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

        public void ShowCaseListingCanvas(bool showCases)
        {
            _createRoomMenu.gameObject.SetActive(!showCases);
            CaseListCanvas.gameObject.SetActive(showCases);
        }
    }
}
