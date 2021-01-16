using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
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
        
        public override void OnEnable()
        {
            base.OnEnable();
            GetCurrentRoomPlayers();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _playerListings.ForEach(listing=>Destroy(listing.gameObject));
            _playerListings.Clear();
        }

        private void GetCurrentRoomPlayers()
        {
            if(!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom==null || PhotonNetwork.CurrentRoom.Players==null)
                return;
            foreach (KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerListing(playerInfo.Value);
            }
        }

        // public void UpdateTextForAllPlayers(Player player,string roleHolder)
        // {
        //     PhotonView photonView = PhotonView.Get(this);
        //     photonView.RPC("UpdatePlayerListingText",RpcTarget.All,player,roleHolder);
        // }
        private void AddPlayerListing(Player player)
        {
            int index = _playerListings.FindIndex(x => x.Player == player);
            if (index!=-1)
            {
                _playerListings[index].SetPlayerInfo(player);
            }
            else
            {
                PlayerListing playerListing = Instantiate(_playerListingPrefab, _contentParent);
                if(playerListing!=null)
                {
                    playerListing.SetPlayerInfo(player);
                    _playerListings.Add(playerListing);
                }
            }
        }
        
        public void CheckRoleStatusAndSet(string targetRole)
        {
            if(!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom==null || PhotonNetwork.CurrentRoom.Players==null)
                return;
            bool validForRole = true;
            _playerListings.ForEach(playerListing =>
            {
                Debug.Log(playerListing.Player+" "+playerListing.Player.CustomProperties["Role"]);
                if (!playerListing.Player.IsLocal && (string) playerListing.Player.CustomProperties["Role"] == targetRole)
                {
                    Debug.Log("Role is already claimed by another player!");
                    validForRole = false;
                }
            });
            if (validForRole && PhotonNetwork.LocalPlayer.IsLocal && PhotonNetwork.LocalPlayer.CustomProperties["Role"] != targetRole)
            {
                Debug.Log("Claiming the role");
                //PhotonNetwork.LocalPlayer.CustomProperties["Role"]=targetRole;
                Hashtable hashtable = new Hashtable {["Role"] = targetRole};
                PhotonNetwork.SetPlayerCustomProperties(hashtable);
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
