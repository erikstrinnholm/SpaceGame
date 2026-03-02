using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "UIFontLibrary", menuName = "UI/Font Library")]
public class UIFontLibrary : ScriptableObject {
    [Header("Titles / Headings")]
    public TMP_FontAsset titleRegular;          //Orbitron or Michroma
    public TMP_FontAsset titleBold;             //Orbitron or Michroam

    [Header("UI / Buttons / Panels")]
    public TMP_FontAsset uiRegular;             //Exo2 or B612
    public TMP_FontAsset uiBold;                //Exo2 or B612
    public TMP_FontAsset uiCompact;             //Teko
    public TMP_FontAsset uiCompactBold;         //Teko

    [Header("Body / Paragraph Text")]
    public TMP_FontAsset bodyRegular;           //Montserrat or B612
    public TMP_FontAsset bodyBold;              //Montserrat or B612

    [Header("Monospace / Terminal / Numeric")]
    public TMP_FontAsset monospaceRegular;      //RobotoMono or B612Mono
    public TMP_FontAsset monospaceBold;         //RobotoMono or B612Mono
}
