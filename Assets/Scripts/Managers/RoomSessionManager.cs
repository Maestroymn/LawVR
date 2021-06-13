using System.Collections;
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
        [SerializeField] private GameObject maleJudge,femaleJudge,malePlaintiff,femalePlaintiff,femaleSpectator,maleSpectator,maleDefendant,femaleDefendant;
        [SerializeField] private AIJudgeGeneralBehaviour _aiJudgeGeneralBehaviour;
        [SerializeField] private List<CourtBuilding> _courtBuildings;
        [SerializeField] private PauseUIManager _pauseUIManager;
        [SerializeField] private Canvas _loadingCanvas;
        [SerializeField] private Canvas _sessionEndCanvas;
        [SerializeField] private Camera _mainCamera;
        [HideInInspector] public CourtBuilding _currentBuilding;
        private PlayerMove _localPlayerMove;

        private void Awake()
        {
            Cursor.visible = false;
            HandleBuildingSpawn();
        }

        private void OnSessionEnded()
        {
            _currentBuilding.SessionEnded -= OnSessionEnded;
            _mainCamera.gameObject.SetActive(false);
            _mainCamera.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _sessionEndCanvas.gameObject.SetActive(true);
        }
        
        private void HandleBuildingSpawn()
        {
            if(PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var obj = GameManager.NetworkInstantiateRoomObj(_courtBuildings.PickRandom().gameObject, Vector3.zero,
                    Quaternion.identity);
                _currentBuilding = obj.GetComponent<CourtBuilding>();
                _currentBuilding.SessionEnded += OnSessionEnded;
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__])
                {
                    GameManager.NetworkInstantiate(_aiJudgeGeneralBehaviour.gameObject,
                        _currentBuilding.JudgeTransform.position, Quaternion.identity);
                }
                LeanTween.delayedCall(.3f, () =>
                {
                    _currentBuilding.InitTimers(
                        (int) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_DURATION__]);
                    _currentBuilding.TotalTurnCountMax =
                        (int) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_COUNT__];
                    _loadingCanvas.gameObject.SetActive(false);
                    _mainCamera.gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    HandleSpawns();
                });
            }
            else
            {
                StartCoroutine(WaitUntilFind());
            }
        }

        private IEnumerator WaitUntilFind()
        {
            while (!_currentBuilding)
            {
                _currentBuilding = FindObjectOfType<CourtBuilding>();
                yield return null;
            }
            _currentBuilding.SessionEnded += OnSessionEnded;
            _loadingCanvas.gameObject.SetActive(false);
            _mainCamera.gameObject.SetActive(false);
            _currentBuilding.InitTimers(
                (int) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_DURATION__]);
            _currentBuilding.TotalTurnCountMax =
                (int) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__TURN_COUNT__];
            HandleSpawns();
        }
        
        private void HandleSpawns()
        {
            GameObject tmpObjHolder = null;
            switch (PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString().ToLower())
            {
                case "plaintiff":
                    if(GameManager.GameSettings.Gender==Gender.Female)
                        tmpObjHolder=GameManager.NetworkInstantiate(femalePlaintiff, _currentBuilding.PlaintiffTransform.position, Quaternion.identity);
                    else
                        tmpObjHolder=GameManager.NetworkInstantiate(malePlaintiff, _currentBuilding.PlaintiffTransform.position, Quaternion.identity);
                    break;
                case "defendant":
                    if(GameManager.GameSettings.Gender==Gender.Female)
                        tmpObjHolder=GameManager.NetworkInstantiate(femaleDefendant, _currentBuilding.DefendantTransform.position, Quaternion.identity);
                    else
                        tmpObjHolder=GameManager.NetworkInstantiate(maleDefendant, _currentBuilding.DefendantTransform.position, Quaternion.identity);
                    break;
                case "judge":
                    if(GameManager.GameSettings.Gender==Gender.Female)
                        tmpObjHolder=GameManager.NetworkInstantiate(femaleJudge, _currentBuilding.JudgeTransform.position, Quaternion.identity);
                    else
                        tmpObjHolder=GameManager.NetworkInstantiate(maleJudge, _currentBuilding.JudgeTransform.position, Quaternion.identity); 
                    break;
                case "spectator":
                    if(GameManager.GameSettings.Gender==Gender.Female)
                        _currentBuilding.InstantiateNextSpectator(femaleSpectator);
                    else 
                        _currentBuilding.InstantiateNextSpectator(maleSpectator);
                    break;
            }

            switch (tmpObjHolder?.GetComponent<PhotonView>().IsMine)
            {
                case false:
                    Destroy(tmpObjHolder);
                    break;
                case true:
                {
                    _localPlayerMove = tmpObjHolder.GetComponent<PlayerMove>();
                    _localPlayerMove.PlayerLook.Initialize();
                    _pauseUIManager.OnPaused += _localPlayerMove.PlayerLook.CloseSummary;
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
