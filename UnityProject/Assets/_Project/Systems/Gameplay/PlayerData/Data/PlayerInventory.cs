using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerInventory {
    // ------------------ DATA ------------------
    public List<InventorySlot> cargoSlots;          // main cargo slots
    public InventorySlot[] quickSlots;              // hotbar
    public Action OnQuickSlotsUpdated;              // EVENTS (UI CAN SUBSCRIBE TO THIS)
    public Action OnCargoSlotsUpdated;              // EVENTS (UI CAN SUBSCRIBE TO THIS)   

    // ------------------ INITIALIZATION ------------------
    public PlayerInventory() {
        cargoSlots = new List<InventorySlot>();
        quickSlots = new InventorySlot[4];
    }
    public PlayerInventory(int cargoCount) : this() {
        for (int i = 0; i < cargoCount; i++) cargoSlots.Add(new InventorySlot());
        for (int i = 0; i < quickSlots.Length; i++) quickSlots[i] = new InventorySlot();
    }

    // ------------------ BASIC QUICK --------------
    private bool IsValidQuickIndex(int index) => index >= 0 && index < quickSlots.Length;
    public void InsertQuickItem(int index, Item item) { 
        if (!IsValidQuickIndex(index) || quickSlots[index].locked) return; 
        quickSlots[index].item = item; 
        Debug.Log($"Inventory instance: {this.GetHashCode()}");
        OnQuickSlotsUpdated?.Invoke();
    }   
    
    public Item TakeQuickItem(int index) {
        if (!IsValidQuickIndex(index)) return null;
        var slot = quickSlots[index];
        if (slot.IsEmpty) return null;
        var item = slot.item;
        slot.Clear();
        OnQuickSlotsUpdated?.Invoke();
        return item;
    }     
    public Item TakeOneQuickItem(int index) {
        if (!IsValidQuickIndex(index)) return null;
        var slot = quickSlots[index];
        if (slot.IsEmpty) return null;

        var one = slot.RemoveItem(1);
        OnQuickSlotsUpdated?.Invoke();
        return one;        
    }
    
    public Item GetQuickItem(int index) => IsValidQuickIndex(index) ? quickSlots[index].item : null;    
    public bool HasQuickItem(int index) => IsValidQuickIndex(index) && !quickSlots[index].IsEmpty;
    public void DeleteQuickItem(int index) {
        if (IsValidQuickIndex(index)) {
            quickSlots[index].Clear();
            OnQuickSlotsUpdated?.Invoke();
        }
    }    
    public void LockQuickIndex(int index) { if (IsValidQuickIndex(index)) quickSlots[index].locked = true; }
    public void UnlockQuickIndex(int index) { if (IsValidQuickIndex(index)) quickSlots[index].locked = false; }

    public bool TryStackQuickItem(int index, Item item) {
        if (!IsValidQuickIndex(index)) return false;

        var slot = quickSlots[index];
        bool result = slot.AddItem(item);

        OnQuickSlotsUpdated?.Invoke();
        return result;
    }

    // ------------------ BASIC CARGO --------------
    private bool IsValidCargoIndex(int index) => index >= 0 && index < cargoSlots.Count;
    public void InsertCargoItem(int index, Item item) { 
        if (!IsValidCargoIndex(index) || cargoSlots[index].locked) return;
        cargoSlots[index].item = item; 
        OnCargoSlotsUpdated?.Invoke();
    }
    
    public Item TakeCargoItem(int index) {
        if (!IsValidCargoIndex(index)) return null;
        var slot = cargoSlots[index];
        if (slot.IsEmpty) return null;
        var item = slot.item;
        slot.Clear();
        OnCargoSlotsUpdated?.Invoke();
        return item;
    }
    public Item TakeOneCargoItem(int index) {
        if (!IsValidCargoIndex(index)) return null;
        var slot = cargoSlots[index];
        if (slot.IsEmpty) return null;

        var one = slot.RemoveItem(1);
        OnCargoSlotsUpdated?.Invoke();
        return one;         
    }
    
    public Item GetCargoItem(int index) => IsValidCargoIndex(index) ? cargoSlots[index].item : null;
    public bool HasCargoItem(int index) => IsValidCargoIndex(index) && !cargoSlots[index].IsEmpty;    
    public void DeleteCargoItem(int index) {
        if (IsValidCargoIndex(index)) {
            cargoSlots[index].Clear();
            OnCargoSlotsUpdated?.Invoke();
        }
    }
    public void LockCargoIndex(int index) { if (IsValidCargoIndex(index)) cargoSlots[index].locked = true; }
    public void UnlockCargoIndex(int index) { if (IsValidCargoIndex(index)) cargoSlots[index].locked = false; }
    
    public bool TryStackCargoItem(int index, Item item) {
        if (!IsValidCargoIndex(index)) return false;

        var slot = cargoSlots[index];
        bool result = slot.AddItem(item);

        OnCargoSlotsUpdated?.Invoke();
        return result;
    }

    // ------------------ STACK + ADD ITEM ------------------
    public bool TryAddCargoItem(Item item, bool tryStackWithExisting = true) {
        if (item == null || item.data == null) return false;

        // ---- TRY STACK EXISTING ----
        if (tryStackWithExisting && item.data.isStackable) {
            foreach (var slot in cargoSlots) {
                if (!slot.IsEmpty && slot.item.data == item.data && slot.item.count < slot.item.data.maxStack) {
                    if (slot.AddItem(item)) {
                        OnCargoSlotsUpdated?.Invoke();
                        return true;    
                    }
                }
            }
        }
        foreach (var slot in cargoSlots) {
            if (slot.IsEmpty && !slot.locked) {
                if (slot.AddItem(item)) {
                    OnCargoSlotsUpdated?.Invoke();
                    return true;
                }
            }
        }
        return false;        // Not fully added
    }        
    public bool TryAddQuickItem(Item item, bool tryStackWithExisting = true) {
        if (item == null || item.data == null) return false;

        // ---- TRY STACK EXISTING ----
        if (tryStackWithExisting && item.data.isStackable) {
            foreach (var slot in quickSlots) {
                if (!slot.IsEmpty && slot.item.data == item.data && slot.item.count < slot.item.data.maxStack) {
                    if (slot.AddItem(item)) {
                        OnQuickSlotsUpdated?.Invoke();
                        return true;
                    }
                }
            }
        }
        foreach (var slot in quickSlots) {
            if (slot.IsEmpty && !slot.locked) {
                if (slot.AddItem(item)) {
                    OnQuickSlotsUpdated?.Invoke();
                    return true;
                }
            }
        }
        return false;         // Not fully added
    }
    
    // ------------------ COMPACT ------------------
    public void CompactCargo() {
        // Move all items to the front and merge stacks
        var allItems = new List<Item>();
        foreach (var slot in cargoSlots) {
            if (!slot.IsEmpty) {
                allItems.Add(slot.item);
                slot.item = null;
            }
        }
        foreach (var item in allItems) {
            TryAddCargoItem(item, true); // merge stacks when possible
        }
        OnCargoSlotsUpdated?.Invoke();
    }

    // ------------------ SORT ------------------
    public void SortCargoByName(bool ascending) {
        CompactCargo();
        cargoSlots = cargoSlots
            .OrderBy(slot => slot.IsEmpty) // non-empty first
            .ThenBy(slot => slot.IsEmpty ? "" : ascending ? slot.item.data.displayName : string.Concat(slot.item.data.displayName.Reverse()))
            .ToList();
        OnCargoSlotsUpdated?.Invoke();
    }
    public void SortCargoByValue(bool ascending) {
        CompactCargo();
        cargoSlots = cargoSlots
            .OrderBy(slot => slot.IsEmpty)
            .ThenBy(slot => slot.IsEmpty ? 0 : ascending ? slot.item.StackValue() : -slot.item.StackValue())
            .ToList();
        OnCargoSlotsUpdated?.Invoke();
    }
    public void SortCargoByRarity(bool ascending) {
        CompactCargo();
        cargoSlots = cargoSlots
            .OrderBy(slot => slot.IsEmpty)
            .ThenBy(slot => slot.IsEmpty ? 0 : ascending ? (int)slot.item.data.rarity : -(int)slot.item.data.rarity)
            .ToList();
        OnCargoSlotsUpdated?.Invoke();
    }



    
    // ------------------ SAVE / LOAD  ------------------
    //Used in InventoryManager to save/load Inventory Data on disk
    public PlayerInventorySaveData ToSaveData() {
        var save = new PlayerInventorySaveData();

        // Cargo
        foreach (var slot in cargoSlots) {
            var item = slot.item;
            save.cargo.Add(new InventorySlotSave {
                itemID = item?.data.id ?? "",
                count  = item?.count ?? 0
            });
        }
        // Quick Slots
        foreach (var slot in quickSlots) {
            var item = slot.item;
            save.quickslots.Add(new InventorySlotSave {
                itemID = item?.data.id ?? "",
                count  = item?.count ?? 0
            });
        }
        return save;
    }
    public void LoadFromSaveData(PlayerInventorySaveData save, ItemDatabase db) {
        // Cargo
        for (int i = 0; i < cargoSlots.Count; i++) {
            if (i >= save.cargo.Count) {  
                cargoSlots[i].Clear();
                continue;
            }
            var s = save.cargo[i];
            if (string.IsNullOrEmpty(s.itemID)) {
                cargoSlots[i].Clear();
                continue;
            }
            var data = db.GetItemByID(s.itemID);
            cargoSlots[i].item = new Item(data, s.count);
        }
        // Quickslots
        for (int i = 0; i < quickSlots.Length; i++) {
            if (i >= save.quickslots.Count) {
                quickSlots[i].Clear();
                continue;
            }
            var s = save.quickslots[i];
            if (string.IsNullOrEmpty(s.itemID)) {
                quickSlots[i].Clear();
                continue;
            }
            var data = db.GetItemByID(s.itemID);
            quickSlots[i].item = new Item(data, s.count);
        }
        OnCargoSlotsUpdated?.Invoke();
        OnQuickSlotsUpdated?.Invoke();
    }  


    //NEW
    public void Clear() {
        foreach (var slot in cargoSlots)
            slot.Clear();
        foreach (var slot in quickSlots)
            slot.Clear();

        OnCargoSlotsUpdated?.Invoke();
        OnQuickSlotsUpdated?.Invoke();
    }    
}