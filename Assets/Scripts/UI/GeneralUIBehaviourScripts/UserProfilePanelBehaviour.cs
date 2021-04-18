using System;
using DatabaseScripts;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GeneralUIBehaviourScripts
{
    public class UserProfilePanelBehaviour : MonoBehaviour
    {
        [SerializeField] private Text _username, _mail,_password;
        [SerializeField] private GameObject _privatePassword;
        [SerializeField] private TMP_InputField _newUsername, _newMail, _newPassword;
        [SerializeField] private TextMeshProUGUI _mainMenuUserName,_maleText,_femaleText;
        private Gender _gender;
        private bool _showPass;
        public void OpenPanel()
        {
            _username.text = GameManager.GameSettings.NickName;
            _mail.text = GameManager.GameSettings.Mail;
            _gender=GameManager.GameSettings.Gender;
            _password.text = GameManager.GameSettings.Password;
            gameObject.SetActive(true);
        }
        
        public void DiscardChangesAndExit()
        {
            gameObject.SetActive(false);
        }

        public void SaveChangesAndExit()
        {
            if (_newUsername.text != String.Empty)
            {
                DatabaseConnection.SetName(_newUsername.text);
                GameManager.GameSettings.NickName = _newUsername.text;
                _mainMenuUserName.text = GameManager.GameSettings.NickName;
            }
            if(_newMail.text!=String.Empty)
            {
                bool validMail;
                try
                {
                    var addr = new System.Net.Mail.MailAddress(_newMail.text);
                    validMail=(addr.Address == _newMail.text)?true:false ;
                }
                catch
                {
                    validMail = false;
                }

                if (validMail)
                {
                    DatabaseConnection.SetEmail(_newMail.text);
                    GameManager.GameSettings.Mail = _newMail.text;
                }
                else
                {
                    var pos = _newMail.transform.position;
                    _newMail.transform.LeanMove(pos+Vector3.right*10f,.1f).setOnComplete(() =>
                    {
                        _newMail.transform.LeanMove(pos, .1f);
                    });
                    return;
                }
            }

            if (_newPassword.text != String.Empty)
            {
                DatabaseConnection.SetPassword(_newPassword.text);
            }
            DatabaseConnection.SetIsFemale(_gender != Gender.Male);
            GameManager.GameSettings.Gender = _gender;
            _newUsername.text = String.Empty;
            _newMail.text = String.Empty;
            _newPassword.text = String.Empty;
            gameObject.SetActive(false);
        }

        public void SetGenderFemale()
        {
            _gender = Gender.Female;
            _maleText.color=Color.gray;
            _femaleText.color=Color.green;
        }
        
        public void SetGenderMale()
        {
            _gender = Gender.Male;
            _femaleText.color=Color.gray;
            _maleText.color=Color.green;
        }

        public void ShowPassword()
        {
            _showPass = !_showPass;
            _password.gameObject.SetActive(_showPass);
            _privatePassword.gameObject.SetActive(!_showPass);
        }
    }
}
