using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoot : MonoBehaviour {
    public static CharacterRoot Instance { get; private set; }

    // ---------- LIFECYCLE ----------
    private void Awake() {
        Singleton();
    }
    private void Singleton() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);        
    }
}
