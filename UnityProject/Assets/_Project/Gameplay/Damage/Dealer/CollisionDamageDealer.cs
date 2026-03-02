using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CollisionDamageDealer : DamageDealer {
    
    [Header("References")]
    [SerializeField] private GameObject root;

    private void Awake() {
        owner = root;
    }

    // Called by DamageDealer after applying damage
    protected override void OnHit(IDamageable target, Collider hitCollider) {
    }
}
