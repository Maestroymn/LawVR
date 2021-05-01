using UI;
using UnityEngine;

namespace Data
{
    public abstract class User
    {
        public string Name;
        public RoleClaimButton.RoleType RoleType;
    }
    public class SessionHistory : MonoBehaviour
    {
        public int CaseID, SessionID;
        public string StartTime, EndTime, Feedback, SimulationType;
        public User[] Users;
    }
}
