using System;
using System.Collections.Generic;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;
using Random = UnityEngine.Random;
using DatabaseScripts;
using TMPro;

namespace Managers
{
    public class FriendListManager : MonoBehaviour
    {
        [SerializeField] private FriendListing _friendListingPrefab;
        [SerializeField] private Transform _contentParent;
        private List<FriendListing> _friends  = new List<FriendListing>();

        public void RefreshFriendList()
        {
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
            FriendListing friendListing = Instantiate(_friendListingPrefab, _contentParent);
            friendListing.SetUserName(name);
            friendListing.SetAvailability(isOnline);
            if (!_friends.Contains(friendListing))
            {
                _friends.Add(friendListing);
            }
        }


    }
}