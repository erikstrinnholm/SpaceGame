using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// MAGAZINE + LOCK-ON 
public class MissileLauncher : WeaponBase, IAmmoUser, ILockOnUser, IReloadInterruptable
{
    // ------------- IAmmoUser Public Interface--------------------
    public int CurrentAmmo => currentMagazine;
    public int MagazineSize => magazineSize;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;
    public float ReloadProgress =>
        isReloading && reloadTime > 0f
            ? Mathf.Clamp01((Time.time - reloadStartTime) / reloadTime)
            : 0f;

    // ------------- ILockOnUser Public Interface------------------
    public bool UsesLockOn => lockOnMode != LockOnMode.None;
    public bool RequiresLockOn => lockOnMode == LockOnMode.Required;
    public bool HasLock => UsesLockOn && hasLock;
    public float LockProgress =>
        UsesLockOn && lockOnTime > 0f
            ? Mathf.Clamp01(currentLockTimer / lockOnTime)
            : 0f;    
    public IDamageable LockedTarget => hasLock ? potentialTarget : null;


    // ------------------ Magazine ----------------------------
    [Header("Ammo")]
    [SerializeField] private int reserveAmmo = -1;  //-1 = infinite
    private int magazineSize;
    private int currentMagazine;
    private float fireRate;
    private float reloadTime;
    private float nextFireTime = 0f;
    private bool isReloading = false;
    private float reloadStartTime = 0f;
    private float reloadEndTime = 0f;
    private string fireSound;
    private string reloadSound;

    // ------------------ Lock-On ----------------------------
    private LockOnMode lockOnMode;
    private float lockOnTime;
    private string lockOnSound;
    private IDamageable potentialTarget;
    private float currentLockTimer = 0f;
    private bool hasLock = false; 


    // ------------------ Missile ----------------------------
    private GameObject missilePrefab;
    private MissileData missileData;
    private float launchSpeed;



    // ================= UNITY =================
    protected override void Start() {
        base.Start();
    }
    private void Update() {
        if (isReloading && Time.time >= reloadEndTime) {
            CompleteReload();
        }
    }


    // ================= INITIALIZATION =================
    public override void Initialize(Weapon runtimeWeapon, List<Transform> assignedFirePoints) {
        base.Initialize(runtimeWeapon, assignedFirePoints);

        var launcherData = runtimeWeapon.MissileLauncher;
        if (launcherData == null) {
            Debug.LogError("MissileLauncher initialized with invalid weapon data!");
            return;
        }

        // --- MAGAZINE SYSTEM ---
        magazineSize        = launcherData.magazineSize;
        currentMagazine     = magazineSize;
        fireRate            = launcherData.fireRate;
        reloadTime          = launcherData.reloadTime;
        fireSound           = launcherData.fireSound;
        reloadSound         = launcherData.reloadSound;

        // --- LOCK-ON SYSTEM ---
        lockOnMode          = launcherData.lockOnMode;
        lockOnTime          = launcherData.lockOnTime;
        lockOnSound         = launcherData.lockOnSound;

        // --- PROJECTILE DATA ---
        missilePrefab       = launcherData.projectilePrefab;
        missileData         = launcherData.missileData;
        launchSpeed         = launcherData.projectileLaunchSpeed;
    }


    // ================= FIRING =================
    public override void Fire(Transform ship, Transform crosshair) {
        if (!CanFire()) return;
        
        // Rate Limiting
        nextFireTime = Time.time + fireRate;

        // Lock-on target
        IDamageable lockedTarget = hasLock ? potentialTarget : null;
    
        // Fire from all assigned fire points, respecting current magazine
        foreach (var fp in firePoints) {
            if (currentMagazine <= 0) break; // stop if no ammo left

            Vector3 fireDir = GetFireDirection(fp, crosshair);
            GameObject missileObj = Instantiate(missilePrefab, fp.position, Quaternion.LookRotation(fireDir), ProjectileSpawnParent);

            // Initialize missile
            if (missileObj.TryGetComponent<Missile>(out var missile))
                missile.Initialize(
                    ship.gameObject,
                    missileLayer,
                    missileData,
                    lockedTarget
                );

            // Launch
            if (missileObj.TryGetComponent<Rigidbody>(out var rb)) {
                Vector3 launchVelocity = fireDir * launchSpeed;
                if (ship.GetComponentInParent<Rigidbody>() is Rigidbody shipRb)
                    launchVelocity += shipRb.velocity;
                rb.velocity = launchVelocity;
            }

            // Consume ammo per missile fired
            currentMagazine = Mathf.Max(0, currentMagazine - 1);
        }
        // Audio
        if (!string.IsNullOrEmpty(fireSound))
            CoreRoot.Instance.Audio.Play(fireSound);
        
        // Reset lock and notify state
        ResetLock();
        NotifyStateChanged(); // ✅ HERE
    }


    private void ResetLock() {
        if (!UsesLockOn) return;
        hasLock = false;
        currentLockTimer = 0f;
        potentialTarget = null;
    }


// ---------------- LOCK-ON LOGIC -----------------
    public void UpdateLockOn(Transform crosshair) {
        if (!UsesLockOn) return;
        IDamageable foundTarget = GetTarget(crosshair);

        if (hasLock) {
            if (foundTarget != potentialTarget) {
                ResetLock();
                NotifyStateChanged(); // ✅ HERE
            }
            return;
        }

        // If still aiming at same target → accumulate lock time
        if (foundTarget != null && foundTarget == potentialTarget) {
            currentLockTimer += Time.deltaTime;

            if (currentLockTimer >= lockOnTime) {
                hasLock = true;
                if (!string.IsNullOrEmpty(lockOnSound))
                    CoreRoot.Instance.Audio.Play(lockOnSound);
                NotifyStateChanged(); // ✅ HERE
            }
        } else {
            potentialTarget = foundTarget;
            currentLockTimer = 0f;
        }
    }


// ---------------- RELOAD LOGIC -----------------
    public override bool CanFire() {
        if (isReloading) return false;
        if (Time.time < nextFireTime) return false;
        if (RequiresLockOn && !hasLock) return false;

        // Auto-reload if empty
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
}
