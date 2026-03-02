using System.Collections;
using System.Collections.Generic;


using UnityEngine;

public class PlayerLootMagnet : MonoBehaviour, ILootMagnetSource {

    [Header("Magnet Range")]
    [SerializeField] float magnetRadius = 15f;
    [SerializeField] float lootDistance = 1.2f;

    [Header("Pull Behavior")]
    [SerializeField] float basePullSpeed = 25f;
    [SerializeField] float maxPullSpeed = 15f;

    [Header("State")]
    [SerializeField] bool canLoot = true;

    [Header("Detection")]
    [SerializeField] LayerMask pickupLayer;

    public Transform MagnetOrigin => transform;
    public float MagnetRadius => magnetRadius;
    public float LootDistance => lootDistance;
    public bool CanLoot => canLoot;

    void Update() {
        AttractNearbyPickups();
    }

    void AttractNearbyPickups() {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            magnetRadius,
            pickupLayer
        );

        foreach (var hit in hits) {
            if (!hit.TryGetComponent(out Pickup pickup))
                continue;

            pickup.AssignTarget(this);
        }
    }

    public float PullSpeedAtDistance(float distance) {
        float t = 1f - Mathf.Clamp01(distance / magnetRadius);
        float speed = basePullSpeed * t;
        return Mathf.Min(speed, maxPullSpeed);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
#endif
}


