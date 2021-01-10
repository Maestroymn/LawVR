using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public Player Player { get; private set; }

        public void SetPlayerInfo(Player player)
        {
            Player = player;
            _text.text = player.NickName;
        }
    }
}
