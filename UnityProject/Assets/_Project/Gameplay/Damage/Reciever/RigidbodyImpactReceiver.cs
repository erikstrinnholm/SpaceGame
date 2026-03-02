using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyImpactReceiver : MonoBehaviour, IImpactReceiver {
    [SerializeField] private float multiplier = 1f;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyImpact(Vector3 force, Vector3 point) {
        if (rb == null) return;
        rb.AddForceAtPosition(force * multiplier, point, ForceMode.Impulse);
    }
}

