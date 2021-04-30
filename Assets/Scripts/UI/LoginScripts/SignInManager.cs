using System;
using TMPro;
using UnityEngine;
using DatabaseScripts;
using Managers;
using UnityEngine.UI;

namespace UI.LoginScripts
{
    public class SignInManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name, _password;
        [SerializeField] private Button _logoutButton;
        private string _newUserName;
        public event Action OnSignedIn; 
        public void ChangeColorToBlack()
        {
            _name.color = Color.black;
            _password.color = Color.black;

        }
        public void OnSubmitClicked()
        {
            _newUserName = _name.text.Substring(0, _name.text.Length-1);
            string NewUserPassword = _password.text.Substring(0, _password.text.Length - 1);
            SignInStatus LoginStatus= DatabaseConnection.SignIn(_name.text.Substring(0, _name.text.Length - 1), _password.text.Substring(0, _password.text.Length - 1));

            switch (LoginStatus)
            { 
                case SignInStatus.SuccesfulLogin:
                    GameManager.GameSettings.NickName = _newUserName;
                    GameManager.GameSettings.Password = NewUserPassword;
                    OnSignedIn?.Invoke();
                    break;
                case SignInStatus.UserDoesntExist:
                    _name.color = Color.red;
                    _name.text = "INVALID NAME";
                    break;
                case SignInStatus.WrongPassword:
                    _password.color = Color.red;
                    _password.text = "WRONG PASSWORD"; 
                    break;
                case SignInStatus.AlreadyLoggedIn:
                    _name.color = Color.red;
                    _name.text = "ALREADY ONLINE";
                    _logoutButton.interactable = true;
                    _logoutButton.transform.localScale=Vector3.zero;
                    _logoutButton.gameObject.SetActive(true);
                    _logoutButton.transform.LeanScale(Vector3.one * 0.5f, .5f);
                    break;
            }
        }

        public void SetOffline()
        {
            ChangeColorToBlack();
            DatabaseConnection.SetUserOffline(_newUserName);
            _logoutButton.interactable = false;
            _logoutButton.transform.LeanScale(Vector3.zero, .5f);
        }
    }
}
