using System.Collections;
using UnityEngine;

public class SatelliteRotation : MonoBehaviour {
    [SerializeField] public float rotationSpeed = 3.0f;

	
	// Update is called once per frame
	private void Update () {
		transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
	}
}
