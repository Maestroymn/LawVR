using System;
using System.Collections.Generic;
using Data;
using DatabaseScripts;
using ExitGames.Client.Photon;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

namespace UI
{
    public class PlayerListingsMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform _contentParent;
        [SerializeField] private PlayerListing _playerListingPrefab;
        [SerializeField] private RoomsCanvases _roomsCanvases;
        [SerializeField] public TextMeshProUGUI ReadyText;
        [SerializeField] public Color ReadyColor, NotReadyColor;
        public List<PlayerListing> _playerListings = new List<PlayerListing>();
        private bool _ready; 

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
                Debug.Log(playerListing.Player+" "+playerListing.Player.CustomProperties[DataKeyValues.__ROLE__]);
                if (!playerListing.Player.IsLocal && (string) playerListing.Player.CustomProperties[DataKeyValues.__ROLE__] == targetRole)
                {
                    Debug.Log("Role is already claimed by another player!");
                    validForRole = false;
                }
            });
            if (validForRole && PhotonNetwork.LocalPlayer.IsLocal && PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__] != targetRole)
            {
                Debug.Log("Claiming the role");
                //PhotonNetwork.LocalPlayer.CustomProperties["Role"]=targetRole;
                Hashtable hashtable = new Hashtable {[DataKeyValues.__ROLE__] = targetRole};
                PhotonNetwork.SetPlayerCustomProperties(hashtable);
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SetReadyUp",RpcTarget.AllBuffered,PhotonNetwork.LocalPlayer,true);
                    photonView.RPC("RPC_ChangeReadyState",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer,true);
                    CheckIfAllReady();
                }
                else if(!PhotonNetwork.IsMasterClient && _ready)
                {
                    photonView.RPC("SetReadyUp",RpcTarget.AllBuffered,PhotonNetwork.LocalPlayer,!_ready);
                    photonView.RPC("RPC_ChangeReadyState",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer,_ready);
                    CheckIfAllReady();
                }
            }
        }
        
        public void OnSessionStarted()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _playerListings.ForEach(player =>
                {
                    if (player.Player != PhotonNetwork.LocalPlayer && !player.Ready)
                    {    
                        return;
                    }
                });
                //Locking room when the session started, if following bools are set to false, then no one can join after session started.
                photonView.RPC("RPC_UpdateUserSessions",RpcTarget.All);
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel(DataKeyValues.__COURT_SCENE__);
            }
        }
        
        

        private void CheckIfAllReady()
        {
#if !UNITY_EDITOR
            _playerListings.ForEach(x =>
            {
                if (!x.Ready)
                { 
                    _roomsCanvases.CurrentRoomCanvas.SetStatusForStartSessionButton(false);
                    return;
                }
            });
            if (_playerListings.Count >= 1)
            {
                _roomsCanvases.CurrentRoomCanvas.SetStatusForStartSessionButton(true);
            }
#elif UNITY_EDITOR
            _roomsCanvases.CurrentRoomCanvas.SetStatusForStartSessionButton(true);
#endif
        }
        
        public void OnClickReady()
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString().ToLower()!="none")
            {
                photonView.RPC("SetReadyUp",RpcTarget.AllBuffered,PhotonNetwork.LocalPlayer,!_ready);
                if (_ready)
                {
                    ReadyText.text = "Not Ready!";
                }
                else
                {
                    ReadyText.text = "Ready!";
                }
                photonView.RPC("RPC_ChangeReadyState",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer,_ready);
                CheckIfAllReady();
            }
        }

        #region PhotonCallbacks
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerListing(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerListing leftPlayerListing=null;
            for (int i = 0; i < _playerListings.Count; i++)
            {
                if (!(_playerListings[i] is null) && otherPlayer.NickName == _playerListings[i].Player.NickName)
                {
                    leftPlayerListing = _playerListings[i];
                    break;
                }            
            }
            if (!leftPlayerListing) return;
            _playerListings.Remove(leftPlayerListing);
            Destroy(leftPlayerListing);

        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            _roomsCanvases.CurrentRoomCanvas.Hide();
        }
        #endregion
        
        #region PunRPC Functions

        [PunRPC]
        private void RPC_UpdateUserSessions()
        {
            DatabaseConnection.UpdateUserSessionID(GameManager.GameSettings.NickName, PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__].ToString());
        }
        
        [PunRPC]
        private void RPC_ChangeReadyState(Player player, bool ready)
        {
            int index = _playerListings.FindIndex(x => x.Player == player);
            if (index != -1)
            {
                _playerListings[index].Ready = ready;
            }
        }
        
        
        [PunRPC]
        private void SetReadyUp(Player player,bool state)
        {
            _ready = state;
            int index = _playerListings.FindIndex(x => x.Player == player);
            if (index != -1)
            {
                if (state)
                {
                    _playerListings[index].Background.color = ReadyColor;
                }
                else
                {
                    _playerListings[index].Background.color = NotReadyColor;
                }
            }
        }
        #endregion
    }
}
