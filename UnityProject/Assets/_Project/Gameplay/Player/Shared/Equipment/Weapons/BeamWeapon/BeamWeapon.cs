using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;


/**
 * Things to improve, better damage Tick manager.
 */
public class BeamWeapon : WeaponBase, IEnergyUser, IHeatUser {
    // ================= PUBLIC INTERFACE =================
    public float CurrentHeat => currentHeat;
    public float MaxHeat => maxHeat;
    public bool IsOverheated => overheated;
    public float OverheatProgress =>
        overheated && overheatDelay > 0f
            ? Mathf.Clamp01((coolingStartTime - Time.time) / overheatDelay)
            : 0f;

    // ================= HEAT =================
    private bool usesHeat;
    private float maxHeat;
    private float heatPerSecond;
    private float heatLossPerSecond;
    private float overheatDelay;
    private string overheatSound;
    private float currentHeat = 0f;
    private bool overheated = false;
    private float coolingStartTime = 0f;
    private float lastFireTime = 0f;
    
    // ================= ENERGY =================
    private EnergyManager energyManager;
    private float energyPerSecond;

    // ================= BEAM DATA =================
    private GameObject beamPrefab;
    private float beamMaxRange;
    private float tickRate;
    private float damagePerTick;
    private string beamSound;
    private DamageType damageType;
    
    // ================= RUNTIME =================
    private bool firing = false;
    private readonly List<GameObject> beamInstances = new();
    private readonly List<VolumetricLineBehavior> beamLines = new();
    private readonly List<Coroutine> beamRoutines = new();

    // ================= VISUALS =================
    private Color color1;
    private Color color2;
    private float minWidth;
    private float maxWidth;
    private float pulseSpeed;


    // ================= UNITY =================
    protected override void Start() {
        base.Start();
        currentHeat = 0f;
        CreateBeamInstances();
    }
    private void Update() {
        HandleHeatCooling();
    }
    private void CreateBeamInstances() {
        foreach (var b in beamInstances)
            if (b != null) Destroy(b);

        beamInstances.Clear();
        beamLines.Clear();
        beamRoutines.Clear();

        foreach (var fp in firePoints) {
            if (fp == null) continue;

            var beamObj = Instantiate(beamPrefab, Vector3.zero, Quaternion.identity, ProjectileSpawnParent);
            beamObj.SetActive(false);

            beamInstances.Add(beamObj);
            beamLines.Add(beamObj.GetComponent<VolumetricLineBehavior>());
            beamRoutines.Add(null);
        }
    }


    // ================= INITIALIZATION =================
    public override void Initialize(Weapon runtimeWeapon, List<Transform> assignedFirePoints) {
        base.Initialize(runtimeWeapon, assignedFirePoints);

        // ----------------- TYPE CHECK -----------------
        var beamData = runtimeWeapon.Beam;
        if (beamData == null) {
            Debug.LogError("Tried to initialize BeamWeapon with non-beam data!");
            return;
        }

        // --- HEAT SYSTEM ---
        usesHeat            = beamData.usesHeat;
        maxHeat             = beamData.maxHeat;
        heatPerSecond       = beamData.heatPerSecond;
        heatLossPerSecond   = beamData.heatLossPerSecond;
        overheatDelay       = beamData.overheatDelay;
        overheatSound       = beamData.overheatSound;

        // --- ENERGY ---
        energyPerSecond     = beamData.energyPerSecond;

        // --- BEAM ---
        beamPrefab          = beamData.projectilePrefab;
        beamMaxRange        = beamData.beamMaxRange;
        tickRate            = beamData.tickRate;
        damagePerTick       = beamData.damagePerTick;
        beamSound           = beamData.beamSound;
        damageType          = beamData.damageType;
        //visuals
        color1              = beamData.beamColor1;
        color2              = beamData.beamColor2;
        minWidth            = beamData.beamMinWidth;
        maxWidth            = beamData.beamMaxWidth;
        pulseSpeed          = beamData.beamPulseSpeed;

        CreateBeamInstances();        
    }    
    
    
    // ================= FIRING =================    
    public override void Fire(Transform ship, Transform crosshair) {
        if (firing || !CanFire()) return;
        firing = true;
        lastFireTime = Time.time;
        NotifyStateChanged();

        // Audio (start Loop)
        if (!string.IsNullOrEmpty(beamSound))
            CoreRoot.Instance.Audio.Play(beamSound);

        // Start one coroutine per beam/firePoint
        for (int i = 0; i < firePoints.Count; i++) {
            int index = i;
            if (beamRoutines[index] != null)
                StopCoroutine(beamRoutines[index]);

            beamRoutines[index] = StartCoroutine(
                FireBeamRoutine(firePoints[index], beamInstances[index], beamLines[index], crosshair)
            );
        }
    }
    public override void StopFiring() {
        if (!firing) return;

        firing = false;
        NotifyStateChanged();

        // Stop looping sound
        if (!string.IsNullOrEmpty(beamSound))
            CoreRoot.Instance.Audio.Stop(beamSound);

        // Stop all beam coroutines
        for (int i = 0; i < beamRoutines.Count; i++) {
            if (beamRoutines[i] != null)
                StopCoroutine(beamRoutines[i]);
            
            beamRoutines[i] = null;
            beamInstances[i]?.SetActive(false);
        }
    }    
    public override bool CanFire() {
        if (usesHeat && overheated) return false;

        float energyRequired = energyPerSecond * Time.deltaTime * firePoints.Count;
        if (!energyManager.HasEnergy(energyRequired)) return false;

        return true;
    }
    

