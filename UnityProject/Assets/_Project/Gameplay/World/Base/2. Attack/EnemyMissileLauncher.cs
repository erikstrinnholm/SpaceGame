using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileLauncher : EnemyAttackBase {
    [Header("Missiles")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private MissileData missileData;
    [SerializeField] private float launchSpeed = 40f;
    [SerializeField] private float lockOnTime = 1.5f;
    private float lockTimer;
    private IDamageable lockedTarget;

    protected override void FireInternal(AimSolution solution, Vector3 finalAimPoint) {
        if (solution.target == null)
            return;

        // New target → reset lock
        if (lockedTarget != solution.target) {
            lockedTarget = solution.target;
            lockTimer = 0f;
            return;
        }
        // Accumulate lock time
        lockTimer += Time.deltaTime;
        if (lockTimer < lockOnTime)
            return;

        SpawnMissile(solution.target);
        lockTimer = 0f;
    }

    private void SpawnMissile(IDamageable target) {
        Vector3 dir = (target.Transform.position - firePoint.position).normalized;

        GameObject missileObj = Instantiate(
            missilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir),
            projectileSpawnParent
        );

        if (missileObj.TryGetComponent<Missile>(out var missile)) {
            missile.Initialize(
                owner: gameObject,
                layer: CollisionLayers.Missiles,
                data: missileData,
                target: target
            );
        }
        if (missileObj.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = dir * launchSpeed;

        ShipRoot.Instance.Indicator.ShowMissileIndicator(missileObj.transform);
    }
}

