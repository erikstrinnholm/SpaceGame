using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpinner : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 180f; // degrees per second

    private void Update() {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.unscaledDeltaTime);
    }
}
