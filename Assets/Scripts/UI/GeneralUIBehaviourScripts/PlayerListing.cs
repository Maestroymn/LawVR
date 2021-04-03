﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerListing : MonoBehaviourPunCallbacks
    {
        [SerializeField] public TextMeshProUGUI PlayerListingText;
        [SerializeField] public RawImage Background;
        public Player Player { get; private set; }
        public bool Ready;

        public void SetPlayerInfo(Player player)
        {
            Ready = false;
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

        #region PhotonCallbacks

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer != null && targetPlayer == Player)
            {
                if (changedProps.ContainsKey("Role"))
                {
                    UpdatePlayerListingText();
                }
            }
        }        

        #endregion
    }
}