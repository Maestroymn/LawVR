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
        [SerializeField] private FriendListing _friendListingPrefab, _waitingListingPrefab;
        [SerializeField] private Transform _friendListScrollView,_waitingInvitationsScrollView;
        [SerializeField] private Transform _friendListContentParent,_waitingInvitationsContentParent;
        [SerializeField] private Button _friendListTabButton, _waitingListTabButton;
        private List<FriendListing> _friends  = new List<FriendListing>();
        private List<FriendListing> _invitations  = new List<FriendListing>();

        public void RefreshFriendList()
        {
            Dictionary<string, bool> DatabaseFriendList = DatabaseConnection.RetrieveFriendList(GameManager.GameSettings.NickName);
            if (DatabaseFriendList.Count != 0)
            {
                foreach (var friend in DatabaseFriendList)
                {
                    AddFriendListing(friend.Key, friend.Value);
                }
            }
        }

        public void RefreshInvitationList()
        {
            Dictionary<string, bool> DatabaseInvitationList = DatabaseConnection.ListFriendshipRequests(GameManager.GameSettings.NickName);
            if (DatabaseInvitationList.Count != 0)
            {
                foreach (var Invitation in DatabaseInvitationList)
                {
                    AddInvitationListing(Invitation.Key, Invitation.Value);
                }
            }
        }

        public void AddNewFriend(TextMeshProUGUI NewFriendName)
        {
            DatabaseConnection.SendFriendshipRequest(NewFriendName.text);

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

        private void AddInvitationListing(string name,bool isOnline)
        {
            FriendListing InvitationFriend = _invitations.Find(friend => friend.GetUserName()== name);
            if (InvitationFriend != null)
            {
                InvitationFriend.SetAvailability(isOnline);
                return;
            }
            FriendListing InvitationListing = Instantiate(_waitingListingPrefab, _waitingInvitationsContentParent);
            InvitationListing.SetUserName(name);
            InvitationListing.SetAvailability(isOnline);
            if (!_invitations.Contains(InvitationListing))
            {
                _invitations.Add(InvitationListing);
            }
        }

        public void ChangeTab(bool friendListTabOpen)
        {
            if (friendListTabOpen)
            {
                RefreshFriendList();
                _waitingInvitationsScrollView.gameObject.SetActive(false);
                _friendListScrollView.gameObject.SetActive(true);
                _friendListTabButton.interactable = false;
                _waitingListTabButton.interactable = true;
            }
            else
            {
                RefreshInvitationList();
                _friendListScrollView.gameObject.SetActive(false);
                _waitingInvitationsScrollView.gameObject.SetActive(true);
                _friendListTabButton.interactable = true;
                _waitingListTabButton.interactable = false;
                
            }

        }

    }
}