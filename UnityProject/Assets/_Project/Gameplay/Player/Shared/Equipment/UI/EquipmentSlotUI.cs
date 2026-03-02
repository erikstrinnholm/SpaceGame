using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("UI Indices")]
    public int weaponIndex = -1;
    public int shipSystemIndex = -1;
    public int storageIndex = -1;

    [Header("Slot Tags")]
    public bool isWeaponSlot = false;
    public bool isShipSystemSlot = false;
    public bool isStorageSlot = false;
    public bool isTrashSlot = false;
    public bool isDropSlot = false;
    public bool isLocked = false;


    [Header("UI References")]
    public Image backgroundImage;
    public Image equipmentImage;
    public Image equipmentHoverImage;
    public Button button;

    public Equipment SlotEquipment { get; private set; }         //Read-only for UI
    private RectTransform rect;
    private bool usingController = false;


    // --------- INITIALIZATION -----------
    private void Awake() {
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();

        if (equipmentImage == null)
            equipmentImage = transform.Find("ItemImage")?.GetComponent<Image>();

        if (equipmentHoverImage == null)
            equipmentHoverImage = transform.Find("HoverImage")?.GetComponent<Image>();

        // Hide if empty
        if (equipmentImage != null && equipmentImage.sprite == null)
            equipmentImage.enabled = false;
    }



    // --------- UI DATA BINDING -----------
    public void BindEquipment(Equipment equipment) {
        SlotEquipment = equipment;

        if (equipment == null) {
            equipmentImage.enabled = false;
            return;
        }
        
        equipmentImage.enabled = true;
        equipmentImage.sprite = equipment.Icon;
    }
    public bool HasEquipment => SlotEquipment != null;



    // --------- POINTER (Mouse) ------------
    public void OnPointerEnter(PointerEventData eventData) {
        if(!HasEquipment) return;
        usingController = false;
        TooltipUIController.Instance?.ShowEquipmentTooltipMouse(SlotEquipment.data);
    }
    public void OnPointerExit(PointerEventData eventData) {
        TooltipUIController.Instance?.HideTooltip();
    }
    public void OnPointerMove(PointerEventData eventData) {
        if (!HasEquipment || usingController) return;
        TooltipUIController.Instance?.UpdateMousePosition(eventData.position);        
    }


    //---------- CONTROLLER (UI Navigation) -----
    public void OnSelect(BaseEventData eventData) {
        if (!HasEquipment) return;
        usingController = true;
        TooltipUIController.Instance?.ShowEquipmentTooltip(SlotEquipment.data, rect);
        //if (equipmentHoverImage) equipmentHoverImage.enabled = true;
    }
    public void OnDeselect(BaseEventData eventData) {
        TooltipUIController.Instance?.HideTooltip();
        //if (equipmentHoverImage) equipmentHoverImage.enabled = false;
    }
}
