using TMPro;
using UnityEngine;

public class LootLabel3D : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] Vector3 offset = Vector3.up * 1.5f;
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float floatAmplitude = 0.25f;

    Transform cam;
    Vector3 startPos;

    private void Awake() {
        cam = Camera.main.transform;
        startPos = transform.localPosition;
    }

    private void Update() {
        FaceCamera();
        Float();
    }

    public void Initialize(string displayName, Rarity rarity) {
        text.text = displayName;
        text.color = RarityColor(rarity);
    }

    private void FaceCamera() {
        transform.rotation = Quaternion.LookRotation(
            transform.position - cam.position
        );
    }

    private void Float() {
        float y = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startPos + offset + Vector3.up * y;
    }

    Color RarityColor(Rarity rarity) {
        return rarity switch {
            Rarity.Common    => Color.white,
            Rarity.Uncommon  => Color.green,
            Rarity.Rare      => Color.cyan,
            Rarity.Epic      => new Color(0.8f, 0.3f, 1f),
            Rarity.Legendary => new Color(1f, 0.7f, 0.2f),
            _ => Color.white
        };
    }
}
