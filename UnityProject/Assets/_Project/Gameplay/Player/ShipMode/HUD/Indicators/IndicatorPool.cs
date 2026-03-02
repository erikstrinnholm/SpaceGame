using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPool : MonoBehaviour {
    public T Get<T>() where T : BaseThreatIndicator {
        return null;
    }
    public void Return(BaseThreatIndicator indicator) {}
}


/*
Benefits:
No GC spikes
Consistent animations
Smooth performance during combat

(Do not instantiate every time.!)
*/