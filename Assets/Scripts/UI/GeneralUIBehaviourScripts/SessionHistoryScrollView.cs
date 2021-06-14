using System.Collections.Generic;
using System.Linq;
using Data;
using DatabaseScripts;
using Managers;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public class SessionHistoryScrollView : MonoBehaviour
    {
        public Transform ContentParent;
        public SessionHistoryListing SessionHistoryListingPrefab;
        public SessionHistoryDetailPanelBehaviour SessionHistoryDetailPanelBehaviourPrefab;
        public GameObject NoHistoryAvailableText;
        private List<SessionHistory> _sessionHistoryList;
        private List<SessionHistoryListing> _sessionHistoryListings;
        
        public void Initialize()
        {
            _sessionHistoryListings = ContentParent.GetComponentsInChildren<SessionHistoryListing>().ToList();
            _sessionHistoryListings.ForEach(x=>Destroy(x.gameObject));
            _sessionHistoryListings.Clear();
            _sessionHistoryList = DatabaseConnection.GetSessionHistories(GameManager.GameSettings.NickName);
            _sessionHistoryList?.ForEach(listing =>
            {
                if(listing.SpeechText.ToString().Length!=0)
                {
                    SessionHistoryListing listingInstantiate = Instantiate(SessionHistoryListingPrefab, ContentParent);
                    var obj = _sessionHistoryListings.Find(x => x.SessionHistory.SessionID == listing.SessionID);
                    if (!obj || !string.IsNullOrEmpty(listing.UserRole))
                    {
                        listingInstantiate.gameObject.SetActive(true);
                        listingInstantiate.SessionHistory = listing;
                        listingInstantiate.SetHistoryListing();
                        _sessionHistoryListings.Add(listingInstantiate);
                        SessionHistoryDetailPanelBehaviour detailPanelBehaviour =
                            Instantiate(SessionHistoryDetailPanelBehaviourPrefab, transform.parent);
                        detailPanelBehaviour.SetContent(listing);
                        listingInstantiate.DetailPanelBehaviour = detailPanelBehaviour;
                    }
                    else
                    {
                        Destroy(listingInstantiate);
                    }
                }
            });
            NoHistoryAvailableText.SetActive(_sessionHistoryListings.Count==0);
        }
    }
}
