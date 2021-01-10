using System;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private RoomsCanvases _roomsCanvases;
        private GameObject _currentCanvas;
        private void Awake()
        {
            _roomsCanvases.FirstInitialize();
        }

        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        public void OnSettingsClicked()
        {
            //Load Settings UI
        }

        public void OnHostClicked()
        {
            _mainMenuUI.HideMainMenu();
            _roomsCanvases.HostRoomCanvas.Show();
            _currentCanvas = _roomsCanvases.HostRoomCanvas.gameObject;
        }

        public void OnJoinClicked()
        {
            _mainMenuUI.HideMainMenu();
            _roomsCanvases.JoinRoomCanvas.Show();
            _currentCanvas = _roomsCanvases.JoinRoomCanvas.gameObject;
        }

        public void OnShowPublicRoomsClicked()
        {
            _roomsCanvases.JoinRoomCanvas.Hide();
            _roomsCanvases.JoinPublicRoomsCanvas.Show();
        }
        
        public void ReturnToMainMenu()
        {
            _currentCanvas.SetActive(false);
            _mainMenuUI.Show();
        }
    }
}
