using UI.CanvasScripts;
using UnityEngine;

namespace UI
{
    public class HostRoomCanvas : MonoBehaviour
    {
        [SerializeField] private CreateRoomMenu _createRoomMenu;
        [SerializeField] private CaseListCanvas _caseListCanvas;
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
            _caseListCanvas.gameObject.SetActive(showCases);
        }
    }
}
