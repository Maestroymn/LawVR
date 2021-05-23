using System.Collections;
using Data;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseScripts;
using System;
using AI;

namespace Managers
{
    public class PauseUIManager : MonoBehaviourPunCallbacks
    {
        public event Action OnPaused;
        public GameObject PauseCanvas;
        public Camera Camera;
        public bool paused = false;

        // Start is called before the first frame update
        void Start()
        {
            PauseCanvas.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!paused)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    PausePanel(true);
                    OnPaused?.Invoke();
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    PausePanel(false);
                }
            }
        }

        public void PausePanel(bool isPaused)
        {
            Camera.gameObject.SetActive(isPaused);
            PauseCanvas.SetActive(isPaused);
            paused = isPaused;
            Cursor.visible = isPaused;
            Cursor.lockState = !isPaused ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        public void Disconnect()
        {

            AIJudgeGeneralBehaviour.AIJudgeDecisionCaller(PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__].ToString() , PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString());
            LeanTween.delayedCall(3f,()=> {
                StartCoroutine(DisconnectFromRoom());
            });
            
        }

        private IEnumerator DisconnectFromRoom()
        {
            
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;
            Cursor.visible = true;
            SceneManager.LoadScene(DataKeyValues.__MAIN_UI_SCENE__);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Cursor.visible = true;
            PhotonNetwork.Reconnect();
            SceneManager.LoadScene(DataKeyValues.__LOGIN_SCENE__);
        }
    }
}
