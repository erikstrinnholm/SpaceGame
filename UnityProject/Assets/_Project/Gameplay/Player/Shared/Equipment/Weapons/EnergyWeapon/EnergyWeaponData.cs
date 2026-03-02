using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ==================== ENERGY PROJECTILE WEAPON =====================
[CreateAssetMenu(menuName = "Equipment/EnergyWeapon")]
public class EnergyWeaponData : WeaponData {
    [Header("Fire Settings")]
    public float fireRate;
    public string fireSound;

    [Header("Projectile")]
    public float projectileLaunchSpeed;
    public float projectileDuration;

    [Header("Damage")]
    public DamageType damageType;
    public float baseDamage;

    [Header("Energy Cost")]
    public float energyPerShot;

    [Header("Heat System")]
    public bool usesHeat = false;
    public float maxHeat;
    public float heatPerShot;
    public float heatPerSecond;
    public float heatLossPerSecond;
    public float overheatDelay;
    public string overheatSound;    
}
