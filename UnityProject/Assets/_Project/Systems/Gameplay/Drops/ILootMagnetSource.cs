using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILootMagnetSource {
    Transform MagnetOrigin { get; }
    float MagnetRadius { get; }

    float PullSpeedAtDistance(float distance);
    float LootDistance { get; }

    bool CanLoot { get; }
}


