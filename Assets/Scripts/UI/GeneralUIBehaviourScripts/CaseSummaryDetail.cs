using System;
using System.Collections.Generic;
using Data;
using DatabaseScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public enum SummaryType
    {
        BeforeSession,
        DuringSession,
    }
    public class CaseSummaryDetail : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _role,_simType,_startTime,_caseName,_turnCount,_turnDuration,_rolesTextField,_caseDetail;
        [SerializeField] private GameObject _readyButton, _closeButton,_InputBlocker;
        public SummaryType SummaryType;
        public event Action OnReady,OnClose;
        private bool _isOpen;
        private Dictionary<string, string> _listOfPlayerswRoles;
        public void Initialize()
        {
            _listOfPlayerswRoles = new Dictionary<string, string>();
            if(SummaryType==SummaryType.BeforeSession)
            {
                _closeButton.SetActive(false);
                _readyButton.SetActive(true);
            }
            var currentRoomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            _startTime.text = DateTime.Now.ToString();
            SetCaseDetail();
            SetRolesTextField();
            _turnCount.text = currentRoomProps[DataKeyValues.__TURN_COUNT__].ToString();
            _turnDuration.text = currentRoomProps[DataKeyValues.__TURN_DURATION__].ToString();
            _role.text = PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString();
            _simType.text = currentRoomProps[DataKeyValues.__SIMULATION_TYPE__].ToString();
            _caseName.text=DatabaseConnection.GetCaseNameById((int)currentRoomProps[DataKeyValues.__CASE_ID__]);
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            OpenSummaryFolder();
        }

        private void SetCaseDetail()
        {
            var cases = DatabaseConnection.GetCourtCases();
            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i].CaseID == (int) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__CASE_ID__])
                {
                    _caseDetail.text = cases[i].CaseText;
                    break;
                }
            }
        }

        private void SetRolesTextField()
        {
            foreach (var currentRoomPlayer in PhotonNetwork.CurrentRoom.Players)
            {
                _listOfPlayerswRoles.Add(currentRoomPlayer.Value.CustomProperties[DataKeyValues.__ROLE__].ToString(),currentRoomPlayer.Value.NickName);
            }
            var str = "JUDGE: " + GetUserNameByRole(DataKeyValues.__JUDGE__) + "\n" +
                      "PLAINTIFF: " + GetUserNameByRole(DataKeyValues.__PLAINTIFF__) + "\n" +
                      "DEFENDANT: " + GetUserNameByRole(DataKeyValues.__DEFENDANT__) + "\n";
            _rolesTextField.text = str;
        }

        private string GetUserNameByRole(string role)
        {
            if (_listOfPlayerswRoles.ContainsKey(role))
            {
                return _listOfPlayerswRoles[role];
            }
            if(role == DataKeyValues.__JUDGE__)
            {
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__])
                {
                    return "NonHuman User";
                }
            }
            return "EMPTY";
        }
        
        public void OpenSummaryFolder()
        {
            if (_isOpen) return;
            _InputBlocker.SetActive(true);
            _isOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            switch (SummaryType)
            {
                case SummaryType.BeforeSession:
                    _closeButton.SetActive(false);
                    _readyButton.SetActive(true);
                    break;
                case SummaryType.DuringSession:
                    _closeButton.SetActive(true);
                    _readyButton.SetActive(false);
                    break;
            }
            gameObject.LeanScale(Vector3.one, .3f);
        }
        
        public void OnReadyClicked()
        {
            if (!_isOpen) return;
            _InputBlocker.SetActive(false);
            gameObject.LeanScale(Vector3.zero, .3f).setOnComplete(() =>
            {
                _isOpen = false;
                SummaryType = SummaryType.DuringSession;
                _closeButton.SetActive(true);
                _readyButton.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                OnReady?.Invoke();
            });
        }
        
        public void OnCloseClicked()
        {
            if (!_isOpen) return;
            _InputBlocker.SetActive(false);
            gameObject.LeanScale(Vector3.zero, .3f).setOnComplete(() =>
            {
                _isOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                OnClose?.Invoke();
            });
        }

        public void DirectlyClose()
        {
            if (!_isOpen) return;
            if (gameObject.LeanIsTweening())
            {
                gameObject.LeanCancel();
            }
            SummaryType = SummaryType.DuringSession;
            _closeButton.SetActive(true);
            _readyButton.SetActive(false);
            _InputBlocker.SetActive(false);
            _isOpen = false;
            transform.localScale=Vector3.zero;
            OnClose?.Invoke();
        }

    }
}