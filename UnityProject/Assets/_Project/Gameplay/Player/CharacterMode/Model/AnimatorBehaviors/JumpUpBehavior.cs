using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class JumpUpBehaviour : StateMachineBehaviour {
    public float jumpForceTime = 0.2f; // when to apply jump force
    private bool applied = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        applied = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!applied && stateInfo.normalizedTime >= jumpForceTime) {
            CharacterMovementController controller = animator.GetComponentInParent<CharacterMovementController>();
            if (controller != null) {
                controller.ApplyJumpForce();
            }
            applied = true;
        }
    }
}

