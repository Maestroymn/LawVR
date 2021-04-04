using System;
using UnityEngine;
using UnityEngine.UI;
using DatabaseScripts;
using Photon.Pun;

namespace UI.LoginScripts
{
    public class LoginUIManager : MonoBehaviour
    {
        [SerializeField] private Button _signInButton,_signUpButton;
        [SerializeField] private Canvas _signInCanvas, _signUpCanvas;
        [SerializeField] private GameObject _noConnection,_connecting;

        private void Awake()
        {

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _noConnection.SetActive(true);
                _connecting.SetActive(false);   
            }
            else
            {
                
                DatabaseConnection.ConnectDatabase();
                _noConnection.SetActive(false);
                _connecting.SetActive(true);   
            }
        }

        public void OnConnected()
        {
            _connecting.SetActive(false);
            _noConnection.SetActive(false);
            _signUpButton.interactable = true;
            _signInButton.interactable = false;
            _signUpCanvas.gameObject.SetActive(false);
            _signInCanvas.gameObject.SetActive(true);
        }

        public void ShowSignInCanvas()
        {
            _signInButton.interactable = false;
            _signUpButton.interactable = true;
            _signUpCanvas.gameObject.SetActive(false);
            _signInCanvas.gameObject.SetActive(true);
        }
        
        public void ShowSignUpCanvas()
        {
            _signUpButton.interactable = false;
            _signInButton.interactable = true;
            _signInCanvas.gameObject.SetActive(false);
            _signUpCanvas.gameObject.SetActive(true);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
