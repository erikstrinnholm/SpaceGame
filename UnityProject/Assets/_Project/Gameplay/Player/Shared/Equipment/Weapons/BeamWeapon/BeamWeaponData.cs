using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ==================== ENERGY BEAM WEAPON =====================
[CreateAssetMenu(menuName = "Equipment/BeamWeapon")]
public class BeamWeaponData : WeaponData {
    [Header("Beam Visual")]
    public Color beamColor1;
    public Color beamColor2;
    public float beamMinWidth;
    public float beamMaxWidth;
    public float beamPulseSpeed;

    [Header("Beam Settings")]
    public float beamMaxRange;
    public string beamSound;

    [Header("Damage")]
    public DamageType damageType;
    public float damagePerTick;
    public float tickRate;

    [Header("Energy Cost")]    
    public float energyPerSecond; 

    [Header("Heat System")]
    public bool usesHeat = false;
    public float maxHeat;
    public float heatPerShot;
    public float heatPerSecond;
    public float heatLossPerSecond;
    public float overheatDelay;
    public string overheatSound;    
}
