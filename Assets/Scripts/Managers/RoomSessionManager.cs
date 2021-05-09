using System.Collections.Generic;
using AI;
using Data;
using DatabaseScripts;
using General;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class RoomSessionManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject judge,plaintiff,spectator,defendant;
        [SerializeField] private AIJudgeGeneralBehaviour _aiJudgeGeneralBehaviour;
        [SerializeField] private List<CourtBuilding> _courtBuildings;
        [SerializeField] private Transform SessionEnvironmentParent;
        private CourtBuilding _currentBuilding;
        private void Awake()
        {
            Cursor.visible = false;
            HandleBuildingSpawn();
        }

        private void HandleBuildingSpawn()
        {
            _currentBuilding=Instantiate(_courtBuildings.PickRandom(),SessionEnvironmentParent);
            if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__])
            {
                GameManager.NetworkInstantiate(_aiJudgeGeneralBehaviour.gameObject, _currentBuilding.JudgeTransform.position, Quaternion.identity);
            }
            HandleSpawns();
        }

        private void HandleSpawns()
        {
            switch (PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString().ToLower())
            {
                case "plaintiff":
                    var obj=GameManager.NetworkInstantiate(plaintiff, _currentBuilding.PlaintiffTransform.position, Quaternion.identity);
                    obj.transform.rotation = _currentBuilding.PlaintiffTransform.rotation;
                    obj.transform.SetParent(_currentBuilding.PlaintiffTransform.parent);
                    break;
                case "defendant":
                    obj=GameManager.NetworkInstantiate(defendant, _currentBuilding.DefendantTransform.position, Quaternion.identity);
                    obj.transform.rotation = _currentBuilding.DefendantTransform.rotation;
                    obj.transform.SetParent(_currentBuilding.DefendantTransform.parent);
                    break;
                case "judge":
                    obj=GameManager.NetworkInstantiate(judge, _currentBuilding.JudgeTransform.position, Quaternion.identity);
                    obj.transform.rotation = _currentBuilding.JudgeTransform.rotation;
                    obj.transform.SetParent(_currentBuilding.JudgeTransform.parent);
                    break;
                case "spectator":
                    obj=GameManager.NetworkInstantiate(spectator, _currentBuilding.SpectatorTransform.position, Quaternion.identity);
                    obj.transform.rotation = _currentBuilding.SpectatorTransform.rotation;
                    obj.transform.SetParent(_currentBuilding.SpectatorTransform.parent);
                    break;
            }
        }

        #region PhotonCallbacks

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            DatabaseConnection.SetUserOffline(GameManager.GameSettings.NickName);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                UIManager.Instance.RoomsCanvases.HostRoomCanvas.CreateRoomMenu.RoomListingsMenu.RemoveRoomListing(PhotonNetwork.CurrentRoom);
                foreach (var otherPlayer in PhotonNetwork.PlayerListOthers)
                {
                    PhotonNetwork.CloseConnection(otherPlayer);
                }
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(DataKeyValues.__LOGIN_SCENE__);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (otherPlayer.IsMasterClient)
            {
                UIManager.Instance.RoomsCanvases.HostRoomCanvas.CreateRoomMenu.RoomListingsMenu.RemoveRoomListing(PhotonNetwork.CurrentRoom);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(DataKeyValues.__LOGIN_SCENE__);
            }
        }

        
        
        #endregion
    }
}
