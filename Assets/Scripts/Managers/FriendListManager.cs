﻿using System;
using System.Collections.Generic;
using System.Linq;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;
using DatabaseScripts;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class FriendListManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private FriendListing _friendListingPrefab, _waitingListingPrefab;
        [SerializeField] private Transform _friendListScrollView,_waitingInvitationsScrollView;
        [SerializeField] private Transform _friendListContentParent,_waitingInvitationsContentParent;
        [SerializeField] private Button _friendListTabButton, _waitingListTabButton;
        private List<FriendListing> _friends;
        private List<FriendListing> _invitations;
        private List<FriendListing> _friendsToBeRemoved;
        private List<FriendListing> _invitesToBeRemoved;
        public void Awake()
        {
            _friendsToBeRemoved = new List<FriendListing>();
            _invitesToBeRemoved = new List<FriendListing>();
            _friends = new List<FriendListing>();
            _invitations = new List<FriendListing>();
        }

        public void CleanFirst()
        {
            var objsToClean = _friendListContentParent.GetComponentsInChildren<Image>().ToList();
            objsToClean.ForEach(x=>
            {
                if(x.name.Contains("FriendListing"))
                    Destroy(x.gameObject);
            });
            objsToClean = _waitingInvitationsContentParent.GetComponentsInChildren<Image>().ToList();
            objsToClean.ForEach(x=>
            {
                if(x.name.Contains("WaitingInvitationsListing"))
                    Destroy(x.gameObject);
            });
        }
        
        public void RefreshFriendList()
        {
            List<Friend> DatabaseFriendList = DatabaseConnection.RetrieveFriendList(GameManager.GameSettings.NickName);
            if (DatabaseFriendList.Count != 0)
            {
                foreach (var friend in DatabaseFriendList)
                {
                    AddFriendListing(friend.Name, friend.IsOnline);
                }
                _friends.ForEach(friend =>
                {
                    if (DatabaseFriendList.Find(x=>x.Name.Equals(friend.GetUserName())).Equals(null))
                    {
                        _friendsToBeRemoved.Add(friend);
                    }

                });
                if (_friendsToBeRemoved.Count != 0)
                {
                    _friendsToBeRemoved.ForEach(OnRemovedFriend);
                    _friendsToBeRemoved.Clear();
                }
            }
        }

        public void ClearLists()
        {
            _friends.ForEach(Destroy);
            _friends.Clear();
            _friendsToBeRemoved.ForEach(Destroy);
            _friendsToBeRemoved.Clear();
            _invitations.ForEach(Destroy);
            _invitations.Clear();
            _invitesToBeRemoved.ForEach(Destroy);
            _invitesToBeRemoved.Clear();
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
                _invitations.ForEach(invitation =>
                {
                    if (!DatabaseInvitationList.ContainsKey(invitation.GetUserName()))
                    {
                        _invitesToBeRemoved.Add(invitation);
                    }
                });
                if (_invitesToBeRemoved.Count != 0)
                {
                    _invitesToBeRemoved.ForEach(OnRejectFriend);
                    _invitesToBeRemoved.Clear();
                }
            }
        }

        private bool _feedbackGiven=false;
        public void AddNewFriend(TextMeshProUGUI NewFriendName)
        {
            if(!_feedbackGiven && NewFriendName.text.Substring(0, NewFriendName.text.Length - 1) != GameManager.GameSettings.NickName && !_invitations.Find(x=> x.GetUserName()==NewFriendName.text.Substring(0, NewFriendName.text.Length - 1)))
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