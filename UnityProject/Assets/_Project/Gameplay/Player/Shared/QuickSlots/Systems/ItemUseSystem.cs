using UnityEngine;

public static class ItemUseSystem {

    // Executes the effect of an item.
    // Returns true if the item was successfully used.
    public static bool TryUse(Item item) {
        if (item == null || item.data == null) return false;

        switch (item.data.id) {
            case "Repair Kit":
                return UseRepairKit();

            default:
                Debug.LogWarning($"[ItemUseSystem] No use behavior defined for item '{item.data.id}'");
                return false;
        }
    }

    // -------- ITEM IMPLEMENTATIONS --------
    private static bool UseRepairKit() {
        Debug.Log("Used Repair Kit - (Placeholder Effect)");
        return true;
    }

    /*
    private static bool UseSmallMedkit(GameObject user) {
        var health = user.GetComponent<PlayerHealth>();
        if (health == null || health.IsFull)
            return false;

        health.Heal(25);
        return true;
    }
    private static bool UseShieldBoost(GameObject user) {
        var shields = user.GetComponent<PlayerShields>();
        if (shields == null)
            return false;

        shields.AddTemporaryBoost(50, 10f);
        return true;
    }
    private static bool DeployRepairDrone(GameObject user) {
        var droneSpawner = user.GetComponent<DroneSpawner>();
        if (droneSpawner == null)
            return false;

        droneSpawner.SpawnRepairDrone();
        return true;
    }
    */
}
