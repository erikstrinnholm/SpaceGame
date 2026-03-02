using UnityEngine;

public class LowerBodyLockOnEnterBehavior : StateMachineBehaviour {
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
}
