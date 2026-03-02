using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyBrainBase : MonoBehaviour {
    [Header("Core References")]
    [SerializeField] protected EnemyAttackBase attackSystem;
    protected IEnemyTargeting targetingSystem;
    //[SerializeField] protected EnemyMovementBase movementSystem;
    //[SerializeField] protected EnemyReactionBase reactionSystem;

    [Header("Brain Settings")]
    [SerializeField] protected float updateInterval = 0.1f;
    protected float nextUpdateTime;

    private void Awake() {
        targetingSystem = GetComponent<IEnemyTargeting>();
        if (targetingSystem == null)
            Debug.LogWarning("No IEnemyTargeting component found on this GameObject!");
    }


    protected virtual void Update() {
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + updateInterval;
        BrainTick();
    }

    // ================= ABSTRACT =================
    protected abstract void BrainTick();

    // ================= PUBLIC =================
    public virtual void ResetBrain() {
        targetingSystem?.ClearTarget();
    }
}

