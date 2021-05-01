using Data;
using TMPro;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public class SessionHistoryListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _date, _role, _simulationType;
        public SessionHistory SessionHistory;
        
        public void SetHistoryListing(string date,string role, string simType)
        {
            _date.text = date;
            _role.text = role;
            _simulationType.text = simType;
        }
        
    }
}
