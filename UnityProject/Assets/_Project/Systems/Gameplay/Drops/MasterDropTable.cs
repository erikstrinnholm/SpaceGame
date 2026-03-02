using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDropTable : MonoBehaviour {
    [Header("Global Modifiers")]
    public float globalDropMultiplier = 1f;


    public void RollAndSpawn(ILootSource source) {
        var table = source.DropTable;
        if (table == null) return;

        for (int i = 0; i < table.rolls; i++) {
            DropEntry entry = RollEntry(table.entries);
            if (entry == null) continue;

            int amount = Mathf.RoundToInt(
                Random.Range(entry.minAmount, entry.maxAmount + 1)
                * globalDropMultiplier
            );

            Vector3 spawnPos = source.DropOrigin.position
                             + Random.insideUnitSphere * 1.5f;

            entry.loot.SpawnPickup(spawnPos, amount);
        }
    }

    DropEntry RollEntry(List<DropEntry> entries) {
        float total = 0f;
        foreach (var e in entries)
            total += e.weight;

        float roll = Random.value * total;

        foreach (var e in entries) {
            roll -= e.weight;
            if (roll <= 0f)
                return e;
        }
        return null;
    }
}