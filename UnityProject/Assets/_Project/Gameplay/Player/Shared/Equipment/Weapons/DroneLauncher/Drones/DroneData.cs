using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Drone")]
public class DroneData : ScriptableObject {

    [Header("Identity")]
    public string id;
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Behavior")]
    public DroneRole role;
    public float lifetime;
    public float maxRange;

    [Header("Combat")]
    public DamageType damageType;
    public float damagePerSecond;
    public float attackInterval;

    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float turnRate;
}
