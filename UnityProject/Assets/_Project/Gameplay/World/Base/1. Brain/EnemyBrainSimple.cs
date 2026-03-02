using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
Simple Aggressive Brain:
- Move toward target if out of range
- Attack if in range
- React to damage
*/


public class EnemyBrainSimple : EnemyBrainBase {
    
    protected override void BrainTick() {
        if (targetingSystem == null || attackSystem == null) return;

        // Update targeting
        targetingSystem.TickTargeting();

        // If there is a target, try to fire
        if (targetingSystem.HasTarget) {
            AimSolution aim = targetingSystem.GetAimSolution();
            attackSystem.TryFire(aim);
        }
    }
}

