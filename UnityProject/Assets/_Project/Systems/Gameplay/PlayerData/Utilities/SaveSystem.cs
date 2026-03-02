using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


// ------------------- GENERIC -----------------------------
[System.Serializable]
public class InventorySlotSave {
    public string itemID;  // null or "" = empty
    public int count;
}
[System.Serializable]
public class EquipmentSlotSave {
    public string equipmentID;  //null or "" = empty
}
// ------------------- PLAYER -----------------------------
[System.Serializable]
public class PlayerInventorySaveData {
    public List<InventorySlotSave> cargo = new();
    public List<InventorySlotSave> quickslots = new();
}
[System.Serializable]
public class PlayerEquipmentSaveData {
    public List<EquipmentSlotSave> weaponSlots = new();
    public List<EquipmentSlotSave> shipSystemSlots = new();
    public List<EquipmentSlotSave> storageSlots = new();
}
[System.Serializable]
public class PlayerMoneySaveData {
    public int credits;
}
// ------------------- OTHERS --------------------------------
[System.Serializable]
public class ShopInventorySaveData {
    public List<InventorySlotSave> stock = new();
}


//-------------------------------------------------------
public static class SaveSystem {
    private static void SaveToFile<T>(T data, string path) {
        if (data == null) return;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }
    private static T LoadFromFile<T>(string path) where T : class {
        if (!File.Exists(path)) return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    //------------------- PLAYER -----------------------
    public static void SavePlayerInventory(PlayerInventory data, string path) {
        if (data == null) return;
        SaveToFile(data.ToSaveData(), path);
    }
    public static PlayerInventorySaveData LoadPlayerInventory(string path) {
        return LoadFromFile<PlayerInventorySaveData>(path);
    }
    public static void SavePlayerEquipment(PlayerEquipment data, string path) {
        if (data == null) return;
        SaveToFile(data.ToSaveData(), path);
    }
    public static PlayerEquipmentSaveData LoadPlayerEquipment(string path) {
        return LoadFromFile<PlayerEquipmentSaveData>(path);
    }
    public static void SavePlayerMoney(PlayerMoney data, string path) {
        if (data == null) return;
        SaveToFile(data.ToSaveData(), path);
    }
    public static PlayerMoneySaveData LoadPlayerMoney(string path) {
        return LoadFromFile<PlayerMoneySaveData>(path);
    }


    // ------------------- OTHERS --------------------------------
    public static void SaveShopInventory(ShopInventory data, string path) {
        if (data == null) return;
        SaveToFile(data.ToSaveData(), path);
    }
    public static ShopInventorySaveData LoadShopInventory(string path) {
        return LoadFromFile<ShopInventorySaveData>(path);
    } 
}
