using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PlayerEquipment {
    // ------------------ DATA ------------------
    public List<EquipmentSlot> weaponSlots;
    public List<EquipmentSlot> shipSystemSlots;
    public List<EquipmentSlot> storageSlots;
    public Action OnEquipmentUpdated;               // EVENTS (UI CAN SUBSCRIBE TO THIS)



    // ------------------ INITIALIZATION ------------------
    public PlayerEquipment() {
        weaponSlots = CreateSlots(4, EquipmentSlotType.Weapon);
        shipSystemSlots  = CreateSlots(6, EquipmentSlotType.ShipSystem);
        storageSlots = CreateSlots(5, EquipmentSlotType.Any);
    }
    private List<EquipmentSlot> CreateSlots(int count, EquipmentSlotType type) {
        var list = new List<EquipmentSlot>(count);
        for (int i = 0; i < count; i++) {
            list.Add(new EquipmentSlot { slotType = type });
        }
        return list;
    }


    // ------------------ VALIDATION ------------------
    private bool IsValidWeaponIndex(int index) => index >= 0 && index < weaponSlots.Count;
    private bool IsValidShipSystemIndex(int index) => index >= 0 && index < shipSystemSlots.Count;
    private bool IsValidStorageIndex(int index) => index >= 0 && index < storageSlots.Count;


    // ------------------ INSERT ------------------
    public bool InsertIntoWeaponSlot(int index, Weapon weapon) {
        if (!IsValidWeaponIndex(index)) return false;
        return weaponSlots[index].AddEquipment(weapon);
    }    
    public bool InsertIntoShipSystemSlot(int index, ShipSystem shipSystem) {
        if (!IsValidShipSystemIndex(index)) return false;
        return shipSystemSlots[index].AddEquipment(shipSystem);
    }    
    public bool InsertIntoStorageSlot(int index, Equipment equipment) {
        if (!IsValidStorageIndex(index)) return false;
        return storageSlots[index].AddEquipment(equipment);
    }

    // ------------------ GET ------------------
    public Weapon GetFromWeaponSlot(int index) {
        if (!IsValidWeaponIndex(index)) return null;
        var eq = weaponSlots[index].GetEquipment();
        return eq as Weapon;    // null if not a Weapon    
    }
    public ShipSystem GetFromShipSystemSlot(int index) {
        if (!IsValidShipSystemIndex(index)) return null;
        var eq = shipSystemSlots[index].GetEquipment();
        return eq as ShipSystem;    // null if not a ShipSystem    
    }
    public Equipment GetFromStorageSlot(int index) {
        return storageSlots[index].GetEquipment(); // storage accepts any Equipment    
    }


    // ------------------ TAKE (Get + Remove) ------------------
    public Weapon TakeFromWeaponSlot(int index) {
        if (!IsValidWeaponIndex(index)) return null;

        var slot = weaponSlots[index];
        var eq = slot.GetEquipment();

        if (eq is Weapon w) {
            slot.RemoveEquipment();
            return w;
        }
        return null;
    }
    public ShipSystem TakeFromShipSystemSlot(int index) {
        if (!IsValidShipSystemIndex(index)) return null;

        var slot = shipSystemSlots[index];
        var eq = slot.GetEquipment();

        if (eq is ShipSystem sys) {
            slot.RemoveEquipment();
            return sys;
        }
        return null;
    }
    public Equipment TakeFromStorageSlot(int index) {
        if (!IsValidStorageIndex(index)) return null;

        var slot = storageSlots[index];
        var eq = slot.GetEquipment();

        if (eq != null) {
            slot.RemoveEquipment();
        }
        return eq;
    }


    // ------------------ EMPTY CHECKS ------------------
    public bool IsWeaponSlotEmpty(int index) => IsValidWeaponIndex(index) && weaponSlots[index].IsEmpty;
    public bool IsShipSystemSlotEmpty(int index) => IsValidShipSystemIndex(index) && shipSystemSlots[index].IsEmpty;
    public bool IsStorageSlotEmpty(int index) => IsValidStorageIndex(index) && storageSlots[index].IsEmpty;


    // ------------------ TRY ADD (AUTO-ASSIGN) ------------------
    public bool TryAddIntoWeaponSlots(Equipment incoming) {
        foreach (EquipmentSlot slot in weaponSlots) {
            if (slot.AddEquipment(incoming)) return true;
        }
        return false;
    }
    public bool TryAddIntoShipSystemSlots(Equipment incoming) {
        foreach (EquipmentSlot slot in shipSystemSlots) {
            if (slot.AddEquipment(incoming)) return true;
        }
        return false;
    }
    public bool TryAddIntoStorageSlots(Equipment incoming) {
        foreach (EquipmentSlot slot in storageSlots) {
            if (slot.AddEquipment(incoming)) return true;
        }
        return false;
    }


    // ------------------ SAVE / LOAD  ------------------
    public PlayerEquipmentSaveData ToSaveData() {
        var save = new PlayerEquipmentSaveData();

        foreach (var slot in weaponSlots) {
            Equipment equipment = slot.GetEquipment();
            save.weaponSlots.Add(new EquipmentSlotSave {
                equipmentID = equipment?.Id ?? ""
            });
        }
        foreach (var slot in shipSystemSlots) {
            Equipment equipment = slot.GetEquipment();
            save.shipSystemSlots.Add(new EquipmentSlotSave {
                equipmentID = equipment?.Id ?? ""
            });            
        }
        foreach (var slot in storageSlots) {
            Equipment equipment = slot.GetEquipment();
            save.storageSlots.Add(new EquipmentSlotSave {
                equipmentID = equipment?.Id ?? ""
            });        
        }
        return save;
    }
    public void LoadFromSaveData(PlayerEquipmentSaveData save, EquipmentDatabase db) {
        //Weapons
        // -------- WEAPON SLOTS --------
        for (int i = 0; i < weaponSlots.Count; i++) {
            if (i >= save.weaponSlots.Count) {
                weaponSlots[i].RemoveEquipment();
                continue;
            }

            string id = save.weaponSlots[i].equipmentID;
            if (string.IsNullOrEmpty(id)) {
                weaponSlots[i].RemoveEquipment();
                continue;
            }

            var data = db.GetEquipment(id);
            if (data == null) {
                Debug.LogWarning($"EquipmentDatabase: Missing weapon '{id}'");
                weaponSlots[i].RemoveEquipment();
                continue;
            }

            // Create correct runtime object
            if (data is WeaponData wd)
                weaponSlots[i].AddEquipment(new Weapon(wd));
            else
                Debug.LogWarning($"Slot expects Weapon but '{id}' is not WeaponData.");
        }

        // -------- SHIP SYSTEM SLOTS --------
        for (int i = 0; i < shipSystemSlots.Count; i++) {
            if (i >= save.shipSystemSlots.Count) {
                shipSystemSlots[i].RemoveEquipment();
                continue;
            }

            string id = save.shipSystemSlots[i].equipmentID;
            if (string.IsNullOrEmpty(id)) {
                shipSystemSlots[i].RemoveEquipment();
                continue;
            }

            var data = db.GetEquipment(id);
            if (data == null) {
                Debug.LogWarning($"EquipmentDatabase: Missing system '{id}'");
                shipSystemSlots[i].RemoveEquipment();
                continue;
            }

            if (data is ShipSystemData sd)
                shipSystemSlots[i].AddEquipment(new ShipSystem(sd));
            else
                Debug.LogWarning($"Slot expects ShipSystem but '{id}' is not ShipSystemData.");
        }

        // -------- STORAGE SLOTS (ANY) --------
        for (int i = 0; i < storageSlots.Count; i++) {
            if (i >= save.storageSlots.Count) {
                storageSlots[i].RemoveEquipment();
                continue;
            }

            string id = save.storageSlots[i].equipmentID;
            if (string.IsNullOrEmpty(id)) {
                storageSlots[i].RemoveEquipment();
                continue;
            }

            var data = db.GetEquipment(id);
            if (data == null) {
                Debug.LogWarning($"Missing storage equipment '{id}'");
                storageSlots[i].RemoveEquipment();
                continue;
            }

            // Auto-detect type
            if (data is WeaponData wd)
                storageSlots[i].AddEquipment(new Weapon(wd));
            else if (data is ShipSystemData sd)
                storageSlots[i].AddEquipment(new ShipSystem(sd));
            else
                Debug.LogWarning($"Unknown equipment type for '{id}'.");
        }        
    }


    //NEW
    public void Clear() {
        foreach (var slot in weaponSlots)
            slot.ClearEquipment();
        foreach (var slot in shipSystemSlots)
            slot.ClearEquipment();
        foreach (var slot in storageSlots)
            slot.ClearEquipment();

        OnEquipmentUpdated?.Invoke();
    }    
}
