using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBodyUnlockOnExitBehavior : StateMachineBehaviour
{
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