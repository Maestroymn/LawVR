using Data;
using TMPro;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public class SessionHistoryListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _date, _role, _simulationType, _lobbyName;
        [HideInInspector] public SessionHistory SessionHistory;
        [HideInInspector] public SessionHistoryDetailPanelBehaviour DetailPanelBehaviour;
        
        public void SetHistoryListing()
        {
            _date.text = SessionHistory.StartTime;
            _role.text = SessionHistory.UserRole;
            _simulationType.text = SessionHistory.SimulationType;
            _lobbyName.text = SessionHistory.LobbyName;
        }

        public void ShowDetail()
        {
            DetailPanelBehaviour.HandleActivation(true);
        }
        
    }
}
