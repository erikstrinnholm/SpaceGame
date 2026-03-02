using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public struct IndicatorContext {
    public Transform source;
    public Vector3 worldPosition;
    public DamageLayer damageLayer;
    public ThreatLevel threatLevel;
    public float intensity;
}
*/


public abstract class BaseThreatIndicator : MonoBehaviour {    
    protected RectTransform rect;
    protected Camera cam;

    protected virtual void Awake(){
        rect = transform as RectTransform;
    }
    public virtual void Initialize(Camera camera) {
        cam = camera;
    }
}


    //public abstract void Initialize(Damage damage,);
    //public abstract void Initialize(IndicatorContext context);


    //public abstract void Tick(float deltaTime);
    //public abstract bool ShouldDespawn();
    //public virtual void SetAlpha(float a) {}


/*
Shared responsibilities
✔ World → screen projection
✔ Edge clamping
✔ Rotation logic
✔ Fade handling
✔ Lifetime control
*/
