using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerListing : MonoBehaviourPunCallbacks
    {
        [SerializeField] public TextMeshProUGUI PlayerListingText;

        public Player Player { get; private set; }

        public void SetPlayerInfo(Player player)
        {
            Player = player;
            UpdatePlayerListingText();
        }
        
        private void UpdatePlayerListingText()
        {
            if ((string) Player.CustomProperties["Role"] != "none")
            {
                PlayerListingText.text = "(" + Player.CustomProperties["Role"] + ") " + Player.NickName;
            }
            else
            {
                PlayerListingText.text = Player.NickName;
            }
        }
        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            print("UPDATE");
            if (targetPlayer != null && targetPlayer == Player)
            {
                if (changedProps.ContainsKey("Role"))
                {
                    UpdatePlayerListingText();
                }
            }
        }
    }
}
