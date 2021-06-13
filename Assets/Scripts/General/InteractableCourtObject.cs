using System;
using System.Collections.Generic;
using Data;
using DatabaseScripts;
using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Utilities;

namespace General
{
    public enum InteractableType
    {
        CaseFile,
        Button,
        Clapper
    }

    public enum ButtonStatus
    {
        Start,
        Pass,
        Wait,
    }
    public class InteractableCourtObject : MonoBehaviour
    {
        private ButtonStatus _buttonStatus;
        public InteractableType InteractableType;
        public TextMeshPro InteractText;
        [SerializeField] public List<MeshRenderer> MeshRenderers;
        [CanBeNull] public Animator Animator;
        public bool IsButton;
        [ConditionalShowInInspector("IsButton", true)] [SerializeField]
        private Color _startColor, _passColor,_waitColor;
        [ConditionalShowInInspector("IsButton", true)] [SerializeField]
        private TMP_Text _buttonText;
        public float OutlineWidthMax;
        private static readonly int Outline = Shader.PropertyToID("_Outline");
        private float _targetOutline;
        private bool _isOutlined = false, _clapperWaiting = false, _sessionEnded = false;
        public bool IsRaycasted=false;
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Hint = Animator.StringToHash("Hint");
        private static readonly int Color = Shader.PropertyToID("_Color");
        private static readonly int Speed = Animator.StringToHash("Speed");
        public event Action OnFileClicked, OnClapperUsed;
        public event Action<ButtonStatus> OnButtonClicked; 
        private void Update()
        {
            if (_sessionEnded) return;
            if (_isOutlined && !IsRaycasted)
            {
                HandleOutline(false);
            }
            if (IsRaycasted && Input.GetKeyDown(DataKeyValues.__INTERACT_KEY__))
            {
                if (InteractableType == InteractableType.Clapper && _clapperWaiting)
                {
                    Animator.SetFloat(Speed,1f);
                }
                OnInteraction();
            }
            if (IsRaycasted && Input.GetKeyDown(DataKeyValues.__CANCEL_KEY__) && InteractableType == InteractableType.Clapper && _clapperWaiting)
            {
                _clapperWaiting = false;
                Animator.SetFloat(Speed,-1f);
            }
        }

        public void HandleOutline(bool isActive)
        {
            if (_isOutlined==isActive || (IsButton && ButtonStatus.Wait==_buttonStatus)) return;
            _isOutlined = isActive;
            _targetOutline = isActive ? OutlineWidthMax : 0f;
            for (int i = 0; i < MeshRenderers.Count; i++)
            {
                var i1 = i;
                if(MeshRenderers[i1].material.IsKeywordEnabled("_Outline")) continue;
                gameObject.LeanValue(MeshRenderers[i1].material.GetFloat(Outline), _targetOutline, .1f).setOnUpdate((float value) =>
                {
                    MeshRenderers[i1].material.SetFloat(Outline,value);
                }).setEase(LeanTweenType.easeInCirc);
            }
            if(isActive)
                InteractText?.gameObject.LeanValue(0f,1f, .1f).setOnUpdate((float value) =>
                {
                    var interactTextColor = InteractText.color;
                    interactTextColor.a = value;
                    InteractText.color = interactTextColor;
                }).setEase(LeanTweenType.easeInCirc);
            else
                InteractText?.gameObject.LeanValue(1f,0f, .1f).setOnUpdate((float value) =>
                {
                    var interactTextColor = InteractText.color;
                    interactTextColor.a = value;
                    InteractText.color = interactTextColor;
                }).setEase(LeanTweenType.easeInCirc);
        }

        public void HandleButtonSettings(ButtonStatus buttonStatus)
        {
            _buttonStatus = buttonStatus;
            switch (buttonStatus)
            {
                case ButtonStatus.Pass:
                    _buttonText.text = "PASS!";
                    MeshRenderers[0].material.SetColor(Color,_passColor); 
                    break;
                case ButtonStatus.Start:
                    _buttonText.text = "START!";
                    MeshRenderers[0].material.SetColor(Color,_startColor); 
                    Animator?.SetBool(Hint,true);
                    break;
                case ButtonStatus.Wait:
                    _buttonText.text = "WAIT!";
                    MeshRenderers[0].material.SetColor(Color,_waitColor); 
                    break;
            }
        }

        public void OnInteraction()
        {
            switch (InteractableType)
            {
                case InteractableType.Clapper:
                    if (_clapperWaiting) return;
                    Animator?.SetTrigger(Hit);
                    break;
                case InteractableType.CaseFile:
                    OnFileClicked?.Invoke();
                    break;
                case InteractableType.Button:
                    switch (_buttonStatus)
                    {
                        case ButtonStatus.Wait:
                            return;
                    }
                    Animator?.SetTrigger(Hit);
                    OnButtonClicked?.Invoke(_buttonStatus);
                    break;
            }
        }

        public void JudgeEndsSession()
        {
            DatabaseConnection.UpdateSessionLog(PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__].ToString(), DateTime.Now.ToString());
            _sessionEnded = true;
            OnClapperUsed?.Invoke();
        }
        
        public void StopAnimation()
        {
            _sessionEnded = true;
            Animator?.SetFloat(Speed,0f);
            _clapperWaiting = true;
            SpeechRecognition.SpeechRecognitionCaller(PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__LANGUAGE__].ToString());
            _sessionEnded = false;
        }
    }
}
