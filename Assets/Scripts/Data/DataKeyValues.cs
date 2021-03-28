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
        #endregion
        
        #region ROLE KEYS
        public static readonly string __PLAINTIFF__ = "plaintiff";
        public static readonly string __DEFENDANT__ = "defendant";
        public static readonly string __SPECTATOR__ = "spectator";
        public static readonly string __JUDGE__ = "judge";
        #endregion
        
        #region SIMULATION MODES
        public static readonly string __COMPETITION_MODE__ = "competition";
        public static readonly string __EDUCATIONAL_MODE__ = "educational";
        public static readonly string __SANDBOX_MODE__ = "sandbox";
        public static readonly string __CHALLENGE_MODE__ = "challenge";
        #endregion
    }
}
