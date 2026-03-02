using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot {
    public Item item;                       // What the slot contains
    public bool locked = false;             // Slot is locked (cannot receive items)
    public bool IsEmpty => item == null;
    public bool IsFull => !IsEmpty && item.count >= item.data.maxStack;
    public int RemainingSpace => IsEmpty ? (item?.data.maxStack ?? 1) : (item.data.maxStack - item.count);


    public bool CanAccept(Item incoming) {
        if (locked || incoming == null || incoming.data == null) return false;
        if (IsEmpty) return true;

        // Non-empty slot: must be stackable and same item type, and not full
        return item.data.isStackable && item.data == incoming.data && item.count < item.data.maxStack;
    }
    public bool AddItem(Item incoming) {
        if (!CanAccept(incoming)) return false;

        // If empty slot, place new stack with proper clamping
        if (IsEmpty) {
            int toAdd = Mathf.Min(incoming.count, incoming.data.maxStack);
            item = new Item(incoming.data, toAdd);
            incoming.count -= toAdd;
            return incoming.count == 0;
        }

        int addAmount = Mathf.Min(RemainingSpace, incoming.count);
        item.count += addAmount;
        incoming.count -= addAmount;

        return incoming.count == 0;
    }
    public Item RemoveItem(int amount) {
        if (IsEmpty || amount <= 0) return null;

        int removeAmount = Mathf.Min(amount, item.count);
        Item removed = new Item(item.data, removeAmount);
        
        item.count -= removeAmount;
        if (item.count <= 0) item = null;

        return removed;
    }
    public void Clear() => item = null;
}

