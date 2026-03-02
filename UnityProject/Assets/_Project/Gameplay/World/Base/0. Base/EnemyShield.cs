using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyShield : MonoBehaviour {

    [Header("Shield Stats")]
    [SerializeField] private float maxShield = 100f;
    [SerializeField] private float rechargeRate = 5f;
    [SerializeField] private float rechargeDelay = 3f;
    [SerializeField] private float respawnDelay = 8f;

    [Header("References")]
    [SerializeField] private ShieldVFX shieldVFX;
    [SerializeField] private DamageVFXLibrary damageVFXLibrary;

    private float currentShield;
    private float lastHitTime;
    private bool shieldActive;

    public float MaxShield => maxShield;
    public float CurrentShield => currentShield;
    public bool IsActive => shieldActive && currentShield > 0f;

    private void Awake() {
        if (maxShield <= 0f) {
            shieldActive = false;
            return;
        }

        currentShield = maxShield;
        shieldActive = true;
        shieldVFX?.SetActive();
    }

    private void Update() {
        HandleRecharge();
    }

    // ================= SHIELD FLOW =================
    public float AbsorbDamage(Damage damage) {
        lastHitTime = Time.time;

        if (!IsActive)
            return damage.Amount;

        float multiplier = DamageModifiers.GetMultiplier(damage.Type, TargetType.Shield);
        float effectiveDamage = damage.Amount * multiplier;
        currentShield -= effectiveDamage;

        // --- Shield VFX ---
        if (shieldVFX != null) {
            shieldVFX.PlayRippleEffect(damage.HitPoint, currentShield / maxShield);
        }

        // --- Shield Particle Impact Effects ---
        ApplyDamageVFX(damage, TargetType.Shield);

        if (currentShield <= 0f) {
            BreakShield();
            return Mathf.Abs(currentShield); // overflow goes to hull
        }

        return 0f;
    }

    private void ApplyDamageVFX(Damage damage, TargetType type) {
        if (damageVFXLibrary == null) return;

        DamageVFX effect = damageVFXLibrary.GetEffect(damage.Type, type);
        if (effect == null) return;

        if (effect.vfxPrefab != null)
            Instantiate(effect.vfxPrefab, damage.HitPoint, Quaternion.identity);

        if (!string.IsNullOrEmpty(effect.audioClipName))
            CoreRoot.Instance.Audio.Play(effect.audioClipName);
    }


    private void BreakShield() {
        currentShield = 0f;
        shieldActive = false;
        shieldVFX?.SetInactive();
    }

    private void ReactivateShield() {
        currentShield = maxShield;
        shieldActive = true;
        shieldVFX?.SetActive();
    }

    private void HandleRecharge() {
        if (maxShield <= 0f)
            return;

        if (shieldActive) {
            if (currentShield < maxShield &&
                Time.time - lastHitTime > rechargeDelay) {
                currentShield = Mathf.Min(
                    maxShield,
                    currentShield + rechargeRate * Time.deltaTime
                );
            }
        }
        else if (Time.time - lastHitTime > respawnDelay) {
            ReactivateShield();
        }
    }
}

