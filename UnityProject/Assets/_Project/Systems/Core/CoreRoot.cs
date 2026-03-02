using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Root object for all persistent, cross-scene systems.
/// Instantiated and owned by SceneBootstrapper.
/// </summary>
public class CoreRoot : MonoBehaviour {
    public static CoreRoot Instance { get; private set; }

    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AudioManager audioManagerPrefab;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private LoadingScreenUIController loadingScreen;


    // ---------- PUBLIC ACCESS ----------
    public InputManager Input => inputManager;
    public AudioManager Audio { get; private set; }
    public SceneLoader Loader => sceneLoader;
    public LoadingScreenUIController LoadingScreen => loadingScreen;

    // ---------- LIFECYCLE ----------
    private void Awake() {
        Singleton();
        InitializeAudio();              //AudioManager
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


    // ---------- INITIALIZATION ----------
    private void InitializeAudio() {
        if (Audio != null) return;
        Audio = Instantiate(audioManagerPrefab, transform);
    }   


    // ---------- VALIDATION ----------
    private void ValidateReferences() {
        if (inputManager == null)
            Debug.LogError("[CoreRoot] InputManager reference missing.");
    
        if (Audio == null)
            Debug.LogError("[CoreRoot] AudioManager reference missing.");
        
        if (sceneLoader == null)
            Debug.LogWarning("[CoreRoot] SceneLoader reference missing.");

        if (loadingScreen == null)
            Debug.LogWarning("[CoreRoot] LoadingScreen reference missing.");     
    } 
}

