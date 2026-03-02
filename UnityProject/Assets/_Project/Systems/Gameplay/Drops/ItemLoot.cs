using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Loot/Item")]
public class ItemLoot : LootDefinition {
    public ItemData itemData;
    public GameObject pickupPrefab;
    private int lootLayer = -1;

    [Header("Spread Settings")]
    public LootSpreadSettings spread;

    void OnEnable() {
        lootLayer = CollisionLayers.Loot;
        if (lootLayer == -1) {
            Debug.LogError("Loot layer does not exist!");
        }
    }    

    public override void SpawnPickup(Vector3 position, int amount, Vector3? impactNormal = null) {
        Vector3 spreadDir = CalculateSpreadDirection(
            impactNormal,
            spread.biasUp
        );

        Vector3 spawnPos = CalculateSpawnPosition(
            position,
            spreadDir,
            spread.minSpawnRadius,
            spread.maxSpawnRadius
        );

        var pickup = Instantiate(
            pickupPrefab,
            spawnPos,
            Quaternion.identity
        );

        SetLayerRecursively(pickup, lootLayer);
        pickup.GetComponent<Pickup>().Initialize(itemData, amount, spreadDir);
    }

    // ---------------- Helpers ----------------
    protected static Vector3 CalculateSpreadDirection(Vector3? impactNormal, float biasUp) {
        if (impactNormal.HasValue) {
            return (impactNormal.Value.normalized + Random.onUnitSphere * 0.5f).normalized;
        }
        return (Random.onUnitSphere + Vector3.up * biasUp).normalized;
    }

    protected static Vector3 CalculateSpawnPosition(
        Vector3 origin,
        Vector3 direction,
        float minRadius,
        float maxRadius
    ) {
        float radius = Random.Range(minRadius, maxRadius);
        return origin + direction * radius;
    }

    protected private void SetLayerRecursively(GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

}
