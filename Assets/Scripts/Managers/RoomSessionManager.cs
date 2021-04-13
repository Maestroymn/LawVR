using System.Collections.Generic;
using AI;
using Data;
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
            switch (PhotonNetwork.LocalPlayer.CustomProperties["Role"].ToString().ToLower())
            {
                case "plaintiff":
                    GameManager.NetworkInstantiate(plaintiff, _currentBuilding.DefendantTransform.position, Quaternion.identity);
                    break;
                case "defendant":
                    GameManager.NetworkInstantiate(defendant, _currentBuilding.PlaintiffTransform.position, Quaternion.identity);
                    break;
                case "judge":
                    GameManager.NetworkInstantiate(judge, _currentBuilding.JudgeTransform.position, Quaternion.identity);
                    break;
                case "spectator":
                    GameManager.NetworkInstantiate(spectator, _currentBuilding.SpectatorTransform.position, Quaternion.identity);
                    break;
            }
        }

        #region PhotonCallbacks

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                UIManager.Instance.RoomsCanvases.HostRoomCanvas.CreateRoomMenu.RoomListingsMenu.RemoveRoomListing(PhotonNetwork.CurrentRoom);
                foreach (var otherPlayer in PhotonNetwork.PlayerListOthers)
                {
                    PhotonNetwork.CloseConnection(otherPlayer);
                }
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(0);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (otherPlayer.IsMasterClient)
            {
                UIManager.Instance.RoomsCanvases.HostRoomCanvas.CreateRoomMenu.RoomListingsMenu.RemoveRoomListing(PhotonNetwork.CurrentRoom);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(0);
            }
        }

        #endregion
    }
}
