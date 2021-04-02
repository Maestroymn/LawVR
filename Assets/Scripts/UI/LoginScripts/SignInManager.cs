using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.LoginScripts
{
    public class SignInManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name, _password;

        public void OnSubmitClicked()
        {
            //Check if such user exist from DB here...
            
            //If user exist & correct passoword loading main menu
            SceneManager.LoadScene(1);
            //Else If user doesn't exist...
            
            //Else If user exist but password doesn't match...
            
        }
    }
}
