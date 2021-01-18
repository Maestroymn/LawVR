using Managers;
using Photon.Pun;
using UnityEngine;

public class QuickInstantiater : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject obj;
    [SerializeField] private Transform _defendantTransform,_plaintiffTransform,_judgeTransform;
    private void Awake()
    {
        HandleSpawns();
    }

    public void HandleSpawns()
    {
        switch (PhotonNetwork.LocalPlayer.CustomProperties["Role"].ToString())
        {
            case "Plaintiff":
                GameManager.NetworkInstantiate(obj, _defendantTransform.position, Quaternion.identity);
                break;
            case "Defendant":
                GameManager.NetworkInstantiate(obj, _plaintiffTransform.position, Quaternion.identity);
                break;
            case "Judge":
                GameManager.NetworkInstantiate(obj, _judgeTransform.position, Quaternion.identity);
                break;
        }
    }
}
