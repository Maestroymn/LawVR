using System.Collections.Generic;
using DatabaseScripts;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UI.CanvasScripts;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private MainSettingsCanvas _mainSettings;
        [SerializeField] private RoomsCanvases _roomsCanvases;
        public RoomsCanvases RoomsCanvases => _roomsCanvases;
        public GameObject CurrentCanvas;
        public List<RoomListing> RoomListings;
        public static UIManager Instance;
        private void Awake()
        {
            RoomListings = new List<RoomListing>();
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
            Cursor.visible = true;
            _mainMenuUI.Show();
            _mainSettings.Initialize();
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
            _mainMenuUI.HideMainMenu();
            CurrentCanvas = _mainSettings.gameObject;
            _mainSettings.gameObject.SetActive(true);
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


        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            DatabaseConnection.SetUserOffline(GameManager.GameSettings.NickName);
        }

        #endregion
    }
}
