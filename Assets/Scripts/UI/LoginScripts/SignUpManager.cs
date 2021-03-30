using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            SceneManager.LoadScene(1);
        }
    }
}
