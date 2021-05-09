using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKController : MonoBehaviour
{

    protected Animator animator;

    public Transform rightVRController;
    public Transform leftVRController;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            float reachR = animator.GetFloat("HandRight");
            float reachL = animator.GetFloat("HandLeft");

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reachR);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, reachL);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightVRController.position);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftVRController.position);
        }
    }
}