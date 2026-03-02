using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject {
    public string id;
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;
    public Rarity rarity;
    public int value;

    public ItemType type;
    public bool isStackable = false;
    public int maxStack = 1;
    public bool canDiscard = true;
    public bool canSell = true;
    public bool isConsumable = false;
    public float cooldown = 0f;

    //editor only?
    private void OnValidate() {
        if (!isStackable) maxStack = 1;
        if (maxStack < 1) maxStack = 1;
    }
}
