using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Responsibilities
- Select who to attack and provide a rough aim point.
*/

public class TargetingNearestVisible : MonoBehaviour, IEnemyTargeting {
    [Header("Targeting")]
    [SerializeField] private float range = 500f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private Transform origin;

    [Header("Performance")]
    [SerializeField] private float retargetInterval = 0.25f;
    private float nextScanTime;
    private IDamageable currentTarget;
    public bool HasTarget => currentTarget != null;

    // =========== UNITY ============
    private void Awake() {
        if (origin == null)
            origin = transform;
    }


    // =========== INTERFACE ============
    public void TickTargeting() {
        if (Time.time < nextScanTime) return;
        nextScanTime = Time.time + retargetInterval;
        currentTarget = AcquireTarget();
    }
    public AimSolution GetAimSolution() {
        if (currentTarget == null)
            return default;

        return new AimSolution {
            target = currentTarget,
            aimPoint = currentTarget.Transform.position
        };
    }
    public void ClearTarget() {
        currentTarget = null;
    }



    // =========== TARGETING ================
    private IDamageable AcquireTarget() {
        Collider[] hits = Physics.OverlapSphere(origin.position, range, targetMask);
        if (hits.Length == 0)
            return null;

        float closestDist = float.MaxValue;
        IDamageable closest = null;

        foreach (var hit in hits) {
            var dmg = hit.GetComponentInParent<IDamageable>();
            if (dmg == null) continue;

            Transform t = dmg.Transform;
            if (t == null) continue;

            float dist = Vector3.Distance(origin.position, t.position);
            if (dist >= closestDist) continue;

            Vector3 dir = (t.position - origin.position).normalized;
            if (!Physics.Raycast(origin.position, dir, dist, obstructionMask)) {
                closestDist = dist;
                closest = dmg;
            }
        }
        return closest;
    }
}
