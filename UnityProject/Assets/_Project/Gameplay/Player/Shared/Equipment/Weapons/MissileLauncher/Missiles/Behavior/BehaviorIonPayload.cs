using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Missile Behaviors/Ion Payload")]
public class BehaviorIonPayload : MissileBehavior {
    public float ionStrength = 1f;
    public float duration = 3f;

    public override void OnEvent(MissileEvent evt) {
        if (evt != MissileEvent.OnImpact) return;

        /*
        if (missile.GetTarget() != null && missile.GetTarget().TryGetComponent<IonReceiver>(out var receiver)) {
            receiver.ApplyIon(ionStrength, duration);
        }
        */
    }
}
