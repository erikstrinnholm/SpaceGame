using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class SpawnSpreadUtility
{

    // Generates evenly-spaced directions around an axis.
    public static Vector3[] GenerateRadialDirections(int count, Vector3 impactDirection) {
        Vector3[] dirs = new Vector3[count];

        // Fallback if impact direction is invalid
        Vector3 forward = impactDirection.sqrMagnitude > 0.001f
            ? impactDirection.normalized
            : Random.onUnitSphere;

        // Build an orthonormal basis
        Vector3 right = Vector3.Cross(forward, Vector3.up);
        if (right.sqrMagnitude < 0.01f)
            right = Vector3.Cross(forward, Vector3.right);

        right.Normalize();
        Vector3 up = Vector3.Cross(right, forward).normalized;

        // Even angular distribution
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++) {
            float angle = angleStep * i * Mathf.Deg2Rad;
            dirs[i] = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)).normalized;
        }
        return dirs;
    }    

}

