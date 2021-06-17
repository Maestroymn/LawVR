using System.Collections;
using Data;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using AI;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
using UnityEngine.XR;

namespace Managers
{
    public class PauseUIManager : MonoBehaviourPunCallbacks
    {
        public event Action OnPaused;
        public GameObject PauseCanvas;
        public Camera Camera;
        public bool paused = false;
        public Button ResumeButton;
        // Start is called before the first frame update
        void Start()
        {
            PauseCanvas.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
                return;
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
            if (PlayerPrefs.GetInt(DataKeyValues.__VR_ENABLE__) == 1)
            {
                if (isPaused) StartCoroutine(ActivateVR("None"));
                else StartCoroutine(ActivateVR("OpenVR"));
            }

            
            Camera.gameObject.SetActive(isPaused);
            PauseCanvas.SetActive(isPaused);
            paused = isPaused;
            Cursor.visible = isPaused;
            Cursor.lockState = !isPaused ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        public void Disconnect()
        {
            if (PlayerPrefs.GetInt(DataKeyValues.__VR_ENABLE__) == 1)
                StartCoroutine(ActivateVR("None"));
            StartCoroutine(DisconnectFromRoom());
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
        public IEnumerator ActivateVR(string devName)
        {
            XRSettings.LoadDeviceByName(devName);
            yield return null;
            XRSettings.enabled = devName=="None"?false:true;

        }
    }
}
