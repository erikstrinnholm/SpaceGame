using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles character movement.
/// - Applies player input for movement and jumping.
/// - Applies gravity and movement via CharacterController.
/// - Updates animation parameters.
/// - Plays movement-related audio (footsteps, jump).
/// 
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CharacterMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterCameraController cameraController;
    [SerializeField] private PlayerAnimatorState animState;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f; 
    [SerializeField] private float acceleration = 12f;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedStickForce = -2f;

    [Header("Footsteps")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.32f;
    [SerializeField] private string surfaceKey = "metal_walk";

    private CharacterController controller;    
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private float currentSpeed;    
    private float verticalVelocity;
    private bool runToggle = true;
    private bool isCrouching = false;
    private float stepTimer = 0f;

    // =========================================================    
    #region UNITY LIFECYCLE
    private void Awake() {
        controller = GetComponent<CharacterController>();
        ValidateDependencies();
    }
    private void Start() {
        animator.SetBool("IsCrouching", isCrouching);
    }
    private void Update() {
        HandleMovement();
        ApplyGravity();
        MoveCharacter();
        UpdateAnimation();
        HandleFootsteps();
    }
    #endregion    
    // =========================================================   


    // =========================================================    
    #region INPUT ACTIONS
    public void OnMove(Vector2 input) {        
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }
    public void OnToggleRun() {
        runToggle = !runToggle;
    }
    public void OnToggleCrouch() {
        if (animState.LowerBodyLock) return;
    
        isCrouching = !isCrouching;
        animator.SetTrigger("CrouchTrigger");
        animator.SetBool("IsCrouching", isCrouching);
    }
    public void OnJump() {
        if (!controller.isGrounded || animState.LowerBodyLock) return;

        animator.SetTrigger("JumpTrigger");
    }
    #endregion    
    // =========================================================


    // =========================================================    
    #region MOVEMENT
    private void HandleMovement() {
        Transform cam = cameraController.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = forward * moveInput.y + right * moveInput.x;
        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        float targetSpeed = CalculateTargetSpeed();
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }
    private void ApplyGravity() {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundedStickForce;

        verticalVelocity += gravity * Time.deltaTime;
    }
    private void MoveCharacter() {
        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
    
    // ========== HELPERS =============
    private float CalculateTargetSpeed() {
        if (moveInput.magnitude < 0.01f)
            return 0f;

        if (isCrouching)
            return walkSpeed;

        if (IsAnalogInput())
            return runSpeed * moveInput.magnitude;

        return runToggle ? runSpeed : walkSpeed;
    }
    private bool IsRunning() {
        if (isCrouching)
            return false;

        if (IsAnalogInput())
            return moveInput.magnitude > 0.5f;
        
        return runToggle;
    }
    private bool IsAnalogInput() {
        return moveInput.magnitude > 0f && moveInput.magnitude < 1f;
    }
    #endregion
    // =========================================================    


    // =========================================================    
    #region ANIMATION
    private void UpdateAnimation() {
        if (!animator) return;

        Vector3 localMove = transform.InverseTransformDirection(moveDirection);

        animator.SetFloat("MoveX", localMove.x, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveY", localMove.z, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }
    #endregion
    // =========================================================


    // =========================================================    
    #region ANIMATION EVENTS
    public void ApplyJumpForce() {
        // Called by JumpUpBehavior (Animator)
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        CoreRoot.Instance.Audio.PlayRandomFootstep("metal_jump_start");
    }
    public void ApplyJumpLand() {
        // Called by JumpLandBehavior (Animator)
        CoreRoot.Instance.Audio.PlayRandomFootstep("metal_jump_land");
        stepTimer = 0f; // prevents immediate step after landing
    }
    #endregion    
    // =========================================================


    // =========================================================    
    #region AUDIO
    private void HandleFootsteps() {
        if (!controller.isGrounded || moveInput.magnitude < 0.1f) {
            stepTimer = 0f;
            return;
        }
        bool isRunning = IsRunning();
        float interval = isRunning ? runStepInterval : walkStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval) {
            stepTimer = 0f;
            string key = isRunning ? surfaceKey.Replace("_walk", "_run") : surfaceKey;
            CoreRoot.Instance.Audio.PlayRandomFootstep(key);
        }
    }
    #endregion
    // =========================================================    
    
    
    private void ValidateDependencies() {
        if (!animator)
            Debug.LogError($"{nameof(CharacterMovementController)}: Missing Animator.");
        if (!cameraController)
            Debug.LogError($"{nameof(CharacterMovementController)}: Missing CameraController.");
    }
}
