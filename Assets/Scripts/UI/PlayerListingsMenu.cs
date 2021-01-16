using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace UI
{
    public class PlayerListingsMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform _contentParent;
        [SerializeField] private PlayerListing _playerListingPrefab;

        public List<PlayerListing> _playerListings = new List<PlayerListing>();

        private void Awake()
        {
            GetCurrentRoomPlayers();
        }

        private void GetCurrentRoomPlayers()
        {
            foreach (KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerListing(playerInfo.Value);
            }
        }

        private void AddPlayerListing(Player player)
        {
            PlayerListing playerListing = Instantiate(_playerListingPrefab, _contentParent);
            if (playerListing != null)
            {
                playerListing.SetPlayerInfo(player);
                if (!_playerListings.Contains(playerListing))
                {
                    _playerListings.Add(playerListing);
                }
                else
                {
                    Destroy(playerListing.gameObject);
                }
            }
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerListing(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            _playerListings.ForEach(player =>
            {
                if (!(player is null) && otherPlayer.NickName == player.Player.NickName)
                {
                    _playerListings.Remove(player);
                    Destroy(player.gameObject);
                }
            });
        }
    }
}
