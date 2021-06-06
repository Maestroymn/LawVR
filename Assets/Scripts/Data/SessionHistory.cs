using UnityEngine;

namespace Data
{

    public class SessionHistory : MonoBehaviour
    {
        public int CaseID, SessionID, TurnCount, TurnDuration;
        public string StartTime, EndTime, SimulationType, SpeechText, UserRole,CaseName,LobbyName;
        public UserFeedback Feedback;
    }
}