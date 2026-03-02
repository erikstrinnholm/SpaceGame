/**
 * This class is a modifed version of a class written by
 * by the youtuber "Brackeys" (code-license, is free to use)
 * source: https://www.youtube.com/watch?v=6OT43pvUyfY
 */

using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;



/// <summary>
/// Description Here
/// </summary>
public class AudioManager : MonoBehaviour {
    [System.Serializable]
    public class Sound {
        public string name;
        public AudioClip clip;
        public bool loop;
        public AudioMixerGroup output;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        [Range(0f, 1f)] public float spatialBlend = 0;
        [HideInInspector] public AudioSource source;
    }
    
    [System.Serializable]
    public class FootstepSurface {
        public string surfaceName;
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public AudioMixerGroup output;
    }    


    // ======= INSPECTOR =======
    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("General")]
    [SerializeField] private Sound[] music;
    [SerializeField] private Sound[] ui;
    [SerializeField] private Sound[] soundEffect;

    [Header("CharacterView")]
    [SerializeField] private FootstepSurface[] footsteps;       //REFERENCED
    [SerializeField] private Sound[] characterWeapons;

    [Header("ShipView")]
    [SerializeField] private Sound[] shipWeapons;


    [Header("Sound Library")]
    [SerializeField] private Sound[] energyWeapons;             //OK
    [SerializeField] private Sound[] beamWeapons;               //OK
    [SerializeField] private Sound[] missileLaunchers;          //OK
    [SerializeField] private Sound[] droneLaunchers;

    [SerializeField] private Sound[] weapons;
    [SerializeField] private Sound[] impact;
    [SerializeField] private Sound[] targets;


    // ======= RUNTIME =========
    private Dictionary<string, Sound> soundDictionary;
    private Dictionary<string, FootstepSurface> footstepDictionary;
    private Dictionary<string, int> lastFootstepIndex;
    private AudioSource oneShotSource;


    // ------ UNITY ------
    private void Awake () {
        BuildSoundDictionary();
        BuildFootstepDictionary();
        RestoreMixerVolumes();

        // Global one-shot source
        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.spatialBlend = 0f;
    }




    // ===== DICTIONARIES ======
    private void BuildSoundDictionary() {
        soundDictionary = new Dictionary<string, Sound>();

        AddCategory(music);             //fixed
        AddCategory(ui);                //fixed
        AddCategory(soundEffect);       //fixed
        AddCategory(characterWeapons);  //fixed
        AddCategory(shipWeapons);       //ongoing

        AddCategory(energyWeapons);
        AddCategory(beamWeapons);
        AddCategory(missileLaunchers);
        AddCategory(droneLaunchers);
        //===========================
        AddCategory(weapons);
        AddCategory(impact);
        AddCategory(targets);
        //expand later
    }
    private void AddCategory(Sound[] category) {
        if (category == null) return;

        foreach (Sound s in category) {
            // Check for duplicate keys before registering
            if (soundDictionary.ContainsKey(s.name)) {
                Debug.LogWarning($"[AudioManager] Duplicate sound name: {s.name}. Skipping.");
                continue;
            }

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;

            soundDictionary.Add(s.name, s);
        }        
    }
    private void BuildFootstepDictionary() {
        footstepDictionary = new Dictionary<string, FootstepSurface>();
        lastFootstepIndex = new Dictionary<string, int>();

        foreach (var surface in footsteps) {
            if (footstepDictionary.ContainsKey(surface.surfaceName)) {
                Debug.LogWarning($"Duplicate footstep surface: {surface.surfaceName}");
                continue;
            }
            footstepDictionary.Add(surface.surfaceName, surface);
            lastFootstepIndex.Add(surface.surfaceName, -1);
        }
    }




    // ===================================================================
    #region BASIC PLAYBACK API
    public void Play(string name) {
        if (soundDictionary.TryGetValue(name, out var s)) s.source.Play();
        else
            Debug.Log("AudioManager missing: " + name);
    }
    public void Pause(string name) {
        if (soundDictionary.TryGetValue(name, out var s)) s.source.Pause();
    }
    public void Stop(string name) {
        if (soundDictionary.TryGetValue(name, out var s)) s.source.Stop();
    }
    public bool IsPlaying(string name) {
        if (soundDictionary.TryGetValue(name, out var s)) return s.source.isPlaying;
        return false;
    }
    #endregion
    // ===================================================================


    // ===================================================================
    #region MUSIC PLAYER API
    private Sound currentMusic;
    public void PlayMusic(string name) {
        if (!soundDictionary.TryGetValue(name, out var newMusic))
            return;

        if (currentMusic != null && currentMusic.source.isPlaying)
            currentMusic.source.Stop();
        
        currentMusic = newMusic;
        currentMusic.source.loop = true;
        currentMusic.source.Play();
    }
    public void StopMusic() {
        if (currentMusic != null) {
            currentMusic.source.Stop();
            currentMusic = null;
        }
    }
    public void PauseMusic() {
        if (currentMusic != null) {
            currentMusic.source.Pause();
        }
    }
    public void ResumeMusic() {
        if (currentMusic != null && !currentMusic.source.isPlaying) {
            currentMusic.source.UnPause();
        }
    }    
    #endregion
    // ===================================================================


    // ===================================================================
    #region VOLUME CONTROL
    private void RestoreMixerVolumes(){
        if (audioMixer == null) return;
        foreach (AudioGroups group in Enum.GetValues(typeof(AudioGroups))) {
            string groupString = group.ToKey();
            if (!PlayerPrefs.HasKey(groupString)) continue;

            float value = PlayerPrefs.GetFloat(groupString);
            value = Mathf.Clamp(value, 0.0001f, 1f);

            float db = Mathf.Log10(value) * 20f;
            audioMixer.SetFloat(groupString, db);
        }
    }    
    public void SetVolume(string soundGroupName, float value) {
        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(clampedValue) * 20f; //sets mixer value logarithmically
        bool found = audioMixer.SetFloat(soundGroupName, dB);
        PlayerPrefs.SetFloat(soundGroupName, value);
        PlayerPrefs.Save();
    }
    public float GetVolume(string soundGroupName) {
        return PlayerPrefs.GetFloat(soundGroupName, 1f);
    }    
    #endregion
    // ===================================================================


    // ===================================================================
    #region FOOTSTEPS
    public void PlayRandomFootstep( string surfaceName, float pitchVariation = 0.05f, float volumeVariation = 0.05f) {
        if (!footstepDictionary.TryGetValue(surfaceName, out var surface)) {
            Debug.LogWarning($"Footstep surface '{surfaceName}' not found.");
            return;
        }

        if (surface.clips == null || surface.clips.Length == 0)
            return;

        int lastIndex = lastFootstepIndex[surfaceName];
        int index;

        do {
            index = UnityEngine.Random.Range(0, surface.clips.Length);
        }
        while (index == lastIndex && surface.clips.Length > 1);

        lastFootstepIndex[surfaceName] = index;
        oneShotSource.outputAudioMixerGroup = surface.output;
        oneShotSource.pitch = surface.pitch + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
        float finalVolume = surface.volume + UnityEngine.Random.Range(-volumeVariation, volumeVariation);
        oneShotSource.PlayOneShot(surface.clips[index], finalVolume);
    }
    public void PlayRandomFootstepAtPosition(string surfaceName, Vector3 position, float pitchVariation = 0.05f, float volumeVariation = 0.05f) {     
        if (!footstepDictionary.TryGetValue(surfaceName, out var surface)) return;
        if (surface.clips == null || surface.clips.Length == 0) return;

        int lastIndex = lastFootstepIndex[surfaceName];
        int index;

        do {
            index = UnityEngine.Random.Range(0, surface.clips.Length);
        }
        while (index == lastIndex && surface.clips.Length > 1);

        lastFootstepIndex[surfaceName] = index;

        GameObject temp = new GameObject("TempFootstepAudio");
        temp.transform.position = position;

        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = surface.clips[index];
        src.outputAudioMixerGroup = surface.output;
        src.spatialBlend = 1f;
        src.minDistance = 2f;
        src.maxDistance = 25f;

        src.pitch = surface.pitch + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
        float finalVolume = surface.volume + UnityEngine.Random.Range(-volumeVariation, volumeVariation);
        src.PlayOneShot(src.clip, finalVolume);
        Destroy(temp, src.clip.length);
    }
    #endregion
    // ===================================================================



    // ===================================================================
    #region UNSORTED    
    //Creates an AudioSource on the called GameObject
    //Called is responsible for Play/Stop
    public AudioSource AttachAudioSource(GameObject owner, string name) {
        if (!soundDictionary.TryGetValue(name, out var s)) {
            Debug.LogWarning($"AudioManager: Sound '{name}' not found.");
            return null;
        }

        // Make new AudioSource on the caller
        var src = owner.AddComponent<AudioSource>();
        src.clip = s.clip;
        src.loop = s.loop;
        src.outputAudioMixerGroup = s.output;   // 🔊 goes through mixer
        src.volume = s.volume;
        src.pitch = s.pitch;
        src.spatialBlend = s.spatialBlend;

        // Optional — distance defaults
        src.minDistance = 2f;
        src.maxDistance = 30f;

        return src;
    }
    /*
    Example Usage
    private AudioSource hum;

    private void Start() {
        hum = AudioManager.Instance.GrabAudio(gameobject, "MachineHum");
        if (hum != null) {
            hum.spatialBlend = 1f;
            hum.minDistance = 10f;
            hum.maxDistance = 100f;
            hum.Play();
        }
    }

    private void OnDisable() => hum?.Stop();
    */

    #endregion
}