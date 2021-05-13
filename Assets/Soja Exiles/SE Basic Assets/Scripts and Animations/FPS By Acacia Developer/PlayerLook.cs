using System.Collections.Generic;
using Data;
using General;
using Photon.Pun;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;

public class PlayerLook : MonoBehaviourPunCallbacks
{
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform playerBody;
    [SerializeField] private CaseSummaryDetail _caseSummaryDetail;
    [SerializeField] private PlayerMove ParentController;
    public Camera Camera;
    private InteractableCourtObject _lastInteractedObj;
    private float xAxisClamp;
    private bool _isEnabled=false;
    
    private void Awake()
    {
        xAxisClamp = 0.0f;
        Cursor.lockState= CursorLockMode.Locked;
        if (!Camera)
        {
            Camera = GetComponentInChildren<Camera>();
        }
        _caseSummaryDetail.enabled = true;
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
                ParentController.GetComponent<PhotonView>().RPC("RPC_StartTurn",RpcTarget.All);
                break;
            case ButtonStatus.Pass:
                ParentController.GetComponent<PhotonView>().RPC("RPC_SwitchTurn",RpcTarget.All);
                break;
        }
    }
    
    private void SetReady()
    {
        _caseSummaryDetail.OnReady -= SetReady;
        _isEnabled = true;
        ParentController.GetComponent<PhotonView>().RPC("RPC_IncreaseReadyPlayerCounter",RpcTarget.All);
    }

    public void CameraRotation()
    {
        if (!_isEnabled) return;
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        playerBody.Rotate(Vector3.up * mouseX);
        transform.Rotate(Vector3.left * mouseY);
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
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
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}