using System;
using Managers;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomListingsMenu : MonoBehaviour
    {
        [SerializeField] public Transform _contentParent;
        [SerializeField] public RoomListing _roomListingPrefab;
        [SerializeField] private UIManager UIManager;
        [SerializeField] private GameObject NoRoomAvailableTextParent;
        [SerializeField] private Button JoinRoomButton;

        private void OnEnable()
        {
            JoinRoomButton.interactable = false;
        }

        public void AddRoomListing(RoomInfo room)
        {
            if (NoRoomAvailableTextParent.activeInHierarchy)
            {
                NoRoomAvailableTextParent.SetActive(false);
            }
            RoomListing roomListing = Instantiate(_roomListingPrefab, _contentParent);
            if (roomListing != null)
            {
                roomListing.SetRoomInfo(room);
                if (!UIManager.RoomListings.Contains(roomListing))
                {
                    UIManager.RoomListings.Add(roomListing);
                    roomListing.OnSelected += OnPublicRoomSelected;
                }
                else
                {
                    Destroy(roomListing.gameObject);
                }
            }
        }

        public void RemoveRoomListing(RoomInfo room)
        {
            var roomListing = UIManager.RoomListings.Find(x => Equals(x.RoomInfo, room));
            if (roomListing)
            {
                UIManager.RoomListings.Remove(roomListing);
                Destroy(roomListing.gameObject);
                if (UIManager.RoomListings.Count == 0 && NoRoomAvailableTextParent.activeInHierarchy)
                {
                    NoRoomAvailableTextParent.SetActive(true);
                }
            }
        }

        private void OnPublicRoomSelected()
        {
            JoinRoomButton.interactable = true;
        }
    }
}
