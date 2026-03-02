using UnityEngine;

public class UpperBodyLockOnEnterBehavior : StateMachineBehaviour {
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
}
