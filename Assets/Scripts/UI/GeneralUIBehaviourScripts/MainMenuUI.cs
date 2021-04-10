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
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Show1 = Animator.StringToHash("Show");
        private bool _showingUserMenu,_showingFriends;
        private LTDescr _ltDescr;
        public void HideMainMenu()
        {
            if(_showingUserMenu)
                UserMenu();
            if(_showingFriends)
                FriendList();
            _ltDescr=LeanTween.delayedCall(1f,()=>gameObject.SetActive(false));
        }
        
        public void Show()
        {
            if (_ltDescr != null)
            {
                LeanTween.cancel(_ltDescr.id);
            }
            SetUserName();
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
        }
        
        public void FriendList()
        {
            _friendListMenuAnimator.SetTrigger(_showingFriends ? Close : Open);
            _showingFriends = !_showingFriends;
            if(_showingFriends)
            {
                friendListManager.RefreshFriendList();
            }
        
        }

        public void Logout()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
    }
}
