using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LootDefinition : ScriptableObject {
    public string id;
    public Sprite icon;
    public abstract void SpawnPickup(Vector3 position,int amount, Vector3? impactNormal = null);
}

