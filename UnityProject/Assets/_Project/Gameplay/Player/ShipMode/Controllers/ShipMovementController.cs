using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Interprets aim intent → moves the ship
public class ShipMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform visualModel;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private PlayerShipCamera followCamera;

    [Header("Speed Settings")]
    [SerializeField] private AnimationCurve thrustCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float minSpeed         = 0f;
    [SerializeField] private float softMaxSpeed     = 100f;     // Throttle Limit
    [SerializeField] private float hardMaxSpeed     = 150f;     // Boosted Limit
    [SerializeField] private float brakePower       = 60f;
    [SerializeField] private float accelerationTime = 3f;       // Time to reach maxSpeed holding throttle

    [Header("State")]
    [SerializeField] private float currentSpeed     = 0f;
    private float throttleProgress = 0f;
    private bool throttleHeld;
    private bool brakeHeld;

    [Header("Boost Settings")]
    [SerializeField] private float boostDuration    = 1.5f;
    [SerializeField] private float boostCooldown    = 10f;
    [SerializeField] private float boostPower       = 100;
    private bool boostActive = false;
    private float boostTimer = 0f;
    private float boostCooldownTimer = 0f;

    [Header("Barrel Roll")]
    [SerializeField] private float barrelRollDuration = 0.5f;
    //[SerializeField] private float barrelRollDistance = 10f;
    private bool isRolling = false;
    private int rollDirection = 0; // -1 = left, 1 = right
    private float rollTimer = 0f;

    [Header("Rotation")]
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float turnSpeed        = 120f;     
    [SerializeField] private float aimStrength      = 1f;
    [SerializeField] private float aimDeadzone      = 0.05f;    


    // =========================================================    
    #region UNITY LIFECYCLE 
    private void Awake() {
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        currentSpeed = minSpeed;
    }
    private void FixedUpdate() {
        UpdateBoostTimers();
        UpdateSpeed();
        UpdateRoll();
        ApplyRotation();
        ApplyMovement();
    }
    #endregion
    // =========================================================



    // ========== BOOST =======================
    public void OnBoost() {
        if (!boostActive && boostCooldownTimer <= 0f) {
            boostActive = true;
            boostTimer = boostDuration;
            boostCooldownTimer = boostCooldown; // start cooldown
        }
    }    
    private void UpdateBoostTimers() {
        if (boostActive) {
            boostTimer -= Time.fixedDeltaTime;
            if (boostTimer <= 0f) {
                boostActive = false;
            }
        }
        if (boostCooldownTimer > 0f) {
            boostCooldownTimer -= Time.fixedDeltaTime;
        }
    }    
    

    // ========== MOVEMENT =======================
    public void OnThrottle(bool active) {
        throttleHeld = active;
    }
    public void OnBrake(bool active) {
        brakeHeld = active;    
    }
    private void UpdateSpeed() {
        float targetSpeed = currentSpeed;

        // BRAKE
        if (brakeHeld) {
            currentSpeed = Mathf.MoveTowards(currentSpeed, minSpeed, brakePower * Time.fixedDeltaTime);
            throttleProgress = currentSpeed / softMaxSpeed;
            return;
        }

        // BOOST
        if (boostActive) {
            currentSpeed = Mathf.MoveTowards(currentSpeed, hardMaxSpeed, boostPower * Time.fixedDeltaTime);
            return;
        }

        // THROTTLE
        if (throttleHeld)
            throttleProgress += Time.fixedDeltaTime / accelerationTime; // Increase throttle
        else
            throttleProgress -= Time.fixedDeltaTime / accelerationTime; // Decay naturally
        
        // Apply thrust curve
        throttleProgress = Mathf.Clamp01(throttleProgress);
        float thrustValue = thrustCurve.Evaluate(throttleProgress);
        targetSpeed = Mathf.Lerp(minSpeed, softMaxSpeed, thrustValue);

        // APPLY SPEED
        float maxDelta = (softMaxSpeed - minSpeed) / accelerationTime * Time.fixedDeltaTime;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta);        
    }
    private void ApplyMovement() {
        if (rb != null)
            rb.velocity = transform.forward * currentSpeed;
    }


    // ========== ROTATION =======================
    private void ApplyRotation() {   
        if (crosshair == null || rb == null || isRolling) return; // freeze rotation while rolling
        
        // Crosshair -> Screen Center
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 delta = (Vector2)crosshair.position - screenCenter;
        delta.x /= screenCenter.x;
        delta.y /= screenCenter.y;

        // Deadzone per axis
        if (Mathf.Abs(delta.x) < aimDeadzone) delta.x = 0f;
        if (Mathf.Abs(delta.y) < aimDeadzone) delta.y = 0f;
        
        // Clamp magnitude to 1
        delta = Vector2.ClampMagnitude(delta, 1f);

        // Apply non-linear rotation curve
        float curvedX = Mathf.Sign(delta.x) * rotationCurve.Evaluate(Mathf.Abs(delta.x));
        float curvedY = Mathf.Sign(delta.y) * rotationCurve.Evaluate(Mathf.Abs(delta.y));
        float pitch = -curvedY * turnSpeed * aimStrength;
        float yaw   =  curvedX * turnSpeed * aimStrength;


        Vector3 angularVelocity = new Vector3(pitch, yaw, 0f) * Mathf.Deg2Rad;
        rb.angularVelocity = Vector3.Lerp(
            rb.angularVelocity,
            transform.TransformDirection(angularVelocity),
            Time.fixedDeltaTime * 6f
        );
    } 


    // ========== ROLLING =======================
    public void OnDodgeLeft() {
        if (!isRolling) StartBarrelRoll(-1);
    }
    public void OnDodgeRight() {
        if (!isRolling) StartBarrelRoll(1);
    }
    private void StartBarrelRoll(int direction) {
        isRolling = true;
        rollDirection = direction;
        rollTimer = 0f;

        if (followCamera != null)
            followCamera.IgnoreRoll(barrelRollDuration);
    }
    private void UpdateRoll() {
        if (!isRolling) return;

        rollTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(rollTimer / barrelRollDuration);

        // Apply only Z-roll to Rigidbody rotation
        Vector3 euler = rb.rotation.eulerAngles;
        Quaternion rollRot = Quaternion.Euler(euler.x, euler.y, -360f * rollDirection * t);
        rb.MoveRotation(rollRot);

        if (rollTimer >= barrelRollDuration) {
            isRolling = false;
            rb.MoveRotation(Quaternion.Euler(euler.x, euler.y, 0f)); // reset roll
        }
    }
}
