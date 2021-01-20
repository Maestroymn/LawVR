using Managers;
using Photon.Pun;
using UnityEngine;

public class QuickInstantiater : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject judge,plaintiff,spectator,defendant;
    [SerializeField] private Transform _defendantTransform,_plaintiffTransform,_judgeTransform,_spectatorTransform;
    private void Awake()
    {
        HandleSpawns();
    }

    public void HandleSpawns()
    {
        switch (PhotonNetwork.LocalPlayer.CustomProperties["Role"].ToString())
        {
            case "Plaintiff":
                GameManager.NetworkInstantiate(plaintiff, _defendantTransform.position, Quaternion.identity);
                break;
            case "Defendant":
                GameManager.NetworkInstantiate(defendant, _plaintiffTransform.position, Quaternion.identity);
                break;
            case "Judge":
                GameManager.NetworkInstantiate(judge, _judgeTransform.position, Quaternion.identity);
                break;
            case "Spectator":
                GameManager.NetworkInstantiate(spectator, _spectatorTransform.position, Quaternion.identity);
                break;
        }
    }
}
