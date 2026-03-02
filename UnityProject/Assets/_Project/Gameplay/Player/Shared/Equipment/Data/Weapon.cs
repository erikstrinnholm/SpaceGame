using UnityEngine;


//When accessing subtype properties, only do so after checking type:

[System.Serializable]
public class Weapon : Equipment {

    // ================== BASE DATA ==================
    public WeaponData WeaponData => data as WeaponData;


    // ================== CONSTRUCTOR ==================
    public Weapon(WeaponData weaponData) : base(weaponData) {
        if (weaponData == null) {
            Debug.LogError("Weapon created with NULL WeaponData!");
        }
    }


    // ================== TYPE CHECKS ==================
    public bool IsKinetic         => data is KineticWeaponData;
    public bool IsEnergy          => data is EnergyWeaponData;
    public bool IsBeam            => data is BeamWeaponData;
    public bool IsMissileLauncher => data is MissileLauncherData;
    public bool IsDroneLauncher   => data is DroneLauncherData;

    // ------------------ SUBTYPE DATA ACCESS ------------------
    public KineticWeaponData    Kinetic         => data as KineticWeaponData;
    public EnergyWeaponData     Energy          => data as EnergyWeaponData;
    public BeamWeaponData       Beam            => data as BeamWeaponData;
    public MissileLauncherData  MissileLauncher => data as MissileLauncherData;
    public DroneLauncherData    DroneLauncher   => data as DroneLauncherData;
}
