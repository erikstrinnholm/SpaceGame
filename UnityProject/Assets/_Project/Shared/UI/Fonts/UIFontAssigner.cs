using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // Runs in Editor and Play mode
[RequireComponent(typeof(TMP_Text))]
public class UIFontAssigner : MonoBehaviour {
    public enum FontRole { Title, UI, CompactUI, Body, Monospace }

    [Header("Font Settings")]
    public FontRole fontRole;
    public bool useBold = false;
    [SerializeField] private UIFontLibrary fontLibrary;

    private TMP_Text tmp;

    private void OnEnable() {
        tmp = GetComponent<TMP_Text>();
        ApplyFont();
    }

#if UNITY_EDITOR
    private void Update() {
        if (!Application.isPlaying)
            ApplyFont();
    }
#endif


    // Assigns the correct TMP_FontAsset based on role and bold toggle.
    public void ApplyFont() {
        if (tmp == null) tmp = GetComponent<TMP_Text>();
        if (tmp == null || fontLibrary == null) return;

        switch (fontRole) {
            case FontRole.Title:
                tmp.font = useBold ? fontLibrary.titleBold : fontLibrary.titleRegular;
                break;
            case FontRole.UI:
                tmp.font = useBold ? fontLibrary.uiBold : fontLibrary.uiRegular;
                break;
            case FontRole.CompactUI:
                tmp.font = useBold ? fontLibrary.uiCompactBold : fontLibrary.uiCompact;
                break;
            case FontRole.Body:
                tmp.font = useBold ? fontLibrary.bodyBold : fontLibrary.bodyRegular;
                break;
            case FontRole.Monospace:
                tmp.font = useBold ? fontLibrary.monospaceBold : fontLibrary.monospaceRegular;
                break;
        }

#if UNITY_EDITOR
        // Mark scene dirty so changes are visible and saved in editor
        if (!Application.isPlaying) {
            EditorUtility.SetDirty(tmp);
        }
#endif
    }
}
