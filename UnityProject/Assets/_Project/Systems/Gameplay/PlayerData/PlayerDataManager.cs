using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



// OWNS THE PLAYER DATA

public class PlayerDataManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private EquipmentDatabase equipmentDatabase;

    [Header("Save Files")]
    [SerializeField] private string inventoryFile = "player_inventory.json";
    [SerializeField] private string equipmentFile = "player_equipment.json";
    //[SerializeField] private string moneyFile = "player_money.json";
    private string InventorySavePath => Path.Combine(Application.persistentDataPath, inventoryFile);
    private string EquipmentSavePath => Path.Combine(Application.persistentDataPath, equipmentFile);
    //private string MoneySavePath => Path.Combine(Application.persistentDataPath, moneyFile);

    // ---------------- PUBLIC DATA CLASSES ----------------------
    public PlayerInventory PlayerInventory { get; private set; }
    public PlayerEquipment PlayerEquipment { get; private set; }
    //public PlayerMoney PlayerMoney { get; private set; }    
    private int cargoSlotCount = 24;    //FIXED SIZE (Cargo Navigation will break if changed)


    // ------------------ INITIALIZATION ------------------
    private void Awake() {
        if (itemDatabase == null)
            Debug.LogError("InventoryManager: ItemDatabase reference missing!");
        if (equipmentDatabase == null)
            Debug.LogError("PlayerDataManager: EquipmentDatabase reference missing!");
        
        // FIX: CREATE PLAYERINVENTORY ONCE AND NEVER CHANGE IT, IT BREAKS THE LISTENERS
        PlayerInventory ??= new PlayerInventory(cargoSlotCount);
        PlayerEquipment ??= new PlayerEquipment();
        //PlayerMoney ??= new PlayerMoney(0);        
        LoadAll();        
    }


    // ------------------ ITEMS / EQUIPMENT CREATION -------------
    private void Start() {
        
        ClearEquipment();
        SaveEquipment();
        LoadEquipment();    

        //              KINETIC WEAPONS
        /*
        Equipment repeater = CreateEquipment("Repeater");
        Equipment autocannon = CreateEquipment("Autocannon");
        Equipment phalanx = CreateEquipment("Phalanx");
        Equipment railgun = CreateEquipment("Railgun");
        PlayerEquipment.TryAddIntoWeaponSlots(repeater);
        PlayerEquipment.TryAddIntoWeaponSlots(autocannon);                
        PlayerEquipment.TryAddIntoWeaponSlots(phalanx);    
        PlayerEquipment.TryAddIntoWeaponSlots(railgun);
        */



        // ---------------------------- MOSTLY DONE ----------------------------
        //              ENERGY WEAPONS P1
        /*
        Equipment laserBlaster = CreateEquipment("Laser Blaster"); 
        Equipment dualBlaster = CreateEquipment("Dual Blaster");
        Equipment quadBlaseter = CreateEquipment("Quad Blaster");
        PlayerEquipment.TryAddIntoWeaponSlots(laserBlaster);
        PlayerEquipment.TryAddIntoWeaponSlots(dualBlaster);
        PlayerEquipment.TryAddIntoWeaponSlots(quadBlaseter);
        */

        //              ENERGY WEAPONS P2
        /*
        Equipment chargeCannon = CreateEquipment("Charge Cannon");
        Equipment plasmaSlugger = CreateEquipment("Plasma Slugger");
        Equipment phaseEmitter = CreateEquipment("Phase Emitter");
        PlayerEquipment.TryAddIntoWeaponSlots(chargeCannon);
        PlayerEquipment.TryAddIntoWeaponSlots(plasmaSlugger);
        PlayerEquipment.TryAddIntoWeaponSlots(phaseEmitter);
        */
        //              BEAM WEAPONS P1
        /*
        Equipment miningLaser = CreateEquipment("Mining Laser");
        Equipment standardLaser = CreateEquipment("Standard Laser");
        Equipment longRangeLaser = CreateEquipment("Long-Range Laser");
        PlayerEquipment.TryAddIntoWeaponSlots(miningLaser);
        PlayerEquipment.TryAddIntoWeaponSlots(standardLaser);
        PlayerEquipment.TryAddIntoWeaponSlots(longRangeLaser);        
        */

        //              BEAM WEAPONS P2
        /*
        Equipment rampUpBeam = CreateEquipment("Ramp-Up Beam");
        Equipment disruptorBeam = CreateEquipment("Disruptor Beam");
        Equipment dualLasers = CreateEquipment("Dual Lasers");
        PlayerEquipment.TryAddIntoWeaponSlots(rampUpBeam);
        PlayerEquipment.TryAddIntoWeaponSlots(disruptorBeam);
        PlayerEquipment.TryAddIntoWeaponSlots(dualLasers);  
        */
        // ----------------------------------------------------------------

        //              MISSILE LAUNCHERS P1
        /*
        Equipment standardMissileLauncher = CreateEquipment("Standard Missile Launcher");
        Equipment specterMissileLauncher = CreateEquipment("Specter Missile Launcher");
        Equipment mineLauncher = CreateEquipment("Mine Launcher");
        PlayerEquipment.TryAddIntoWeaponSlots(standardMissileLauncher);
        PlayerEquipment.TryAddIntoWeaponSlots(specterMissileLauncher);
        PlayerEquipment.TryAddIntoWeaponSlots(mineLauncher);
        */

        //              MISSILE LAUNCHERS P2
        /*
        Equipment heavyMissileLauncher = CreateEquipment("Heavy Missile Launcher");
        Equipment techMissileLauncher = CreateEquipment("Tech Missile Launcher");
        Equipment heracyMissileLauncher = CreateEquipment("Heracy Missile Launcher");
        PlayerEquipment.TryAddIntoWeaponSlots(heavyMissileLauncher);
        PlayerEquipment.TryAddIntoWeaponSlots(techMissileLauncher);
        PlayerEquipment.TryAddIntoWeaponSlots(heracyMissileLauncher);
        */

        //              ONE OF EACH
        Equipment dualBlaster = CreateEquipment("Dual Blaster");
        PlayerEquipment.TryAddIntoWeaponSlots(dualBlaster); 

        Equipment repeater = CreateEquipment("Repeater");
        PlayerEquipment.TryAddIntoWeaponSlots(repeater);

        Equipment standardMissileLauncher = CreateEquipment("Standard Missile Launcher");
        PlayerEquipment.TryAddIntoWeaponSlots(standardMissileLauncher);


        SaveEquipment();
        LoadEquipment();
        
    }

    // ------------------ LOAD ----------------
    public void LoadAll() {
        LoadInventory();
        LoadEquipment();
        //LoadMoney();
    }
    public void LoadInventory() {
        var save = SaveSystem.LoadPlayerInventory(InventorySavePath);
        PlayerInventory.Clear();    //TEST
        if (save != null) PlayerInventory.LoadFromSaveData(save, itemDatabase);        
    }
    public void LoadEquipment() {
        var save = SaveSystem.LoadPlayerEquipment(EquipmentSavePath);
        PlayerEquipment.Clear();
        if (save != null) PlayerEquipment.LoadFromSaveData(save, equipmentDatabase);
    }
    /*
    public void LoadMoney() {
        var save = SaveSystem.LoadPlayerMoney(MoneySavePath);
        PlayerMoney.ResetCredits();
        if (save != null) PlayerMoney.LoadFromSaveData(save);
    }
    */
    
    // ------------------ SAVE ----------------
    public void SaveAll() {
        SaveInventory();
        SaveEquipment();
        //SaveMoney();
    }
    public void SaveInventory() {
        if (PlayerInventory == null) return;
        SaveSystem.SavePlayerInventory(PlayerInventory, InventorySavePath);        
    }
    public void SaveEquipment() {
        if (PlayerEquipment == null) return;
        SaveSystem.SavePlayerEquipment(PlayerEquipment, EquipmentSavePath);
        
    }
    /*
    public void SaveMoney() {
        if (PlayerMoney == null) return;
        SaveSystem.SavePlayerMoney(PlayerMoney, MoneySavePath); 
    }
    */

    // ------------------ CLEAR ----------------
    public void ClearAll() {
        ClearInventory();
        ClearEquipment();
        //ClearMoney();
    }
    public void ClearInventory() {
        PlayerInventory.Clear();
        SaveInventory();
    }
    public void ClearEquipment() {
        PlayerEquipment.Clear();
        SaveEquipment();
    }
    /*
    public void ClearMoney() {
        PlayerMoney.ResetCredits();
        SaveMoney();
    }
    */


    // ------------------ UI HELPER -------------
    public Item CreateItem(string itemId, int count = 1) {
        if (itemDatabase == null) return null;
        var data = itemDatabase.GetItemByID(itemId);
        if (data == null) {
            Debug.LogWarning($"PlayerDataManager: No item found with ID '{itemId}'");
            return null;
        }
        return new Item(data, count);
    }
    public Equipment CreateEquipment(string equipmentId) {
        if (equipmentDatabase == null) return null;
        var data = equipmentDatabase.GetEquipment(equipmentId);
        if (data == null) {
            Debug.LogWarning($"PlayerDataManager: No equipment found with ID '{equipmentId}'");
            return null;
        }
        Equipment equipment = data switch {
            WeaponData weaponData => new Weapon(weaponData),
            ShipSystemData shipSystemData => new ShipSystem(shipSystemData),
            _ => null
        };
        return equipment;
    }

}
