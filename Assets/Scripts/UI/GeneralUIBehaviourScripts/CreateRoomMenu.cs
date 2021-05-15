using System;
using Data;
using DatabaseScripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

namespace UI
{
    public class CreateRoomMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField _roomName, _password;
        [SerializeField] private RoomListingsMenu _roomListingsMenu;
        [SerializeField] private TMP_Dropdown _simulationType,_turnCount,_turnDuration;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Transform _caseListingButton;
        public RoomListingsMenu RoomListingsMenu { get; private set; }
        private RoomsCanvases _roomsCanvases;
        private bool _roomCreated;
        private RoomOptions _roomOptions;

        #region Initializations
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomCreated = false;
            _roomOptions = new RoomOptions();
            HandleInitialRoomOptions();
            _roomsCanvases = roomsCanvases;
            RoomListingsMenu = _roomListingsMenu;
        }

        private void HandleInitialRoomOptions()
        {
            _roomOptions.MaxPlayers = 16;
            _roomOptions.IsVisible = true;
            _roomOptions.BroadcastPropsChangeToAll = true;
            _roomOptions.CustomRoomProperties = new Hashtable
            {
                {DataKeyValues.__PASSWORD_KEY__, "EMPTY"},
                {DataKeyValues.__SIMULATION_TYPE__, DataKeyValues.__COMPETITION_MODE__},
                {DataKeyValues.__AI_JUDGE__, false},
                {DataKeyValues.__ROOM_NAME__, ""},
                {DataKeyValues.__CASE_ID__, -1},
                {DataKeyValues.__SESSION_ID__, ""},
                {DataKeyValues.__TURN_COUNT__, int.Parse(_turnCount.options[_turnCount.value].text)},
                {DataKeyValues.__TURN_DURATION__, int.Parse(_turnDuration.options[_turnDuration.value].text)}
            };
        }
        #endregion

        #region RoomHostUIInteractions

        public void OnSimulationTypeChanged()
        {
            if (_roomCreated) return;
            switch (_simulationType.value)
            {
                case 0: //Competition
                    _roomOptions.CustomRoomProperties[DataKeyValues.__SIMULATION_TYPE__]=DataKeyValues.__COMPETITION_MODE__;
                    break;
                case 1: //Educational
                    _roomOptions.CustomRoomProperties[DataKeyValues.__SIMULATION_TYPE__]=DataKeyValues.__EDUCATIONAL_MODE__;
                    break;
                case 2: //Sandbox
                    _roomOptions.CustomRoomProperties[DataKeyValues.__SIMULATION_TYPE__]=DataKeyValues.__SANDBOX_MODE__;
                    break;
                case 3: //Challenge
                    _roomOptions.CustomRoomProperties[DataKeyValues.__SIMULATION_TYPE__]=DataKeyValues.__CHALLENGE_MODE__;
                    break;
            }
        }

        public void OnAIJudgeValueChanged()
        {
            if (_roomCreated) return;
            _roomOptions.CustomRoomProperties[DataKeyValues.__AI_JUDGE__]=_toggle.isOn;
        }
        
        public void OnClickCreateRoom()
        {
            if (!IsRequiredFieldsFilled()) return;
            if (!PhotonNetwork.IsConnected || _roomCreated)
            {
                Debug.Log("NOTCONNECTED"+_roomCreated);
                return;
            }
            if (_password.text.Length!=0)
            {
                _roomOptions.CustomRoomProperties[DataKeyValues.__PASSWORD_KEY__]=_password.text;
            }
            _roomOptions.CustomRoomProperties[DataKeyValues.__TURN_COUNT__] = int.Parse(_turnCount.options[_turnCount.value].text);
            _roomOptions.CustomRoomProperties[DataKeyValues.__TURN_DURATION__] = int.Parse(_turnDuration.options[_turnDuration.value].text);
            _roomOptions.CustomRoomProperties[DataKeyValues.__ROOM_NAME__] = _roomName.text;
            var sessionID = DatabaseConnection.CreateSessionLog(_roomName.text,
                _roomsCanvases.HostRoomCanvas.CaseListCanvas.SelectedCase.CaseID.ToString(),
                DateTime.Now.ToString(),
                _roomOptions.CustomRoomProperties[DataKeyValues.__SIMULATION_TYPE__].ToString(),
                _roomOptions.CustomRoomProperties[DataKeyValues.__TURN_COUNT__].ToString(),
                _roomOptions.CustomRoomProperties[DataKeyValues.__TURN_DURATION__].ToString());
            _roomOptions.CustomRoomProperties[DataKeyValues.__CASE_ID__] =
                _roomsCanvases.HostRoomCanvas.CaseListCanvas.SelectedCase.CaseID;
            _roomOptions.CustomRoomProperties[DataKeyValues.__SESSION_ID__] = sessionID;
            PhotonNetwork.CreateRoom(_roomName.text, _roomOptions, TypedLobby.Default);
        }

        private bool IsRequiredFieldsFilled()
        {
            if (_roomName.text.Length == 0)
            {
                if (_roomName.transform.gameObject.LeanIsTweening()) return false;
                _roomName.transform.LeanMoveX(_roomName.transform.position.x + 4f, .1f).setOnComplete(() =>
                {
                    _roomName.transform.LeanMoveX(_roomName.transform.position.x - 8f, .1f).setOnComplete(() =>
                    {
                        _roomName.transform.LeanMoveX(_roomName.transform.position.x + 4f, .1f);
                    });
                });
                return false;
            }
            if (!_roomsCanvases.HostRoomCanvas.CaseListCanvas.SelectedCase)
            {
                if (_caseListingButton.transform.gameObject.LeanIsTweening()) return false;
                _caseListingButton.transform.LeanMoveX(_caseListingButton.transform.position.x + 4f, .1f).setOnComplete(() =>
                {
                    _caseListingButton.transform.LeanMoveX(_caseListingButton.transform.position.x - 8f, .1f).setOnComplete(() =>
                    {
                        _caseListingButton.transform.LeanMoveX(_caseListingButton.transform.position.x + 4f, .1f);
                    });
                });
                return false;
            }
            return true;
        }
        #endregion

        #region PhotonCallbacks
        public override void OnJoinedRoom()
        {
            Debug.Log("katıldım");
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("lobi katıldım");
        }

        public override void OnCreatedRoom()
        {
            _roomCreated = true;
            Debug.Log("Room is created its name is "+_roomName.text+" and its password: "+_password.text);
            PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__] = "none";
            _roomsCanvases.CurrentRoomCanvas.Show(_roomName.text,true);
            print("SESSION ID SET: "+PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__]);
            _roomCreated=false;
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room couldn't be created ",_roomName);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            if (PhotonNetwork.IsMasterClient)
            {
                _roomListingsMenu.RemoveRoomListing(PhotonNetwork.CurrentRoom);
            }
        }
                #endregion

    }
}
