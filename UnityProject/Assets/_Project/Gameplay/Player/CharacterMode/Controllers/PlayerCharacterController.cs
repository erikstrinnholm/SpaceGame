using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Responsibilities:
/// - Receives input from InputManager and forwards it to:
/// -   Movement
/// -   Camera
/// -   Combat
/// 
/// Contains no gameplay logic.
/// Acts as a coordination layer between input and character systems.
/// </summary>
public class PlayerCharacterController : MonoBehaviour
{    
    [Header("References")]
    [SerializeField] private CharacterMovementController movement;
    [SerializeField] private CharacterCameraController cameraController;
    [SerializeField] private CharacterCombatController combat;

    // =========================================================    
    #region UNITY LIFECYCLE
    private void Awake() {
        ValidateDependencies();
    }   
    private void Start() {
        HookInput();
        //SetCursorState(true);
    }
    private void OnDestroy() {
        UnhookInput();
        //SetCursorState(false);
    }
    #endregion    
    // =========================================================    


    // =========================================================    
    #region Input Hooking
    private void HookInput() {
        var input = CoreRoot.Instance.Input; // InputManager access via CoreRoot singleton
        if (input == null) return;
        
        input.OnMove            += movement.OnMove;
        input.OnToggleCrouch    += movement.OnToggleCrouch;
        input.OnToggleRun       += movement.OnToggleRun;
        input.OnJump            += movement.OnJump;

        input.OnFire            += combat.OnAttack;
        input.OnReload          += combat.OnReload;
        input.OnSwitchWeapon    += combat.OnSwitchWeapon;

        input.OnLook            += cameraController.OnLook;
    }
    private void UnhookInput() {
        var input = CoreRoot.Instance.Input; // InputManager access via CoreRoot singleton
        if (input == null) return;
         
        input.OnMove            -= movement.OnMove;
        input.OnToggleCrouch    -= movement.OnToggleCrouch;
        input.OnToggleRun       -= movement.OnToggleRun;
        input.OnJump            -= movement.OnJump;
        
        input.OnFire            -= combat.OnAttack;
        input.OnReload          -= combat.OnReload;  
        input.OnSwitchWeapon    -= combat.OnSwitchWeapon;

        input.OnLook            -= cameraController.OnLook;
    }
    #endregion     
    // =========================================================    

    private void SetCursorState(bool gameplayActive) {
        Cursor.lockState = gameplayActive
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !gameplayActive;
    }

    private void ValidateDependencies() {
        if (!movement)
            Debug.LogError($"{nameof(PlayerCharacterController)}: MovementController reference missing.");
        if (!cameraController)
            Debug.LogError($"{nameof(PlayerCharacterController)}: CameraController reference missing.");
        if (!combat)
            Debug.LogError($"{nameof(PlayerCharacterController)}: CombatController reference missing.");
    }
}
