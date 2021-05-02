using Data;
using TMPro;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public class SessionHistoryDetailPanelBehaviour : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _feedbackArea, _logArea,_role,_simType,_startTime,_caseName;
        private SessionHistory _sessionHistory;
        
        public void HandleActivation(bool isOpen)
        {
            if (isOpen && !_sessionHistory)
            {
                SetContent(_sessionHistory);
            }
            transform.LeanScale(!isOpen ? Vector3.zero : Vector3.one, .3f);
        }

        public void SetContent(SessionHistory sessionHistory)
        {
            _sessionHistory = sessionHistory;
            _caseName.text = _sessionHistory.CaseName;
            _feedbackArea.text = _sessionHistory.Feedback;
            _logArea.text = _sessionHistory.SpeechText;
            _simType.text = _sessionHistory.SimulationType;
            _startTime.text = _sessionHistory.StartTime;
            _role.text = _sessionHistory.UserRole;
        }
    }
}
