using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public interface IStartMenuActions {
    void PlayGame();
    void OpenOptions();
    void OpenAbout();
    void CloseAbout();
    void QuitGame();
}
public interface IPauseMenuActions {
    void ResumeGame();
    void OpenOptions();
    void QuitToMenu();
    void OpenConfirm();
    void CloseConfirm();
}
public interface IOptionsMenuActions {
    void OpenGameplay();
    void CloseGameplay();

    void OpenAudio();
    void OnMasterVolumeChanged(float value);
    void OnMusicVolumeChanged(float value);
    void OnUIVolumeChanged(float value);
    void OnSFXVolumeChanged(float value);
    void OnDialogueVolumeChanged(float value);
    void ResetDefaultAudio();
    void CloseAudio();

    void OpenVideo();
    void CloseVideo();

    void OpenControls();
    void CloseControls();

    void CloseOptions();
}


