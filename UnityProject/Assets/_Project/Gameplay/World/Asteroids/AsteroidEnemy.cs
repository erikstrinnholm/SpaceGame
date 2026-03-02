using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class AsteroidEnemy : EnemyBase {
    [Header("Unity References")]
    [SerializeField] private Collider damageCollider;           // Trigger collider for DamageDealer
    [SerializeField] private Collider physicsCollider;          // Solid collider for physics

    [Header("Setup")]
    [SerializeField] private List<AsteroidSizeConfig> sizeConfigs;
    [SerializeField] private AsteroidSize initialSize = AsteroidSize.Size3;

    [Header("Splitting")]
    [SerializeField] private bool splitting = true;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float ignoreChildCollisionDelay = 0.15f;

    //Internal
    private AsteroidSizeConfig currentSizeConfig;
    private Rigidbody rb;


    // ================= UNITY =================
    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        ApplySize(initialSize);
        ApplyRotation();
    }


    
    public void ApplySize(AsteroidSize size) {
        currentSizeConfig = GetConfig(size);
        if (currentSizeConfig == null) {
            Debug.LogError($"Missing AsteroidSizeConfig for {size}", this);
            return;
        }

        SetMaxHP(currentSizeConfig.maxHP);

        float scale = Random.Range(
            currentSizeConfig.scaleRange.x,
            currentSizeConfig.scaleRange.y
        );

        transform.localScale = Vector3.one * scale;
        initialSize = size;
    }
    private AsteroidSizeConfig GetConfig(AsteroidSize size) {
        foreach (var cfg in sizeConfigs) {
            if (cfg != null && cfg.size == size)
                return cfg;
        }
        return null;
    }
    private void ApplyRotation() {
        if (rb == null || currentSizeConfig == null) return;
        Vector3 randomRotation = new Vector3(
            Random.Range(currentSizeConfig.spinMin, currentSizeConfig.spinMax) * (Random.value < 0.5f ? -1f : 1f),
            Random.Range(currentSizeConfig.spinMin, currentSizeConfig.spinMax) * (Random.value < 0.5f ? -1f : 1f),
            Random.Range(currentSizeConfig.spinMin, currentSizeConfig.spinMax) * (Random.value < 0.5f ? -1f : 1f)
        );
        // Apply as radians per second
        rb.angularVelocity = randomRotation * Mathf.Deg2Rad;
    }


    // ================= DEATH =================
    protected override void Death(Damage killingDamage) {
        if (splitting && initialSize != AsteroidSize.Size1) {
            SplitAsteroid(killingDamage.Direction);
        }
        base.Death(killingDamage);
    }


    // ================= SPLITTING =================
    private void SplitAsteroid(Vector3 impactDirection) {
        // Determine next size
        AsteroidSize nextSize = initialSize switch {
            AsteroidSize.Size3 => AsteroidSize.Size2,
            AsteroidSize.Size2 => AsteroidSize.Size1,
            _ => AsteroidSize.Size1
        };

        // Get size config
        AsteroidSizeConfig nextConfig = GetConfig(nextSize);
        if (nextConfig == null) return;

        Transform parent = transform.parent;
        Vector3 inheritedVelocity = rb.velocity * currentSizeConfig.velocityInheritance;

        Vector3[] directions = SpawnSpreadUtility.GenerateRadialDirections(
            currentSizeConfig.splitCount,
            impactDirection
        );
        
        // Arrays to track colliders for ignoring temporarily
        Collider[] childPhysicsColliders = new Collider[currentSizeConfig.splitCount];
        Collider[] childDamageColliders = new Collider[currentSizeConfig.splitCount];


        for (int i = 0; i < currentSizeConfig.splitCount; i++) {
            // Instantiate child asteroid
            GameObject child = Instantiate(
                asteroidPrefab,
                transform.position,
                Quaternion.identity,
                parent
            );

            AsteroidEnemy childAsteroid = child.GetComponent<AsteroidEnemy>();
            childAsteroid.ApplySize(nextSize);
            childAsteroid.ApplyRotation();
            
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            childRb.velocity = inheritedVelocity;

            // Offset to prevent initial overlap
            float childRadius = childAsteroid.GetApproximateRadius();
            child.transform.position += directions[i] * childRadius;

            // Add impulse for separation
            childRb.AddForce(directions[i] * currentSizeConfig.splitImpulse, ForceMode.Impulse);

            // Store references to child colliders
            childPhysicsColliders[i] = childAsteroid.physicsCollider;
            childDamageColliders[i] = childAsteroid.damageCollider;

            // Ignore parent ↔ child physics collision immediately
            if (physicsCollider && childAsteroid.physicsCollider)
                Physics.IgnoreCollision(physicsCollider, childAsteroid.physicsCollider, true);
        }
        
        // Coroutine handles sibling collisions and triggers
        StartCoroutine(IgnoreSiblingCollisions(childPhysicsColliders, childDamageColliders));
    }

    // ================= COLLISION HELPERS =================
    private IEnumerator IgnoreSiblingCollisions(Collider[] physicsCols, Collider[] damageCols) {
        // Disable damage colliders temporarily
        foreach (var dc in damageCols)
            if (dc) dc.enabled = false;

        // Ignore child ↔ child physics collisions temporarily
        for (int i = 0; i < physicsCols.Length; i++) {
            for (int j = i + 1; j < physicsCols.Length; j++) {
                if (physicsCols[i] && physicsCols[j])
                    Physics.IgnoreCollision(physicsCols[i], physicsCols[j], true);
            }
        }

        yield return new WaitForSeconds(ignoreChildCollisionDelay);

        // Restore physics collisions
        for (int i = 0; i < physicsCols.Length; i++) {
            for (int j = i + 1; j < physicsCols.Length; j++) {
                if (physicsCols[i] && physicsCols[j])
                    Physics.IgnoreCollision(physicsCols[i], physicsCols[j], false);
            }
        }
        // Re-enable triggers
        foreach (var dc in damageCols)
            if (dc) dc.enabled = true;
    }
    private float GetApproximateRadius() {
        if (!physicsCollider) return transform.localScale.x * 0.4f;
        return physicsCollider.bounds.extents.magnitude;
    }
}
