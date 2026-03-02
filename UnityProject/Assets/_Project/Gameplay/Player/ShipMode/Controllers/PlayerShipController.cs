using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Receives input from InputManager and forwards it to:
/// - Ship movement
/// - Ship weapons
/// - Ship crosshair
/// - Quick items
/// 
/// Contains no gameplay logic.
/// Acts as a coordination layer between input and ship systems.
/// </summary>
public class PlayerShipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShipWeaponController       weapons;
    [SerializeField] private ShipCrosshairController    crosshair;
    [SerializeField] private ShipMovementController     movement;
    [SerializeField] private QuickItemController        quickItems;


    // =========================================================
    #region UNITY LIFECYCLE
    private void Awake() { ValidateDependencies(); }       
    private void Start() { HookInput(); }
    private void OnDestroy() { UnhookInput(); }
    #endregion
    // =========================================================
        

    // =========================================================    
    #region Input Hooking
    private void HookInput() {
        var input = CoreRoot.Instance.Input;    // InputManager access via CoreRoot singleton
        if (input == null) return;
        
        input.OnAim             += crosshair.OnAim;
        input.OnFire            += crosshair.OnFire;

        input.OnFire            += weapons.OnFire;
        input.OnSwitchWeapon    += weapons.OnSwitchWeapon;
        input.OnReload          += weapons.OnReload;   

        input.OnThrottle        += movement.OnThrottle;         
        input.OnBrake           += movement.OnBrake;            
        input.OnDodgeLeft       += movement.OnDodgeLeft;        
        input.OnDodgeRight      += movement.OnDodgeRight;       
        input.OnBoost           += movement.OnBoost;            

        input.OnQuick1Up        += quickItems.OnUseQuickItem1;  
        input.OnQuick2Right     += quickItems.OnUseQuickItem2;  
        input.OnQuick3Down      += quickItems.OnUseQuickItem3;  
        input.OnQuick4Left      += quickItems.OnUseQuickItem4;
    }

    private void UnhookInput() {
        var input = CoreRoot.Instance.Input;   // InputManager access via CoreRoot singleton
        if (input == null) return;
        
        input.OnAim             -= crosshair.OnAim;     
        input.OnFire            -= crosshair.OnFire;            

        input.OnFire            -= weapons.OnFire;              
        input.OnSwitchWeapon    -= weapons.OnSwitchWeapon;      
        input.OnReload          -= weapons.OnReload;            

        input.OnThrottle        -= movement.OnThrottle;         
        input.OnBrake           -= movement.OnBrake;            
        input.OnDodgeLeft       -= movement.OnDodgeLeft;        
        input.OnDodgeRight      -= movement.OnDodgeRight;       
        input.OnBoost           -= movement.OnBoost;            

        input.OnQuick1Up        -= quickItems.OnUseQuickItem1; 
        input.OnQuick2Right     -= quickItems.OnUseQuickItem2;  
        input.OnQuick3Down      -= quickItems.OnUseQuickItem3;
        input.OnQuick4Left      -= quickItems.OnUseQuickItem4;
    }
    #endregion     
    // =========================================================     


    private void ValidateDependencies() {
        if (!movement)
            Debug.LogError($"{nameof(PlayerShipController)}: Missing ShipMovementController.");
        if (!weapons)
            Debug.LogError($"{nameof(PlayerShipController)}: Missing ShipWeaponController.");
        if (!crosshair)
            Debug.LogError($"{nameof(PlayerShipController)}: Missing ShipCrosshairController.");
        if (!quickItems)
            Debug.LogError($"{nameof(PlayerShipController)}: Missing QuickItemController.");
    }

}
