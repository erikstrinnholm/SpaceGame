using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventorySlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("UI Indices")]
    public int cargoIndex = -1;  
    public int quickIndex = -1;

    [Header("Slot Tags")]
    public bool isCargoSlot = false;
    public bool isTrashSlot = false;
    public bool isQuickSlot = false;
    public bool isDropSlot = false;
    public bool isLocked = false;

    [Header("UI References")]
    public Image backgroundImage;
    public Image itemImage;
    public Image itemHoverImage;
    public TextMeshProUGUI countText; 
    public Button button;

    public Item SlotItem {get; private set;}    //Read-only for UI
    private RectTransform rect;
    private bool usingController = false;

    // --------- INITIALIZATION -----------
    private void Awake() {
        //Auto-grab components if they are missing
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();

        if (itemImage == null) 
            itemImage = transform.Find("ItemImage")?.GetComponent<Image>();
        if (itemHoverImage == null) 
            itemHoverImage = transform.Find("HoverImage")?.GetComponent<Image>();
        if (countText == null) 
            countText = transform.Find("CountText")?.GetComponent<TextMeshProUGUI>();

        // Disable item image if empty
        if (itemImage != null && itemImage.sprite == null) itemImage.enabled = false;
    }



    // --------- UI DATA BINDING -----------
    public void BindItem(Item item) {
        SlotItem = item;
        if (item == null) {
            itemImage.enabled = false;
            countText.text = "";
            return;
        }
        itemImage.enabled = true;
        itemImage.sprite = item.data.icon;
        if (isCargoSlot || isQuickSlot) countText.text = item.count > 1 ? item.count.ToString() : "";
        else countText.text = "";
    }
    public bool HasItem => SlotItem != null;



    // --------- POINTER (Mouse) ------------
    public void OnPointerEnter(PointerEventData eventData) {
        if (!HasItem) return;
        usingController = false;
        TooltipUIController.Instance?.ShowItemTooltipMouse(SlotItem.data);
    }
    public void OnPointerExit(PointerEventData eventData) {
        TooltipUIController.Instance?.HideTooltip();
    }
    public void OnPointerMove(PointerEventData eventData) {
        if (!HasItem || usingController) return;
        TooltipUIController.Instance?.UpdateMousePosition(eventData.position);
    }   


    //---------- CONTROLLER (UI Navigation) -----
    public void OnSelect(BaseEventData eventData) {
        if (!HasItem) return;
        usingController = true;
        TooltipUIController.Instance?.ShowItemTooltip(SlotItem.data, rect);
    }
    public void OnDeselect(BaseEventData eventData) {
        TooltipUIController.Instance?.HideTooltip();
    }    
}
