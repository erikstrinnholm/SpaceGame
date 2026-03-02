using UnityEngine;

public class LayerSetter : MonoBehaviour {
    [SerializeField] private string layerName;

    private void Awake() {
        int targetLayer = LayerMask.NameToLayer(layerName);
        if (targetLayer < 0) {
            Debug.LogWarning($"Invalid layer '{layerName}' on {name}", this);
            return;
        }
        SetLayerRecursive(gameObject, targetLayer);
    }

    private void SetLayerRecursive(GameObject obj, int targetLayer) {
        obj.layer = targetLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, targetLayer);
    }
}
