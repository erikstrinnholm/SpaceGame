using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalStation : MonoBehaviour {
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 200f;

    [Header("Firing")]
    [SerializeField] private Transform firePoint; // point where projectiles spawn
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float projectileDamage = 10;
    [SerializeField] private float projectileDuration = 3f;
    [SerializeField] private float fireRate = 1f; // shots per second


    [SerializeField] private GameObject missilePrefab;


    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstructionMask;
    private int projectileLayer;



    private float nextFireTime = 0f;
    private Transform currentTarget;
    [SerializeField] protected Transform ProjectileSpawnParent;


    protected virtual void Start() {
        projectileLayer = CollisionLayers.EnemyProjectiles;
    }


    private void Update() {
        AcquireTarget();
        if (currentTarget != null)
        {
            TryShootAtTarget();
        }
    }

    private void AcquireTarget() {
        // Find enemies within radius
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (hits.Length == 0) {
            currentTarget = null;
            return;
        }

        // Pick the closest enemy
        float closestDist = float.MaxValue;
        Transform closest = null;
        foreach (var hit in hits) {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }
        currentTarget = closest;
    }

    private void TryShootAtTarget() {
        if (Time.time < nextFireTime) return;

        Vector3 dir = (currentTarget.position - firePoint.position).normalized;
        float dist = Vector3.Distance(firePoint.position, currentTarget.position);

        // Raycast to check line of sight
        if (!Physics.Raycast(firePoint.position, dir, dist, obstructionMask)) {
            ShootProjectile(dir);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void ShootProjectile(Vector3 direction) {
        // 1. Spawn Projectile
        if (projectilePrefab == null) return;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction), ProjectileSpawnParent);

        // 3. Initialize Shot
        if (proj.TryGetComponent<EnergyShot>(out var blast)) {
            blast.Initialize(
                this.gameObject,
                projectileLayer,
                DamageType.Energy,
                projectileDamage,
                projectileDuration);
        }

        // 4. Launch Shot
        if (proj.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.velocity = direction * projectileSpeed;
        }

        // 5. Audio
        //TODO - make it spatial and grab from AudioManager to this gameobject
    }



    private void ShootMissile() {

    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
