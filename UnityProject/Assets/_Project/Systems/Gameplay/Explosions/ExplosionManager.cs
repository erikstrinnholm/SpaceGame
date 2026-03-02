using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private DamageVFXLibrary damageVFXLibrary;
    [SerializeField] private Transform vfxSpawnParent;
    private readonly Queue<Damage> explosionQueue = new();


    public void ScheduleExplosion(Damage explosionDamage, GameObject optionalCustomVFX = null) {
        explosionQueue.Enqueue(explosionDamage);

        // Optional: spawn custom prefab
        if (optionalCustomVFX != null) {
            GameObject vfx = Instantiate(optionalCustomVFX, explosionDamage.HitPoint, Quaternion.identity, vfxSpawnParent);
            float scaleFactor = explosionDamage.AreaOfEffect / 1f; // adjust if prefab scale != 1
            vfx.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    
    private void LateUpdate() {
        // Process queued explosions once per frame
        while (explosionQueue.Count > 0) {
            Damage explosion = explosionQueue.Dequeue();
            ProcessExplosion(explosion);
        }
    }

    private void ProcessExplosion(Damage explosion) {
        // --- 1. VFX + Audio from DamageVFXConfig ---
        if (damageVFXLibrary != null) {
            DamageVFX effect = damageVFXLibrary.GetEffect(explosion.Type, TargetType.Normal);
            if (effect != null) {
                if (effect.vfxPrefab != null) {
                    GameObject vfx = Instantiate(effect.vfxPrefab, explosion.HitPoint, Quaternion.identity, vfxSpawnParent);
                    float scaleFactor = explosion.AreaOfEffect / 1f;
                    vfx.transform.localScale = Vector3.one * scaleFactor;
                }
                if (!string.IsNullOrEmpty(effect.audioClipName))
                    CoreRoot.Instance.Audio.Play(effect.audioClipName);
            }
        }
        // --- 2. Apply AoE damage ---
        if (explosion.AreaOfEffect <= 0f) return;

        Collider[] hits = Physics.OverlapSphere(explosion.HitPoint, explosion.AreaOfEffect);
        HashSet<IDamageable> damaged = new HashSet<IDamageable>();

        foreach (var hit in hits) {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target != null && !damaged.Contains(target)) {
                // Pass full Damage struct
                target.TakeDamage(explosion);
                damaged.Add(target);
            }
        }
    }
}
