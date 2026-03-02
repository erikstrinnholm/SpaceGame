using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public abstract class WeaponBase : MonoBehaviour {
    public Weapon weapon;   // runtime weapon data
    public event Action OnWeaponStateChanged;
    protected void NotifyStateChanged() => OnWeaponStateChanged?.Invoke();

    [Header("References")]
    [SerializeField] protected Transform vfxSpawnParent;
    [SerializeField] protected Transform ProjectileSpawnParent;
    
    [Header("Fire Points")]
    protected List<Transform> firePoints = new();

    [Header("Layer")]
    protected int projectileLayer;
    protected int missileLayer;
    protected int droneLayer;
    protected LayerMask lockOnMask;     //missile & drone
    protected LayerMask rayMask;        
    protected GameObject owner;

    // ================= UNITY =================
    protected virtual void Start() {
        owner = transform.root.gameObject;
        
        projectileLayer = CollisionLayers.PlayerProjectiles;
        missileLayer = CollisionLayers.Missiles;
        droneLayer = CollisionLayers.Drones;
        lockOnMask = CollisionLayers.PlayerLockOnMask;
        rayMask = CollisionLayers.PlayerRaycastingMask;
    }
    
    public virtual void Initialize(Weapon runtimeWeapon, List<Transform> assignedFirePoints) {
        if (runtimeWeapon == null) {
            Debug.LogError($"{name}: Weapon runtime is NULL!");
            return;
        }
        weapon = runtimeWeapon;

        // Assign fire points dynamically
        firePoints.Clear();
        if (assignedFirePoints != null && assignedFirePoints.Count > 0) {
            firePoints.AddRange(assignedFirePoints);
        }
    }

    // ---------------- PUBLIC API ----------------
    public abstract void Fire(Transform ship, Transform crosshair);
    public virtual void StopFiring() {}    
    public virtual void Reload() {}         
    public virtual bool CanFire() => true;



    // Used to aim from weapon origin toward the crosshair
    protected Vector3 GetFireDirection(Transform origin, Transform crosshair, float maxRange = 5000f) {
        Camera cam = Camera.main;
        if (cam == null) return origin.forward;
        Ray ray = cam.ScreenPointToRay(crosshair.position);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, rayMask, QueryTriggerInteraction.Ignore)) {
            return (hit.point - origin.position).normalized;
        }
        return (ray.GetPoint(maxRange) - origin.position).normalized;
    }



    // Returns first valid lock-on target under crosshair
    protected IDamageable GetTarget(Transform crosshair, float maxRange = 5000f) {
        Camera cam = Camera.main;
        if (cam == null) return null;

        Ray ray = Camera.main.ScreenPointToRay(crosshair.position);
        LayerMask combinedMask = rayMask | lockOnMask;

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, combinedMask, QueryTriggerInteraction.Ignore)) {
            /*
            // Ignore self/owner hits
            if (owner != null && hit.collider.transform.IsChildOf(owner.transform))
                return null;
            */
            // Only return if hit layer is in lockOnMask
            if (((1 << hit.collider.gameObject.layer) & lockOnMask.value) != 0) {
                return hit.collider.GetComponentInParent<IDamageable>();
            }
        }
        return null;
    }




}

