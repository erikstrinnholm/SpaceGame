using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipmentSlotType {
    Any,
    Weapon,
    ShipSystem
}

[System.Serializable]
public class EquipmentSlot {
    public EquipmentSlotType slotType = EquipmentSlotType.Any;

    public Weapon weapon;
    public ShipSystem shipSystem;

    public bool locked = false;
    public bool IsEmpty => weapon == null && shipSystem == null;


    public bool CanAccept(Equipment incoming) {
        if (locked) return false;
        if (!IsEmpty) return false;
        if (incoming == null || incoming.data == null) return false;

        return slotType switch {
            EquipmentSlotType.Weapon => incoming is Weapon,
            EquipmentSlotType.ShipSystem => incoming is ShipSystem,
            EquipmentSlotType.Any => true,
            _ => false
        };
    }
    public bool AddEquipment(Equipment incoming) {
        if (!CanAccept(incoming)) return false;
        ClearEquipment();

        if (incoming is Weapon w) {
            weapon = w;
        } 
        else if (incoming is ShipSystem s) {
            shipSystem = s;
        }
        return true;
    }    
    public bool RemoveEquipment() {
        if (locked) return false;
        ClearEquipment();
        return true;
    }
    public Equipment GetEquipment() {
        if (weapon != null) return weapon;
        if (shipSystem != null) return shipSystem;
        return null;
    }
    public void ClearEquipment() {
        weapon = null;
        shipSystem = null;
    }
}
