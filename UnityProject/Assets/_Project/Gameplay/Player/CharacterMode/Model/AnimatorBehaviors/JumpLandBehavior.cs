using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLandBehavior : StateMachineBehaviour {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            CharacterMovementController controller = animator.GetComponentInParent<CharacterMovementController>();


        if (controller != null)
            controller.ApplyJumpLand();
    }
}

