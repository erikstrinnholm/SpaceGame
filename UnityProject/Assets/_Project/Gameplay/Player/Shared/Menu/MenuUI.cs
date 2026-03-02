using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;


public enum MenuID {
    Main, About,
    Pause, Confirm,
    Options, Gameplay, Audio, Video, Controls
}

public class MenuUI : MonoBehaviour {
    [Serializable]
    public class MenuPanel {
        public MenuID id;
        public GameObject panel;
        public GameObject firstSelectable;
    }

    [Serializable]
    private class AudioSliderView {
        public AudioGroups group;
        public Slider slider;
        public TextMeshProUGUI valueText;
    }

    [SerializeField] private MenuPanel startMenu;
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button openOptionsButton1;
    [SerializeField] private Button openAboutButton;
    [SerializeField] private Button quitGameButton;

    [SerializeField] private MenuPanel aboutMenu;
    [SerializeField] private Button closeAboutButton;

    [SerializeField] private MenuPanel pauseMenu;
    [SerializeField] private Button resumeGameButton;
    [SerializeField] private Button openOptionsButton2;
    [SerializeField] private Button openConfirmButton;

    [SerializeField] private MenuPanel confirmMenu;
    [SerializeField] private Button quitToMenuButton;
    [SerializeField] private Button closeConfirmButton;

    [SerializeField] private MenuPanel optionsMenu;
    [SerializeField] private Button openGameplayButton;
    [SerializeField] private Button openAudioButton;
    [SerializeField] private Button openVideoButton;
    [SerializeField] private Button openControlsButton;
    [SerializeField] private Button closeOptionsButton;

    [SerializeField] private MenuPanel gameplayMenu;
    [SerializeField] private Button closeGameplayButton;

    [SerializeField] private MenuPanel audioMenu;
    [SerializeField] private AudioSliderView[] audioSliders;
    [SerializeField] private Button resetDefaultAudioButton;
    [SerializeField] private Button closeAudioButton;

    [SerializeField] private MenuPanel videoMenu;
    [SerializeField] private Button closeVideoButton;

    [SerializeField] private MenuPanel controlsMenu;
    [SerializeField] private Button closeControlsButton;

    // ===== Internal =====
    private MenuPanel[] allMenus;

    private void Awake() {
        allMenus = new MenuPanel[] {
            startMenu,
            aboutMenu,
            pauseMenu,
            confirmMenu,
            optionsMenu,
            gameplayMenu,
            audioMenu,
            videoMenu,
            controlsMenu
        };
    }

    // ================= MENU LOGIC =================
    public void OpenMenu(MenuID id) {
        HideAll();
        var menu = GetMenu(id);
        if (menu != null)
        {
            menu.panel.SetActive(true);
            if (menu.firstSelectable != null)
                EventSystem.current.SetSelectedGameObject(menu.firstSelectable);
        }
    }
    public void HideAll() {
        foreach (var menu in allMenus)
            menu.panel.SetActive(false);
    }
    private MenuPanel GetMenu(MenuID id) {
        foreach (var menu in allMenus)
            if (menu.id == id)
                return menu;

        Debug.LogError($"Menu {id} not found!");
        return null;
    }
    public GameObject GetFirstSelectableOfActiveMenu() {
        foreach (var panel in allMenus) {
            if (panel.panel.activeSelf && panel.firstSelectable != null)
                return panel.firstSelectable;
        }
        return null;
    }


    // ================= BINDING =================
    public void Bind(IStartMenuActions start, IPauseMenuActions pause, IOptionsMenuActions options) {
        ClearAllListeners();
        BindStartMenu(start);
        BindPauseMenu(pause);
        BindOptionsMenu(options);
    }

