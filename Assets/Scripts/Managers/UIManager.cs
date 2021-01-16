using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private RoomsCanvases _roomsCanvases;
        private GameObject _currentCanvas;
        public List<RoomListing> RoomListings = new List<RoomListing>();

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
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomList.ForEach(room =>
            {
                // Removed from rooms list.
                if (room.RemovedFromList)
                {
                    int index =RoomListings.FindIndex(x => x.RoomInfo.Name == room.Name);
                    if (index != -1)
                    {
                        Destroy(RoomListings[index].gameObject);
                        RoomListings.RemoveAt(index);
                    }
                }
                // Added to rooms list.
                else
                {
                    Debug.Log("ADDED ROOM");
                    _roomsCanvases.HostRoomCanvas.CreateRoomMenu.RoomListingsMenu.AddRoomListing(room);
                }
            });
        }
    }
}
