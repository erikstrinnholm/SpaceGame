using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 *              Shield      Normal      Armored
 *  ION         2.5x        0x          0x
 *  Energy      1.5x        1.0x        0.5x
 *  Kinetic     1.0x        1.25x       1.5x
 *  Impact      0.5x        1.0x        0.75x
 *  Explosive   0.5x        1.0x        0.75x
 */

// Damage modifier table
public static class DamageModifiers {
    private static readonly Dictionary<DamageType, Dictionary<TargetType, float>> modifiers =
        new Dictionary<DamageType, Dictionary<TargetType, float>>() {
            { DamageType.ION, new Dictionary<TargetType, float>
                {
                    { TargetType.Shield, 2.5f },
                    { TargetType.Normal, 0f },
                    { TargetType.Armored, 0f }
                }
            },
            { DamageType.Energy, new Dictionary<TargetType, float>
                {
                    { TargetType.Shield, 1.5f },
                    { TargetType.Normal, 1.0f },
                    { TargetType.Armored, 0.5f }
                }
            },
            { DamageType.Kinetic, new Dictionary<TargetType, float>
                {
                    { TargetType.Shield, 1.0f },
                    { TargetType.Normal, 1.25f },
                    { TargetType.Armored, 1.5f }
                }
            },
            { DamageType.Impact, new Dictionary<TargetType, float>
                {
                    { TargetType.Shield, 0.5f },
                    { TargetType.Normal, 1.0f },
                    { TargetType.Armored, 0.75f }
                }
            },
            { DamageType.Explosive, new Dictionary<TargetType, float>
                {
                    { TargetType.Shield, 0.5f },
                    { TargetType.Normal, 1.0f },
                    { TargetType.Armored, 0.75f }
                }
            }
        };


    public static float GetMultiplier(DamageType damageType, TargetType targetType)
    {
        if (modifiers.TryGetValue(damageType, out var targetDict))
        {
            if (targetDict.TryGetValue(targetType, out var multiplier))
            {
                return multiplier;
            }
        }
        return 1f; // fallback neutral
    }
}
