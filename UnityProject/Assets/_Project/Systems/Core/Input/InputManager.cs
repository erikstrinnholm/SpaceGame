using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;


/// <summary>
/// Responsibilities:
/// - Manages and switches between action maps (Ship, Character, Menu, Inventory).
/// - Converts Input Actions into high-level gameplay events.
/// - Decouples input handling from gameplay systems via C# events.
/// 
/// Acts as an abstraction layer between Unity input and game logic.
/// </summary>
public class InputManager : MonoBehaviour
{
    public ActionMapType CurrentMap { get; private set; }
    private PlayerControls controls;

    // ---------- UNIVERSAL --------
    public event Action OnInventoryToggle;                  
    public event Action OnMenuToggle;

    // ---------- MENU ----------
    public event Action<Vector2>    OnMenuNavigate;         // Navigation (keyboard/controller)
    public event Action<Vector2>    OnMenuMouseMove;        // Mouse Move
    public event Action OnMenuBack;                         //

    // ---------- INVENTORY ----------
    public event Action<Vector2>    OnInventoryNavigate;    // Navigation (keyboard/controller)
    public event Action<Vector2>    OnInventoryMouseMove;   // Mouse Move
    public event Action             OnInventoryConfirm;     //
    public event Action             OnInventoryCancel;      //          
    public event Action             OnInventoryLeftTab;     //
    public event Action             OnInventoryRightTab;    //

    // ---------- SPACE GAMEPLAY ----------
    public event Action<InputAction.CallbackContext> OnAim; //
    public event Action<bool>       OnFire;                 // hold to fire, release to stop firing
    public event Action<bool>       OnThrottle;             // hold to accelerate, release to stop accelerating
    public event Action<bool>       OnBrake;                // hold to brake, release to stop braking
    public event Action             OnDodgeRight;           // Evasive Roll Dodge
    public event Action             OnDodgeLeft;            // Evasive Roll Dodge
    public event Action             OnBoost;                //
    public event Action             OnSwitchWeapon;         //
    public event Action             OnReload;               //
    public event Action             OnQuick1Up;             //
    public event Action             OnQuick2Right;          //
    public event Action             OnQuick3Down;           //
    public event Action             OnQuick4Left;           //

    // ---------- CHARACTER GAMEPLAY ----------------
    public event Action<Vector2>    OnMove;                 // move character (WASD or STICK)
    public event Action             OnToggleCrouch;
    public event Action             OnToggleRun;
    public event Action             OnJump;
    public event Action<Vector2>    OnLook;
    public event Action             OnSwitchView;
    public event Action             OnInteract;
    //public event Action           OnReload (reuse)
    //public event Action           OnSwitchWeapon (reuse)
    //public event Action.          OnFire (reuse)


