using UnityEngine;



[System.Serializable]
public abstract class Equipment {
    public EquipmentData data;

    protected Equipment(EquipmentData data) {
        this.data = data;
    }

    // 🔥 Common accessors for all equipment types
    public string Id          => data != null ? data.id : "";
    public string Name        => data != null ? data.displayName : "";
    public Sprite Icon        => data != null ? data.icon : null;
    public string Description => data != null ? data.description : "";
    public Rarity Rarity      => data != null ? data.rarity : Rarity.Common;
    public int Value          => data != null ? data.value : 0;
}
