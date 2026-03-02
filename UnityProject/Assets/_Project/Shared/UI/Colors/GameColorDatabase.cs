using UnityEngine;

[CreateAssetMenu(menuName = "UI/Color Database")]
public class GameColorDatabase : ScriptableObject {
    public Color fallback;

    [Header("Damage Types")]
    public Color energy;
    public Color ion;
    public Color kinetic;
    public Color impact;
    public Color explosive;

    [Header("Rarity")]
    public Color common;
    public Color uncommon;
    public Color rare;
    public Color epic;
    public Color legendary;

    [Header("Heat")]
    public Color heat;
    public Color overheat;

    [Header("UI")]
    public Color warning;
    public Color positive;

    public Color GetDamageColor(DamageType type) {
        return type switch {
            DamageType.Energy     => energy,
            DamageType.ION        => ion,
            DamageType.Kinetic    => kinetic,
            DamageType.Impact     => impact,
            DamageType.Explosive  => explosive,
            _                     => fallback
        };
    }

    public Color GetRarityColor(Rarity rarity) {
        return rarity switch {
            Rarity.Common         => common,
            Rarity.Uncommon       => uncommon,
            Rarity.Rare           => rare,
            Rarity.Epic           => epic,
            Rarity.Legendary      => legendary,
            _                     => fallback
        };
    }
}
