using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class PauseUIManager : MonoBehaviourPunCallbacks
    {

        public GameObject PauseCanvas;
        private bool paused = false;

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
                    PausePanel(true);
                }
                else
                {
                    PausePanel(false);
                }
            }
        }

        public void PausePanel(bool isPaused)
        {
            PauseCanvas.SetActive(isPaused);
            paused = isPaused;
        }
        
        public void Disconnect()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            //SceneManager.LoadScene(1);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }
    }
}
