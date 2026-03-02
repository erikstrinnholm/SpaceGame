using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpperBodyLockDuringStateBehavior : StateMachineBehaviour {
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        PlayerAnimatorState animState =
            animator.GetComponent<PlayerAnimatorState>();

        if (animState != null)
            animState.SetUpperBodyLock(true);
    }

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        PlayerAnimatorState animState =
            animator.GetComponent<PlayerAnimatorState>();

        if (animState != null)
            animState.SetUpperBodyLock(false);
    }
}
