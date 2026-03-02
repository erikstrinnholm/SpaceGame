using UnityEngine;



public class QuickSlotPanelUI : MonoBehaviour {
    [Header("Slots")]
    [SerializeField] private QuickSlotUI[] slots;
    [SerializeField] private QuickItemController controller;
    private PlayerInventory inventory =>    GameRoot.Instance.PlayerData.PlayerInventory;


    private void Start() {
        for (int i = 0; i < slots.Length; i++) {
            slots[i].Initialize(i, controller);
        }
        inventory.OnQuickSlotsUpdated += RefreshAll;
        RefreshAll();
    }


    private void OnDestroy() {
        if (inventory != null)
            inventory.OnQuickSlotsUpdated -= RefreshAll;
    }

    private void RefreshAll() {
        foreach (var slot in slots)
            slot.RefreshUI();
    }
}
