using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatusPanelUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerDamageReceiver playerReceiver;

    [Header("Hull HP UI")]
    [SerializeField] private Image hpBarFill;
    [SerializeField] private TMP_Text hpValueText;

    [Header("Shield UI")]
    [SerializeField] private Image shieldBarFill;
    [SerializeField] private TMP_Text shieldValueText;

    [Header("Animation")]
    [SerializeField] private float shieldSmoothSpeed = 4f;
    [SerializeField] private float hpRegenSmoothSpeed = 2f;

    private float displayedHP;
    private float displayedShield;
    private float targetHP;
    private float targetShield;



    // ================= UNITY ===============
    private void Start() {
        if (playerReceiver == null) {
            Debug.LogError("ShipStatusUI: Missing PlayerDamageReceiver!");
            enabled = false;
            return;
        }
        // Initialize values
        displayedHP = targetHP = playerReceiver.CurrentHP;
        displayedShield = targetShield = playerReceiver.CurrentShield;
        UpdateBarsImmediate();

        // Subscribe
        playerReceiver.OnHPChanged += OnHPChanged;
        playerReceiver.OnShieldChanged += OnShieldChanged;
        playerReceiver.OnDeath += HandleDeath;
    }
    private void OnDestroy() {
        if (playerReceiver == null) return;
        playerReceiver.OnHPChanged -= OnHPChanged;
        playerReceiver.OnShieldChanged -= OnShieldChanged;
        playerReceiver.OnDeath -= HandleDeath;
    }
    private void Update() {
        UpdateShieldUI();
        UpdateHPUI();
    }


    // ================= EVENTS ===============
    private void OnHPChanged(float current, float max) {
        targetHP = current;
        if (hpValueText != null)
            hpValueText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
    private void OnShieldChanged(float current, float max) {
        targetShield = current;
        if (shieldValueText != null)
            shieldValueText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }


    // ================= UPDATE LOOPS ===============
    private void UpdateHPUI() {
        // HP damage should feel instant
        if (targetHP < displayedHP) {
            displayedHP = targetHP;
        }
        // Optional regen smoothing
        else {
            displayedHP = Mathf.MoveTowards(
                displayedHP,
                targetHP,
                hpRegenSmoothSpeed * Time.deltaTime * playerReceiver.MaxHP
            );
        }
        if (hpBarFill != null)
            hpBarFill.fillAmount = playerReceiver.MaxHP > 0f
                ? displayedHP / playerReceiver.MaxHP
                : 0f;
    }
    private void UpdateShieldUI() {
        // Shield is always smooth
        displayedShield = Mathf.MoveTowards(
            displayedShield,
            targetShield,
            shieldSmoothSpeed * Time.deltaTime * playerReceiver.MaxShield
        );

        if (shieldBarFill != null)
            shieldBarFill.fillAmount = playerReceiver.MaxShield > 0f
                ? displayedShield / playerReceiver.MaxShield
                : 0f;
    }



    // ================= HELPERS ===============
    private void UpdateBarsImmediate() {
        if (hpBarFill != null)
            hpBarFill.fillAmount = playerReceiver.HPPercent;

        if (shieldBarFill != null)
            shieldBarFill.fillAmount = playerReceiver.ShieldPercent;
    }
    private void HandleDeath() {
        Debug.Log("Player died — trigger death UI here.");
    }

}
