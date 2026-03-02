using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Missile Behaviors/Spawn Hazard")]
public class BehaviorSpawnHazard : MissileBehavior {

    public GameObject hazardPrefab;
    public MissileEvent triggerEvent = MissileEvent.OnImpact;

    public override void OnEvent(MissileEvent evt) {
        if (evt != triggerEvent) return;

        Instantiate(
            hazardPrefab,
            missile.transform.position,
            Quaternion.identity
        );
    }
}
