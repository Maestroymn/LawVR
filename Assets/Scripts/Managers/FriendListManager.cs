using System;
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
        private List<FriendListing> _friends;
        private List<FriendListing> _invitations;

        public void Initialize()
        {
            _friends = new List<FriendListing>();
            _invitations = new List<FriendListing>();
        }
        
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

        private bool _feedbackGiven=false;
        public void AddNewFriend(TextMeshProUGUI NewFriendName)
        {
            if(!_feedbackGiven)
            {
                var invitationStatus = DatabaseConnection.SendFriendshipRequest(NewFriendName.text);
                _feedbackGiven = true;
                if (invitationStatus == SendInvitationStatus.Sent)
                {
                    NewFriendName.color = Color.green;
                    NewFriendName.text = "Invitation Send!";
                    return;
                }
                if (invitationStatus == SendInvitationStatus.UserDoesntExist)
                {
                    NewFriendName.color = Color.red;
                    NewFriendName.text = "User Doesn't Exist!";
                }
                else if (invitationStatus == SendInvitationStatus.AlreadyExistingInvitation)
                {
                    NewFriendName.color = Color.red;
                    NewFriendName.text = "Already Invited!";
                }else if (invitationStatus == SendInvitationStatus.AlreadyFriend)
                {
                    NewFriendName.color = Color.red;
                    NewFriendName.text = "Already Friend!";
                }
                if (NewFriendName.gameObject.LeanIsTweening())
                {
                    NewFriendName.gameObject.LeanCancel();
                }

                NewFriendName.gameObject.LeanMove(NewFriendName.transform.position + Vector3.right, .1f)
                    .setOnComplete(
                        () =>
                        {
                            NewFriendName.gameObject.LeanMove(NewFriendName.transform.position - Vector3.right,
                                .1f);
                        });
            }
        }
        
        private void AddFriendListing(string name,bool isOnline)
        {
            FriendListing searchedFriend = _friends.Find(friend => friend.GetUserName().Equals(name));
            if (searchedFriend != null)
            {
                searchedFriend.SetAvailability(isOnline);
                return;
            }
            FriendListing friendListing = Instantiate(_friendListingPrefab, _friendListContentParent);
            friendListing.SetUserName(name);
            friendListing.SetAvailability(isOnline);
            friendListing.OnRemoved += OnRemovedFriend;
            if (!_friends.Contains(friendListing))
            {
                _friends.Add(friendListing);
            }
            else
            {
                _friends.Remove(friendListing);
                Destroy(friendListing);
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
            InvitationListing.OnReject+= OnRejectFriend;
            if (!_invitations.Contains(InvitationListing))
            {
                _invitations.Add(InvitationListing);
            }
            else
            {
                _invitations.Remove(InvitationListing);
                Destroy(InvitationListing);
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

        public void ResetAddFriendTextField(TextMeshProUGUI textField)
        {
            _feedbackGiven = false;
            textField.color=Color.black;
            textField.text=String.Empty;
        }

        private void OnRemovedFriend(FriendListing friendListing)
        {
            friendListing.OnRemoved -= OnRemovedFriend;
            _friends.Remove(friendListing);
            Destroy(friendListing.gameObject);
        }
        
        private void OnRejectFriend(FriendListing friendListing)
        {
            friendListing.OnReject -= OnRejectFriend;
            _invitations.Remove(friendListing);
            Destroy(friendListing.gameObject);
        }
    }
}