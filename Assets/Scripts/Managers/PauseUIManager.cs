using System.Collections;
using Data;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class PauseUIManager : MonoBehaviourPunCallbacks
    {
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
        }
        
        public void Disconnect()
        { 
            StartCoroutine(DisconnectFromRoom());
        }

        private IEnumerator DisconnectFromRoom()
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;
            Cursor.visible = true;
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Cursor.visible = true;
            PhotonNetwork.Reconnect();
            SceneManager.LoadScene(DataKeyValues.__LOGIN_SCENE__);
        }
    }
}
