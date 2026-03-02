using UnityEngine;
using System.Collections.Generic;
using System.IO;


[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject {
    public List<ItemData> itemsData;
    private Dictionary<string, ItemData> cache;

    // ----------------- PUBLIC API ----------------------
    public ItemData GetItemByID(string id) {
        cache ??= BuildCache();
        return cache.TryGetValue(id, out var data) ? data : null;
    }

     // ----------------- INTERNAL ----------------------
    private Dictionary<string, ItemData> BuildCache() {
        var dict = new Dictionary<string, ItemData>();
        foreach (var itemData in itemsData)
            if (!dict.ContainsKey(itemData.id))
                dict.Add(itemData.id, itemData);
        return dict;
    }
}