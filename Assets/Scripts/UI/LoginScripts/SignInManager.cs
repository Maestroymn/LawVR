using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseScripts;

namespace UI.LoginScripts
{

    public class SignInManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name, _password;
        public void ChangeColorToBlack()
        {
            _name.color = Color.black;
            _password.color = Color.black;

        }
        public void OnSubmitClicked()
        {
            //Check if such user exist from DB here...
            SignInStatus LoginStatus= DatabaseConnection.SignIn(_name.text.Substring(0, _name.text.Length - 1), _password.text.Substring(0, _password.text.Length - 1));

            switch (LoginStatus)
            {

                case SignInStatus.SuccesfulLogin:

                    SceneManager.LoadScene(1);

                break;


                case SignInStatus.UserDoesntExist:
                    _name.color = Color.red;
                    _name.text = "INVALID NAME";
                break;

                case SignInStatus.WrongPassword:
                    _password.color = Color.red;
                    _password.text = "WRONG PASSWORD"; 
                    break;
            }
                        
        }
    }
}