    #region UNITY LIFECYCLE
    // =========================================================
    private void Awake() {
        controls = new PlayerControls();
        BindInputs();
        SwitchActionMap(ActionMapType.Ship);
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    private void OnDestroy() => controls.Dispose();
    // =========================================================    
    #endregion


    #region Binding
    // =========================================================
    private void BindInputs() {
        BindUniversal();
        BindMenu();
        BindInventory();
        BindShip();
        BindCharacter();
    }
    private void BindUniversal() {
        controls.Universal.InventoryToggle.performed    += _ => OnInventoryToggle?.Invoke();
        controls.Universal.MenuToggle.performed         += _ => OnMenuToggle?.Invoke();
    }
    private void BindMenu() {
        controls.Menu.Navigate.performed                += ctx => OnMenuNavigate?.Invoke(ctx.ReadValue<Vector2>());
        controls.Menu.MouseMove.performed               += _   => OnMenuMouseMove?.Invoke(Mouse.current.position.ReadValue());
        controls.Menu.Back.performed                    += _   => OnMenuBack?.Invoke();
    }
    private void BindInventory() {
        controls.Inventory.Navigate.performed           += ctx => OnInventoryNavigate?.Invoke(ctx.ReadValue<Vector2>());
        controls.Inventory.MouseMove.performed          += _   => OnInventoryMouseMove?.Invoke(Mouse.current.position.ReadValue());
        controls.Inventory.Confirm.performed            += _   => OnInventoryConfirm?.Invoke();
        controls.Inventory.Cancel.performed             += _   => OnInventoryCancel?.Invoke();
        controls.Inventory.LeftTab.performed            += _   => OnInventoryLeftTab?.Invoke();
        controls.Inventory.RightTab.performed           += _   => OnInventoryRightTab?.Invoke();
    }
    private void BindShip () {
        controls.Ship.Aim.performed                     += ctx => OnAim?.Invoke(ctx);   //
        controls.Ship.Aim.canceled                      += ctx => OnAim?.Invoke(ctx);   //

        controls.Ship.Fire.performed                    += _   => OnFire?.Invoke(true);
        controls.Ship.Fire.canceled                     += _   => OnFire?.Invoke(false);
        controls.Ship.Throttle.performed                += _   => OnThrottle?.Invoke(true);
        controls.Ship.Throttle.canceled                 += _   => OnThrottle?.Invoke(false);
        controls.Ship.Brake.performed                   += _   => OnBrake?.Invoke(true);
        controls.Ship.Brake.canceled                    += _   => OnBrake?.Invoke(false);
        controls.Ship.DodgeRight.performed              += _   => OnDodgeRight?.Invoke();
        controls.Ship.DodgeLeft.performed               += _   => OnDodgeLeft?.Invoke();
        controls.Ship.Boost.performed                   += _   => OnBoost?.Invoke();
        controls.Ship.SwitchWeapon.performed            += _   => OnSwitchWeapon?.Invoke();
        controls.Ship.Reload.performed                  += _   => OnReload?.Invoke();        
        controls.Ship.Quick1Up.performed                += _   => OnQuick1Up?.Invoke();
        controls.Ship.Quick2Right.performed             += _   => OnQuick2Right?.Invoke();
        controls.Ship.Quick3Down.performed              += _   => OnQuick3Down?.Invoke();
        controls.Ship.Quick4Left.performed              += _   => OnQuick4Left?.Invoke();
    }
    private void BindCharacter() {
        controls.Character.Move.performed               += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Character.Move.canceled                += _   => OnMove?.Invoke(Vector2.zero);
        controls.Character.ToggleCrouch.performed       += _   => OnToggleCrouch?.Invoke();
        controls.Character.ToggleRun.performed          += _   => OnToggleRun?.Invoke();
        controls.Character.Jump.performed               += _   => OnJump?.Invoke();
        controls.Character.Reload.performed             += _   => OnReload?.Invoke();
        controls.Character.SwitchWeapon.performed       += _   => OnSwitchWeapon?.Invoke();
        controls.Character.Fire.performed               += _   => OnFire?.Invoke(true);
        controls.Character.Fire.canceled                += _   => OnFire?.Invoke(false);
        //
        controls.Character.Look.performed               += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
        controls.Character.Look.canceled                += _   => OnLook?.Invoke(Vector2.zero);
        controls.Character.SwitchView.performed         += _   => OnSwitchView?.Invoke();
        controls.Character.Interact.performed           += _   => OnInteract?.Invoke();
    }
    // =========================================================    
    #endregion


    #region ActionMap Switching
    // =========================================================    
    public void SwitchActionMap(ActionMapType type) {
        CurrentMap = type;

        controls.Universal.Enable();
        controls.Menu.Disable();
        controls.Inventory.Disable();
        controls.Ship.Disable();
        controls.Character.Disable();

        switch (type) {
            case ActionMapType.Menu:
                controls.Menu.Enable();
                break;
            case ActionMapType.Inventory:
                controls.Inventory.Enable();
                break;
            case ActionMapType.Ship:
                controls.Ship.Enable();
                break;
            case ActionMapType.Character:
                controls.Character.Enable();
                break;
        }
    }
    // =========================================================    
    #endregion
}