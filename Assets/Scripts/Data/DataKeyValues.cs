using UnityEngine;

namespace Data
{
    public class DataKeyValues : MonoBehaviour
    {
        #region ROOMKEYS
        public static readonly string __PASSWORD_KEY__ = "password";
        public static readonly string __SIMULATION_TYPE__ = "simulation_type";
        public static readonly string __AI_JUDGE__ = "ai_judge";
        public static readonly string __ROOM_NAME__ = "room_name";
        public static readonly string __SESSION_ID__ = "session_id";
        public static readonly string __CASE_ID__ = "case_id";
        #endregion
        
        #region ROLE KEYS
        public static readonly string __PLAINTIFF__ = "plaintiff";
        public static readonly string __DEFENDANT__ = "defendant";
        public static readonly string __SPECTATOR__ = "spectator";
        public static readonly string __JUDGE__ = "judge";
        public static readonly string __ROLE__ = "Role";
        #endregion
        
        #region SIMULATION MODES
        public static readonly string __COMPETITION_MODE__ = "competition";
        public static readonly string __EDUCATIONAL_MODE__ = "educational";
        public static readonly string __SANDBOX_MODE__ = "sandbox";
        public static readonly string __CHALLENGE_MODE__ = "challenge";
        #endregion

        #region SCENE TAGS
        public static readonly int __LOGIN_SCENE__ = 0;
        public static readonly int __MAIN_UI_SCENE__ = 1;
        public static readonly int __COURT_SCENE__ = 2; 
        public static readonly int __FEMALE_CUSTOMIZATION_SCENE__ = 3;
        public static readonly int __MALE_CUSTOMIZATION_SCENE__ = 4;
        #endregion

        public static readonly string __VR_ENABLE__ = "__VR_ENABLE__";
    }
}
