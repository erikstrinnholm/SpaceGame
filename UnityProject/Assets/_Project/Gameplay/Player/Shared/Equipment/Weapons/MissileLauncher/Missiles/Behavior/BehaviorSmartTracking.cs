using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Missile Behaviors/Smart Tracking")]
public class BehaviorSmartTracking : MissileBehavior {

    public float retargetInterval = 0.5f;
    private float timer;

    public override void OnEvent(MissileEvent evt) {
        if (evt == MissileEvent.OnLaunch) {
            missile.StartCoroutine(RetargetRoutine());
        }
    }

    private IEnumerator RetargetRoutine() {
        while (true) {
            yield return new WaitForSeconds(retargetInterval);
            // Reacquire closest hostile
            missile.TryReacquireTarget();
        }
    }
}

