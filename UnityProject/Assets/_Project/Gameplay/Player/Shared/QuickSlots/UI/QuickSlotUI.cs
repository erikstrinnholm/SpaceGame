using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



//Single Slot
public class QuickSlotUI : MonoBehaviour {
    
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay; 
    [SerializeField] private Image borderHighlight; // Optional: flash on use / cooldown
    [SerializeField] private TextMeshProUGUI countText; 

    private int slotIndex;
    private QuickItemController controller;
    private PlayerInventory playerInventory;              //cache, updated with refreshUI


    private void Awake() {
        if (iconImage != null) iconImage.enabled = false;
        if (countText != null) countText.text = "";
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        if (borderHighlight != null) borderHighlight.enabled = false;        
    }


    // ------------------ INITIALIZATION ------------------
    public void Initialize(int index, QuickItemController quickController) {
        slotIndex = index;
        controller = quickController;

        playerInventory = GameRoot.Instance.PlayerData.PlayerInventory;
        RefreshUI();
    }
    private void Update() {
        UpdateCooldown();
    }


    // -------- UI LOGIC --------
    public void RefreshUI() {
        Item item = playerInventory.GetQuickItem(slotIndex);

        if (item == null) {
            if (iconImage != null) iconImage.enabled = false;
            if (countText != null) countText.text = "";
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (borderHighlight != null) borderHighlight.enabled = false;
            return;
        }
        iconImage.enabled = true;
        iconImage.sprite = item.data.icon;
        countText.text = item.count > 1 ? item.count.ToString() : "";

        UpdateCooldown();
    }
    private void UpdateCooldown() {
        if (cooldownOverlay == null || controller == null) return;
        cooldownOverlay.fillAmount = controller.GetCooldownNormalized(slotIndex);
    }

    // -------- OPTIONAL VISUAL FEEDBACK --------
    public void FlashHighlight() {
        if (borderHighlight == null) return;
        borderHighlight.enabled = true;
        Invoke(nameof(HideHighlight), 0.15f);
    }
    private void HideHighlight() {
        if (borderHighlight != null) borderHighlight.enabled = false;
    }
}
