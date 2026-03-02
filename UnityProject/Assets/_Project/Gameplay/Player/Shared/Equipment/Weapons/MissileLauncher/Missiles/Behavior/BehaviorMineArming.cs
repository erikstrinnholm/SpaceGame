using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Missile Behaviors/Mine Arming")]
public class BehaviorMineArming : MissileBehavior {

    public float armTime = 1.5f;

    public override void OnEvent(MissileEvent evt) {
        if (evt == MissileEvent.OnLaunch) {
            missile.StartCoroutine(ArmRoutine());
        }
    }

    private IEnumerator ArmRoutine() {
        yield return new WaitForSeconds(armTime);
        missile.StopMovement();
        missile.TriggerEvent(MissileEvent.OnArmed);
    }
}