    private void BindStartMenu(IStartMenuActions start) {
        startMenu.panel.SetActive(start != null);
        aboutMenu.panel.SetActive(false);

        if (start == null) return;

        playGameButton.onClick.AddListener(start.PlayGame);
        openOptionsButton1.onClick.AddListener(start.OpenOptions);
        openAboutButton.onClick.AddListener(start.OpenAbout);
        closeAboutButton.onClick.AddListener(start.CloseAbout);
        quitGameButton.onClick.AddListener(start.QuitGame);
    }

    private void BindPauseMenu(IPauseMenuActions pause) {
        pauseMenu.panel.SetActive(pause != null);
        confirmMenu.panel.SetActive(false);

        if (pause == null) return;

        resumeGameButton.onClick.AddListener(pause.ResumeGame);
        openOptionsButton2.onClick.AddListener(pause.OpenOptions);      
        openConfirmButton.onClick.AddListener(pause.OpenConfirm);
        quitToMenuButton.onClick.AddListener(pause.QuitToMenu);
        closeConfirmButton.onClick.AddListener(pause.CloseConfirm);
    }

    private void BindOptionsMenu(IOptionsMenuActions options) {
        optionsMenu.panel.SetActive(options != null);
        gameplayMenu.panel.SetActive(false);
        audioMenu.panel.SetActive(false);
        videoMenu.panel.SetActive(false);
        controlsMenu.panel.SetActive(false);

        if (options == null) return;

        openGameplayButton.onClick.AddListener(options.OpenGameplay);
        closeGameplayButton.onClick.AddListener(options.CloseGameplay);
        openAudioButton.onClick.AddListener(options.OpenAudio);
        foreach (var a in audioSliders) {
            switch (a.group) {
                case AudioGroups.Master: 
                    a.slider.onValueChanged.AddListener(options.OnMasterVolumeChanged);
                    break;
                case AudioGroups.Music:
                    a.slider.onValueChanged.AddListener(options.OnMusicVolumeChanged);
                    break;
                case AudioGroups.UI:
                    a.slider.onValueChanged.AddListener(options.OnUIVolumeChanged);
                    break;
                case AudioGroups.SFX:
                    a.slider.onValueChanged.AddListener(options.OnSFXVolumeChanged);
                    break;
                case AudioGroups.Dialogue:
                    a.slider.onValueChanged.AddListener(options.OnDialogueVolumeChanged);
                    break;
            }
        }
        resetDefaultAudioButton.onClick.AddListener(options.ResetDefaultAudio);
        closeAudioButton.onClick.AddListener(options.CloseAudio);

        openVideoButton.onClick.AddListener(options.OpenVideo);
        closeVideoButton.onClick.AddListener(options.CloseVideo);

        openControlsButton.onClick.AddListener(options.OpenControls);
        closeControlsButton.onClick.AddListener(options.CloseControls);

        closeOptionsButton.onClick.AddListener(options.CloseOptions);
    }



    // ================= UTIL =================
    private void ClearAllListeners() {
        foreach (var b in GetComponentsInChildren<Button>(true))
            b.onClick.RemoveAllListeners();

        foreach (var s in GetComponentsInChildren<Slider>(true))
            s.onValueChanged.RemoveAllListeners();
    }




    public void SetAudioSlider(AudioGroups group, int value, int max) {
        var view = GetAudio(group);
        if (view == null) return;

        view.slider.SetValueWithoutNotify(value);
        UpdateValueText(view.valueText, value, max);
    }
    public void UpdateAudioValue(AudioGroups group, int value, int max) {
        var view = GetAudio(group);
        if (view == null) return;
        UpdateValueText(view.valueText, value, max);
    }
    private AudioSliderView GetAudio(AudioGroups group) {
        foreach (var a in audioSliders)
            if (a.group == group)
                return a;
        Debug.LogWarning($"Audio group {group} not found in MenuUI");
        return null;
    }
    private void UpdateValueText(TextMeshProUGUI text, int value, int max) {
        int percent = Mathf.RoundToInt(value / (float)max * 100f);
        text.text = $"{percent}%";
    } 

}
