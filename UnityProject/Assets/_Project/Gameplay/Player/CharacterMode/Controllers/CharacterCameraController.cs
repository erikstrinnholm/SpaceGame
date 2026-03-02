using UnityEngine;

/// <summary>
/// Handles character camera rotation.
/// 
/// - Applies yaw to the character.
/// - Applies pitch to the camera pivot.
/// - Updates crosshair positioning.
/// 
/// </summary>
public class CharacterCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private RectTransform crosshairRoot;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    private Vector2 lookInput;
    private float pitch;

    // ========== UNITY LIFECYCLE ===============================
    private void Awake() {
        ValidateDependencies();
    }
    private void LateUpdate() {
        HandleRotation();
        CenterCrosshair();
    }


    // ========== INPUT ACTION ==================================
    public void OnLook(Vector2 input) {
        lookInput = input;
    }

    private void HandleRotation() {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Yaw (rotate character)
        transform.Rotate(Vector3.up * mouseX);

        // Pitch (rotate camera pivot)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    private void CenterCrosshair() {
        if (crosshairRoot == null) return;

        crosshairRoot.position = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );
    }
    private void ValidateDependencies() {
        if (!cameraPivot)
            Debug.LogError($"{nameof(CharacterCameraController)}: Missing camera pivot reference.");
        if (!crosshairRoot)
            Debug.LogError($"{nameof(CharacterCameraController)}: Missing crosshair root reference.");
    }    
}
