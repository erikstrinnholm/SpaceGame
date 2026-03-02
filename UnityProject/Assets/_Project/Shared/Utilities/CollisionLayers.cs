using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Layers answer “what am I?”
//Masks answer “what system cares about me?”

public static class CollisionLayers {
    // ===================== LAYERS =====================
    private static int? _player;
    public static int Player => _player ??= LayerMask.NameToLayer("Player");
    private static int? _enemy;
    public static int Enemy => _enemy ??= LayerMask.NameToLayer("Enemy");
    private static int? _asteroids;
    public static int Asteroids => _asteroids ??= LayerMask.NameToLayer("Asteroids");
    private static int? _environment;
    public static int Environment => _environment ??= LayerMask.NameToLayer("Environment");
    private static int? _playerProjectiles;
    public static int PlayerProjectiles => _playerProjectiles ??= LayerMask.NameToLayer("PlayerProjectiles");
    private static int? _enemyProjectiles;
    public static int EnemyProjectiles => _enemyProjectiles ??= LayerMask.NameToLayer("EnemyProjectiles");
    private static int? _missiles;
    public static int Missiles => _missiles ??= LayerMask.NameToLayer("Missiles");
    private static int? _drones;
    public static int Drones => _drones ??= LayerMask.NameToLayer("Drones");
    private static int? _loot;
    public static int Loot => _loot ??= LayerMask.NameToLayer("Loot");



    // ===================== GENERAL MASKS =====================
    // What PD systems should track as valid targets
    public static readonly LayerMask PointDefenseTargetMask =
        (1 << Missiles) |
        (1 << Drones);

    // ===================== PLAYER MASKS =====================
    public static readonly LayerMask PlayerMask         = 
        (1 << Player);
    // What player weapons can lock on to
    public static readonly LayerMask PlayerLockOnMask    = 
        (1 << Enemy) |
        (1 << Asteroids);
    // Player raycasting mask
    public static readonly LayerMask PlayerRaycastingMask =
        (1 << Enemy) |
        (1 << Environment) |
        (1 << Asteroids) |
        (1 << Drones);
    // What player-fired weapons can hit
    public static readonly LayerMask PlayerWeaponMask    =
        (1 << Enemy) |
        (1 << Asteroids) |
        (1 << Environment) |
        (1 << Missiles) |
        (1 << Drones);
    //
    public static readonly LayerMask PlayerLootMask     =
        (1 << Loot);    

    // ===================== ENEMY MASKS =====================
    public static readonly LayerMask EnemyMask          = 
        (1 << Enemy);
    // What enemy weapons can lock on to
    public static readonly LayerMask EnemyLockOnMask    = 
        (1 << Player) |
        (1 << Asteroids);
    // Enemy beam raycasting collision mask
    public static readonly LayerMask EnemyBeamCollisionMask =
        (1 << Player) |
        (1 << Environment) |
        (1 << Asteroids) |
        (1 << Missiles) |
        (1 << Drones);
    // What enemy-fired weapons can hit
    public static readonly LayerMask EnemyWeaponMask    =
        (1 << Player) |
        (1 << Asteroids) |
        (1 << Environment) |
        (1 << Missiles) |
        (1 << Drones);

    // ASTEROID IMPACT MASK
    public static readonly LayerMask GeneralImpactMask =
        (1 << Player) |
        (1 << Enemy) |
        (1 << Asteroids) |
        (1 << Environment);
}

