using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class PlayerShipCamera : MonoBehaviour {
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Positioning")]
    [SerializeField] private float followDistance = 6f;
    [SerializeField] private float cameraElevation = 2.5f;
    [SerializeField] private float followTightness = 6f;

    [Header("Rotation")]
    [SerializeField] private float rotationTightness = 8f;

    [Header("Yaw Offset (Arcade Feel)")]
    [SerializeField] private float yawOffsetStrength = 1.5f;


    [Header("Barrel Roll Effect")]
    private bool ignoreRollRotation = false;
    private float ignoreRollEndTime = 0f; // absolute time when roll ignore ends


    // ================= UNITY =================
    private void LateUpdate() {
        if (target == null) return;

        if (ignoreRollRotation && Time.time >= ignoreRollEndTime) {
            ignoreRollRotation = false;
        }

        UpdatePosition();
        UpdateRotation();
    }


    private void UpdatePosition() {
        Vector3 desiredPos =
            target.position
            - target.forward * followDistance
            + target.up * cameraElevation;

        // Arcade-style lateral offset
        float yaw = Vector3.Dot(target.right, target.forward);
        desiredPos += target.right * yaw * yawOffsetStrength;

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followTightness * Time.deltaTime
        );
    }
    private void UpdateRotation() {
        Vector3 lookDir = target.position - transform.position;

        // Target rotation ignoring roll if requested
        Quaternion targetRot = Quaternion.LookRotation(
            lookDir,
            ignoreRollRotation ? Vector3.up : target.up // keep upright if ignoring roll
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationTightness * Time.deltaTime
        );
    }

    // Freeze roll rotation temporarily while still allowing pitch/yaw updates
    public void IgnoreRoll(float duration) {
        ignoreRollRotation = true;
        ignoreRollEndTime = Time.time + duration; // absolute end time
    }







}
