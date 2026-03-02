using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ==================== BASE WEAPON CLASS  ============================
public abstract class WeaponData : EquipmentData {
    [Header("Weapon Base")]
    public GameObject projectilePrefab;
    public int firePointsCount = 1;     //accepts 1, 2, or 4
}