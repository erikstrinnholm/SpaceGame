using UnityEngine;



[System.Serializable]
public class ShipSystem : Equipment {
    public ShipSystemData systemData;

    public ShipSystem(ShipSystemData data) : base(data) {
        this.systemData = data;
    }
}
