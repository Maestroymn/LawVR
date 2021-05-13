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
        private bool _plaintiffTurn;
        private int _currentTurnCount=0, _totalTurnCountMax;
        private PlayerMove _localPlayerMove;
        private GameObject tmpObjHolder;

        private void Awake()
        {
            Cursor.visible = false;
            HandleBuildingSpawn();
            _totalTurnCountMax = (int)PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_COUNT__];
        }

        private void HandleBuildingSpawn()
        {
            _currentBuilding=Instantiate(_courtBuildings.PickRandom(),SessionEnvironmentParent);
            if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__])
            {
                GameManager.NetworkInstantiate(_aiJudgeGeneralBehaviour.gameObject, _currentBuilding.JudgeTransform.position, Quaternion.identity);
            }
            HandleSpawns();
            _currentBuilding.InitTimers((int)PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_DURATION__]);
        }

        private void HandleSpawns()
        {
            switch (PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString().ToLower())
            {
                case "plaintiff":
                    tmpObjHolder=GameManager.NetworkInstantiate(plaintiff, _currentBuilding.PlaintiffTransform.position, Quaternion.identity);
                    tmpObjHolder.transform.rotation = _currentBuilding.PlaintiffTransform.rotation;
                    tmpObjHolder.transform.SetParent(_currentBuilding.PlaintiffTransform.parent);
                    break;
                case "defendant":
                    tmpObjHolder=GameManager.NetworkInstantiate(defendant, _currentBuilding.DefendantTransform.position, Quaternion.identity);
                    tmpObjHolder.transform.rotation = _currentBuilding.DefendantTransform.rotation;
                    tmpObjHolder.transform.SetParent(_currentBuilding.DefendantTransform.parent);
                    break;
                case "judge":
                    tmpObjHolder=GameManager.NetworkInstantiate(judge, _currentBuilding.JudgeTransform.position, Quaternion.identity);
                    tmpObjHolder.transform.rotation = _currentBuilding.JudgeTransform.rotation;
                    tmpObjHolder.transform.SetParent(_currentBuilding.JudgeTransform.parent);
                    break;
                case "spectator":
                    tmpObjHolder=GameManager.NetworkInstantiate(spectator, _currentBuilding.SpectatorTransform.position, Quaternion.identity);
                    tmpObjHolder.transform.rotation = _currentBuilding.SpectatorTransform.rotation;
                    tmpObjHolder.transform.SetParent(_currentBuilding.SpectatorTransform.parent);
                    break;
            }
            if(photonView.IsMine)
            {
                _localPlayerMove = tmpObjHolder.GetComponent<PlayerMove>();
                _localPlayerMove.PlayerLook.RegisterForInteractables(_currentBuilding.InteractableCourtObjects);
                _localPlayerMove.OnSwitchTurn += SwitchTurnEvent;
                _localPlayerMove.OnStartTurn += StartTurnEvent;
                _pauseUIManager.OnPaused += _localPlayerMove.PlayerLook.CloseSummary;
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    _localPlayerMove.OnStartSession += StartSessionEvent;
                    _localPlayerMove.OnAllReady += StartSession;
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
        
        #region RPC Funcs

        private void StartSession()
        {
            _localPlayerMove.OnAllReady -= StartSession;
            _localPlayerMove.photonView.RPC("RPC_StartSession",RpcTarget.All);
        }
        
        public void StartSessionEvent()
        {
            _localPlayerMove.OnStartSession -= StartSessionEvent;
            SwitchTurnEvent();
        }
        
        public void SwitchTurnEvent()
        {
            _currentTurnCount++;
            if (_currentTurnCount > _totalTurnCountMax)
            {
                // SESSION FINISHED HERE
                print("SESSION ENDED!!!!");
            }
            if (_plaintiffTurn)
            {
                _plaintiffTurn = false;
                _currentBuilding.PlaintiffTimer.HandleTimer(false);
                _currentBuilding.DefendantTimer.timeText.text = "START!";
                _currentBuilding.DefendantStartButton.HandleButtonSettings(ButtonStatus.Start);
                _currentBuilding.PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Wait);
          
            }
            else
            {
                _plaintiffTurn = true;
                _currentBuilding.DefendantTimer.HandleTimer(false);
                _currentBuilding.PlaintiffTimer.timeText.text = "START!";
                _currentBuilding.PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Start);
                _currentBuilding.DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);
            }
        }

        public void StartTurnEvent()
        {
            if (_plaintiffTurn)
            {
                _currentBuilding.DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);
                _currentBuilding.PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Pass);
                _currentBuilding.PlaintiffTimer.HandleTimer(true);
            }
            else
            {
                _currentBuilding.PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Wait);
                _currentBuilding.DefendantStartButton.HandleButtonSettings(ButtonStatus.Pass);
                _currentBuilding.DefendantTimer.HandleTimer(true);
            }
        }
        #endregion
    }
}
