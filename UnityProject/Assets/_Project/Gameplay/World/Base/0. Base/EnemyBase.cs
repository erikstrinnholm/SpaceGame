using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Includes everything lootable that can be destroyed and take damage! (not player)
public class EnemyBase : Lootable, IDamageable {
    [Header("Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private TargetType targetType;
    private float currentHP;

    [Header("Shield")]
    [SerializeField] private EnemyShield enemyShield;

    [Header("VFX & Audio")]
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private string deathSound;
    [SerializeField] private DamageVFXLibrary damageVFXLibrary;

    
    public Transform Transform => transform;


    // ================= IDAMAGEABLE =================
    public virtual void TakeDamage(Damage damage) {
        if (currentHP <= 0f) return;        
        float remainingDamage = damage.Amount;

        if (enemyShield != null) {
            remainingDamage = enemyShield.AbsorbDamage(damage);
        }
        if (remainingDamage > 0f) {
            ApplyHealthDamage(damage, remainingDamage);
        }

        if (currentHP <= 0f) {
            Death(damage);
        }
    }

    // ================= DAMAGE =================
    private void ApplyHealthDamage(Damage damage, float amount) {
        float multiplier = DamageModifiers.GetMultiplier(damage.Type, targetType);
        float effectiveDamage = amount * multiplier;
        currentHP -= effectiveDamage;
        currentHP = Mathf.Max(0f, currentHP);
        OnHealthDamageTaken(damage, effectiveDamage);
    }
    protected virtual void OnHealthDamageTaken(Damage damage, float effectiveDamage) {
        //Hit reactions, AI alerts, sparks, etc.
        if (damageVFXLibrary == null) return;
        DamageVFX effect = damageVFXLibrary.GetEffect(damage.Type, targetType);
        if (effect == null) return;

        if (effect.vfxPrefab != null)
            Instantiate(effect.vfxPrefab, damage.HitPoint, Quaternion.identity);

        if (!string.IsNullOrEmpty(effect.audioClipName))
            CoreRoot.Instance.Audio.Play(effect.audioClipName);        
    }


    // ================= LIFE CYCLE =================
    protected virtual void Awake() {
        currentHP = Mathf.Max(maxHP, 1f);
    }
    protected virtual void Death(Damage killingDamage) {
        PlayDeathVFX(killingDamage);
        DropLoot();
        Destroy(gameObject);
    }
    private void PlayDeathVFX(Damage killingDamage) {
        if (deathVFX != null) {
            Instantiate(
                deathVFX,
                transform.position,
                Quaternion.LookRotation(-killingDamage.Direction)
            );
        }
        if (!string.IsNullOrEmpty(deathSound)) {
            CoreRoot.Instance.Audio.Play(deathSound);
        }
    }

    protected void SetMaxHP(float hp) {
        maxHP = hp;
        currentHP = maxHP;
    }        
}
