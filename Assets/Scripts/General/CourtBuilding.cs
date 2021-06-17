﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Data;
using Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Valve.VR;

namespace General
{
    public class CourtBuilding : MonoBehaviourPunCallbacks
    {
        public event Action SessionEnded;
        public Transform DefendantTransform,PlaintiffTransform,JudgeTransform;
        public List<Transform> SpectatorTransforms;
        public Timer DefendantTimer, PlaintiffTimer;
        public InteractableCourtObject PlaintiffStartButton, DefendantStartButton;
        public List<InteractableCourtObject> InteractableCourtObjects;
        public int TotalTurnCountMax;
        public GameObject VRInitialize;
        public SteamVR_Action_Boolean clickAction;
        public SteamVR_Action_Boolean grabAction;
        public SteamVR_Input_Sources trackedObject;

        private int _currentTurnCount=0;
        private bool _plaintiffTurn=false;
        private int _currentSeatIndexForSpec=0;
        private void Awake()
        {
            InteractableCourtObjects = GetComponentsInChildren<InteractableCourtObject>().ToList();
            if (PlayerPrefs.GetInt(DataKeyValues.__VR_ENABLE__) == 1 && PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString()!=DataKeyValues.__SPECTATOR__)
            {
                VRInitialize.SetActive(true);
                InteractableCourtObjects.ForEach(x => clickAction.onStateUp += x.OnInteractionClicked);
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                _currentTurnCount++;
                _plaintiffTurn = true;
                PlaintiffTimer.timeText.text = "START!";
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Start);
                DefendantTimer.HandleTimer(false);
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);   
            }
        }

        public void InitTimers(float timeLimit)
        {
            DefendantTimer.timeLimit = timeLimit;
            PlaintiffTimer.timeLimit = timeLimit;
            DefendantTimer.HandleTimer(false);
            PlaintiffTimer.timeText.text = "START!";
            DefendantTimer.OnTimesUp += SwitchTurn;
            PlaintiffTimer.OnTimesUp += SwitchTurn;
        }

        public void InstantiateNextSpectator(GameObject CharacterModel)
        {
            var obj= GameManager.NetworkInstantiate(CharacterModel, SpectatorTransforms[_currentSeatIndexForSpec].transform.position, Quaternion.identity);
            obj.GetComponent<CourtSpectatorBehaviour>().Initialize();
            photonView.RPC("IncreaseSeatNumber",RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void IncreaseSeatNumber()
        {
            _currentSeatIndexForSpec++;
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
            LeanTween.delayedCall(2f, () =>
            {
                Cursor.visible = true;
                SceneManager.LoadScene(DataKeyValues.__MAIN_UI_SCENE__);
            });
        }
        
        #region RPC Funcs

        public void SwitchTurn()
        {
            Debug.Log("SWITCHING TURN");
            photonView.RPC("RPC_SwitchTurn",RpcTarget.AllBuffered);
        }
        
        public void StartTurnForCurrentUser()
        {
            Debug.Log("STARTING TURN");
            photonView.RPC("RPC_StartTurn",RpcTarget.AllBuffered);
        }
        
        [PunRPC]
        private void RPC_SwitchTurn()
        {
            if (_currentTurnCount % 2 == 0)
            {
                print("PLAINTIFF TURN!!!!");
                _plaintiffTurn = true;
                PlaintiffTimer.timeText.text = "START!";
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Start);
                DefendantTimer.HandleTimer(false);
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);
            }
            else
            {
                print("DEFENDANT TURN!!!!");
                _plaintiffTurn = false;
                PlaintiffTimer.HandleTimer(false);
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Wait);
                DefendantTimer.timeText.text = "START!";
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Start);
            }
            _currentTurnCount++;
            if (_currentTurnCount > TotalTurnCountMax)
            {
                // SESSION FINISHED HERE
                SessionEnded?.Invoke();
                string SessionID = PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__].ToString();
                string UserRole = PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString();
                string UserName = GameManager.GameSettings.NickName;
                if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__] && PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__LANGUAGE__].ToString() == "tr-TR")
                    AIJudgeGeneralBehaviour.AIJudgeDecisionCaller(SessionID, UserRole, UserName);
                Disconnect();
            }
        }

        public void OnJudgeEndsSession()
        {
            SessionEnded?.Invoke();
            gameObject.LeanDelayedCall(2f, Disconnect);
        }
        
        
        [PunRPC]
        private void RPC_StartTurn()
        {
            if (!_plaintiffTurn)
            {
                PlaintiffTimer.HandleTimer(false);
                PlaintiffTimer.timeText.text = "WAIT!";
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Wait);
                DefendantTimer.HandleTimer(true);
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Pass);
            }
            else
            {
                PlaintiffTimer.HandleTimer(true);
                PlaintiffStartButton.HandleButtonSettings(ButtonStatus.Pass);
                DefendantTimer.HandleTimer(false);
                DefendantTimer.timeText.text = "WAIT!";
                DefendantStartButton.HandleButtonSettings(ButtonStatus.Wait);
            }
        }
        
        #endregion
    }
}
