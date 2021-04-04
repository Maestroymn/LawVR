﻿using UnityEngine;
using UnityEngine.UI;
using DatabaseScripts;
using General;
using UnityEngine.SceneManagement;

namespace UI.LoginScripts
{
    public class LoginUIManager : MonoBehaviour
    {
        [SerializeField] private Button _signInButton,_signUpButton;
        [SerializeField] private GameObject _noConnection,_connecting;
        [SerializeField] private UserConnect _userConnect;
        [SerializeField] private SignInManager _signInManager;
        [SerializeField] private SignUpManager _signUpManager;
        private void Awake()
        {
            CheckInternetConnection();
            _signInManager.OnSignedIn += LoggingIn;
            _signUpManager.OnSignedUp+= LoggingIn;
        }

        public void CheckInternetConnection()
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
                ShowSignInCanvas();
            }
        }

        private void LoggingIn()
        {
            _signInManager.gameObject.SetActive(false);
            _signUpManager.gameObject.SetActive(false);
            _connecting.SetActive(true);
            _userConnect.ConnectPhoton();
        }

        public void OnConnected()
        {
            _signInManager.OnSignedIn -= LoggingIn;
            _signUpManager.OnSignedUp -= LoggingIn;
            _connecting.SetActive(false);
            _noConnection.SetActive(false);
            SceneManager.LoadScene(1);
        }

        public void ShowSignInCanvas()
        {
            _signInButton.interactable = false;
            _signUpButton.interactable = true;
            _signUpManager.gameObject.SetActive(false);
            _signInManager.gameObject.SetActive(true);
        }
        
        public void ShowSignUpCanvas()
        {
            _signUpButton.interactable = false;
            _signInButton.interactable = true;
            _signInManager.gameObject.SetActive(false);
            _signUpManager.gameObject.SetActive(true);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
