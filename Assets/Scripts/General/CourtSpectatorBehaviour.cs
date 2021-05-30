using System.Collections;
using AdvancedCustomizableSystem;
using Data;
using Managers;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace General
{
    public class CourtSpectatorBehaviour : MonoBehaviourPunCallbacks
    {
        public Animator Animator;
        public Camera Camera;

        private static readonly int NormalSit = Animator.StringToHash("normal_sit");
        private float xAxisClamp,yAxisClamp;
        private bool _isEnabled=false;
        [SerializeField] private string mouseXInputName, mouseYInputName;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private Transform head;
        [SerializeField] private SkinnedMeshRenderer _cloth;
        [Header("VR Components")] [SerializeField] private GameObject _playerVR;
        private PauseUIManager _pauseUIManager;
        
        public void Initialize()
        {
            if (!photonView.IsMine) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Animator.SetBool(NormalSit,true);
            _cloth.material.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
            xAxisClamp = 0.0f;
            yAxisClamp = 0f;
            if (!Camera)
            {
                Camera = GetComponentInChildren<Camera>();
            }
            _pauseUIManager = FindObjectOfType<PauseUIManager>();
            _pauseUIManager.ResumeButton.onClick.AddListener(HandleEnable);
            if (PlayerPrefs.GetInt(DataKeyValues.__VR_ENABLE__)==1)
            {
                StartCoroutine(ActivateVR("OpenVR"));
            }
            _isEnabled = true;
        }

        private void HandleEnable()
        {
            _isEnabled = true;
        }
        
        public IEnumerator ActivateVR(string deviceName)
        {
            XRSettings.LoadDeviceByName(deviceName);
            yield return null;
            _playerVR.gameObject.SetActive(true);
            XRSettings.enabled = true;
        }
        
        private void Update()
        {
            if (!photonView.IsMine) return;
            CameraRotation();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isEnabled = !_isEnabled;
            }
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

            head.Rotate(Vector3.up * mouseX);
            head.Rotate(Vector3.left * mouseY);
        }
        
    }
}
