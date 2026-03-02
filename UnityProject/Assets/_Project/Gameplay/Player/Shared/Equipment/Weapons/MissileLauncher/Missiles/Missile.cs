using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum MissileEvent {
    OnLaunch,
    OnImpact,
    OnExpire,
    OnArmed
}

[RequireComponent(typeof(Rigidbody))]
public class Missile : DamageDealer, IDamageable {

    // -------- Runtime Data --------    
    private MissileData missileData;
    public MissileData GetMissileData() => missileData;

    private float spawnTime;
    private float launchSpeed;

    private bool hasExploded;
    private bool hasExpired;
    private bool isArmed;

    // ---- Split control (runtime only) ----
    private bool allowSplit = true;
    private bool hasSplit = false;
    public bool CanSplit => allowSplit && !hasSplit;
    public void DisableSplit() => allowSplit = false;
    public void MarkSplit() => hasSplit = true;

    // -------- Components --------
    private Rigidbody rb;
    private IDamageable lockedTarget;
    
    // ---------------- Behaviors ----------------
    private readonly List<IMissileBehavior> behaviors = new();


    // -------- Unity --------
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }



    // -------- Initialization --------
    public void Initialize(GameObject owner, int layer, MissileData data, IDamageable target, bool allowSplit = true) {
        this.owner = owner;
        missileData = data;
        lockedTarget = target;
        this.allowSplit = allowSplit;
        hasSplit = false;
        hasExploded = false;
        hasExpired = false;
        isArmed = data.startArmed;
        
        spawnTime = Time.time;

        // ---- DamageDealer setup ----        
        base.Initialize(owner, layer, DamageType.Impact, 0f);

        // ---- Capture launcher velocity ----
        launchSpeed = rb.velocity.magnitude;
        
        // ---- Instantiate Behaviors ----
        behaviors.Clear();
        if (data.behaviors != null) {
            foreach (var behavior in data.behaviors) {
                if (behavior == null) continue;

                var instance = Instantiate(behavior);
                instance.Initialize(this);
                behaviors.Add(instance);
            }
        }

        TriggerEvent(MissileEvent.OnLaunch);
        StartCoroutine(LifetimeRoutine());
    }


    // -------- Movement --------
    private void FixedUpdate() {
        if (hasExploded || hasExpired) return;

        float elapsed       = Time.time - spawnTime;

        // --- Acceleration ---
        float accelFactor   = missileData.acceleration <= 0f ? 1f : Mathf.Clamp01(elapsed / missileData.acceleration);
        float speed = Mathf.Lerp(launchSpeed, missileData.maxSpeed, accelFactor);

        // --- Homing ---
        if (lockedTarget != null) {
            Vector3 dir = (lockedTarget.Transform.position - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            rb.MoveRotation(
                Quaternion.RotateTowards(
                    rb.rotation,
                    targetRot,
                    missileData.turnRate * Time.fixedDeltaTime
                )
            );
        } else {
            // Straight flight: prevent spinning
            rb.angularVelocity = Vector3.zero;      //setting angular velocity of a kinematic body is not supported
        }
        // --- Apply forward velocity ---
        rb.velocity = transform.forward * speed;
    }


    // -------- IDamageable --------
    public Transform Transform => transform;
    public void TakeDamage(Damage damage) {
        if (hasExploded) return;
        TriggerExplosion();
    }

    // -------- Collision (override DamageDealer hook) --------
    protected override void OnHit(IDamageable target, Collider hitCollider) {
        if (!isArmed || hasExploded) return;
        TriggerExplosion();
    }


    // -------- Lifecycle --------
    private IEnumerator LifetimeRoutine() {
        yield return new WaitForSeconds(missileData.lifetime);
        HandleExpire();
    }    
    private void HandleExpire() {
        if (hasExploded || hasExpired) return;
        hasExpired = true;
        TriggerEvent(MissileEvent.OnExpire);
        Destroy(gameObject);
    }
    private void OnDestroy() {
        if (rb != null) rb.constraints = RigidbodyConstraints.None;
    }

    // ---------------- Explosion ----------------
    private void TriggerExplosion() {
        if (hasExploded) return;

        hasExploded = true;
        TriggerEvent(MissileEvent.OnImpact);

        Damage damagePacket = new Damage(
            amount: missileData.damage,
            type: missileData.damageType,
            hitPoint: transform.position,
            direction: rb.velocity.normalized,
            source: this.gameObject,
            dealer: this,
            areaOfEffect: missileData.radius
        );

        GameRoot.Instance.Explosion.ScheduleExplosion(
            damagePacket,
            missileData.explosionVFX
        );

        Destroy(gameObject, 0.05f);
    }


    // ---------------- Helpers (For Behaviors) ----------------
    public IDamageable GetTarget() => lockedTarget;
    public Rigidbody GetRigidbody() => rb;

    public void SetArmed(bool value) {
        isArmed = value;
        if (value) TriggerEvent(MissileEvent.OnArmed);
    }
    public void StopMovement() {
        if (!rb) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void TriggerEvent(MissileEvent evt) {
        foreach (var b in behaviors)
            b.OnEvent(evt);
    }
    public void TryReacquireTarget() {
        // Implement later

        /*
        if (lockedTarget == null) {
            // Find closest hostile target
            Transform newTarget = TargetingManager.Instance.GetClosestHostileTarget(transform.position);
            if (newTarget != null) {
                lockedTarget = newTarget;
            }
        }
        */
    }
}
