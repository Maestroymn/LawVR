using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveInput : MonoBehaviour
{
    
    public static SteamVR_Input_ActionSet_default ActionSet;
    void Start()
    {
        ActionSet = new SteamVR_Input_ActionSet_default();

    }
    // Update is called once per frame
    void Update()
    {
        if(ActionSet.GrabGrip.GetLastStateDown(SteamVR_Input_Sources.Any))
        {
            Debug.Log("Gribbing broo");
        }
        if (ActionSet.GrabGrip.GetLastStateUp(SteamVR_Input_Sources.Any))
        {
            Debug.Log("Releasing broo");
        }
    }
}
