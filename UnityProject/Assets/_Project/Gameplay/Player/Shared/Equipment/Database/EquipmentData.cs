using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ==================== GENERAL EQUIPMENT ============================
public abstract class EquipmentData : ScriptableObject {
    public string id;                               
    public string displayName;                      
    public Sprite icon;                             
    [TextArea] public string description;           
    public Rarity rarity;                           
    public int value;                               
}
