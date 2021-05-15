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
        [SerializeField] private PauseUIManager _pauseUIManager;
        private CourtBuilding _currentBuilding;
        private PlayerMove _localPlayerMove;

        private void Awake()
        {
            Cursor.visible = false;
            HandleBuildingSpawn();
        }

        private void HandleBuildingSpawn()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var obj=GameManager.NetworkInstantiate(_courtBuildings.PickRandom().gameObject, Vector3.zero, Quaternion.identity);
                _currentBuilding = obj.GetComponent<CourtBuilding>();
                //=Instantiate(_courtBuildings.PickRandom(),SessionEnvironmentParent);
            }
            if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__])
            {
                GameManager.NetworkInstantiate(_aiJudgeGeneralBehaviour.gameObject, _currentBuilding.JudgeTransform.position, Quaternion.identity);
            }
            HandleSpawns();
            _currentBuilding.InitTimers((int)PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_DURATION__]);
            _currentBuilding.TotalTurnCountMax = (int)PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_COUNT__];
        }

        private void HandleSpawns()
        {
            GameObject tmpObjHolder = null;
            switch (PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString().ToLower())
            {
                case "plaintiff":
                    tmpObjHolder=GameManager.NetworkInstantiate(plaintiff, _currentBuilding.PlaintiffTransform.position, Quaternion.identity);
                    break;
                case "defendant":
                    tmpObjHolder=GameManager.NetworkInstantiate(defendant, _currentBuilding.DefendantTransform.position, Quaternion.identity);
                    break;
                case "judge":
                    tmpObjHolder=GameManager.NetworkInstantiate(judge, _currentBuilding.JudgeTransform.position, Quaternion.identity);
                    break;
                case "spectator":
                    tmpObjHolder=GameManager.NetworkInstantiate(spectator, _currentBuilding.SpectatorTransform.position, Quaternion.identity);
                    break;
            }

            switch (tmpObjHolder.GetComponent<PhotonView>().IsMine)
            {
                case false:
                    Destroy(tmpObjHolder);
                    break;
                case true:
                {
                    _localPlayerMove = tmpObjHolder.GetComponent<PlayerMove>();
                    _localPlayerMove.OnStartTurn += _currentBuilding.StartTurnForCurrentUser;
                    _localPlayerMove.OnSwitchTurn += _currentBuilding.SwitchTurn;
                    _localPlayerMove.PlayerLook.RegisterForInteractables(_currentBuilding.InteractableCourtObjects);
                    break;
                }
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
