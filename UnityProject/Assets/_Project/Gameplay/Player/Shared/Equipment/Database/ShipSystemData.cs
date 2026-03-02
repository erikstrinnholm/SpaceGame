using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//“This describes what the system is — not how it works internally.”
// ==================== SYSTEM ============================
[CreateAssetMenu(menuName = "Equipment/ShipSystem")]
public class ShipSystemData : EquipmentData {

    [Header("Energy Cost")]
    public float energyPerSecond;
}
