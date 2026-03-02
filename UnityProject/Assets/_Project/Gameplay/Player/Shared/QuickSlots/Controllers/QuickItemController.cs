using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//✔ UI should prevent non-consumables from entering quick slots (good comment)

// Design:
// - 4 fixed quick slots
// - Cooldowns tracked per slot
// - Item is consumed ONLY after successful use
public class QuickItemController : MonoBehaviour {
    private PlayerInventory playerInventory =>  GameRoot.Instance.PlayerData.PlayerInventory;
    private const int QuickSlotCount = 4;
    private float[] cooldownTimers = new float[QuickSlotCount];
    private bool hasActiveCooldowns;


    // ------------ UNITY LIFECYCLE -------------
    private void Update() {
        if (!hasActiveCooldowns) {
            enabled = false; // stop Update()
            return;
        }
        TickCooldowns();
    }    


    // ----------- INPUT EVENTS ------------
    public void OnUseQuickItem1() { OnUseQuickSlot(0); }
    public void OnUseQuickItem2() { OnUseQuickSlot(1); }
    public void OnUseQuickItem3() { OnUseQuickSlot(2); }
    public void OnUseQuickItem4() { OnUseQuickSlot(3); }
    private void OnUseQuickSlot(int index) {
        if (!IsValidSlot(index)) return;
        if (cooldownTimers[index] > 0f) return;

        // Peek first
        Item item = playerInventory.GetQuickItem(index);
        if (item == null) return;        

        bool usedSuccessfully = ItemUseSystem.TryUse(item);
        if (!usedSuccessfully) {
            Debug.LogWarning("QuickItemController: Failed to use item " + item.data.displayName);
            return;
        }

        // Consume only after success
        playerInventory.TakeOneQuickItem(index);
        StartCooldown(index, item.data.cooldown);
    }


    // -------- COOLDOWN --------
    private void StartCooldown(int index, float duration) {
        if (duration <= 0f) return;
        cooldownTimers[index] = duration;
        hasActiveCooldowns = true;
        enabled = true; // ensure Update() runs        
    }
    private void TickCooldowns() {
        hasActiveCooldowns = false;

        for (int i = 0; i < cooldownTimers.Length; i++) {
            if (cooldownTimers[i] <= 0f)
                continue;

            cooldownTimers[i] = Mathf.Max(0f, cooldownTimers[i] - Time.deltaTime);

            if (cooldownTimers[i] > 0f)
                hasActiveCooldowns = true;
        }
    }

    // ------------ UI ACCESS -------------
    public float GetCooldownNormalized(int index) {
        if (!IsValidSlot(index)) return 0f;

        Item item = playerInventory.GetQuickItem(index);
        if (item == null || item.data.cooldown <= 0f) return 0f;

        return Mathf.Clamp01(cooldownTimers[index] / item.data.cooldown);
    }
    private bool IsValidSlot(int index) {
        return index >= 0 && index < QuickSlotCount;
    }
}
