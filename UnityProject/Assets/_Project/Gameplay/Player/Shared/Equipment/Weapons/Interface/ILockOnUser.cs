using UnityEngine;

public interface ILockOnUser {
    bool UsesLockOn { get; }
    bool RequiresLockOn { get; }
    bool HasLock { get; }
    float LockProgress { get; }     // 0–1
    IDamageable LockedTarget { get; }
    void UpdateLockOn(Transform crosshair);
}
