using Managers;
using Photon.Realtime;
using UnityEngine;

namespace UI
{
    public class RoomListingsMenu : MonoBehaviour
    {
        [SerializeField] public Transform _contentParent;
        [SerializeField] public RoomListing _roomListingPrefab;
        [SerializeField] private UIManager UIManager;

        public void AddRoomListing(RoomInfo room)
        {
            RoomListing roomListing = Instantiate(_roomListingPrefab, _contentParent);
            if (roomListing != null)
            {
                roomListing.SetRoomInfo(room);
                if (!UIManager.RoomListings.Contains(roomListing))
                {
                    UIManager.RoomListings.Add(roomListing);
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
            }
        }
    }
}
