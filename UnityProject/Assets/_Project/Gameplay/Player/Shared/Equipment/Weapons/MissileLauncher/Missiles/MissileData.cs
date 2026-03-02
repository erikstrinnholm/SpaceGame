using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Equipment/Missile")]
public class MissileData : ScriptableObject {

    [Header("Identity")]
    public string id;
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float turnRate;
    public float lifetime;

    [Header("Explosion")]
    public DamageType damageType;
    public float damage;
    public float radius;
    public string explosionSound;
    public GameObject explosionVFX;


    [Header("Behaviors")]
    public List<MissileBehavior> behaviors;
    public bool startArmed = true;
    public bool canSplit = true;
}
