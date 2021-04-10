using System.Collections.Generic;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;
using DatabaseScripts;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class FriendListManager : MonoBehaviour
    {
        [SerializeField] private FriendListing _friendListingPrefab;
        [SerializeField] private Transform _friendListContentParent,_waitingInvitationsContentParent;
        [SerializeField] private Button _friendListTabButton, _waitingListTabButton;
        private List<FriendListing> _friends  = new List<FriendListing>();

        public void RefreshFriendList()
        {
            //Update needed with new table structure 
            /*
            string DatabaseFriendList = DatabaseConnection.RetrieveFriendList(GameManager.GameSettings.NickName);
            if(DatabaseFriendList!="")
            {
                DatabaseFriendList = DatabaseFriendList.Substring(1, DatabaseFriendList.Length - 2); // remove {} at the beginning and the end
                string[] friends = DatabaseFriendList.Split(',');

                bool[] FriendsOnlineStatus = DatabaseConnection.RetrieveFriendStatus(friends);


                for (var i = 0; i < friends.Length; i++)
                {
                    Debug.Log(friends[i] + " " + FriendsOnlineStatus[i]);
                    AddFriendListing(friends[i], FriendsOnlineStatus[i]);
                }

            }
            */

        }

        public void AddNewFriend(TextMeshProUGUI NewFriendName)
        {
            DatabaseConnection.AddFriend(NewFriendName.text);

        }
        
        private void AddFriendListing(string name,bool isOnline)
        {
            FriendListing searchedFriend = _friends.Find(friend => friend.GetUserName()== name);
            if (searchedFriend != null)
            {
                searchedFriend.SetAvailability(isOnline);
                return;
            }
            FriendListing friendListing = Instantiate(_friendListingPrefab, _friendListContentParent);
            friendListing.SetUserName(name);
            friendListing.SetAvailability(isOnline);
            if (!_friends.Contains(friendListing))
            {
                _friends.Add(friendListing);
            }
        }

        public void ChangeTab(bool friendListTabOpen)
        {
            if (friendListTabOpen)
            {
                _waitingInvitationsContentParent.gameObject.SetActive(false);
                _friendListContentParent.gameObject.SetActive(true);
                _friendListTabButton.interactable = false;
                _waitingListTabButton.interactable = true;
            }
            else
            {
                _friendListContentParent.gameObject.SetActive(false);
                _waitingInvitationsContentParent.gameObject.SetActive(true);
                _friendListTabButton.interactable = true;
                _waitingListTabButton.interactable = false;
            }
        }

    }
}