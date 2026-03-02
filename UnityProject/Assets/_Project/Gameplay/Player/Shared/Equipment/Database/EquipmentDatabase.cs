using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Database/EquipmentDatabase")]
public class EquipmentDatabase : ScriptableObject {

    // ================== INSPECTOR LISTS ==================
    [Header("Ship Systems")]
    public List<ShipSystemData> shipSystems = new();

    [Header("Weapons")]
    public List<KineticWeaponData> kineticWeapons = new();
    public List<EnergyWeaponData> energyWeapons = new();
    public List<BeamWeaponData> beamWeapons = new();
    public List<MissileLauncherData> missileLaunchers = new();
    public List<DroneLauncherData> droneLaunchers = new();

    [Header("Ammo / Drones")]
    public List<MissileData> missiles = new();
    public List<DroneData> drones = new();


    // ================== INTERNAL CACHES =====================
    private Dictionary<string, EquipmentData> equipmentCache;
    private Dictionary<string, MissileData> missileCache;
    private Dictionary<string, DroneData> droneCache;


    // ================== PUBLIC LOOKUPS ==================
    // Base equipment lookup by ID.
    public EquipmentData GetEquipment(string id) {
        equipmentCache ??= BuildEquipmentCache();
        equipmentCache.TryGetValue(id, out var result);
        return result;
    }
    // Typed equipment lookup.
    public T GetEquipment<T>(string id) where T : EquipmentData {
        return GetEquipment(id) as T;
    }

    // -------- Convenience wrappers (optional but nice) --------
    public ShipSystemData GetShipSystem(string id) => GetEquipment<ShipSystemData>(id);
    public KineticWeaponData GetKineticWeapon(string id) => GetEquipment<KineticWeaponData>(id);
    public EnergyWeaponData GetEnergyWeapon(string id) => GetEquipment<EnergyWeaponData>(id);
    public BeamWeaponData GetBeamWeapon(string id) => GetEquipment<BeamWeaponData>(id);
    public MissileLauncherData GetMissileLauncher(string id) => GetEquipment<MissileLauncherData>(id);
    public DroneLauncherData GetDroneLauncher(string id) => GetEquipment<DroneLauncherData>(id);

    // -------- Missiles --------
    public MissileData GetMissile(string id) {
        missileCache ??= BuildMissileCache();
        missileCache.TryGetValue(id, out var result);
        return result;
    }
    
    // -------- Drones --------
    public DroneData GetDrone(string id) {
        droneCache ??= BuildDroneCache();
        droneCache.TryGetValue(id, out var result);
        return result;
    }

    // ================== CACHE BUILDERS ==================

    private Dictionary<string, EquipmentData> BuildEquipmentCache() {
        var dict = new Dictionary<string, EquipmentData>();

        void AddList<T>(IEnumerable<T> list) where T : EquipmentData {
            foreach (var item in list) {
                if (item == null) continue;

                if (string.IsNullOrWhiteSpace(item.id)) {
                    Debug.LogWarning($"{item.name} is missing an ID!");
                    continue;
                }

                if (dict.ContainsKey(item.id)) {
                    Debug.LogWarning($"Duplicate Equipment ID detected: {item.id}");
                    continue;
                }

                dict.Add(item.id, item);
            }
        }

        AddList(shipSystems);
        AddList(kineticWeapons);
        AddList(energyWeapons);
        AddList(beamWeapons);
        AddList(missileLaunchers);
        AddList(droneLaunchers);
        return dict;
    }

    private Dictionary<string, MissileData> BuildMissileCache() {
        var dict = new Dictionary<string, MissileData>();

        foreach (var missile in missiles) {
            if (missile == null) continue;

            if (string.IsNullOrWhiteSpace(missile.id)) {
                Debug.LogWarning($"{missile.name} is missing an ID!");
                continue;
            }

            if (!dict.TryAdd(missile.id, missile)) {
                Debug.LogWarning($"Duplicate Missile ID detected: {missile.id}");
            }
        }
        return dict;
    }
    private Dictionary<string, DroneData> BuildDroneCache() {
        var dict = new Dictionary<string, DroneData>();

        foreach (var drone in drones) {
            if (drone == null) continue;

            if (string.IsNullOrWhiteSpace(drone.id)) {
                Debug.LogWarning($"{drone.name} is missing an ID!");
                continue;
            }

            if (!dict.TryAdd(drone.id, drone)) {
                Debug.LogWarning($"Duplicate Drone ID detected: {drone.id}");
            }
        }
        return dict;
    }

    // ================== EDITOR SAFETY ==================
    private void OnValidate() {
        equipmentCache = null;
        missileCache = null;
        droneCache = null;
    }
}
