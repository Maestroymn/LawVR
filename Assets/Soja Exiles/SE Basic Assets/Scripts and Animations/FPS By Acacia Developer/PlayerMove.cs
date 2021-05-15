using System;
using System.Collections;
using Data;
using General;
using Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public event Action OnAllReady,OnStartTurn,OnStartSession,OnSwitchTurn;
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;

    [SerializeField] private float walkSpeed, runSpeed;
    [SerializeField] private float runBuildUpSpeed;
    [SerializeField] private KeyCode runKey;

    private float movementSpeed;
    private int _readyCount = 0, _totalPlayerCount;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    [SerializeField] public PlayerLook PlayerLook;
    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private Animator _animator;
    public Status Status;
    [Header("VR Components")] [SerializeField]
    private GameObject _playerVR;
    
    private bool isJumping;
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Nervous = Animator.StringToHash("nervous");
    private static readonly int NormalSit = Animator.StringToHash("normal_sit");
    private PauseUIManager _pauseUIManager;
    
    private void Awake()
    {
        _pauseUIManager = FindObjectOfType<PauseUIManager>();
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _totalPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (!photonView.IsMine)
        {
            PlayerLook.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _animator.SetBool(Walk,false);
        }
        else
        {
            SetStartingStatus();
            if (PlayerPrefs.GetInt(DataKeyValues.__VR_ENABLE__)==1)
            {
                StartCoroutine(ActivateVR("OpenVR"));
            }
        }
    }

    private void SetStartingStatus()
    {
        switch (Status)
        {
            case General.Status.NervousSitting:
                _animator.SetBool(Nervous,true);
                break;
            case General.Status.NormalSitting:
                _animator.SetBool(NormalSit,true);
                break;
        }
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
        if(!PhotonNetwork.IsConnected || !photonView.IsMine || !PhotonNetwork.InRoom || _pauseUIManager.paused)
            return;
        if (photonView.IsMine)
        {
            if(PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SIMULATION_TYPE__].ToString().Equals(DataKeyValues.__SANDBOX_MODE__))
                PlayerMovement();
            PlayerLook.CameraRotation();
        }
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName);
        float vertInput = Input.GetAxis(verticalInputName);

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        if (Mathf.Abs(horizInput) < 0.06f && Mathf.Abs(vertInput) < 0.06f)
        {
            _animator.SetBool(Walk,false);
        }
        else
        {
            _animator.SetBool(Walk,true);
        }
        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * movementSpeed);
        if ((vertInput != 0 || horizInput != 0) && OnSlope())
        {
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);
        }
        SetMovementSpeed();
        JumpInput();
    }

    private void SetMovementSpeed()
    {
        if (Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }
    }


    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, charController.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
            {
                print("OnSlope");
                return true;
            }

        return false;
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }


    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;
        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;
    }

    [PunRPC]
    public void RPC_IncreaseReadyPlayerCounter()
    {
        _readyCount++;
        if (_readyCount >= _totalPlayerCount)
        {
            InvokeSwitchTurnEvent();
        }
    }
    
    public void IncreaseReadyCounter()
    {
        photonView.RPC("RPC_IncreaseReadyPlayerCounter",RpcTarget.All);
    }

    public void InvokeSwitchTurnEvent()
    {
        OnSwitchTurn?.Invoke();
    }
    
    public void InvokeStartTurnEvent()
    {
        OnStartTurn?.Invoke();
    }
    
}