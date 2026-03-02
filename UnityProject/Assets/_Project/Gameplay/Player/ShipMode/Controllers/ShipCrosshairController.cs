using UnityEngine;
using UnityEngine.InputSystem;


 
/// Handles crosshair movement (mouse/controller) and visual feedback (fire bloom).
public class ShipCrosshairController : MonoBehaviour
{    
    [Header("References")]
    [SerializeField] private RectTransform crosshairRoot;
    [SerializeField] private RectTransform crosshairVisual;

    [Header("Controller Settings")]
    [SerializeField] private float controllerMaxOffset      = 400f;     // max distance from center for stick
    [SerializeField] private float controllerSensitivity    = 800f;     // pixels/sec
    [SerializeField] private float controllerReturnSpeed    = 6f;       // how fast stick drifts back
    [SerializeField] private float controllerSmoothSpeed    = 15f;      // smoothing factor

    [Header("Fire Bloom Settings")]
    [SerializeField] private float fireBloomPerShot         = 0.10f;
    [SerializeField] private float maxBloom                 = 0.30f;
    [SerializeField] private float bloomDecaySpeed          = 3f;

    // ================= STATE =================
    private Vector2 controllerInput;   // value from OnAim for sticks
    private Vector2 controllerOffset;  // smoothed controller offset
    private Vector2 smoothedOffset;    // final offset from screen center
    private bool usingMouse;           // current active input device

    private Vector3 originalScale;
    private float bloomCurrent;


    // ================= UNITY =================
    private void Awake() {
        originalScale = crosshairVisual.localScale;
    }
    private void Update() {
        UpdateAim();
        UpdateCrosshairPosition();
        UpdateVisuals();
    }


    // ================= INPUT CALLBACK =================
    public void OnAim(InputAction.CallbackContext ctx) {
        if (ctx.control.device is Mouse) {
            usingMouse = true;
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            smoothedOffset = Mouse.current.position.ReadValue() - screenCenter;
        }
        else {
            controllerInput = ctx.ReadValue<Vector2>();
            if (controllerInput.sqrMagnitude > 0.001f)
                usingMouse = false;
        }
    }


    // ================= AIM LOGIC =================
    private void UpdateAim() {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        if (usingMouse) {
            // Mouse relative to screen center
            Vector2 mousePos = Mouse.current.position.ReadValue();
            smoothedOffset = mousePos - screenCenter;
        }
        else {
            // Controller stick [-1,1] → relative offset
            Vector2 stick = Vector2.ClampMagnitude(controllerInput, 1f);
            // Apply sensitivity
            controllerOffset += stick * controllerSensitivity * Time.deltaTime;
            // Clamp radius
            controllerOffset = Vector2.ClampMagnitude(controllerOffset, controllerMaxOffset);
            // Drift back to center
            controllerOffset = Vector2.Lerp(controllerOffset, Vector2.zero, controllerReturnSpeed * Time.deltaTime);
            // Smooth final offset
            smoothedOffset = Vector2.Lerp(smoothedOffset, controllerOffset, controllerSmoothSpeed * Time.deltaTime);            
        }
    }


    // ================= CROSSHAIR POSITION =================
    private void UpdateCrosshairPosition() {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        crosshairRoot.position = screenCenter + smoothedOffset;
    }


    // ================= FIRE BLOOM =================
    public void OnFire(bool check) {
        bloomCurrent += fireBloomPerShot;
        bloomCurrent = Mathf.Min(bloomCurrent, maxBloom);
    }
    private void UpdateVisuals() {
        bloomCurrent = Mathf.MoveTowards(bloomCurrent, 0f, bloomDecaySpeed * Time.deltaTime);
        crosshairVisual.localScale = originalScale * (1f + bloomCurrent);
    }  


    // ================= FUTURE EXTENSIONS =================
    public void TriggerHitFlash() {}
    private void ApplyAimAssist() {}
}