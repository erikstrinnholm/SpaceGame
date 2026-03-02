using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Responsibilities
- Single point of entry for all threat visuals
- Receives gameplay events
- Decides what indicator to spawn
- Resolves priority & lifetime
*/
public class ThreatIndicatorSystem : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private DamageIndicator damageIndicatorPrefab;
    [SerializeField] private MissileIndicator missileIndicatorPrefab;

    private Camera playerCamera;
    private RectTransform damageIndicatorRoot;
    private RectTransform missileIndicatorRoot;


    public void Bind(Camera cam, RectTransform damageRoot, RectTransform missileRoot) {
        playerCamera = cam;
        damageIndicatorRoot = damageRoot;
        missileIndicatorRoot = missileRoot;
    }


    public void ShowDamageIndicator(Damage damage) {
        var indicator = Instantiate(damageIndicatorPrefab, damageIndicatorRoot);
        indicator.Initialize(playerCamera, damage, damageIndicatorRoot);
    }

    public void ShowMissileIndicator(Transform missile) {
        var indicator = Instantiate(missileIndicatorPrefab, missileIndicatorRoot);
        indicator.Initialize(playerCamera, missile, missileIndicatorRoot); 
    }

    //public void ShowMissileLock(Transform missile, LockState state) {}
    //public void ShowEnemyPresence(Transform enemy, ThreatLevel level) {}
}

