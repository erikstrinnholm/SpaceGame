using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Missile Behaviors/Missile Split")]
public class BehaviorMissileSplit : MissileBehavior {

    [Header("Split Settings")]
    public int count = 4;
    public float spreadAngle = 12f;
    public float splitDelay = 0.2f;
    public bool inheritTarget = true;


    public override void OnEvent(MissileEvent evt) {
        if (evt == MissileEvent.OnLaunch && missile.CanSplit) {
            missile.StartCoroutine(SplitRoutine());
        }
    }

    private IEnumerator SplitRoutine() {
        yield return new WaitForSeconds(splitDelay);

        if (!missile.CanSplit)
            yield break;

        missile.MarkSplit();

        Rigidbody parentRb = missile.GetRigidbody();
        Vector3 velocity = parentRb.velocity;
        IDamageable target = inheritTarget ? missile.GetTarget() : null;

        for (int i = 0; i < count; i++) {
            Quaternion rot = Quaternion.Euler(Random.insideUnitSphere * spreadAngle);

            GameObject obj = Instantiate(
                missile.gameObject,
                missile.transform.position,
                rot * missile.transform.rotation
            );

            if (obj.TryGetComponent(out Missile child)) {
                child.Initialize(
                    
                    missile.gameObject,
                    CollisionLayers.Missiles,
                    missile.GetMissileData(),
                    target,
                    allowSplit: false // 🚫 children never split
                );

                child.GetRigidbody().velocity = velocity;
            }
        }
        Destroy(missile.gameObject);
    }
}

