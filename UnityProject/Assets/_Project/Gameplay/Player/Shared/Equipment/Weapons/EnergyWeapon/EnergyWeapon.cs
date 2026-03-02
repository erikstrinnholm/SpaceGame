using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ENERGY + HEAT + MULTIPLE FIREPOINTS
public class EnergyWeapon : WeaponBase, IEnergyUser, IHeatUser {
    //Public Interface
    public float CurrentHeat => currentHeat;
    public float MaxHeat => maxHeat;
    public bool IsOverheated => overheated;
    public float OverheatProgress =>
        overheated && overheatDelay > 0f
            ? Mathf.Clamp01((coolingStartTime - Time.time) / overheatDelay)
            : 0f;
   
    // ------------------ Heat System -------------------------
    private bool usesHeat;
    private float maxHeat;
    private float heatPerShot;
    private float heatLossPerSecond;
    private float overheatDelay;
    private string overheatSound;
    //Runtime
    private float currentHeat = 0f;
    private bool overheated = false;
    private float coolingStartTime = 0f;
    private float lastFireTime = 0f;
    
    // ------------------ Energy System -------------------------
    private EnergyManager energyManager;
    private float energyPerShot;    
    private string fireSound;
    private float fireRate;
    private float nextFireTime = 0f;

    // ------------------ Projectile ----------------------------
    private GameObject projectilePrefab;
    private float speed;
    private float duration;
    private float damage;
    private DamageType damageType;



// ---------------- UNITY LOGIC --------------------
    protected override void Start() {
        base.Start();
    }
    private void Update() {
        HandleHeatCooling();
    }
    private void HandleHeatCooling() {
        if (!usesHeat) return;

        // Overheated? Wait for delay
        if (overheated && Time.time >= coolingStartTime) {
            overheated = false;
            NotifyStateChanged();
        }
        // Passive cooling when not firing
        else if (Time.time - lastFireTime > 0.1f && currentHeat > 0f) {
            currentHeat -= heatLossPerSecond * Time.deltaTime;
            currentHeat = Mathf.Max(0f, currentHeat);
        }
    }



// ---------------- WEAPON BASE LOGIC --------------------
    public override void Initialize(Weapon runtimeWeapon, List<Transform> assignedFirePoints) {
        base.Initialize(runtimeWeapon, assignedFirePoints);

        // ----------------- TYPE CHECK -----------------
        var energy = runtimeWeapon.Energy;
        if (energy == null) {
            Debug.LogError("Tried to initialize EnergyWeapon with non-energy data!");
            return;
        }

        // --- HEAT SYSTEM ---
        usesHeat            = energy.usesHeat;
        maxHeat             = energy.maxHeat;
        heatPerShot         = energy.heatPerShot;
        heatLossPerSecond   = energy.heatLossPerSecond;
        overheatDelay       = energy.overheatDelay;
        overheatSound       = energy.overheatSound;

        // --- ENERGY SYSTEM ---
        energyPerShot       = energy.energyPerShot;
        fireSound           = energy.fireSound;
        fireRate            = energy.fireRate;

        // --- PROJECTILE DATA ---
        projectilePrefab    = energy.projectilePrefab;
        speed               = energy.projectileLaunchSpeed;
        duration            = energy.projectileDuration;
        damage              = energy.baseDamage;
        damageType          = energy.damageType;
    }
    public override void Fire(Transform ship, Transform crosshair) {
        if (!CanFire()) return;

        // Rate Limiting
        lastFireTime = Time.time;
        nextFireTime = Time.time + fireRate;

        // Fire from all assigned firePoints
        foreach (var fp in firePoints) {
            if (!energyManager.Consume(energyPerShot)) break;       //ENERGY PER SHOT
            if (overheated) break;
            if(usesHeat) AddHeat();                                 //HEAT PER SHOT

            Vector3 fireDir = GetFireDirection(fp, crosshair);
            GameObject shotObj = Instantiate(projectilePrefab, fp.position, Quaternion.LookRotation(fireDir), ProjectileSpawnParent);

            // Initialize projectile
            if (shotObj.TryGetComponent<EnergyShot>(out var shot))
                shot.Initialize(
                    ship.gameObject,
                    projectileLayer,
                    damageType,
                    damage,
                    duration
                );

            // Launch
            if (shotObj.TryGetComponent<Rigidbody>(out var rb)) {
                Vector3 launchVelocity = fireDir * speed;
                if (ship.GetComponentInParent<Rigidbody>() is Rigidbody shipRb)
                    launchVelocity += shipRb.velocity;
                rb.velocity = launchVelocity;
            }
        }
        // Audio
        if (!string.IsNullOrEmpty(fireSound))
            CoreRoot.Instance.Audio.Play(fireSound);

        NotifyStateChanged();
    }
    public override bool CanFire() {
        if (Time.time < nextFireTime)
            return false;

        // ---- Heat ----
        if (usesHeat && overheated) {
                return false;
        }
        // ---- Energy via reactor ----
        if (!energyManager.HasEnergy(energyPerShot)) {
            return false;
        }
        return true;
    }



// ---------------- HEAT LOGIC --------------------
    private void AddHeat() {
        if (overheated) return;
        currentHeat += heatPerShot;
        currentHeat = Mathf.Min(currentHeat, maxHeat);

        if (currentHeat >= maxHeat) {
            TriggerOverheat();
        }
    }
    private void TriggerOverheat() {
        overheated = true;
        coolingStartTime = Time.time + overheatDelay;
        if (!string.IsNullOrEmpty(overheatSound))
            CoreRoot.Instance.Audio.Play(overheatSound);
        NotifyStateChanged(); // ✅ HERE

    }
    public void ResetHeat() {
        currentHeat = 0f;
        overheated = false;
        coolingStartTime = 0f;
        lastFireTime = 0f;
        NotifyStateChanged(); // ✅ HERE
    }



// ---------------- ENERGY LOGIC --------------------
    public void SetEnergyManager(EnergyManager energy) {
        energyManager = energy;
    }
}
