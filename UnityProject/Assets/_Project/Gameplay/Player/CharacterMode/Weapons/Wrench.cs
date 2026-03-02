using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
    Wrench melee system:
    - Light combo: light → light → heavy
    - Hold to charge → heavy attack
    - Combo resets after timer
    - Light attack buffering
    - Animation event driven damage & swing sound
    - Forward motion handled in code
 */


public class Wrench : CharacterWeaponBase {
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private LayerMask hitMask;

    [Header("Damage")]
    [SerializeField] private float lightDamage = 25f;
    [SerializeField] private float heavyDamage = 60f;
    [SerializeField] private float hitRadius = 1.2f;

    [Header("Timing")]
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float chargeTime = 0.45f;
    [SerializeField] private float maxAttackDuration = 1.0f; // safety fallback

    [Header("Audio")]
    [SerializeField] private string lightSwingSound = "";     
    [SerializeField] private string heavySwingSound = "";     
    [SerializeField] private string wrenchImpactSound    = "";     // placeholder

    private int comboIndex = 0;
    private float lastAttackTime;
    private float attackPressedTime;
    private float attackStartTime;
    private bool isCharging;
    private bool isAttacking;
    private bool lightQueued;
    public override bool IsBusy => isAttacking || isCharging;


    // ================= UNITY =================
    private void Update() {
        // auto trigger heavy attack if held long enough
        if (isCharging && !isAttacking && Time.time - attackPressedTime >= chargeTime) {
            isCharging = false;
            PerformHeavyAttack();
        }
        // safety fallback in case animation event missed
        if (isAttacking && Time.time - attackStartTime > maxAttackDuration) {
            EndAttack();
        }        
    }
    void LateUpdate() {
        if (isAttacking) {
            var p = transform.localPosition;
            p.x = 0;
            p.z = 0;
            transform.localPosition = p;
        }
    }

    // ================= INPUT =================
    public override void OnReload() { } // Melee has no reload
    public override void OnAttackHeld() {
        if (isCharging) return;

        // Allow combo buffering during attack
        if (isAttacking) {
            lightQueued = true;
            return;
        }
        attackPressedTime = Time.time;
        isCharging = true;
    }
    public override void OnAttackReleased() {
        if (!isCharging) return;
        isCharging = false;
        PerformLightAttack();
    }


    // ============== LIGHT ATTACK ================
    private void PerformLightAttack() {
        if (isAttacking) return;

        // combo reset
        if (Time.time > lastAttackTime + comboResetTime)
            comboIndex = 0;

        comboIndex++;
        if (comboIndex > 3) comboIndex = 1;

        animator.SetTrigger("LightAttack" + comboIndex);

        isAttacking = true;
        attackStartTime = Time.time;
        lastAttackTime = Time.time;
    }

    // ============== HEAVY ATTACK ================
    private void PerformHeavyAttack() {
        animator.SetTrigger("HeavyAttack");
        comboIndex = 0;
        isAttacking = true;
        attackStartTime = Time.time;
        lastAttackTime = Time.time;
    }



    // ================= ANIMATION EVENTS (AnimationEventRelay) =================
    public void DealLightDamage() {
        DealDamage(lightDamage);
        if (!string.IsNullOrEmpty(wrenchImpactSound))
            CoreRoot.Instance.Audio.Play(wrenchImpactSound);        
    }
    public void DealHeavyDamage() {
        DealDamage(heavyDamage);
        if (!string.IsNullOrEmpty(wrenchImpactSound))
            CoreRoot.Instance.Audio.Play(wrenchImpactSound);           
    }
    public void PlayLightSwingSound() {
        if (!string.IsNullOrEmpty(lightSwingSound))
            CoreRoot.Instance.Audio.Play(lightSwingSound);        

    }
    public void PlayHeavySwingSound() {
        if (!string.IsNullOrEmpty(heavySwingSound))
            CoreRoot.Instance.Audio.Play(heavySwingSound);           
    }



    private void DealDamage(float damageAmount) {
        Collider[] hits = Physics.OverlapSphere(hitPoint.position, hitRadius, hitMask);

        foreach (var hit in hits) {
            if (hit.TryGetComponent<IDamageable>(out var damageable)) {
                //damageable.TakeDamage(damageAmount);
            }
        }
    }
    public void EndAttack() {
        isAttacking = false;
        if (lightQueued) {
            lightQueued = false;
            PerformLightAttack();
        }
    }
}
