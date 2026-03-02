using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnergyShot : DamageDealer {

    public void Initialize(GameObject owner, int layer, DamageType type, float damageAmount, float lifetime) {
        base.Initialize(owner, layer, type, damageAmount);
        Destroy(gameObject, lifetime);
    }
    protected override void OnHit(IDamageable target, Collider hitCollider) {
        Destroy(gameObject);
    }
}
