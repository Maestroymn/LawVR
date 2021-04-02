using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Sprite _lockedSprite, _unlockedSprite;
        [SerializeField] private Image _publicHolderImage;
        public RoomInfo RoomInfo { get; private set; }

        public void SetRoomInfo(RoomInfo room)
        {
            RoomInfo = room;
            if (room.CustomProperties.ContainsKey("password"))
            {
                _publicHolderImage.sprite = _lockedSprite;
            }
            else
            {
                _publicHolderImage.sprite = _unlockedSprite;
            }
            _text.text = RoomInfo.Name;
        }
    }
}
