using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPresenceIndicator : BaseThreatIndicator {
    /*
    private Transform enemy;
    private float maxRange;

    public override void Initialize(IndicatorContext context) {}
    public override void Tick(float dt) {}
    public override bool ShouldDespawn() {
        return true;
    }
    */
}


/*
Rules baked in
✔ Only visible if enemy off-screen
✔ Only within range
✔ No pulse
✔ Fades when irrelevant
(Low priority, informational)
*/