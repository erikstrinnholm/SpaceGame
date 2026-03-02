using UnityEngine;

[System.Serializable]
public struct Damage {
    public float Amount;            // Damage value
    public DamageType Type;         // Type of damage

    // Impact data
    public Vector3 HitPoint;        // World hit position
    public Vector3 Direction;       // FROM source → target (normalized)

    // Attribution
    public GameObject Source;       // Who caused it (ship, turret, hazard)
    public DamageDealer Dealer;     // What caused it (projectile, beam, etc.)

    // Optional
    public float AreaOfEffect;      // > 0 = explosion / AoE

    // Constructor
    public Damage(
        float amount,
        DamageType type,
        Vector3 hitPoint,
        Vector3 direction,
        GameObject source,
        DamageDealer dealer,
        float areaOfEffect = 0f
    ) {
        Amount = amount;
        Type = type;
        HitPoint = hitPoint;
        Direction = direction.normalized;
        Source = source;
        Dealer = dealer;
        AreaOfEffect = areaOfEffect;
    }
}
