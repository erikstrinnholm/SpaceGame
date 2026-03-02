using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameMenuUIController : MonoBehaviour, IPauseMenuActions, IOptionsMenuActions 
{
    [Header("View")]
    [SerializeField] private MenuUI menuUI;
    // ===== Audio Constants =====
    private const int MAX_SLIDER_VALUE = 20;
    private const float MAX_AUDIO_VALUE = 1f;
    private const float MIN_AUDIO_VALUE = 0.0001f;
    private const float DEFAULT_VOLUME = 0.75f;

    private bool isPaused = false;


    // --------- Unity lifecycle ----------
    private void Start() {
        menuUI.Bind(null, this, this);    
        ResumeGame();
        if (CoreRoot.Instance.Input != null) {
            CoreRoot.Instance.Input.OnMenuToggle += TogglePause;
            CoreRoot.Instance.Input.OnMenuMouseMove += OnMouseMove;
            CoreRoot.Instance.Input.OnMenuNavigate += OnNavigate;
        }
    }
    private void OnDestroy() {
        if (CoreRoot.Instance.Input != null) {
            CoreRoot.Instance.Input.OnMenuToggle -= TogglePause;
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
    private void TogglePause() {
        if (isPaused) ResumeGame();
        else PauseGame();
    }
    public void PauseGame() {
        isPaused = true;
        Time.timeScale = 0f;

        CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Menu);             //INPUT MAP (MENU)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        menuUI.OpenMenu(MenuID.Pause);
    }
    
//BUTTON INPUTS
    // ================= PAUSE =================
    public void ResumeGame() {
        isPaused = false;
        Time.timeScale = 1f;
        menuUI.HideAll();
        EventSystem.current.SetSelectedGameObject(null);

        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene) {
            case "TutorialScene":
                CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Ship);
                Cursor.visible = false;                
                break;
            case "HangarScene":
                CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Character);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;             
        }        
    }  
    public void OpenOptions() => menuUI.OpenMenu(MenuID.Options);
    public void OpenConfirm() => menuUI.OpenMenu(MenuID.Confirm);

    // ================= CONFIRM =================
    public void QuitToMenu() {
        EventSystem.current.SetSelectedGameObject(null);
        CoreRoot.Instance.Loader.LoadScene("StartScene");
    }
    public void CloseConfirm() => menuUI.OpenMenu(MenuID.Pause);



    // ================= OPTIONS =================
    public void OpenGameplay() => menuUI.OpenMenu(MenuID.Gameplay);
    public void OpenAudio() {
        menuUI.OpenMenu(MenuID.Audio);
        InitializeAudio();        
    }
    public void OpenVideo() => menuUI.OpenMenu(MenuID.Video);
    public void OpenControls() => menuUI.OpenMenu(MenuID.Controls);
    public void CloseOptions() => menuUI.OpenMenu(MenuID.Pause);


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

