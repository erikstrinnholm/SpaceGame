using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEnergyWeapon : EnemyAttackBase {
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 120f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private DamageType damageType;
    [SerializeField] private float damage = 10f;


    protected override void Awake() {
        base.Awake();
        speedPrediction = projectileSpeed; // feeds prediction
    }

    protected override void FireInternal(AimSolution solution, Vector3 finalAimPoint) {
        Vector3 dir = (finalAimPoint - firePoint.position).normalized;

        GameObject shotObj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir),
            projectileSpawnParent
        );
        
        if (shotObj.TryGetComponent<EnergyShot>(out var shot)) {
            shot.Initialize(
                owner: gameObject,
                layer: CollisionLayers.EnemyProjectiles,
                type: damageType,
                damageAmount: damage,
                lifetime: lifetime
            );
        }
        
        if (shotObj.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = dir * projectileSpeed;
    }
}
