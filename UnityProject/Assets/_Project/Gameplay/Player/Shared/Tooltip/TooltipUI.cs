using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour {
    [Header("Root")]
    [SerializeField] public RectTransform rectTransform;            //root panel
    [Header("Header")]
    [SerializeField] public Image icon;                             //optional
    [SerializeField] public TextMeshProUGUI displayName;
    [SerializeField] public TextMeshProUGUI type;
    [SerializeField] public Image rarityFrame;                      //optional
    [Header("Body")]
    [SerializeField] public TextMeshProUGUI description;
    [SerializeField] public TextMeshProUGUI statBlock;              //optional (weapon)
    [Header("Footer")]
    [SerializeField] public TextMeshProUGUI value;                  //optional

    public bool IsActive => gameObject.activeSelf;

    private void Reset() {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    public void ClearOptional() {
        if (icon != null) icon.gameObject.SetActive(false);
        if (rarityFrame != null) rarityFrame.gameObject.SetActive(false);
        if (statBlock != null) statBlock.gameObject.SetActive(false);
        if (value != null) value.gameObject.SetActive(false);
    }

    public void SetHeader(Sprite icon, string displayName, string type, Color? rarityColor) {
        if (this.icon != null) {
            this.icon.gameObject.SetActive(icon != null);
            this.icon.sprite = icon;
        }
        this.displayName.text = displayName;
        this.type.text = type;

        if (rarityFrame != null) {
            rarityFrame.gameObject.SetActive(rarityColor.HasValue);
            if (rarityColor.HasValue)
                rarityFrame.color = rarityColor.Value;
        }
    }


}
