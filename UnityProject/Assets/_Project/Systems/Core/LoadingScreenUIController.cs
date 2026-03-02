using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoadingScreenUIController : MonoBehaviour {
    [Header("Progress Bar")]
    [SerializeField] private Image loadingBarFill;

    [Header("Tips")]
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private string[] tips;
    [SerializeField] private float tipChangeInterval = 4f;

    [Header("Spinner")]
    [SerializeField] private RectTransform spinner;
    [SerializeField] private float spinnerSpeed = 180f;

    private Coroutine tipRoutine;

    private void Awake() {
        gameObject.SetActive(false);
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = 0f;
    }

    private void Update() {
        if (spinner != null)
            spinner.Rotate(0f, 0f, -spinnerSpeed * Time.unscaledDeltaTime);
    }

    // ---------- PUBLIC API ----------
    public void Show() {
        gameObject.SetActive(true);
        SetProgress(0f);

        if (tips != null && tips.Length > 0 && tipText != null)
            tipRoutine = StartCoroutine(TipRoutine());
    }
    public void Hide() {
        if (tipRoutine != null) {
            StopCoroutine(tipRoutine);
            tipRoutine = null;
        }
        gameObject.SetActive(false);
    }



    public void SetProgress(float value) {
        if (loadingBarFill == null) return;
        loadingBarFill.fillAmount = Mathf.Clamp01(value);
    }

    // ---------- TIP CYCLE ----------
    private IEnumerator TipRoutine() {
        while (true) {
            tipText.text = tips[Random.Range(0, tips.Length)];
            yield return new WaitForSecondsRealtime(tipChangeInterval);
        }
    }
}
