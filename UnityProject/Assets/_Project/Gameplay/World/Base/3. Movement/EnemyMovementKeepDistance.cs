using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Maintain distance while facing the target
public class EnemyMovementKeepDistance : EnemyMovementBase {
    [SerializeField] private float desiredDistance = 20f;
    private Transform target;

    public void SetTarget(Transform t) {
        target = t;
    }

    protected override void TickMovement(float deltaTime) {
        if (target == null)
            return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        Vector3 dir = toTarget.normalized;

        if (distance < desiredDistance)
            SetVelocity(-dir * maxSpeed);
        else
            Stop();

        FaceDirection(dir);
        ApplyMovement(deltaTime);
    }

    protected void ApplyMovement(float deltaTime) {
        rb.velocity = Vector3.MoveTowards(
            rb.velocity,
            hasMoveOrder ? desiredVelocity : Vector3.zero,
            acceleration * deltaTime
        );
        Quaternion rot = Quaternion.LookRotation(desiredFacingDirection, Vector3.up);
        rb.rotation = Quaternion.Slerp(rb.rotation, rot, rotationSpeed * deltaTime);
    }
}

