using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSticky : MonoBehaviour {
    /*
    [SerializeField] private IEnemyTargeting fallback;
    [SerializeField] private float maxLoseDistance = 600f;
    private IDamageable currentTarget;

    public IDamageable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null;

    private Transform origin;

    private void Awake() {
        origin = transform;
        fallback = GetComponent<TargetingNearestVisible>();
    }

    public void TickTargeting() {
        if (currentTarget != null) {
            float dist = Vector3.Distance(origin.position, currentTarget.Transform.position);
            if (dist <= maxLoseDistance)
                return;
        }

        currentTarget = fallback?.CurrentTarget;
    }

    public void ClearTarget() {
        currentTarget = null;
    }
    */
}

