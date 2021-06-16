using System;
using Data;
using DatabaseScripts;
using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _usernameTextArea;
        [SerializeField] private Animator _userMenuAnimator;
        [SerializeField] private Animator _friendListMenuAnimator;
        [SerializeField] private FriendListManager friendListManager;

        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Show1 = Animator.StringToHash("Show");
        private bool _showingUserMenu,_showingFriends;
        private LTDescr _ltDescr;
        private static readonly int Close = Animator.StringToHash("Close");

        public void HideMainMenu()
        {
            if(_showingFriends)
                FriendList();
            friendListManager.ClearLists();
            _ltDescr=LeanTween.delayedCall(.3f,()=>gameObject.SetActive(false));
        }

        public void Show()
        {
            if (_ltDescr != null)
            {
                LeanTween.cancel(_ltDescr.id);
            }

            if (PlayerPrefs.HasKey("__SHOWING_USER_MENU__"))
            {
                _showingUserMenu = PlayerPrefs.GetInt("__SHOWING_USER_MENU__") == 1;
            }
            //friendListManager.Initialize();
            SetUserName();
            friendListManager.CleanFirst();
            gameObject.SetActive(true);    
        }

        public void SetUserName()
        {
            _usernameTextArea.text = GameManager.GameSettings.NickName;
        }

        public void UserMenu()
        {
            _userMenuAnimator.SetBool(Show1,!_showingUserMenu);
            _showingUserMenu = !_showingUserMenu;
            PlayerPrefs.SetInt("__SHOWING_USER_MENU__",_showingUserMenu?1:0);
        }
        
        public void FriendList()
        {
            _showingFriends = !_showingFriends;
            _friendListMenuAnimator.SetBool(Open,_showingFriends);
            if(_showingFriends)
            {
                friendListManager.RefreshFriendList();
            }
        }

        public void LoadAvatarCustomizationScene()
        {
            if (GameManager.GameSettings.Gender == Gender.Female)
            {
                SceneManager.LoadScene(DataKeyValues.__FEMALE_CUSTOMIZATION_SCENE__);
            }
            else if(GameManager.GameSettings.Gender==Gender.Male)
            {
                SceneManager.LoadScene(DataKeyValues.__MALE_CUSTOMIZATION_SCENE__);
            }
        }

        public void Logout()
        {
            DatabaseConnection.SetUserOffline(GameManager.GameSettings.NickName);
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(DataKeyValues.__LOGIN_SCENE__);
        }
    }
}
