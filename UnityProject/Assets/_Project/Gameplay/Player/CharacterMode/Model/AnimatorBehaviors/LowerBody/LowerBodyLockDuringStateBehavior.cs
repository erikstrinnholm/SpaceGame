using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LowerBodyLockDuringStateBehavior : StateMachineBehaviour {
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        PlayerAnimatorState animState =
            animator.GetComponent<PlayerAnimatorState>();

        if (animState != null)
            animState.SetLowerBodyLock(true);
    }

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        PlayerAnimatorState animState =
            animator.GetComponent<PlayerAnimatorState>();

        if (animState != null)
            animState.SetLowerBodyLock(false);
    }
}
