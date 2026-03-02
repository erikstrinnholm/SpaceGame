using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Root object for all gameplay-session systems.
/// Spawned by SceneBootstrapper when entering a game scene.
/// </summary>
public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }

    [Header("Shared Gameplay Systems")]
    [SerializeField] private PlayerDataManager playerData;                  //GAME
    [SerializeField] private MasterDropTable masterDropTable;               //GAME
    [SerializeField] private ExplosionManager explosionManager;
    

    // ---------- PUBLIC ACCESS ----------
    public PlayerDataManager PlayerData => playerData;
    public MasterDropTable Drop => masterDropTable;
    public ExplosionManager Explosion => explosionManager;


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
        if (masterDropTable == null) {
            Debug.LogError("[GameRoot] MasterDropTable reference missing.");
        }
    }
}
