using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace UI
{
    public class RoomListingsMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform _contentParent;
        [SerializeField] private RoomListing _roomListingPrefab;
    
        public List<RoomListing> RoomListings = new List<RoomListing>();

        public void AddRoomListing(RoomInfo room)
        {
            RoomListing roomListing = Instantiate(_roomListingPrefab, _contentParent);
            if (roomListing != null)
            {
                roomListing.SetRoomInfo(room);
                if (!RoomListings.Contains(roomListing))
                {
                    RoomListings.Add(roomListing);
                }
                else
                {
                    Destroy(roomListing.gameObject);
                }
            }
        }
        
    }
}
