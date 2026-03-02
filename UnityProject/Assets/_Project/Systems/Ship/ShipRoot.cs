using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRoot : MonoBehaviour
{
    public static ShipRoot Instance { get; private set; }

    [Header("Ship Gameplay Systems")]
    [SerializeField] private ThreatIndicatorSystem threatIndicators;
    [SerializeField] private EnergyManager energyManager;
    public ThreatIndicatorSystem Indicator => threatIndicators;
    public EnergyManager Energy => energyManager;


    private void Awake() {
        Singleton();
        ValidateReferences();
    }

    private void Singleton() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);        
    }


    private void ValidateReferences() {
        if (energyManager == null)
            Debug.Log("ShipRoot: missing EnergyManager reference!");
        if (threatIndicators == null)
            Debug.Log("ShipRoot: missing ThreatIndicatorSystem reference");
        // Add any necessary reference checks here
    }
    public void BindScene(SceneContext context) {
        threatIndicators.Bind(
            context.playerCamera,
            context.damageIndicatorRoot,
            context.missileIndicatorRoot
        );
    }    
}
