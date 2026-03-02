using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovementSimple : EnemyMovementBase {

    // ================= UNITY =================
    protected override void Awake() {
        base.Awake();
        // Space-style movement assumptions
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    // ================= BASE CLASS =================
    protected override void TickMovement(float deltaTime) {
        HandleMovement(deltaTime);
        HandleRotation(deltaTime);
    }


    // ================= MOVEMENT =================
    private void HandleMovement(float deltaTime) {
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = hasMoveOrder ? desiredVelocity : Vector3.zero;
        // Smooth acceleration / deceleration
        Vector3 newVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * deltaTime
        );
        rb.velocity = newVelocity;
    }


    // ================= ROTATION =================
    private void HandleRotation(float deltaTime) {
        Vector3 facingDirection = Vector3.zero;

        if (hasRotationOrder) {
            // Explicit rotation command from Brain
            facingDirection = desiredFacingDirection;
        }
        else if (rb.velocity.sqrMagnitude > 0.01f) {
            // Default behavior: face movement direction
            facingDirection = rb.velocity.normalized;
        }

        if (facingDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(facingDirection, Vector3.up);


        rb.rotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * deltaTime
        );
    }

}
