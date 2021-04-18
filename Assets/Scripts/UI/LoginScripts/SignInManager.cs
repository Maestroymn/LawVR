using System;
using TMPro;
using UnityEngine;
using DatabaseScripts;
using Managers;

namespace UI.LoginScripts
{
    public class SignInManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name, _password;
        public event Action OnSignedIn; 
        public void ChangeColorToBlack()
        {
            _name.color = Color.black;
            _password.color = Color.black;

        }
        public void OnSubmitClicked()
        {
            string NewUserName = _name.text.Substring(0, _name.text.Length-1);
            string NewUserPassword = _password.text.Substring(0, _password.text.Length - 1);
            SignInStatus LoginStatus= DatabaseConnection.SignIn(_name.text.Substring(0, _name.text.Length - 1), _password.text.Substring(0, _password.text.Length - 1));

            switch (LoginStatus)
            { 
                case SignInStatus.SuccesfulLogin:
                    GameManager.GameSettings.NickName = NewUserName;
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
                    break;
            }
                        
        }
    }
}
