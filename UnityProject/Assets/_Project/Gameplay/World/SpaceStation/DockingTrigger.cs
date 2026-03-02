using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DockingTrigger : MonoBehaviour {
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "HangarScene";
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        if (hasTriggered)
            return;

        hasTriggered = true;
        Debug.Log("Docking trigger activated. Loading scene: " + sceneToLoad);
        CoreRoot.Instance.Loader.LoadScene(sceneToLoad);
    }
}

