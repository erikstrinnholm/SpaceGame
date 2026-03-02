using UnityEngine;


public abstract class DamageDealer : MonoBehaviour {
    [Header("Damage")]
    [SerializeField] protected DamageType damageType;
    [SerializeField] protected float damageAmount = 10f;

    [Header("Behavior")]
    [SerializeField] protected bool singleUse = true;
    [SerializeField] protected float hitCooldown = 0.5f;
    [SerializeField] protected bool destroyOnHit = true;
    [SerializeField] float impactForce = 10f;
    [SerializeField] bool applyForceOnHit = false;

    [Header("Layer Filters")]
    [Tooltip("Layers that should only receive impact, no damage")]
    [SerializeField] protected LayerMask impactOnlyMask;

    // Runtime
    protected GameObject owner;
    protected float lastHitTime = -999f;
    protected bool hasDealtDamage;


    // ================= INITIALIZATION =================
    public virtual void Initialize(GameObject owner, int layer, DamageType type, float amount) {
        this.owner   = owner;
        damageType   = type;
        damageAmount = amount;

        if (layer >= 0)
            SetLayer(layer);
    }
    protected virtual void OnEnable() {
        hasDealtDamage = false;
        lastHitTime = -999f;
    }

    // ================= PHYSICS ENTRY =================
    protected virtual void OnTriggerEnter(Collider other) {
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        TryApplyDamageOrImpact(other, hitPoint);
    }

    // ================= CORE DAMAGE LOGIC =================
    protected bool TryApplyDamageOrImpact(Collider hitCollider, Vector3 hitPoint)
    {
        if (!CanDealDamage())
            return false;

        // Owner protection
        if (owner && hitCollider.transform.IsChildOf(owner.transform))
            return false;

        int otherLayer = hitCollider.gameObject.layer;
        bool didDamage = false;

        IDamageable target = hitCollider.GetComponentInParent<IDamageable>();
        if (target == null)
            return false;

        // Only apply damage if it's NOT in impact-only mask
        if (((int)impactOnlyMask & (1 << otherLayer)) == 0) {
            ApplyDamage(target, hitPoint);
            didDamage = true;
        }

        // Always apply impact force if enabled
        if (applyForceOnHit)
            ApplyImpactForce(hitCollider, hitPoint);        

        if (didDamage)
            RegisterHit();

        OnHit(target, hitCollider);

        if (destroyOnHit && didDamage)
            Destroy(gameObject);

        return true;
    }

    protected bool CanDealDamage() {
        if (singleUse && hasDealtDamage)
            return false;

        return Time.time - lastHitTime >= hitCooldown;
    }
    protected void RegisterHit() {
        hasDealtDamage = true;
        lastHitTime = Time.time;
    }



    // ================= DAMAGE APPLICATION =================
    protected virtual void ApplyDamage(IDamageable target, Vector3 hitPoint) {
        Vector3 sourcePos = owner != null ? owner.transform.position : transform.position;
        Vector3 direction = (hitPoint - sourcePos).normalized;

        Damage damage = new Damage(
            amount: damageAmount,
            type: damageType,
            hitPoint: hitPoint,
            direction: direction,
            source: owner,
            dealer: this
        );

        target.TakeDamage(damage);
    }

    // ================= HOOK =================
    protected virtual void OnHit(IDamageable target, Collider hitCollider) { }

    // ================= LAYER HELPER =================
    protected void SetLayer(int layer) {
        gameObject.layer = layer;
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;
    }



    protected void ApplyImpactForce(Collider hitCollider, Vector3 hitPoint) {
        IImpactReceiver impactReceiver = hitCollider.GetComponentInParent<IImpactReceiver>();   //CHECK THIS!!
        if (impactReceiver == null) return;

        Vector3 direction = (hitCollider.transform.position - transform.position).normalized;
        impactReceiver.ApplyImpact(direction * impactForce, hitPoint);
    }
}
