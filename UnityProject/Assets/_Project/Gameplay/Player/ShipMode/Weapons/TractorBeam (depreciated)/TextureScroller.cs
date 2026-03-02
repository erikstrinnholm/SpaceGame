using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeedU = 0f;  // horizontal speed
    [SerializeField] private float scrollSpeedV = 0.5f; // vertical speed (along the tunnel)
    [SerializeField] private bool reverseDirection = false; // Flip scroll direction

    private Renderer rend;
    private Vector2 offset;

    void Start() {
        rend = GetComponent<Renderer>();
        offset = rend.material.mainTextureOffset;
    }

    void Update() {
        float direction = reverseDirection ? -1f : 1f;

        offset.x += scrollSpeedU * direction * Time.deltaTime;
        offset.y += scrollSpeedV * direction * Time.deltaTime;
        
        rend.material.mainTextureOffset = offset;
    }
}
