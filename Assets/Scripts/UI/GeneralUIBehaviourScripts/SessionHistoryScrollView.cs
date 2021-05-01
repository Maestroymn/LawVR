using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public class SessionHistoryScrollView : MonoBehaviour
    {
        public SessionHistoryListing SessionHistoryListingPrefab;
        public GameObject NoHistoryAvailableText;
        
        public void Initialize()
        {
            //TODO: Instantiate history listings from DB
        }
    }
}
