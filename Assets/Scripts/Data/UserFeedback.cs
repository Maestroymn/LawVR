using UnityEngine;



namespace Data
{
    public class UserFeedback : MonoBehaviour
    {
        public int SessionID, FeedbackID;
        public string UserName, Result, PositiveKeywords, NegativeKeywords, UserRole;

        public string GetPositiveKeywords()
        {
            if (PositiveKeywords.Length != 0)
            {
                return PositiveKeywords;
            }
            return "N/A";
        }
        
        public string GetNegaiveKeywords()
        {
            if (NegativeKeywords.Length != 0)
            {
                return NegativeKeywords;
            }
            return "N/A";
        }
        
        public string GetResult()
        {
            if (Result.Length != 0)
            {
                return Result;
            }
            return "N/A";
        }
    }

}