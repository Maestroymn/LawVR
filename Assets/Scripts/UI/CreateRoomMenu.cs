using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

namespace UI
{
    public class CreateRoomMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _roomName, _password;

        private RoomsCanvases _roomsCanvases;   
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
        }
        public void OnClickCreateRoom()
        {
            RoomOptions _roomOptions = new RoomOptions();
            //TypedLobby typedLobby = new TypedLobby("competitive",);
            _roomOptions.MaxPlayers = 16;
            _roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            _roomOptions.CustomRoomProperties.Add("password",_password.text);
            PhotonNetwork.JoinOrCreateRoom(_roomName.text,_roomOptions,TypedLobby.Default);
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log("Room is created its name is "+_roomName.text+" and its password: "+_password.text);
            _roomsCanvases.CurrentRoomCanvas.Show(_roomName.text);
           // PhotonNetwork.JoinRoom(_roomName.text);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room couldn't be created ",_roomName);
        }
    }
}
