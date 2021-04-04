using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DatabaseScripts;
using Managers;

namespace UI.LoginScripts
{
    public class SignUpManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nickname, _email, _password;
        [SerializeField] private Toggle _male, _female;
        public event Action OnSignedUp;
        public void ChangeColorToBlack()
        {
            _nickname.color = Color.black;
            _email.color = Color.black;
        }
        public void OnSubmitClicked()
        {
            string NewUserName = _nickname.text.Substring(0, _nickname.text.Length-1);
            string NewUserMail = _email.text.Substring(0, _email.text.Length - 1);
            string NewUserPassword = _password.text.Substring(0, _password.text.Length - 1);

            bool IsNewUserMale = _female.isOn;

            SignUpStatus RegisterStatus = DatabaseConnection.SignUp(NewUserName, NewUserMail, NewUserPassword, IsNewUserMale);

            switch (RegisterStatus)
            {

                case SignUpStatus.SuccesfulCreation:
                    GameManager.GameSettings.NickName = NewUserName;
                    OnSignedUp?.Invoke();
                    break;
                case SignUpStatus.UserExists:
                    _nickname.color = Color.red;
                    _nickname.text = "ALREADY USED";
                    break;
                case SignUpStatus.InvalidMail:
                    _email.color = Color.red;
                    _email.text = "INVALID MAIL";
                    break;
            }


        }
    }
}