    private IEnumerator FireBeamRoutine( Transform fp, GameObject beamObj, VolumetricLineBehavior line, Transform crosshair) {
        float tickTimer = 0f;
        beamObj.SetActive(true);

        while (firing) {
            float dt = Time.deltaTime;

            if (!TryConsumeEnergy(dt)) break;

            if (usesHeat) {
                AddHeat(dt);
                if (overheated) break;
            }

            lastFireTime = Time.time;

            // ---------------- Beam start & direction ----------------
            Vector3 dir   = GetFireDirection(fp, crosshair, beamMaxRange);
            Vector3 start = fp.position + dir * 0.5f; // muzzle offset
            Vector3 end   = start + dir * beamMaxRange;

            // ---------------- Raycast to detect hit ----------------
            if (Physics.Raycast(start, dir, out RaycastHit hit, beamMaxRange, rayMask, QueryTriggerInteraction.Ignore)) {
                end = hit.point;

                // Deal damage if hit is damageable
                if (hit.collider.GetComponentInParent<IDamageable>() is IDamageable target) {

                    tickTimer += dt;
                    if (tickTimer >= tickRate) {
                        Vector3 damageDir = (hit.point - (owner != null ? owner.transform.position : fp.position)).normalized;
                        Damage dmg = new Damage(
                            amount: damagePerTick,
                            type: damageType,
                            hitPoint: hit.point,
                            direction: damageDir,
                            source: owner != null ? owner : fp.gameObject,
                            dealer: null
                        );
                        target.TakeDamage(dmg);
                        tickTimer = 0f;
                    }
                }
            }
            // ---------------- Beam visuals ----------------
            UpdateBeamVisual(line, start, end);
            yield return null;
        }
        beamObj.SetActive(false);
    }



// ================= BEAM VISUALS =================
    private void UpdateBeamVisual(VolumetricLineBehavior line, Vector3 start, Vector3 end) {
        if (line == null) return;
        line.SetStartAndEndPoints(start, end);

        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        line.LineColor = Color.Lerp(color1, color2, pulse);
        line.LineWidth = Mathf.Lerp(minWidth, maxWidth, pulse);
    }


// ---------------- HEAT LOGIC --------------------
    private void HandleHeatCooling() {
        if (!usesHeat) return;

        if (overheated && Time.time >= coolingStartTime) {
            overheated = false;
            NotifyStateChanged();
        } else if (Time.time - lastFireTime > 0.1f && currentHeat > 0f) {
            currentHeat -= heatLossPerSecond * Time.deltaTime;
            currentHeat = Mathf.Max(0f, currentHeat);
        }
    }
    private void AddHeat(float deltaTime) {
        if (overheated) return;

        currentHeat += heatPerSecond * deltaTime;
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
        
        NotifyStateChanged();
    }
    public void ResetHeat() {
        currentHeat = 0f;
        overheated = false;
        coolingStartTime = 0f;
        lastFireTime = 0f;
        NotifyStateChanged();
    }    


// ---------------- ENERGY LOGIC --------------------
    public void SetEnergyManager(EnergyManager energy) {
        energyManager = energy;
    } 
    private bool TryConsumeEnergy(float dt) {
        float amount = energyPerSecond * dt;
        if (!energyManager.HasEnergy(amount)) return false;
        energyManager.Consume(amount);
        return true;
    }
}
