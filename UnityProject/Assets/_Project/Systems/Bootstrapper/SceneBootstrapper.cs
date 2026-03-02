using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Handles automatic scene setup:
// - Ensures persistent systems exist (CoreRoot, GameRoot)
// - Allows scene-specific configuration
// - Works if you start in any scene
public class SceneBootstrapper : MonoBehaviour {
    [Header("Persistent Prefabs")]
    [SerializeField] private GameObject coreRootPrefab;
    [SerializeField] private GameObject gameRootPrefab;
    [SerializeField] private GameObject shipRootPrefab;
    [SerializeField] private GameObject characterRootPrefab;

    private bool coreRootLoaded = false;
    private bool gameRootLoaded = false;
    private bool shipRootLoaded = false;
    private bool characterRootLoaded = false;

    private const string StartScene     = "StartScene";
    private const string TutorialScene  = "TutorialScene";          
    private const string HangarScene    = "HangarScene";
    

    private void Awake() {
        LoadCoreFeatures();

        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene) {
            case StartScene:
                break;
            case TutorialScene:
                LoadGameFeatures();
                LoadShipFeatures();
                break;
            case HangarScene:
                LoadGameFeatures();
                LoadCharacterFeatures();
                break;                
        }
        // Optional: you can add more scene-specific setup here (UI, lighting, etc.)
    }

    private void Start() {
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene) {
            case StartScene:
                CoreRoot.Instance.Audio.PlayMusic("Menu_Music");
                break;
            case TutorialScene:
                CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Ship);
                BindSceneContext();
                break;         
            case HangarScene:
                CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Character);
                break;
        }

    }


    // ========== SHARED BY ALL SCENES ==========
    private bool CoreRootExists() => FindObjectOfType<CoreRoot>() != null;
    private bool GameRootExists() => FindObjectOfType<GameRoot>() != null;
    private bool ShipRootExists() => FindObjectOfType<ShipRoot>() != null;
    private bool CharacterRootExists() => FindObjectOfType<CharacterRoot>() != null;

    private void LoadCoreFeatures() {
        if (coreRootLoaded) return; //if already loaded, skip
        if (!CoreRootExists() && coreRootPrefab != null) {
            GameObject core = Instantiate(coreRootPrefab);
            DontDestroyOnLoad(core);
            coreRootLoaded = true;
        }
    }
    private void LoadGameFeatures() {
        if (gameRootLoaded) return; //if already loaded, skip
        if (!GameRootExists() && gameRootPrefab != null) {
            GameObject gameRoot = Instantiate(gameRootPrefab);
            DontDestroyOnLoad(gameRoot);
            gameRootLoaded = true;
        }
    }
    private void LoadShipFeatures() {
        if (shipRootLoaded) return; //if already loaded, skip
        if (!ShipRootExists() && shipRootPrefab != null) {
            GameObject shipRoot = Instantiate(shipRootPrefab);
            DontDestroyOnLoad(shipRoot);
            shipRootLoaded = true;
        }
    }
    private void LoadCharacterFeatures() {
        if (characterRootLoaded) return; //if already loaded, skip
        if (!CharacterRootExists() && characterRootPrefab != null) {
            GameObject characterRoot = Instantiate(characterRootPrefab);
            DontDestroyOnLoad(characterRoot);
            characterRootLoaded = true;
        }
    }

#if UNITY_EDITOR
    private void OnApplicationQuit() {
    }
#endif


    private void BindSceneContext() {
        SceneContext context = FindObjectOfType<SceneContext>();
        if (context == null) {
            Debug.LogWarning("[SceneBootstrapper] No SceneContext found.");
            return;
        }
        ShipRoot.Instance.BindScene(context);
    }
}

