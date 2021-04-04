using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GeneralUIBehaviourScripts
{
    public class FriendListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _username;
        [SerializeField] private Image _availability;
        
        public void SetUserName(string name)
        {
            _username.text = name;
        }

        public void SetAvailability(bool isOnline)
        {
            _availability.color = isOnline ? Color.green : Color.gray;
        }
        
        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}
