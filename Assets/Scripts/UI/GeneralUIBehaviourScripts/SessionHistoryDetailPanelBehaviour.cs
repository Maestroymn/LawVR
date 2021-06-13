using Data;
using TMPro;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public class SessionHistoryDetailPanelBehaviour : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _positiveKeywords,_negativeKeywords,_result, _logArea,_role,_simType,_startTime,_caseName;
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
            _positiveKeywords.text = _sessionHistory?.Feedback.GetPositiveKeywords();
            _negativeKeywords.text = _sessionHistory?.Feedback.GetNegaiveKeywords();
            _result.text = _sessionHistory?.Feedback.GetResult();
            _logArea.text = _sessionHistory.SpeechText;
            _simType.text = _sessionHistory.SimulationType;
            _startTime.text = _sessionHistory.StartTime;
            _role.text = _sessionHistory.UserRole;
        }
    }
}
