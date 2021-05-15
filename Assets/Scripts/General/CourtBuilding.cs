using System.Collections.Generic;
using System.Linq;
using Managers;
using Photon.Pun;
using UnityEngine;
using Utilities;

namespace General
{
    public class CourtBuilding : MonoBehaviourPunCallbacks
    {
        public Transform DefendantTransform,PlaintiffTransform,JudgeTransform,SpectatorTransform;
        public Timer DefendantTimer, PlaintiffTimer;
        public InteractableCourtObject PlaintiffStartButton, DefendantStartButton;
        public List<InteractableCourtObject> InteractableCourtObjects;
        public int TotalTurnCountMax;
        private int _currentTurnCount=0;
        private bool _plaintiffTurn;
        
        private void Awake()
        {
            InteractableCourtObjects = GetComponentsInChildren<InteractableCourtObject>().ToList();
        }

        public void InitTimers(float timeLimit)
        {
            DefendantTimer.timeLimit = timeLimit;
            DefendantTimer.timeText.text = "WAIT";
            PlaintiffTimer.timeText.text = "WAIT";
            PlaintiffTimer.timeLimit = timeLimit;
        }
        
        #region RPC Funcs

        public void SwitchTurn()
        {
            print("SWITCHING TURN");
            photonView.RPC("RPC_SwitchTurn",RpcTarget.All);
        }
        
        public void StartTurnForCurrentUser()
        {
            print("STARTING TURN");
            photonView.RPC("RPC_StartTurn",RpcTarget.All);
        }
        
        [PunRPC]
        private void RPC_SwitchTurn()
        {
            _currentTurnCount++;
            if (_currentTurnCount > TotalTurnCountMax)
            {
                // SESSION FINISHED HERE
                print("SESSION ENDED!!!!");
            }
            if (_plaintiffTurn)
            {
                _plaintiffTurn = false;
                PlaintiffTimer.HandleTimer(false);
                DefendantTimer.timeText.text = "START!";
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Start);
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Wait);
          
            }
            else
            {
                _plaintiffTurn = true;
                DefendantTimer.HandleTimer(false);
                PlaintiffTimer.timeText.text = "START!";
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Start);
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);
            }
        }
        
        [PunRPC]
        private void RPC_StartTurn()
        {
            if (_plaintiffTurn)
            {
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Pass);
                PlaintiffTimer.HandleTimer(true);
            }
            else
            {
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Wait);
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Pass);
                DefendantTimer.HandleTimer(true);
            }
        }

        public void SetCourtBuildingForAll()
        {
            photonView.RPC("RPC_SetCourtBuilding",RpcTarget.All);    
        }
        
        [PunRPC]
        public void RPC_SetCourtBuilding()
        {
            FindObjectOfType<RoomSessionManager>()._currentBuilding = this;
        }
        #endregion
    }
}
