using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Features: Floats and drifts, optional tractorbeam, collision or pickup radius.
public class Pickup : MonoBehaviour {
    [Header("Initial Spread")]
    [SerializeField] private float initialSpeed = 3.0f;
    [SerializeField] private float initialDamping = 0.98f;

    [Header("Audio")]
    [SerializeField] private string pickupSound = "Loot_Pickup";

    object lootData;
    int amount;

    ILootMagnetSource target;
    Vector3 velocity;


    public void Initialize(object lootData, int amount, Vector3 initialDirection) {
        this.lootData = lootData;
        this.amount = amount;
        velocity = initialDirection.normalized * initialSpeed;
    }

    //========= Movement ============
    private void Update() {
        if (target != null && target.CanLoot) {
            MagnetMove();
        } else {
            BallisticMove();
        }
    }
    private void BallisticMove() {
        transform.position += velocity * Time.deltaTime;
        velocity *= initialDamping;
    }
    private void MagnetMove() {
        Vector3 toTarget = target.MagnetOrigin.position - transform.position;
        float distance = toTarget.magnitude;
        if (distance <= target.LootDistance) {
            Loot();
            return;
        }
        float speed = target.PullSpeedAtDistance(distance);
        transform.position += toTarget.normalized * speed * Time.deltaTime;
    }


    public void AssignTarget(ILootMagnetSource magnetSource) {
        target = magnetSource;
    }

    private void Loot() {
        CoreRoot.Instance.Audio.Play(pickupSound);
        Destroy(gameObject);
    }
}

