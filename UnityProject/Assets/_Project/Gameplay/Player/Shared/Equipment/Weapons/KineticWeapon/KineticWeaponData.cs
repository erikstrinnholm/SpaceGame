using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ==================== KINETIC WEAPON =====================
[CreateAssetMenu(menuName = "Equipment/KineticWeapon")]
public class KineticWeaponData : WeaponData {

    [Header("Fire Settings")]
    public float fireRate;
    public string fireSound;

    [Header("Projectile")]
    public float projectileLaunchSpeed;
    public float projectileDuration;

    [Header("Damage")]
    public DamageType damageType;
    public float baseDamage;

    [Header("Magazine Settings")]
    public int magazineSize;
    public float reloadTime;
    public string reloadSound = "";

    [Header("Heat System")]
    public bool usesHeat = false;
    public float maxHeat;
    public float heatPerShot;
    public float heatPerSecond;
    public float heatLossPerSecond;
    public float overheatDelay;
    public string overheatSound;    
}
