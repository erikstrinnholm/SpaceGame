using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ==================== LAUNCHER ============================
[CreateAssetMenu(menuName = "Equipment/MissileLauncher")]
public class MissileLauncherData : WeaponData {

    [Header("Fire Settings")]
    public float fireRate;
    public string fireSound;

    [Header("Projectile")]
    public float projectileLaunchSpeed;

    [Header("Magazine Settings")]
    public int magazineSize;
    public float reloadTime;
    public string reloadSound = "";

    [Header("Missile")]
    public MissileData missileData;

    [Header("Lock-On")]
    public LockOnMode lockOnMode = LockOnMode.None;
    public string lockOnSound = "";
    public float lockOnTime;
}
