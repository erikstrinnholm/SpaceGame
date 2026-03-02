using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/* Responsibilities
- Given an AimSolution, decide how imperfectly to execute it.
*/
public abstract class EnemyAttackBase : MonoBehaviour {
    [Header("Weapon Settings")]
    [SerializeField] protected float fireCooldown = 1f;
    [SerializeField] protected float range = 500f;
    [SerializeField] protected Transform firePoint;

    [Header("Accuracy & Prediction")]
    [Tooltip("Max angular error in degrees")]
    [SerializeField] protected float aimErrorDegrees = 0f;
    [Tooltip("Projectile speed (0 = no prediction)")]
    [SerializeField] protected float speedPrediction = 0f;
    [SerializeField] protected float maxPredictionTime = 3f;


    protected float nextFireTime;
    [Header("References")]
    [SerializeField] protected Transform projectileSpawnParent;


    // ================= UNITY =================

    protected virtual void Awake() {
        if (firePoint == null)
            firePoint = transform;
    }

    // ================= PUBLIC API =================
    public bool CanFire() {
        return Time.time >= nextFireTime;
    }
    public bool IsInRange(Vector3 aimPoint) {
        return Vector3.Distance(firePoint.position, aimPoint) <= range;
    }
    public void TryFire(AimSolution solution) {
        if (!CanFire()) return;
        if (solution.aimPoint == Vector3.zero) return;
        if (!IsInRange(solution.aimPoint)) return;

        Vector3 finalAimPoint = GetFinalAimPoint(solution);
        FireInternal(solution, finalAimPoint);

        nextFireTime = Time.time + fireCooldown;
    }

    // ================= FIRING =================
    protected abstract void FireInternal(AimSolution solution, Vector3 finalAimPoint);

    // ================= AIMING =================
    protected Vector3 GetFinalAimPoint(AimSolution solution) {
        Vector3 aimPoint = solution.aimPoint;

        if (solution.target != null && speedPrediction > 0f)
            aimPoint = PredictAimPoint(solution.target);

        aimPoint = ApplyAimError(aimPoint);
        return aimPoint;
    }

    protected Vector3 PredictAimPoint(IDamageable target) {
        Transform t = target.Transform;
        Rigidbody rb = t.GetComponent<Rigidbody>();
        if (rb == null)
            return t.position;

        float distance = Vector3.Distance(firePoint.position, t.position);
        float time = Mathf.Min(distance / speedPrediction, maxPredictionTime);

        return t.position + rb.velocity * time;
    }

    protected Vector3 ApplyAimError(Vector3 aimPoint) {
        if (aimErrorDegrees <= 0f)
            return aimPoint;

        Vector3 dir = (aimPoint - firePoint.position).normalized;

        Quaternion errorRot = Quaternion.Euler(
            Random.Range(-aimErrorDegrees, aimErrorDegrees),
            Random.Range(-aimErrorDegrees, aimErrorDegrees),
            Random.Range(-aimErrorDegrees, aimErrorDegrees)
        );

        dir = errorRot * dir;
        return firePoint.position + dir * 1000f;
    }
}
