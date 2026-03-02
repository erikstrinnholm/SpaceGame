using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    [Header("References")]
    [SerializeField] private LoadingScreenUIController loadingScreen;

    [Header("Settings")]
    [SerializeField] private float minimumLoadTime = 2f;

    public void LoadScene(string sceneName) {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName) {
        loadingScreen?.Show();
        loadingScreen?.SetProgress(0f);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;


        // ---------------- STEP 1 ----------------
        while (op.progress < 0.9f) {
            float async01 = Mathf.Clamp01(op.progress / 0.9f);
            float progress = async01 * 0.75f;

            loadingScreen?.SetProgress(progress);
            yield return null;
        }
        // ---------------- STEP 2 ----------------
        // Minimum load time: 0.75 → 1.0
        float timer = 0f;
        while (timer < minimumLoadTime) {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / minimumLoadTime);
            float progress = Mathf.Lerp(0.75f, 1f, t);

            loadingScreen?.SetProgress(progress);
            yield return null;
        }
        // Allow the scene to activate
        op.allowSceneActivation = true;        
        yield return null;
        loadingScreen?.Hide();
    }
}
