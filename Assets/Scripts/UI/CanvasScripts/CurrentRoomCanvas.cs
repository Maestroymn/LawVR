using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CurrentRoomCanvas : MonoBehaviour
    {
        private RoomsCanvases _roomsCanvases;
        [SerializeField] private TextMeshProUGUI _roomName;
        [SerializeField] private Button StartSessionButton, ReadyButton;
        [SerializeField] private List<RoleClaimButton> _roleClaimButtons;
        public void FirstInitialize(RoomsCanvases roomsCanvases)
        {
            _roomsCanvases = roomsCanvases;
        }

        public void Show(string roomName,bool isHost)
        {
            _roleClaimButtons.ForEach(x=>x.Initialize());
            _roomName.text = roomName;
            if (isHost)
            {
                ReadyButton.gameObject.SetActive(false);
                SetStatusForStartSessionButton(false);
            }
            else
            {
                StartSessionButton.gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
            _roomsCanvases.HostRoomCanvas.Hide();
        }

        public void SetStatusForStartSessionButton(bool isActive)
        {
            var image = StartSessionButton.image;
            var color = image.color;
            if(isActive)
            {
                StartSessionButton.interactable = true;
                color.a = 1f;
                image.color = color;
            }
            else
            {
                StartSessionButton.interactable = false;
                color.a = .5f;
                image.color = color;
            }
        }
        
        public void Hide()
        {
            StartCoroutine(Disconnect());
        }

        private IEnumerator Disconnect()
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;
            PhotonNetwork.LocalPlayer.CustomProperties["Role"] = "none";
            gameObject.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                _roomsCanvases.HostRoomCanvas.Show();
                _roomsCanvases.SetCurrentActiveCanvas(_roomsCanvases.HostRoomCanvas.gameObject);
            }
            else
            {
                _roomsCanvases.JoinRoomCanvas.Show();
                _roomsCanvases.SetCurrentActiveCanvas(_roomsCanvases.JoinRoomCanvas.gameObject);
            }
        }
    }
}
