using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unarmed : CharacterWeaponBase {
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private LayerMask hitMask;

    [Header("Damage")]
    //[SerializeField] private float punchDamage = 20f;
    [SerializeField] private float hitRadius = 1.0f;

    [Header("Timing")]
    [SerializeField] private float comboResetTime = 0.8f;
    [SerializeField] private float maxAttackDuration = 0.9f; // safety fallback

    [Header("Audio")]
    [SerializeField] private string punchSwingSound = "";
    [SerializeField] private string punchImpactSound = "";

    private int comboIndex = 0;
    private float lastAttackTime;
    private float attackStartTime;
    private bool isAttacking;
    private bool attackQueued;

    public override bool IsBusy => isAttacking;


    // ================= UNITY =================
    private void Update() {
        // Safety fallback if animation event fails
        if (isAttacking && Time.time - attackStartTime > maxAttackDuration) {
            EndAttack();
        }
    }

    // ================= INPUT =================
    public override void OnAttackHeld() {
        // Buffer next punch if already attacking
        if (isAttacking) {
            attackQueued = true;
            return;
        }
        PerformPunch();
    }
    public override void OnAttackReleased() { }
    public override void OnReload() { }


    // ================= ATTACK LOGIC =================
    private void PerformPunch() {
        // Reset combo if too slow
        if (Time.time > lastAttackTime + comboResetTime)
            comboIndex = 0;

        comboIndex++;
        if (comboIndex > 2)
            comboIndex = 1;

        // Trigger correct animation
        animator.SetTrigger("Punch" + comboIndex);

        isAttacking = true;
        attackStartTime = Time.time;
        lastAttackTime = Time.time;
    }

    // ================= ANIMATION EVENTS =================
    public void DealDamage() {
        Collider[] hits = Physics.OverlapSphere(hitPoint.position, hitRadius, hitMask);

        foreach (var hit in hits) {
            if (hit.TryGetComponent<IDamageable>(out var damageable)) {
                //damageable.TakeDamage(punchDamage);
            }
        }
        if (!string.IsNullOrEmpty(punchImpactSound))
            CoreRoot.Instance.Audio.Play(punchImpactSound);
    }
    public void PlaySwingSound() {
        if (!string.IsNullOrEmpty(punchSwingSound))
            CoreRoot.Instance.Audio.Play(punchSwingSound);
    }
    public void EndAttack() {
        isAttacking = false;
        if (attackQueued) {
            attackQueued = false;
            PerformPunch();
        }
    }
}
