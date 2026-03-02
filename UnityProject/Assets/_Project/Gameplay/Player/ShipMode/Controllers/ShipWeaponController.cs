using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class ShipWeaponController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private RectTransform crosshair;       
    [SerializeField] private Transform playerShip;          

    [Header("Fire Points")]
    [SerializeField] private Transform left;
    [SerializeField] private Transform leftCenter;
    [SerializeField] private Transform center;
    [SerializeField] private Transform rightCenter;
    [SerializeField] private Transform right;


    [Header("Weapon Prefabs")]
    [SerializeField] private WeaponBase kineticWeaponPrefab;
    [SerializeField] private WeaponBase energyWeaponPrefab;
    [SerializeField] private WeaponBase beamWeaponPrefab;
    [SerializeField] private WeaponBase missileLauncherPrefab;
    [SerializeField] private WeaponBase droneLauncherPrefab;    


    //runtime
    private List<WeaponBase> weapons = new List<WeaponBase>();
    private int currentWeaponIndex = 0;
    private bool fireInput = false;

    public event Action<WeaponBase> OnWeaponSwitched;                   // 🔹 New Event


    private PlayerEquipment playerEquipment =>  GameRoot.Instance.PlayerData.PlayerEquipment;
    public WeaponBase CurrentWeapon => GetCurrentWeapon();


    // ------------------ UNITY LIFECYCLE ------------------
    private void Start() {
        RefreshWeaponList();
        currentWeaponIndex = 0;
        OnWeaponSwitched?.Invoke(GetCurrentWeapon());
    }


    private void Update() {
        var current = GetCurrentWeapon();
        if (current == null) return;

        // 1️⃣ Handle firing input
        if (fireInput)
            current.Fire(playerShip, crosshair);
        else
            current.StopFiring();
    
        // 2️⃣ Update lock-on if the weapon uses it
        if (current is ILockOnUser lockOnWeapon)
            lockOnWeapon.UpdateLockOn(crosshair);
    }


    // --------------- PUBLIC INPUT HANDLERS --------------
    public void OnFire(bool pressed) {
        fireInput = pressed;   //new
    }

    public void OnSwitchWeapon() {
        if (weapons.Count == 0) return;
        
        // --- stop current weapon safely ---
        GetCurrentWeapon()?.StopFiring();

        // --- switch to next weapon ---
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        var newWeapon = GetCurrentWeapon();
        OnWeaponSwitched?.Invoke(newWeapon);                    // 🔹 Notify listeners
    }

    public void OnReload() {
        var current = GetCurrentWeapon();
        if (current == null) return;

        current.StopFiring();        
        current.Reload();
        Debug.Log("Reloading");
    }


    // ----------- HELPERS --------------
    private WeaponBase GetCurrentWeapon() {
        if (weapons.Count == 0) return null;
        return weapons[currentWeaponIndex];
    }



    // --------- REFRESHES LIST --------
    public void RefreshWeaponList() {
        foreach(var w in weapons) {
            if (w != null) Destroy(w.gameObject);
        }
        InitializeList();
    }

    private void InitializeList() {
        weapons.Clear();

        // Ship fire points as array for easy indexing
        Transform[] allFirePoints = new Transform[] { left, leftCenter, center, rightCenter, right };

        for (int i = 0; i < playerEquipment.weaponSlots.Count; i++) {
            Weapon weapon = playerEquipment.GetFromWeaponSlot(i);
            if (weapon == null) {
                Debug.LogWarning($"No weapon found in weapon slot {i}");
                continue;
            }
            WeaponBase prefab = GetPrefab(weapon);
            if (prefab == null) continue;

            WeaponBase instance = Instantiate(prefab, transform);

            // Determine which fire points to assign
            int count = weapon.WeaponData.firePointsCount;
            List<Transform> assignedFirePoints = count switch {
                1 => new List<Transform> { allFirePoints[2] }, // Center
                2 => new List<Transform> { allFirePoints[0], allFirePoints[4] }, // Left + Right
                4 => new List<Transform> { allFirePoints[0], allFirePoints[1], allFirePoints[3], allFirePoints[4] }, // Left, LeftCenter, RightCenter, Right
                _ => new List<Transform> { allFirePoints[2] } // fallback to center
            };

            //💡 Base initialize
            instance.Initialize(weapon, assignedFirePoints);

            //💡 optional dependency injection
            if (instance is IEnergyUser energyUser)
                energyUser.SetEnergyManager(ShipRoot.Instance.Energy);

            weapons.Add(instance);
        }
    }

    private WeaponBase GetPrefab(Weapon weapon) {
        if (weapon.IsKinetic) return kineticWeaponPrefab;
        if (weapon.IsEnergy) return energyWeaponPrefab;
        if (weapon.IsBeam) return beamWeaponPrefab;
        if (weapon.IsMissileLauncher) return missileLauncherPrefab;
        if (weapon.IsDroneLauncher) return droneLauncherPrefab;

        Debug.LogWarning("WeaponController: No prefab for weapon category");
        return null;
    }
}
