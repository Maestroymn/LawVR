using Data;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace UI
{
    public class CreateRoomMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _roomName, _password;
        [SerializeField] private RoomListingsMenu _roomListingsMenu;
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Toggle _toggle;
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
            _roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            _roomOptions.CustomRoomProperties.Add(DataKeyValues.__PASSWORD_KEY__,"");
            _roomOptions.CustomRoomProperties.Add(DataKeyValues.__SIMULATION_TYPE__,"");
            _roomOptions.CustomRoomProperties.Add(DataKeyValues.__AI_JUDGE__,false);
            _roomOptions.CustomRoomProperties.Add(DataKeyValues.__ROOM_NAME__,"");
        }
        #endregion

        #region RoomHostUIInteractions

        public void OnSimulationTypeChanged()
        {
            if (_roomCreated) return;
            switch (_dropdown.value)
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
            if (!PhotonNetwork.IsConnected || _roomCreated)
            {
                Debug.Log("NOTCONNECTED"+_roomCreated);
                return;
            }
            if (_password.text.Length != 0)
            {
                _roomOptions.CustomRoomProperties["password"]=_password.text;
            }
            _roomOptions.CustomRoomProperties[DataKeyValues.__ROOM_NAME__] = _roomName.text;
            PhotonNetwork.CreateRoom(_roomName.text, _roomOptions, TypedLobby.Default);
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
           // PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby,null);
           _roomsCanvases.CurrentRoomCanvas.Show(_roomName.text,true);
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
