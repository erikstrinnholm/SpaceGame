using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerDamageReceiver : MonoBehaviour, IDamageable
{
    // ===================== STATS =====================
    [Header("Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float maxShield = 200f;
    private float currentHP;
    private float currentShield;

    public float MaxHP => maxHP;
    public float MaxShield => maxShield;
    public float CurrentHP => currentHP;
    public float CurrentShield => currentShield;

    public float HPPercent => maxHP > 0f ? currentHP / maxHP : 0f;
    public float ShieldPercent => maxShield > 0f ? currentShield / maxShield : 0f;
    
    public event Action<float, float> OnHPChanged;              // (current, max)
    public event Action<float, float> OnShieldChanged;          // (current, max)
    public event Action OnDeath;

    // ===================== SHIELDS =====================
    [Header("Shield Settings")]
    [SerializeField] private float shieldRechargeRate = 5f;     // Shield Per Second   
    [SerializeField] private float shieldRechargeDelay = 3f;    // Second Delay Until Shield Starts Recharge
    [SerializeField] private float shieldRespawnDelay = 10f;    // Second Delay Until
    
    private float lastHitTime;
    private bool shieldActive = true;
    
    // ===================== REFERENCES =====================
    [Header("References")]
    [SerializeField] private ShieldVFX shieldVFX;
    [SerializeField] private DamageVFXLibrary damageVFXLibrary;


    // ================= AUDIO ==============
    [SerializeField] private string lowHealthWarning;
    private bool warningAudioStarted = false;

    // ===================== UNITY =====================
    private void Awake() {
        currentHP = maxHP;
        currentShield = maxShield;
        shieldVFX?.SetActive();
    }
    private void Update() {
        HandleShieldRecharge();

        // AUDIO WARNING LOW HEALTH
        if (HPPercent <= 0.3 && !warningAudioStarted) {
            CoreRoot.Instance.Audio.Play(lowHealthWarning);
            warningAudioStarted = true;
        } else if (HPPercent > 0.3 && warningAudioStarted) {
            CoreRoot.Instance.Audio.Stop(lowHealthWarning);
            warningAudioStarted = false;
        }
    }


    // ===================== SHIELD LOGIC =====================
    private void HandleShieldRecharge() {
        if (shieldActive) {
            if (currentShield < maxShield &&
                Time.time - lastHitTime > shieldRechargeDelay) {
                RestoreShield(shieldRechargeRate * Time.deltaTime);
            }
        }
        else if (maxShield > 0f &&
                 Time.time - lastHitTime > shieldRespawnDelay) {
            ReactivateShield();
        }
    }    
    public void RestoreShield(float amount) {
        currentShield = Mathf.Min(maxShield, currentShield + amount);
        shieldActive = currentShield > 0f;
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }
    public void ReactivateShield() {
        currentShield = maxShield;
        shieldActive = true;
        shieldVFX?.SetActive();
        OnShieldChanged?.Invoke(currentShield, maxShield);

    }
    public void DeactivateShield() {
        currentShield = 0;
        shieldActive = false;
        shieldVFX?.SetInactive();
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }



    public void RestoreHP(float amount) {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    // ===================== IDAMAGEABLE =====================
    public Transform Transform => transform;
    public void TakeDamage(Damage damage) {
        lastHitTime = Time.time;

        if (shieldActive && currentShield > 0f) {
            ApplyShieldDamage(damage);
        }
        else {
            ApplyHullDamage(damage);
        }
    }


    // ===================== DAMAGE APPLICATION =====================
    private void ApplyShieldDamage(Damage damage) {
        // --- Damage ---
        float multiplier = DamageModifiers.GetMultiplier(damage.Type, TargetType.Shield);
        float effectiveDamage = damage.Amount * multiplier;
        currentShield = Mathf.Max(0f, currentShield - effectiveDamage);
        OnShieldChanged?.Invoke(currentShield, maxShield);

        // --- VFX ---
        shieldVFX?.PlayRippleEffect(
            damage.HitPoint,
            ShieldPercent
        );
        ApplyDamageVFX(damage, TargetType.Shield);

        // --- Damage Indicator ---
        ShipRoot.Instance.Indicator.ShowDamageIndicator(damage);

        if (currentShield <= 0f) {
            DeactivateShield();
        }
    }

    private void ApplyHullDamage(Damage damage) {
        if (currentHP <= 0f) return;

        // --- Damage ---
        float multiplier = DamageModifiers.GetMultiplier(damage.Type, TargetType.Armored);
        float effectiveDamage = damage.Amount * multiplier;
        currentHP = Mathf.Max(0f, currentHP - effectiveDamage);
        OnHPChanged?.Invoke(currentHP, maxHP);

        // --- VFX ---
        ApplyDamageVFX(damage, TargetType.Armored);

        // --- Damage Indicator ---
        ShipRoot.Instance.Indicator.ShowDamageIndicator(damage);

        if (currentHP <= 0f) {
            OnDeath?.Invoke();
        }
    }

    // ===================== VFX =====================
    private void ApplyDamageVFX(Damage damage, TargetType targetType) {
        if (damageVFXLibrary == null) return;
        DamageVFX effect = damageVFXLibrary.GetEffect(damage.Type, targetType);
        if (effect == null) return;

        if (effect.vfxPrefab != null)
            Instantiate(effect.vfxPrefab, damage.HitPoint, Quaternion.identity);

        if (!string.IsNullOrEmpty(effect.audioClipName))
            CoreRoot.Instance.Audio.Play(effect.audioClipName);
    }
}
