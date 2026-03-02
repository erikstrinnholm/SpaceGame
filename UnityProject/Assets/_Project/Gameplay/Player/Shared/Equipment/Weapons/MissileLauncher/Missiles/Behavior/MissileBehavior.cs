using UnityEngine;

public abstract class MissileBehavior : ScriptableObject, IMissileBehavior {
    protected Missile missile;

    public virtual void Initialize(Missile missile) {
        this.missile = missile;
    }

    public abstract void OnEvent(MissileEvent evt);
}
