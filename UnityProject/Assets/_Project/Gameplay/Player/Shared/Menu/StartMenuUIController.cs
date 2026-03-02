using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartMenuUIController : MonoBehaviour, IStartMenuActions, IOptionsMenuActions
{
    [Header("Title")]
    [SerializeField] private GameObject gameTitle;
    [Header("View")]
    [SerializeField] private MenuUI menuUI;

    // ===== Audio Constants =====
    private const int MAX_SLIDER_VALUE = 20;
    private const float MAX_AUDIO_VALUE = 1f;
    private const float MIN_AUDIO_VALUE = 0.0001f;
    private const float DEFAULT_VOLUME = 0.75f;


    // --------- Unity lifecycle ----------
    private void Start() {
        menuUI.Bind(this, null, this);
        menuUI.OpenMenu(MenuID.Main);
        gameTitle.SetActive(true);

        if (CoreRoot.Instance.Input != null) {
            CoreRoot.Instance.Input.OnMenuMouseMove += OnMouseMove;
            CoreRoot.Instance.Input.OnMenuNavigate += OnNavigate;
        }
    }
    private void OnDestroy() {
        if (CoreRoot.Instance.Input != null) {
            CoreRoot.Instance.Input.OnMenuMouseMove -= OnMouseMove;
            CoreRoot.Instance.Input.OnMenuNavigate -= OnNavigate;
        }        
    }


    // ---------- Input Actions -----------
    private void OnMouseMove(Vector2 navigate) {
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
    private void OnNavigate(Vector2 navigate) {
        if (EventSystem.current.currentSelectedGameObject == null)
            SetSelectedFirst(menuUI);
    } 
    private void SetSelectedFirst(MenuUI ui) {
        GameObject first = ui.GetFirstSelectableOfActiveMenu();
        if (first != null)
            EventSystem.current.SetSelectedGameObject(first);
    }


    // ================= START MENU =================
    public void PlayGame() {
        CoreRoot.Instance.Loader.LoadScene("TutorialScene");
    }
    public void OpenOptions() {
        menuUI.OpenMenu(MenuID.Options);
        gameTitle.SetActive(false);
    }
    public void OpenAbout() {
        menuUI.OpenMenu(MenuID.About);
        gameTitle.SetActive(false);
    }
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    // ================= ABOUT =================
    public void CloseAbout() {
        menuUI.OpenMenu(MenuID.Main);
        gameTitle.SetActive(true);
    }

    // ================= OPTIONS =================
    public void OpenGameplay() => menuUI.OpenMenu(MenuID.Gameplay);
    public void OpenAudio() {
        menuUI.OpenMenu(MenuID.Audio);        
        InitializeAudio();        
    }
    public void OpenVideo() => menuUI.OpenMenu(MenuID.Video);
    public void OpenControls() => menuUI.OpenMenu(MenuID.Controls);
    public void CloseOptions() {
        menuUI.OpenMenu(MenuID.Main);
        gameTitle.SetActive(true);
    }

    // ================= GAMEPLAY =================
    public void CloseGameplay() => menuUI.OpenMenu(MenuID.Options);


    // ================= AUDIO =================
    public void OnMasterVolumeChanged(float value) => ApplySlider(AudioGroups.Master, value);
    public void OnMusicVolumeChanged(float value) => ApplySlider(AudioGroups.Music, value);
    public void OnUIVolumeChanged(float value) => ApplySlider(AudioGroups.UI, value);
    public void OnSFXVolumeChanged(float value) => ApplySlider(AudioGroups.SFX, value);
    public void OnDialogueVolumeChanged(float value) => ApplySlider(AudioGroups.Dialogue, value);
    public void ResetDefaultAudio() {
        int defaultSlider = Mathf.RoundToInt(DEFAULT_VOLUME * MAX_SLIDER_VALUE);
        ApplySlider(AudioGroups.Master, defaultSlider);
        ApplySlider(AudioGroups.Music, defaultSlider);
        ApplySlider(AudioGroups.UI, defaultSlider);
        ApplySlider(AudioGroups.SFX, defaultSlider);
        ApplySlider(AudioGroups.Dialogue, defaultSlider);
        PlayerPrefs.Save();
    }
    public void CloseAudio() => menuUI.OpenMenu(MenuID.Options);
    //private helpers
    private void InitializeAudio() {
        InitializeGroup(AudioGroups.Master);
        InitializeGroup(AudioGroups.Music);
        InitializeGroup(AudioGroups.UI);
        InitializeGroup(AudioGroups.SFX);
        InitializeGroup(AudioGroups.Dialogue);
    }
    private void InitializeGroup(AudioGroups group) {
        string groupString = GlobalEnums.ToKey(group);
        float normalized = PlayerPrefs.GetFloat(groupString, DEFAULT_VOLUME);
        int sliderValue = Mathf.RoundToInt(normalized * MAX_SLIDER_VALUE);
        ApplySlider(group, sliderValue);
    }    
    private void ApplySlider(AudioGroups group, float sliderValue) {
        int intValue = Mathf.RoundToInt(sliderValue);
        float normalized = intValue / (float)MAX_SLIDER_VALUE;
        float audioValue = Mathf.Lerp(MIN_AUDIO_VALUE, MAX_AUDIO_VALUE, normalized);

        string groupString = GlobalEnums.ToKey(group);

        CoreRoot.Instance.Audio.SetVolume(groupString, audioValue);
        PlayerPrefs.SetFloat(groupString, normalized);
        menuUI.SetAudioSlider(group, intValue, MAX_SLIDER_VALUE);
    }



    // ================= VIDEO =================
    public void CloseVideo() => menuUI.OpenMenu(MenuID.Options);


    // ================= CONTROLS =================
    public void CloseControls() => menuUI.OpenMenu(MenuID.Options);

}







//Old Audio
/*
    public void ResetDefaultAudio() {
        int defaultSliderValue = Mathf.RoundToInt(DEFAULT_VOLUME * MAX_SLIDER_VALUE);

        // Master
        masterSlider.SetValueWithoutNotify(defaultSliderValue);
        HandleVolumeChange(masterSlider, masterAudioGroup, masterValueText, defaultSliderValue);
        PlayerPrefs.SetFloat(masterAudioGroup, DEFAULT_VOLUME);

        // Music
        musicSlider.SetValueWithoutNotify(defaultSliderValue);
        HandleVolumeChange(musicSlider, musicAudioGroup, musicValueText, defaultSliderValue);
        PlayerPrefs.SetFloat(musicAudioGroup, DEFAULT_VOLUME);

        // Ambiance
        ambianceSlider.SetValueWithoutNotify(defaultSliderValue);
        HandleVolumeChange(ambianceSlider, ambianceAudioGroup, ambianceValueText, defaultSliderValue);
        PlayerPrefs.SetFloat(ambianceAudioGroup, DEFAULT_VOLUME);

        // SFX
        sfxSlider.SetValueWithoutNotify(defaultSliderValue);
        HandleVolumeChange(sfxSlider, sfxAudioGroup, sfxValueText, defaultSliderValue);
        PlayerPrefs.SetFloat(sfxAudioGroup, DEFAULT_VOLUME);

        // Dialogue
        dialogueSlider.SetValueWithoutNotify(defaultSliderValue);
        HandleVolumeChange(dialogueSlider, dialogueAudioGroup, dialogueValueText, defaultSliderValue);
        PlayerPrefs.SetFloat(dialogueAudioGroup, DEFAULT_VOLUME);

        // Save all changes
        PlayerPrefs.Save();
    }
    public void OnMasterVolumeChanged(float value) => HandleVolumeChange(masterSlider, masterAudioGroup, masterValueText, value);
    public void OnMusicVolumeChanged(float value) => HandleVolumeChange(musicSlider, musicAudioGroup, musicValueText, value);
    public void OnAmbianceVolumeChanged(float value) => HandleVolumeChange(ambianceSlider, ambianceAudioGroup, ambianceValueText, value);
    public void OnSFXVolumeChanged(float value) => HandleVolumeChange(sfxSlider, sfxAudioGroup, sfxValueText, value);
    public void OnDialogueVolumeChanged(float value) => HandleVolumeChange(dialogueSlider, dialogueAudioGroup, dialogueValueText, value);
    // Private methods
    private void InitializeAudioSliders() {
        SetSliderValue(masterSlider, masterAudioGroup, masterValueText);
        SetSliderValue(musicSlider, musicAudioGroup, musicValueText);
        SetSliderValue(ambianceSlider, ambianceAudioGroup, ambianceValueText);
        SetSliderValue(sfxSlider, sfxAudioGroup, sfxValueText);
        SetSliderValue(dialogueSlider, dialogueAudioGroup, dialogueValueText);
    }
    private void SetSliderValue(Slider slider, string groupName, TextMeshProUGUI valueText) {
        if (slider == null) return;
        float savedValue = PlayerPrefs.GetFloat(groupName, DEFAULT_VOLUME);
        savedValue = Mathf.Clamp(savedValue, 0f, 1f);
        int sliderValue = Mathf.RoundToInt(savedValue * MAX_SLIDER_VALUE);
        slider.SetValueWithoutNotify(sliderValue);
        float audioValue = MapSliderToAudio(sliderValue);
        CoreRoot.Instance.Audio.SetVolume(groupName, audioValue);
        UpdateAudioValueText(valueText, sliderValue);
    }
    private void HandleVolumeChange(Slider slider, string groupName, TextMeshProUGUI valueText, float sliderValue) {
        int intSliderValue = Mathf.RoundToInt(sliderValue);
        float audioValue = MapSliderToAudio(intSliderValue);

        CoreRoot.Instance.Audio.SetVolume(groupName, audioValue);
        UpdateAudioValueText(valueText, intSliderValue);

        float normalized = intSliderValue / (float)MAX_SLIDER_VALUE;
        PlayerPrefs.SetFloat(groupName, normalized);
    }
    private float MapSliderToAudio(int sliderValue) {
        return MIN_AUDIO_VALUE + ((MAX_AUDIO_VALUE - MIN_AUDIO_VALUE) / MAX_SLIDER_VALUE) * sliderValue;
    }
    private void UpdateAudioValueText(TextMeshProUGUI text, int sliderValue) {
        if (text != null)
        {
            int percent = Mathf.RoundToInt(sliderValue / (float)MAX_SLIDER_VALUE * 100f);
            text.text = $"{percent}%";
        }
    }
*/