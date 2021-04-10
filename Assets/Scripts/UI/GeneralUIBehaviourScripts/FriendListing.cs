using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DatabaseScripts;

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
        public string GetUserName()
        {
            return _username.text ;
        }


        public void SetAvailability(bool isOnline)
        {
            _availability.color = isOnline ? Color.green : Color.gray;
        }
        
        public void Remove()
        {

            DatabaseConnection.RemoveFriend(GetUserName());
            Destroy(gameObject);
        }

        public void RejectInvitation()
        {
            DatabaseConnection.RejectFriendshipRequest(_username.text);
            Destroy(gameObject);
        }

        public void AcceptInvitation()
        {
            DatabaseConnection.AcceptFriendshipRequest(_username.text);
            Destroy(gameObject);
        }
    }
}
