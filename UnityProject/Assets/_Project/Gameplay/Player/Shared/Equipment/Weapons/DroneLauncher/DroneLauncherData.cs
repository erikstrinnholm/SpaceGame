using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================== DRONE CONTROLLER ============================
[CreateAssetMenu(menuName = "Equipment/DroneLauncher")]
public class DroneLauncherData : WeaponData {
    [Header("Fire Settings")]
    public float fireRate;
    public string fireSound;    

    [Header("Projectile")]
    public float projectileLaunchSpeed;

    [Header("Magazine Settings")]
    public int magazineSize;
    public float reloadTime;
    public string reloadSound = "";

    [Header("Drone")]
    public DroneData droneData;
    public int maxActiveDrones;

    [Header("Lock-On")]
    public LockOnMode lockOnMode = LockOnMode.None;    
    public string lockOnSound = "";
    public float lockOnTime;
}
