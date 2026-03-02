using UnityEngine;
using System;

/**
Its responsibilities:
- Maintain the full ship energy pool
- Recharging
- Provide energy to weapons
- Shared by all ship systems
 */
public class EnergyManager : MonoBehaviour {
    public Action OnEnergyChanged;      // Notify listeners when energy changes
    
    [Header("Settings")]
    [SerializeField] private float rechargePerSecond = 20f;
    [SerializeField] public float maxEnergy = 100f;
    
    //Public Access
    public float MaxEnergy => maxEnergy;
    public float CurrentEnergy { get; private set; }
    public float Normalized => maxEnergy > 0f ? CurrentEnergy / maxEnergy : 0f;

    // Unity
    private void Awake() {
        CurrentEnergy = maxEnergy;
        NotifyChanged();
    }
    
    private void Update() {
        if (CurrentEnergy >= maxEnergy) return;
        float oldValue = CurrentEnergy;
        CurrentEnergy += rechargePerSecond * Time.deltaTime;
        CurrentEnergy = Mathf.Min(CurrentEnergy, maxEnergy);

        if (!Mathf.Approximately(oldValue, CurrentEnergy))
            NotifyChanged();
    }


    // Public API
    public bool HasEnergy(float amount) => CurrentEnergy >= amount;
    public bool Consume(float amount) {
        if (!HasEnergy(amount)) return false;
        CurrentEnergy -= amount;
        NotifyChanged();
        return true;
    }
    private void NotifyChanged() {
        OnEnergyChanged?.Invoke();
    }
}
