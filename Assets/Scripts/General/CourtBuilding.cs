using System.Collections.Generic;
using System.Linq;
using AI;
using Data;
using Managers;
using Photon.Pun;
using UnityEngine;
using Utilities;

namespace General
{
    public class CourtBuilding : MonoBehaviourPunCallbacks
    {
        public Transform DefendantTransform,PlaintiffTransform,JudgeTransform;
        public List<Transform> SpectatorTransforms;
        public Timer DefendantTimer, PlaintiffTimer;
        public InteractableCourtObject PlaintiffStartButton, DefendantStartButton;
        public List<InteractableCourtObject> InteractableCourtObjects;
        public int TotalTurnCountMax;
        private int _currentTurnCount=0;
        private bool _plaintiffTurn=false;
        private int _currentSeatIndexForSpec=0;
        private void Awake()
        {
            InteractableCourtObjects = GetComponentsInChildren<InteractableCourtObject>().ToList();
        }

        public void InitTimers(float timeLimit)
        {
            DefendantTimer.OnTimesUp += SwitchTurn;
            PlaintiffTimer.OnTimesUp += SwitchTurn;
            DefendantTimer.timeLimit = timeLimit;
            DefendantTimer.timeText.text = "WAIT";
            PlaintiffTimer.timeText.text = "WAIT";
            PlaintiffTimer.timeLimit = timeLimit;
        }

        public void InstantiateNextSpectator(GameObject CharacterModel)
        {
            var obj= GameManager.NetworkInstantiate(CharacterModel, SpectatorTransforms[_currentSeatIndexForSpec++].transform.position, Quaternion.identity);
            obj.GetComponent<CourtSpectatorBehaviour>().Initialize();
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
                string SessionID = PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__].ToString();
                string UserRole = PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString();
                string UserName = GameManager.GameSettings.NickName;
                if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__] && PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__LANGUAGE__].ToString() == "tr-TR")
                    AIJudgeGeneralBehaviour.AIJudgeDecisionCaller(SessionID, UserRole, UserName);
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
        
        #endregion
    }
}
