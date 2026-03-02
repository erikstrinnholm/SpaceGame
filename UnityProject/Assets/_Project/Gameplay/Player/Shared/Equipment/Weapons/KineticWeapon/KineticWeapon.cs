using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// MAGAZINE + HEAT?
public class KineticWeapon : WeaponBase, IAmmoUser, IHeatUser, IReloadInterruptable {
    //Public Interface
    public int CurrentAmmo => currentMagazine;
    public int MagazineSize => magazineSize;
    public int ReserveAmmo => reserveAmmo;      //think about this one
    public bool IsReloading => isReloading;
    public float ReloadProgress =>
        isReloading && reloadTime > 0f
            ? Mathf.Clamp01((Time.time - reloadStartTime) / reloadTime)
            : 0f;

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
    //Runtime Variables
    private float currentHeat = 0f;
    private bool overheated = false;
    private float coolingStartTime = 0f;
    private float lastFireTime = 0f;

    // ------------------ Magazine ----------------------------
    [Header("Ammo")]
    [SerializeField] private int reserveAmmo = -1;  //-1 = infinite
    private int magazineSize;
    private float reloadTime;
    private float fireRate;
    private string reloadSound;
    private string fireSound;
    //Runtime Variables
    private int currentMagazine;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private float reloadStartTime = 0f;
    private float reloadEndTime = 0f;

    // ------------------ Projectile ----------------------------
    [Header("Projectile")]
    private GameObject projectilePrefab;
    private float speed;
    private float duration;
    private float damage;
    private DamageType damageType;


    //---- UNITY LIFE CYCLE -------
    protected override void Start() {
        base.Start();
    }
    private void Update() {
        // ---------------- Heat Cooling ----------------
        if(usesHeat) {
            // Overheated? Wait until delay ends        
            if (overheated) {
                if (Time.time >= coolingStartTime) {
                    overheated = false; // cooling can begin
                    NotifyStateChanged(); // ✅ HERE
                }
            }
            // Start cooling after not firing for a bit
            else if (Time.time - lastFireTime > 0.1f && currentHeat > 0f) {
                currentHeat -= heatLossPerSecond * Time.deltaTime;
                currentHeat = Mathf.Max(0f, currentHeat);
            }
        }
        
        // ---------------- Reload Timing ----------------
        if (isReloading && Time.time >= reloadEndTime) {
            CompleteReload();
        }
    }
    public override void Initialize(Weapon runtimeWeapon, List<Transform> assignedFirePoints) {
        base.Initialize(runtimeWeapon, assignedFirePoints);

        // ----------------- TYPE CHECK -----------------
        var kinetic = runtimeWeapon.Kinetic;
        if (kinetic == null) {
            Debug.LogError("Tried to initialize KineticWeapon with non-kinetic data!");
            return;
        }

        // --- HEAT SYSTEM ---
        usesHeat            = kinetic.usesHeat;
        maxHeat             = kinetic.maxHeat;
        heatPerShot         = kinetic.heatPerShot;
        heatLossPerSecond   = kinetic.heatLossPerSecond;
        overheatDelay       = kinetic.overheatDelay;
        overheatSound       = kinetic.overheatSound;

        // --- MAGAZINE SYSTEM ---
        fireRate            = kinetic.fireRate;
        fireSound           = kinetic.fireSound;
        magazineSize        = kinetic.magazineSize;
        currentMagazine     = magazineSize;        
        reloadTime          = kinetic.reloadTime;
        reloadSound         = kinetic.reloadSound;
        // reserveAmmo = runtimeWeapon.Kinetic_ReserveAmmo; (later if I add it)

        // --- PROJECTILE DATA ---
        projectilePrefab    = kinetic.projectilePrefab;
        speed               = kinetic.projectileLaunchSpeed;
        duration            = kinetic.projectileDuration;
        damage              = kinetic.baseDamage;
        damageType          = kinetic.damageType;
    }    


    //---- FIRE METHOD ---------
    public override void Fire(Transform ship, Transform crosshair) {
        if (!CanFire()) return;

        // Heat
        if(usesHeat) AddHeat();

        // Rate Limiting
        lastFireTime = Time.time;
        nextFireTime = Time.time + fireRate;

        // Fire from all assigned fire points, stop if magazine is empty
        foreach (var fp in firePoints) {
            if (currentMagazine <= 0) break;

            Vector3 fireDir = GetFireDirection(fp, crosshair);
            GameObject shotObj = Instantiate(projectilePrefab, fp.position, Quaternion.LookRotation(fireDir), ProjectileSpawnParent);

            // Initialize projectile
            if (shotObj.TryGetComponent<KineticShot>(out var shot))
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

            // Consume ammo per firePoint
            currentMagazine = Mathf.Max(0, currentMagazine - 1);
        }

        // Audio
        if (!string.IsNullOrEmpty(fireSound))
            CoreRoot.Instance.Audio.Play(fireSound);

        NotifyStateChanged();
    }



//HEAT+MAGAZINE
    public override bool CanFire() {
        // Step 1: Heat
        if (usesHeat && (overheated || currentHeat + heatPerShot > maxHeat)) 
            return false;

        // Step 2: Magazine / Fire rate
        if (isReloading) return false;
        if (Time.time < nextFireTime) return false;
        if (currentMagazine <= 0) {
            Reload();
            return false;
        }
        return true;
    }





// ---------------- RELOAD LOGIC -----------------
    public override void Reload() {
        if (!CanReload()) return;
        BeginReload();    
    }
    private bool CanReload() {
        if (isReloading) return false;
        if (currentMagazine >= magazineSize) return false;
        if (reserveAmmo == 0) return false;
        return true;        
    }
    private void BeginReload() {
        isReloading = true;
        reloadStartTime = Time.time;
        reloadEndTime = reloadStartTime + reloadTime;
        if (!string.IsNullOrEmpty(reloadSound))
            CoreRoot.Instance.Audio.Play(reloadSound);        
        NotifyStateChanged(); // ✅ HERE
    }
    private void CompleteReload() {
        isReloading = false;
        
        int ammoToLoad = CalculateAmmoToLoad();
        currentMagazine += ammoToLoad;

        if (reserveAmmo != -1) 
            reserveAmmo -= ammoToLoad;
        
        NotifyStateChanged(); // ✅ HERE
    }
    private int CalculateAmmoToLoad() {
        int needed = magazineSize - currentMagazine;
        if (reserveAmmo == -1) return needed;
        return Mathf.Min(needed, reserveAmmo);    
    }
    public void InterruptReload() {
        if (!isReloading) return;
        isReloading = false;
        NotifyStateChanged();
    }




// ---------------- HEAT LOGIC --------------------
    //Call this during firing to add heat
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
}
