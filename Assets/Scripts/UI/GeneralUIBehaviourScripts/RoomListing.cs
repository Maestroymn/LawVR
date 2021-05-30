using System;
using Managers;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomListing : MonoBehaviour
    {
        public event Action OnSelected;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Sprite _lockedSprite, _unlockedSprite;
        [SerializeField] private Image _publicHolderImage;
        [SerializeField] private Button Button;
        private bool _isSelected;
        public RoomInfo RoomInfo { get; private set; }

        public void SetRoomInfo(RoomInfo room)
        {
            RoomInfo = room;
            if (room.CustomProperties.ContainsKey("password"))
            {
                _publicHolderImage.sprite = _lockedSprite;
                Button.interactable = false;
            }
            else
            {
                _publicHolderImage.sprite = _unlockedSprite;
                Button.onClick.AddListener(OnClicked);
            }
            _text.text = RoomInfo.Name;
        }

        private void OnClicked()
        {
            _isSelected = !_isSelected;
            if (_isSelected)
            {
                GameManager.GameSettings.PublicSelectedRoomName = RoomInfo.Name;
                Button.image.color=Color.gray;
                OnSelected?.Invoke();
            }
            else
            {
                GameManager.GameSettings.PublicSelectedRoomName = "";
                Button.image.color=Color.white;
            }
        }
    }
}
