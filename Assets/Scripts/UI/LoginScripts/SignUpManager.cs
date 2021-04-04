using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DatabaseScripts;

namespace UI.LoginScripts
{
    public class SignUpManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nickname, _email, _password;
        [SerializeField] private Toggle _male, _female;
        public void OnSubmitClicked()
        {
            //Check if such user with the nickname exist

            //If user is unique 

            string NewUserName = _nickname.text.Substring(0, _nickname.text.Length-1);
            string NewUserMail = _email.text.Substring(0, _email.text.Length - 1);
            string NewUserPassword = _password.text.Substring(0, _password.text.Length - 1);

            bool IsNewUserMale = _male.isOn;

            SignUpStatus RegisterStatus = DatabaseConnection.SignUp(NewUserName, NewUserMail, NewUserPassword, IsNewUserMale);

            switch (RegisterStatus)
            {

                case SignUpStatus.SuccesfulCreation:

                    SceneManager.LoadScene(1);

                    break;


                case SignUpStatus.UserExists:
                    Debug.Log("ben varım ki");
                    break;

                case SignUpStatus.InvalidMail:
                    _email.color = Color.red;
                    Debug.Log("ben malım ki");
                    break;
            }


        }
    }
}
