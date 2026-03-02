using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Responsibilities
Enemy circles a target at a preferred distance.

Behavior rules:
- Maintain orbit radius
- Move tangentially
- Face target
*/

public class EnemyMovementOrbit : EnemyMovementBase {
    [Header("Orbit Settings")]
    [SerializeField] private float orbitDistance = 15f;
    [SerializeField] private float orbitStrength = 1f;
    private Transform orbitTarget;


    // ================= COMMAND =================
    public void SetOrbitTarget(Transform target) {
        orbitTarget = target;
    }

    // ================= CORE =================
    protected override void TickMovement(float deltaTime) {
        if (orbitTarget == null)
            return;

        Vector3 toTarget = orbitTarget.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        Vector3 radialDir = toTarget.normalized;

        // Tangential direction (circle)
        Vector3 tangent = Vector3.Cross(Vector3.up, radialDir).normalized;

        // Pull in / push out to maintain orbit distance
        float distanceError = distance - orbitDistance;
        Vector3 radialCorrection = radialDir * Mathf.Clamp(distanceError, -1f, 1f);

        Vector3 desiredMove =
            tangent * orbitStrength +
            radialCorrection;

        SetVelocity(desiredMove * maxSpeed);
        FaceDirection(toTarget);

        ApplyMovement(deltaTime);
    }

    // ================= SHARED PHYSICS =================
    protected void ApplyMovement(float deltaTime) {
        Vector3 newVelocity = Vector3.MoveTowards(
            rb.velocity,
            hasMoveOrder ? desiredVelocity : Vector3.zero,
            acceleration * deltaTime
        );

        rb.velocity = newVelocity;
        HandleRotation(deltaTime);
    }
    protected void HandleRotation(float deltaTime) {
        if (!hasRotationOrder)
            return;
        Quaternion targetRot = Quaternion.LookRotation(desiredFacingDirection, Vector3.up);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * deltaTime);
    }
}

