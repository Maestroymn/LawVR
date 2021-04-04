using System;
using System.Collections.Generic;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class FriendListManager : MonoBehaviour
    {
        [SerializeField] private FriendListing _friendListingPrefab;
        [SerializeField] private Transform _contentParent;
        private List<FriendListing> _friends;

        private void Awake()
        {
            //Database Friend list search
        }

        private bool Luck()
        {
            float chance = Random.Range(0, 1);
            if (chance <= 0.5f) return true;
            return false;
        }

        public void AddFriend(string name,bool isOnline)
        {
            var searchedFriend = _friends.Find(friend => friend.name == name);
            if (searchedFriend) return;
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