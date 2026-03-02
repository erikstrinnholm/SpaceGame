using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorState : MonoBehaviour {
    public bool LowerBodyLock { get; private set; }
    public bool UpperBodyLock { get; private set; }

    public void SetUpperBodyLock(bool value) {
        UpperBodyLock = value;
    }

    public void SetLowerBodyLock(bool value) {
        LowerBodyLock = value;
    }
}