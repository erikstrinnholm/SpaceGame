using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public enum DroneEvent {
}

[RequireComponent(typeof(Rigidbody))]
public class Drone : DamageDealer, IDamageable {
    public event Action OnDroneDestroyed;

    // -------- Runtime Data --------    
    private DroneData droneData;
    public DroneData GetDroneData() => droneData;

    private float spawnTime;
    private float launchSpeed;

    // -------- Components --------
    private Rigidbody rb;
    private IDamageable lockedTarget;
    private GameObject owner2;

    // ================= UNITY =================
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void Update() {
        if (Time.time - spawnTime >= droneData.lifetime) {
            DestroySelf();
        }
    }


    // ================= INITIALIZATION =================
    public void Initialize(GameObject owner, int layer, DroneData data, IDamageable target) {
        droneData = data;
        lockedTarget = target;
        this.owner2 = owner;
        spawnTime = Time.time;   

        // Initialize base DamageDealer
        base.Initialize(owner, layer, DamageType.Impact, 1);

        // Capture launcher velocity
        launchSpeed = rb.velocity.magnitude;
    }


    private void DestroySelf() {
        OnDroneDestroyed?.Invoke();
        Destroy(gameObject);
    }

    // ================= IDAMAGEABLE =================
    public Transform Transform => transform;
    public void TakeDamage(Damage damage) {
        DestroySelf();
    }
    
}
