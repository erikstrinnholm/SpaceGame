using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour {

    [Header("Shop Stock")]
    [SerializeField] private List<InventorySlot> stock = new List<InventorySlot>();








    // ------------------ SAVE / LOAD  ------------------
    public ShopInventorySaveData ToSaveData() {return null; }
    public void LoadFromSaveData(ShopInventorySaveData save, ItemDatabase db) {}
}
