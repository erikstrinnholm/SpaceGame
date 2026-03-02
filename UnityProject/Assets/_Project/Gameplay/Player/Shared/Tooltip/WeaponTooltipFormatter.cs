using System.Text;
using UnityEngine;

public static class WeaponTooltipFormatter {
    //Reference
    public static GameColorDatabase Colors;


    public static string Build(WeaponData weapon) {
        var sb = new StringBuilder();
        AppendBase(sb, weapon);

        switch (weapon) {
            case KineticWeaponData kinetic:
                AppendKinetic(sb, kinetic);
                break;

            case EnergyWeaponData energy:
                AppendEnergy(sb, energy);
                break;

            case BeamWeaponData beam:
                AppendBeam(sb, beam);
                break;

            case MissileLauncherData missile:
                AppendMissileLauncher(sb, missile);
                break;

            case DroneLauncherData drone:
                AppendDroneLauncher(sb, drone);
                break;

            default:
                sb.AppendLine("Unknown weapon type.");
                break;
        }
        return sb.ToString().TrimEnd();
    }

    private static string ColorHex(DamageType type) => ColorUtility.ToHtmlStringRGB(Colors.GetDamageColor(type));
    private static string ColorHex(Color c) => ColorUtility.ToHtmlStringRGB(c);


    // ================= BASE =================
    private static void AppendBase(StringBuilder sb, WeaponData w) {
        if (w.firePointsCount > 1) {
            sb.AppendLine($"Mounts: {FormatFirePoints(w.firePointsCount)}");
        }
        sb.AppendLine();
    }
    private static string FormatFirePoints(int count) {
        return count switch {
            2 => "x2",
            4 => "x4",
            _ => "x1"
        };
    }
    private static void AppendLockOn(StringBuilder sb, LockOnMode mode, float lockTime) {
        switch (mode) {
            case LockOnMode.Optional:
                sb.AppendLine($"• {lockTime:0.0}s) LockOn · Mode: Optional");
                break;

            case LockOnMode.Required:
                sb.AppendLine($"• {lockTime:0.0}s) LockOn · Mode: Required");
                break;
        }
    }     



    private static void AppendKinetic(StringBuilder sb, KineticWeaponData w) {
        sb.AppendLine("<b>Weapon Stats</b>");
        string dmgHex = ColorHex(w.damageType);
        sb.AppendLine($"<color=#{dmgHex}>• {w.baseDamage} {w.damageType} Damage</color> · {w.fireRate:0.0} FireRate");
        sb.AppendLine($"• {w.magazineSize}-round magazine · {w.reloadTime:0.0}s reload");
        if(w.usesHeat) {
            string heatHex = ColorHex(Colors.heat);
            sb.AppendLine($"<color=#{heatHex}>• Generates {w.heatPerShot} Heat per shot</color>");
            sb.AppendLine($"<color=#{heatHex}><i>• Sustained fire may cause overheating</i></color>");
        }
    }
    private static void AppendEnergy(StringBuilder sb, EnergyWeaponData w) {
        sb.AppendLine("<b>Weapon Stats</b>");
        string dmgHex = ColorHex(w.damageType);
        sb.AppendLine($"<color=#{dmgHex}>• {w.baseDamage} {w.damageType} Damage</color> · {w.fireRate:0.0} FireRate");

        string energyHex = ColorHex(Colors.energy);
        sb.AppendLine($"<color=#{energyHex}>• Drains {w.energyPerShot} Energy per shot</color>");
        sb.AppendLine($"<color=#{energyHex}><i>• Sustained fire may cause system failure</i></color>");
        
        if(w.usesHeat) {
            string heatHex = ColorHex(Colors.heat);
            sb.AppendLine($"<color=#{heatHex}>• Generates {w.heatPerShot} Heat per shot</color>");
            sb.AppendLine($"<color=#{heatHex}><i>• Sustained fire may cause overheating</i></color>");
        }
    }
    private static void AppendBeam(StringBuilder sb, BeamWeaponData w) {
        sb.AppendLine("<b>Weapon Stats</b>");
        string dmgHex = ColorHex(w.damageType);
        float ticksPerSecond = w.tickRate > 0f ? 1f / w.tickRate : 0f;
        float dps = w.damagePerTick * ticksPerSecond;
        sb.AppendLine($"<color=#{dmgHex}>• {dps} {w.damageType} Damage (per second)</color> · {w.beamMaxRange} Range");

        string energyHex = ColorHex(Colors.energy);
        sb.AppendLine($"<color=#{energyHex}>• Drains {w.energyPerSecond} Energy (per second)</color>");
        sb.AppendLine($"<color=#{energyHex}><i>• Sustained fire may cause system failure</i></color>");
        
        if(w.usesHeat) {
            string heatHex = ColorHex(Colors.heat);
            sb.AppendLine($"<color=#{heatHex}>• Generates {w.heatPerSecond} Heat (per second)</color>");
            sb.AppendLine($"<color=#{heatHex}><i>• Sustained fire may cause overheating</i></color>");
        }
    }
    private static void AppendMissileLauncher(StringBuilder sb, MissileLauncherData w) {
        sb.AppendLine("<b>Launcher Stats</b>");
        AppendLockOn(sb, w.lockOnMode, w.lockOnTime);
        sb.AppendLine($"{w.fireRate:0.0} FireRate");
        sb.AppendLine($"• {w.magazineSize}-round magazine · {w.reloadTime:0.0}s reload");

        sb.AppendLine($"<b>{w.missileData.displayName}</b>");
        string dmgHex = ColorHex(w.missileData.damageType);
        sb.AppendLine($"<color=#{dmgHex}>• {w.missileData.damage} {w.missileData.damageType} Damage</color> · {w.missileData.radius} Explosion Radius");

        if (w.missileData.behaviors != null && w.missileData.behaviors.Count > 0)
            sb.AppendLine("• Special missile behavior");        
    }

    private static void AppendDroneLauncher(StringBuilder sb, DroneLauncherData w) {
        sb.AppendLine("<b>Launcher Stats</b>");
        AppendLockOn(sb, w.lockOnMode, w.lockOnTime);
        sb.AppendLine($"{w.fireRate:0.0} FireRate");
        sb.AppendLine($"• {w.magazineSize}-round magazine · {w.reloadTime:0.0}s reload");
        sb.AppendLine($"{w.maxActiveDrones} Max Active Drones");

        sb.AppendLine($"<b>{w.droneData.displayName}</b>");
        sb.AppendLine("TODO-ADD DRONE DATA");
    }
}
