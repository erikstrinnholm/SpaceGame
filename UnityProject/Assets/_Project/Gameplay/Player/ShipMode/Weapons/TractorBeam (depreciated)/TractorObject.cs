using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TractorObject : MonoBehaviour
{
    [Header("Tractor Object Settings")]
    [SerializeField] private float maxPullSpeed = 100f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogError("TractorObject requires a Rigidbody component.");
    }


    public void ApplyTractorForce(Vector3 directionToTarget, float distance, float force, float deltaTime) {
        if (rb == null || distance < 0.01f) return;
        //Debug.Log("Applying tractor force to " + gameObject.name);
        Vector3 velocityChange = directionToTarget * force * deltaTime;

        // Limit velocity to maxPullSpeed
        Vector3 newVelocity = rb.velocity + velocityChange;
        if (newVelocity.magnitude > maxPullSpeed) {
            newVelocity = newVelocity.normalized * maxPullSpeed;
        }

        rb.velocity = newVelocity;
    }
}
