using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f; // degrees per second
    [SerializeField] private bool randomizeOnAwake = true;

    private Vector3 rotationAxis;

    private void Awake() {
        rotationAxis = randomizeOnAwake
            ? Random.onUnitSphere
            : Vector3.up;
    }

    private void Update() {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
