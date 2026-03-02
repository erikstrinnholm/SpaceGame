using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContext : MonoBehaviour {
    [Header("Player")]
    public Camera playerCamera;

    [Header("UI Roots")]
    public RectTransform damageIndicatorRoot;
    public RectTransform missileIndicatorRoot;
}
