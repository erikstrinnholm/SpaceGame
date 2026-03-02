using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Rifle : CharacterWeaponBase {
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private Transform firepoint;
    [SerializeField] private Transform ProjectileSpawnParent;
    [SerializeField] private LayerMask rayMask = ~0;            // Default: everything
    [SerializeField] private float maxRange = 2000f;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int reserveAmmo = 90;   // -1 = infinite

    [Header("Timing")]
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float reloadTime = 1.8f;

    [Header("Audio")]
    [SerializeField] private string fireSound = "";
    [SerializeField] public string reloadSound = "";

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private DamageType projectileDamageType = DamageType.Energy;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileDuration = 3f;



    // Runtime
    private int currentMagazine;
    private bool isReloading;
    private float nextFireTime;
    private float reloadFinishTime;
    private int projectileLayer;
    public override bool IsBusy => isReloading;

    // ================= UNITY =================
    private void Awake() {
        currentMagazine = magazineSize;
    }
    private void Start() {
        projectileLayer = CollisionLayers.PlayerProjectiles;
    }
    private void Update() {
        if (isReloading && Time.time >= reloadFinishTime) {
            CompleteReload();
        }
    }
    
    // ================= ATTACK =================
    public override void OnAttackHeld() {
        if (!CanFire()) return;
        Fire();
    }
    public override void OnAttackReleased() {
        animator.SetBool("IsShooting", false);
    }


    // ================= FIRE =================
    private void Fire() {
        // Cooldown
        nextFireTime = Time.time + fireRate;

        // Reduce ammo
        currentMagazine--;

        // Animation
        animator.SetBool("IsShooting", true);
        animator.SetTrigger("ShootTrigger");

        // Audio
        if (!string.IsNullOrEmpty(fireSound))
            CoreRoot.Instance.Audio.Play(fireSound);

        // Spawn projectile
        SpawnProjectile();

        // Auto reload if empty
        if (currentMagazine <= 0)
            OnReload();
    }
    private void SpawnProjectile() {
        if (projectilePrefab == null || firepoint == null) return;

        // Get Aim Direction
        Camera cam = Camera.main;
        if (cam == null) return;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, rayMask, QueryTriggerInteraction.Ignore))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxRange);
        Vector3 fireDir = (targetPoint - firepoint.position).normalized;

        // Spawn projectile
        GameObject shotObj = Instantiate(
            projectilePrefab,
            firepoint.position,
            Quaternion.LookRotation(fireDir),
            ProjectileSpawnParent
        );

        // Initialize projectile
        if (shotObj.TryGetComponent<EnergyShot>(out var shot)) {
            shot.Initialize(
                playerCharacter,
                projectileLayer,
                projectileDamageType,
                projectileDamage,
                projectileDuration
            );
        }

        // Apply launch velocity
        if (shotObj.TryGetComponent<Rigidbody>(out var rb)) {
            Vector3 launchVelocity = fireDir * projectileSpeed;
            rb.velocity = launchVelocity;
        }        
    }

    // ================= FIRE CHECK =================
    private bool CanFire() {
        if (isReloading) return false;
        if (Time.time < nextFireTime) return false;

        if (currentMagazine <= 0) {
            OnReload();
            return false;
        }
        return true;
    }

    // ================= RELOAD =================
    public override void OnReload() {
        if (!CanReload()) return;
        isReloading = true;
        reloadFinishTime = Time.time + reloadTime;

        animator.SetTrigger("ReloadTrigger");
    }
    private bool CanReload() {
        if (isReloading) return false;
        if (currentMagazine >= magazineSize) return false;
        if (reserveAmmo == 0) return false;
        return true;
    }
    private void CompleteReload() {
        isReloading = false;
        int needed = magazineSize - currentMagazine;
        int ammoToLoad = reserveAmmo == -1 ? needed : Mathf.Min(needed, reserveAmmo);
        currentMagazine += ammoToLoad;

        if (reserveAmmo != -1)
            reserveAmmo -= ammoToLoad;
    }

}