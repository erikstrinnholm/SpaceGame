using UnityEngine;

[CreateAssetMenu(fileName = "DamageVFXLibrary", menuName = "Combat/Damage VFX Library")]
public class DamageVFXLibrary : ScriptableObject {
    [Header("Default Effect")]
    [SerializeField] private DamageVFX defaultEffect;

    [Header("ION Effects")]
    [SerializeField] private DamageVFX ionAgainstShield;
    [SerializeField] private DamageVFX ionAgainstNormal;
    [SerializeField] private DamageVFX ionAgainstArmored;

    [Header("Energy Effects")]
    [SerializeField] private DamageVFX energyAgainstShield;
    [SerializeField] private DamageVFX energyAgainstNormal;
    [SerializeField] private DamageVFX energyAgainstArmored;

    [Header("Kinetic Effects")]
    [SerializeField] private DamageVFX kineticAgainstShield;
    [SerializeField] private DamageVFX kineticAgainstNormal;
    [SerializeField] private DamageVFX kineticAgainstArmored;

    [Header("Impact Effects")]
    [SerializeField] private DamageVFX impactAgainstShield;
    [SerializeField] private DamageVFX impactAgainstNormal;
    [SerializeField] private DamageVFX impactAgainstArmored;

    [Header("Explosive Effects")]
    [SerializeField] private DamageVFX explosiveAgainstShield;
    [SerializeField] private DamageVFX explosiveAgainstNormal;
    [SerializeField] private DamageVFX explosiveAgainstArmored;

    /// <summary>
    /// Returns the DamageVFX (VFX + audio) for the given damage vs target.
    /// Falls back to defaultEffect if none assigned.
    /// </summary>
    public DamageVFX GetEffect(DamageType damageType, TargetType targetType)
    {
        DamageVFX effect = damageType switch
        {
            DamageType.ION => targetType switch
            {
                TargetType.Shield => ionAgainstShield,
                TargetType.Normal => ionAgainstNormal,
                TargetType.Armored => ionAgainstArmored,
                _ => defaultEffect
            },
            DamageType.Energy => targetType switch
            {
                TargetType.Shield => energyAgainstShield,
                TargetType.Normal => energyAgainstNormal,
                TargetType.Armored => energyAgainstArmored,
                _ => defaultEffect
            },
            DamageType.Kinetic => targetType switch
            {
                TargetType.Shield => kineticAgainstShield,
                TargetType.Normal => kineticAgainstNormal,
                TargetType.Armored => kineticAgainstArmored,
                _ => defaultEffect
            },
            DamageType.Impact => targetType switch
            {
                TargetType.Shield => impactAgainstShield,
                TargetType.Normal => impactAgainstNormal,
                TargetType.Armored => impactAgainstArmored,
                _ => defaultEffect
            },
            DamageType.Explosive => targetType switch
            {
                TargetType.Shield => explosiveAgainstShield,
                TargetType.Normal => explosiveAgainstNormal,
                TargetType.Armored => explosiveAgainstArmored,
                _ => defaultEffect
            },
            _ => defaultEffect
        };

        return effect != null ? effect : defaultEffect;
    }
}
