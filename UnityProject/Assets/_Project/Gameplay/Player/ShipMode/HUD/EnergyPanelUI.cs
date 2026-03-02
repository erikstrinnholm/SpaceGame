using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EnergyPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image energyFill;
    [SerializeField] private Image regenRing;
    [SerializeField] private Image pulseGlow;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("Smoothing")]
    [SerializeField] private float fillSmoothSpeed = 8f;

    [Header("Thresholds")]
    [SerializeField] private float lowEnergyThreshold = 0.3f;
    [SerializeField] private float criticalEnergyThreshold = 0.1f;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float maxPulseAlpha = 0.6f;
    [SerializeField] private Color lowEnergyColor = new Color32(255, 216, 90, 255);    // Yellow (#FFD85A)
    [SerializeField] private Color criticalEnergyColor = new Color32(255, 59, 59, 255); // Red (#FF3B3B)    

    // Internal State
    private EnergyManager energyManager;
    private float displayedEnergy;
    private float targetEnergy;
    private float maxEnergy;
    private float pulseTimer;


    // ================= UNITY =================
    private void Start() {
        energyManager = ShipRoot.Instance.Energy;
        maxEnergy = energyManager.MaxEnergy;
        targetEnergy = energyManager.CurrentEnergy;
        displayedEnergy = targetEnergy;
        energyManager.OnEnergyChanged += OnEnergyChanged;
        UpdateVisualsImmediate();
    }
    private void Destroy() {
        energyManager.OnEnergyChanged -= OnEnergyChanged;
    }
    private void Update() {
        SmoothEnergy();
        UpdatePulseGlow();
        UpdateRegenRing();
    }
    private void OnEnergyChanged() {
        if (energyManager == null) return;
        targetEnergy = energyManager.CurrentEnergy;
    }


    // ================= ENERGY =================
    private void SmoothEnergy() {
        // Drop instantly, regen smoothly
        if (targetEnergy < displayedEnergy) {
            displayedEnergy = targetEnergy;
        } else {
            displayedEnergy = Mathf.MoveTowards(
                displayedEnergy,
                targetEnergy,
                fillSmoothSpeed * Time.deltaTime * maxEnergy
            );
        }

        float normalized = maxEnergy > 0f ? displayedEnergy / maxEnergy : 0f;
        energyFill.fillAmount = normalized;

        if (energyText != null) {
            energyText.text =
                $"{Mathf.CeilToInt(displayedEnergy)} / {Mathf.CeilToInt(maxEnergy)}";
        }
    }

    // ================= PULSE =================
    private void UpdatePulseGlow() {
        float normalized = maxEnergy > 0f ? displayedEnergy / maxEnergy : 0f;

        if (normalized > lowEnergyThreshold) {
            pulseGlow.color = SetAlpha(pulseGlow.color, 0f);
            pulseTimer = 0f;
            return;
        }

        pulseTimer += Time.deltaTime * pulseSpeed;
        float pulse = Mathf.Sin(pulseTimer) * 0.5f + 0.5f;

        float intensity = normalized <= criticalEnergyThreshold
            ? maxPulseAlpha
            : maxPulseAlpha * 0.5f;

        Color targetColor =
            normalized <= criticalEnergyThreshold
                ? criticalEnergyColor
                : lowEnergyColor;

        targetColor.a = pulse * intensity;
        pulseGlow.color = targetColor;
    }

    // ================= REGEN =================
    private void UpdateRegenRing() {
        regenRing.transform.Rotate(0f, 0f, -30f * Time.deltaTime);
    }


    // ================= HELPERS =================
    private void UpdateVisualsImmediate() {
        if (energyManager == null) return;

        displayedEnergy = energyManager.CurrentEnergy;
        energyFill.fillAmount = energyManager.Normalized;
    }
    private Color SetAlpha(Color c, float a) {
        c.a = a;
        return c;
    }
}



/*
    private void Start() {
        maxEnergy = GameRoot.Instance.Energy.MaxEnergy;
        displayedEnergy = GameRoot.Instance.Energy.CurrentEnergy;

        //energySystem.OnEnergyChanged += OnEnergyChanged;
        UpdateVisualsImmediate();
    }
    private void OnDestroy() {  
        if (energySystem != null)
            energySystem.OnEnergyChanged -= OnEnergyChanged;
    }
    private void Update() {
        UpdateEnergyFill();
        UpdatePulseGlow();
        UpdateRegenRing();
    }
    // ---------------- ENERGY ----------------
    private void OnEnergyChanged() {
        // Intentionally empty
        // Target value is read from energySystem in Update()
    }
*/

    