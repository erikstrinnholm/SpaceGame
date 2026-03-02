using UnityEngine;

public interface IMissileBehavior {
    void Initialize(Missile missile);
    void OnEvent(MissileEvent evt);
}
