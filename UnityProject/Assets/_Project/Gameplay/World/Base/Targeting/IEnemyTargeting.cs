using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemyTargeting {
    bool HasTarget { get; }
    AimSolution GetAimSolution();
    void TickTargeting();
    void ClearTarget();
}
