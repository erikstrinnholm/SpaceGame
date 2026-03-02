using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
    Responsibilities
    - Brain gives intent
    - Movement decides how to execute it
*/
public abstract class EnemyMovementBase : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] protected float maxSpeed = 20f;
    [SerializeField] protected float acceleration = 40f;

    [Header("Rotation Settings")]
    [SerializeField] protected float rotationSpeed = 5f;

    protected Vector3 desiredVelocity;
    protected bool hasMoveOrder;

    // Rotation intent
    protected bool hasRotationOrder;
    protected Vector3 desiredFacingDirection;
    protected Rigidbody rb;


    // ================= LIFECYCLE =================
    protected virtual void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    protected virtual void OnEnable() {
        hasMoveOrder = false;
        hasRotationOrder = false;
        desiredVelocity = Vector3.zero;
    }
    protected virtual void OnDisable() {
        Stop();
        ClearRotation();
    }
    protected virtual void FixedUpdate() {
        TickMovement(Time.fixedDeltaTime);
    }


    // ================= MOVEMENT COMMANDS =================
    public virtual void MoveTowards(Vector3 worldPosition) {
        Vector3 dir = worldPosition - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        desiredVelocity = dir.normalized * maxSpeed;
        hasMoveOrder = true;
    }
    public virtual void SetVelocity(Vector3 velocity) {
        velocity.y = 0f;
        desiredVelocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        hasMoveOrder = true;
    }
    public virtual void Stop() {
        desiredVelocity = Vector3.zero;
        hasMoveOrder = false;
    }


    // ================= ROTATION COMMANDS =================
    public virtual void RotateTowards(Vector3 worldPosition) {
        Vector3 dir = worldPosition - transform.position;
        FaceDirection(dir);
    }
    public virtual void FaceDirection(Vector3 direction) {
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f)
            return;

        desiredFacingDirection = direction.normalized;
        hasRotationOrder = true;
    }
    public virtual void ClearRotation() {
        hasRotationOrder = false;
    }


    // ================= ABSTRACT =================
    protected abstract void TickMovement(float deltaTime);
}

