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
        public RoomsCanvases RoomsCanvases => _roomsCanvases;
        public GameObject CurrentCanvas;
        public List<RoomListing> RoomListings = new List<RoomListing>();
        public static UIManager Instance;
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
                Instance = this;
            }
            InitializeCanvases();
            _roomsCanvases.FirstInitialize();
        }

        private void InitializeCanvases()
        {
            _mainMenuUI.Show();
            _roomsCanvases.HostRoomCanvas.gameObject.SetActive(false);
            _roomsCanvases.JoinRoomCanvas.gameObject.SetActive(false);
            _roomsCanvases.CurrentRoomCanvas.gameObject.SetActive(false);
        }
        
        #region UI Interactables

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
            CurrentCanvas = _roomsCanvases.HostRoomCanvas.gameObject;
        }

        public void OnJoinClicked()
        {
            _mainMenuUI.HideMainMenu();
            _roomsCanvases.JoinRoomCanvas.Show();
            CurrentCanvas = _roomsCanvases.JoinRoomCanvas.gameObject;
        }

        public void OnShowPublicRoomsClicked()
        {
            _roomsCanvases.JoinRoomCanvas.Hide();
            _roomsCanvases.JoinPublicRoomsCanvas.Show();
        }
        
        public void ReturnToMainMenu()
        {
            CurrentCanvas.SetActive(false);
            _mainMenuUI.Show();
        }

        public void SelectCase()
        {
            //Case Listing Button 2
        }

        public void OpenDetails()
        {
            //Case Listing Button 1
            //Instantiate 
        }

        #endregion

        #region PhotonCallbacks

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
        
        #endregion
    }
}
