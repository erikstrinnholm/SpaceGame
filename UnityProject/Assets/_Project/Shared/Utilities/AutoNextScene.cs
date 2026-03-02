using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoNextScene : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 10f;

    private void Start() {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay() {
        yield return new WaitForSeconds(delaySeconds);

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}
