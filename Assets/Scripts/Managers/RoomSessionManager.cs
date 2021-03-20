using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Managers
{
    public class RoomSessionManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject judge,plaintiff,spectator,defendant;
        [SerializeField] private Transform _defendantTransform,_plaintiffTransform,_judgeTransform,_spectatorTransform;
        private void Awake()
        {
            HandleSpawns();
        }

        public void HandleSpawns()
        {
            switch (PhotonNetwork.LocalPlayer.CustomProperties["Role"].ToString())
            {
                case "Plaintiff":
                    GameManager.NetworkInstantiate(plaintiff, _defendantTransform.position, Quaternion.identity);
                    break;
                case "Defendant":
                    GameManager.NetworkInstantiate(defendant, _plaintiffTransform.position, Quaternion.identity);
                    break;
                case "Judge":
                    GameManager.NetworkInstantiate(judge, _judgeTransform.position, Quaternion.identity);
                    break;
                case "Spectator":
                    GameManager.NetworkInstantiate(spectator, _spectatorTransform.position, Quaternion.identity);
                    break;
            }
        }

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
    }
}
