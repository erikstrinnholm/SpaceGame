using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Handles character combat and weapon management.
/// - Processes attack and reload input.
/// - Manages weapon switching and animation sync.
/// - Delegates attack behavior to the active weapon.
/// </summary>
public class CharacterCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAnimatorState animState;     //syncs InputActions with Animator
    [SerializeField] private Animator animator;

    [Header("Weapons")]
    [SerializeField] private List<CharacterWeaponBase> weapons = new();

    [Header("Audio")]
    [SerializeField] private string equipWeaponSound;
    [SerializeField] private string unequipWeaponSound;

    //Internal
    private int currentWeaponIndex;
    private int pendingWeaponIndex = -1;    
    private bool attackHeld;
    private CharacterWeaponBase CurrentWeapon =>
        weapons != null && weapons.Count > 0
            ? weapons[currentWeaponIndex]
            : null;


    // =========================================================    
    #region UNITY LIFECYCLE
    private void Awake() {
        ValidateDependencies();
        InitializeWeapons();
    }
    private void Update() {
        HandleHeldAttack();
    }
    #endregion    
    // =========================================================    

    // =========================================================    
    #region INPUT ACTIONS
    public void OnAttack(bool pressed) {
        attackHeld = pressed;

        if (!pressed)
            CurrentWeapon?.OnAttackReleased();
    }
    public void OnReload() {
        if (!CanAct()) return;
        CurrentWeapon?.OnReload();
    }
    public void OnSwitchWeapon() {
        if (!CanAct()) return;
        if (pendingWeaponIndex != -1) return;

        pendingWeaponIndex = GetNextWeaponIndex();
        animState.SetUpperBodyLock(true);

        var nextWeapon = weapons[pendingWeaponIndex];
        animator.SetInteger("WeaponType", (int)nextWeapon.Type);
        animator.SetTrigger("SwitchWeapon");
    }
    #endregion    
    // =========================================================    

    // =========================================================    
    #region Animation Events
    public void OnUnequipAnimationFinished() {
        CurrentWeapon?.AttachToBackSocket();

        currentWeaponIndex = pendingWeaponIndex;
        pendingWeaponIndex = -1;
        
        PlaySound(unequipWeaponSound);       
    }
    public void OnEquipAnimationMiddle() {
        CurrentWeapon?.AttachToHandSocket();            
    }
    public void OnEquipAnimationFinished() {
        animState.SetUpperBodyLock(false);
        PlaySound(equipWeaponSound);         
    }
    public void OnReloadAudio() {
        if (CurrentWeapon is Rifle rifle)
            PlaySound(rifle.reloadSound);
    }
    #endregion
    // =========================================================    


    private void HandleHeldAttack() {
        if (!attackHeld) return;
        if (!CanAct()) return;
        CurrentWeapon?.OnAttackHeld();
    }
    private bool CanAct() {
        return !animState.UpperBodyLock &&
               CurrentWeapon != null &&
               !CurrentWeapon.IsBusy;
    }    
    private void InitializeWeapons() {
        if (weapons == null || weapons.Count == 0) {
            Debug.LogError($"{nameof(CharacterCombatController)}: No weapons assigned.");
            enabled = false;
            return;
        }
        foreach (var weapon in weapons)
            weapon.AttachToBackSocket();

        SetAnimatorWeapon(CurrentWeapon.Type);
    }
    private int GetNextWeaponIndex() {
        int next = currentWeaponIndex + 1;
        if (next >= weapons.Count)
            next = 0;
        return next;
    }
    private void SetAnimatorWeapon(WeaponType type) {
        animator.SetInteger("WeaponType", (int)type);
    }   
    private void PlaySound(string soundstring){
        if (!string.IsNullOrEmpty(soundstring))
            CoreRoot.Instance.Audio.Play(soundstring);
    }
    private void ValidateDependencies() {
        if (!animState)
            Debug.LogError($"{nameof(CharacterCombatController)}: Missing AnimatorState reference.");
        if (!animator)
            Debug.LogError($"{nameof(CharacterCombatController)}: Missing Animator reference.");
    }
}
