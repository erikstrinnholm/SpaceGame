using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImpactReceiver {
    void ApplyImpact(Vector3 force, Vector3 point);
}

//ships can implement it
//asteroids can implement it
//static environment doesn't