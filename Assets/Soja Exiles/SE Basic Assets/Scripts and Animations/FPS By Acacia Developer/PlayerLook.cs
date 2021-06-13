using System;
using System.Collections.Generic;
using Data;
using General;
using Photon.Pun;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;
using Utilities;

public class PlayerLook : MonoBehaviourPunCallbacks
{
    public event Action OnJudgeFinished;
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform playerBody;
    [SerializeField] private CaseSummaryDetail _caseSummaryDetail;
    [SerializeField] private PlayerMove ParentController;
    public Camera Camera;
    private InteractableCourtObject _lastInteractedObj;
    private float xAxisClamp,yAxisClamp;
    private bool _isEnabled=false;
    private InteractableCourtObject _clapperReference;
    
    public void Initialize()
    {
        xAxisClamp = 0.0f;
        yAxisClamp = 0f;
        if (!Camera)
        {
            Camera = GetComponentInChildren<Camera>();
        }
        _caseSummaryDetail.Initialize();
        _caseSummaryDetail.OnReady += SetReady;
        _caseSummaryDetail.OnClose += EnableRotation;
    }

    public void RegisterForInteractables(List<InteractableCourtObject> interactableCourtObjects)
    {
        interactableCourtObjects.ForEach(obj =>
        {
            switch (obj.InteractableType)
            {
                case InteractableType.CaseFile:
                    obj.OnFileClicked += ShowSummary;
                    break;
                case InteractableType.Clapper:
                    if (PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString() == DataKeyValues.__JUDGE__)
                    {
                        _clapperReference = obj;
                        _clapperReference.OnClapperUsed += OnClapperHit;
                    }
                    break;
                case InteractableType.Button:
                    if (PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString() != DataKeyValues.__JUDGE__)
                    {
                        obj.OnButtonClicked += ButtonClicked;
                    }
                    break;
            }
        });
    }

    private void OnClapperHit()
    {
        _clapperReference.OnClapperUsed -= OnClapperHit;
        OnJudgeFinished?.Invoke();
    }

    private void ShowSummary()
    {
        _isEnabled = false;
        _caseSummaryDetail.OpenSummaryFolder();
    }

    private void EnableRotation()
    {
        _isEnabled = true;
    }

    public void CloseSummary()
    {
        _caseSummaryDetail.DirectlyClose();
    }
    
    private void ButtonClicked(ButtonStatus buttonStatus)
    {
        switch (buttonStatus)
        {
            case ButtonStatus.Start:
                SpeechRecognition.SpeechRecognitionCaller(PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__LANGUAGE__].ToString());
                ParentController.InvokeStartTurnEvent();
                break;
            case ButtonStatus.Pass:
                ParentController.InvokeSwitchTurnEvent();
                break;
        }
    }

    private void SetReady()
    {
        _caseSummaryDetail.OnReady -= SetReady;
        _isEnabled = true;
        ParentController.IncreaseReadyCounter();
    }

    public void CameraRotation()
    {
        if (!_isEnabled) return;
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        //TODO Clamp rotations for logical visuals
        /*xAxisClamp += mouseX;
        yAxisClamp += mouseY;
        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseX = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseX = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }*/

        playerBody.Rotate(Vector3.up * mouseX);
        transform.Rotate(Vector3.left * mouseY);
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) 
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Interactable"))
            {
                if (_lastInteractedObj && _lastInteractedObj.transform!=objectHit)
                {
                    _lastInteractedObj.IsRaycasted = false;
                }
                _lastInteractedObj= objectHit.GetComponent<InteractableCourtObject>();
                _lastInteractedObj.IsRaycasted = true;
                _lastInteractedObj.HandleOutline(true);
            }
            else
            {
                if (_lastInteractedObj)
                {
                    _lastInteractedObj.IsRaycasted = false;
                }
            }
        }
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRot = playerBody.eulerAngles;
        eulerRot.x = value;
        playerBody.eulerAngles = eulerRot;
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}