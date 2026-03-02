using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using System;


public class TooltipUIController : MonoBehaviour {
    public static TooltipUIController Instance;

    [Header("References")]
    [SerializeField] private TooltipUI tooltipUI;
    [SerializeField] private GameColorDatabase gameColors;

    [Header("Settings")]
    [SerializeField] private float showDelay = 0.15f;
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f);

    //runtime
    private bool usingController = true;
    private Coroutine delayRoutine;


    // -------- UNITY ----------
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        tooltipUI.SetActive(false);
        WeaponTooltipFormatter.Colors = gameColors;     //test
    }
    private void Update() {
        if (!usingController && tooltipUI.IsActive) {
            PositionAtMouse(tooltipUI.rectTransform);
        }
    }
    public void HideTooltip() {
        if (delayRoutine != null)
            StopCoroutine(delayRoutine);
        tooltipUI.SetActive(false);
    }


    // ------------------- PUBLIC API ----------------
    public void ShowGeneralTooltipMouse(string text) {
        ShowDelayed(() => FillGeneral(text), null, false);
    }
    public void ShowGeneralTooltip(string text, RectTransform anchor) {
        ShowDelayed(() => FillGeneral(text), anchor, true);
    }

    public void ShowItemTooltipMouse(ItemData itemData) {
        ShowDelayed(() => FillItem(itemData), null, false);
    }
    public void ShowItemTooltip(ItemData itemData, RectTransform anchor) {
        ShowDelayed(() => FillItem(itemData), anchor, true );
    }

    public void ShowEquipmentTooltipMouse(EquipmentData equipmentData) {
        ShowDelayed(() => FillEquipment(equipmentData), null, false);
    }
    public void ShowEquipmentTooltip(EquipmentData equipmentData, RectTransform anchor) {
        ShowDelayed(() => FillEquipment(equipmentData), anchor, true);
    }

    public void UpdateMousePosition(Vector2 pos) {
        if (tooltipUI.rectTransform.gameObject.activeSelf) {
            tooltipUI.rectTransform.position = pos + offset;
        }
    }

    // -------- CORE DELAY HANDLER --------
    private void ShowDelayed(Action fillAction, RectTransform anchor, bool anchored) {
        usingController = anchored;
        HideTooltip();
        delayRoutine = StartCoroutine(ShowDelayedRoutine(fillAction, anchor));
    }
    private IEnumerator ShowDelayedRoutine(Action fillAction, RectTransform anchor) {
        yield return new WaitForSeconds(showDelay);

        tooltipUI.ClearOptional();
        fillAction.Invoke();
        tooltipUI.SetActive(true);

        if (usingController && anchor != null)
            PositionAnchored(tooltipUI.rectTransform, anchor);
        else
            PositionAtMouse(tooltipUI.rectTransform);
    }


    // ---------- FILL METHODS ----------
    private void FillGeneral(string descriptionText) {
        tooltipUI.SetHeader(
            null,
            "",
            "",
            null
        );
        //Body
        tooltipUI.description.text = descriptionText;
        tooltipUI.statBlock.gameObject.SetActive(false);
        //Footer
        tooltipUI.value.gameObject.SetActive(false);
    }
    private void FillItem(ItemData itemData) {
        tooltipUI.SetHeader(
            null,
            itemData.displayName,
            FormatItemType(itemData.type),
            gameColors.GetRarityColor(itemData.rarity)
        );        
        //Body
        tooltipUI.description.text = itemData.description;
        tooltipUI.statBlock.gameObject.SetActive(false);
        //Footer
        tooltipUI.value.gameObject.SetActive(true);
        tooltipUI.value.text = itemData.value + " CR";
    }


    private void FillEquipment(EquipmentData data) {
        tooltipUI.SetHeader(
            null,  
            data.displayName,
            GetEquipmentTypeLabel(data),
            gameColors.GetRarityColor(data.rarity)
        );
        tooltipUI.description.text = data.description;
        tooltipUI.value.gameObject.SetActive(true);
        tooltipUI.value.text = $"{data.value} CR";

        if (data is WeaponData weaponData) {
            tooltipUI.statBlock.gameObject.SetActive(true);
            tooltipUI.statBlock.text = WeaponTooltipFormatter.Build(weaponData);
        } else {
            tooltipUI.statBlock.gameObject.SetActive(false);
        }
    }


    // -------- HELPERS --------
    private static string GetEquipmentTypeLabel(EquipmentData data) {
        if (data is WeaponData weapon) {
            if (weapon is KineticWeaponData)   return "Kinetic Weapon";
            if (weapon is EnergyWeaponData)    return "Energy Weapon";
            if (weapon is BeamWeaponData)      return "Beam Weapon";
            if (weapon is MissileLauncherData) return "Missile Launcher";
            if (weapon is DroneLauncherData)   return "Drone Launcher";
            return "Unknown Weapon";
        }
        if (data is ShipSystemData shipSystem)  return "Ship System";
        return "Unknown Equipment";
    }    
    private string FormatItemType(ItemType type) {
        return type switch {
            ItemType.None => "",
            ItemType.Consumable => "Consumable",
            ItemType.SystemResource => "System Resource",
            ItemType.QuestItem => "Quest Item",
            ItemType.KeyItem => "Key Item",
            ItemType.UpgradeMaterial => "Upgrade Material",
            _ => type.ToString()
        };
    }
    private void PositionAnchored(RectTransform tooltip, RectTransform anchor) {
        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners);
        tooltip.position = corners[3] + (Vector3)offset;
    }
    private void PositionAtMouse(RectTransform tooltip) {
        if (Mouse.current == null) return;
        tooltip.position = Mouse.current.position.ReadValue() + offset;
    }







/*
    // --------- PUBLIC API ------------
    public void ShowItemTooltipMouse(Item item) => ShowItemTooltip(item, null, false);
    public void ShowItemTooltip(Item item, RectTransform anchor) => ShowItemTooltip(item, anchor, true);
    public void ShowGeneralTooltipMouse(string text) => ShowGeneralTooltip(text, null, false);
    public void ShowGeneralTooltip(string text, RectTransform anchor) => ShowGeneralTooltip(text, anchor, true);

    public void UpdateMousePosition(Vector2 pos) {
        if (itemTooltipUI.rectTransform.gameObject.activeSelf)
            itemTooltipUI.rectTransform.position = pos + offset;
        if (generalTooltipUI.rectTransform.gameObject.activeSelf)
            generalTooltipUI.rectTransform.position = pos + offset;
    }





    // --------- INTERNAL ---------
    private void ShowItemTooltip(Item item, RectTransform anchor, bool anchored) {
        usingController = anchored;
        StartDelayedItemTooltip(item, anchor);
    }
    private void ShowGeneralTooltip(string text, RectTransform anchor, bool anchored) {
        usingController = anchored;
        StartDelayedGeneralTooltip(text, anchor);
    }

    // --------- DELAY HANDLING ---------
    private void StartDelayedItemTooltip(Item item, RectTransform anchor) {
        HideTooltip();
        delayRoutine = StartCoroutine(ShowItemTooltipDelayed(item, anchor));
    }
    private void StartDelayedGeneralTooltip(string text, RectTransform anchor) {
        HideTooltip();
        delayRoutine = StartCoroutine(ShowGeneralTooltipDelayed(text, anchor));
    }
    private IEnumerator ShowItemTooltipDelayed(Item item, RectTransform anchor) {
        yield return new WaitForSeconds(showDelay);

        FillItemTooltip(item);
        itemTooltipUI.rectTransform.gameObject.SetActive(true);

        if (usingController && anchor != null) PositionAnchored(itemTooltipUI.rectTransform, anchor);
        else PositionAtMouse(itemTooltipUI.rectTransform);
    }
    private IEnumerator ShowGeneralTooltipDelayed(string text, RectTransform anchor) {
        yield return new WaitForSeconds(showDelay);

        FillGeneralTooltip(text);
        generalTooltipUI.rectTransform.gameObject.SetActive(true);

        if (usingController && anchor != null) PositionAnchored(generalTooltipUI.rectTransform, anchor);
        else PositionAtMouse(generalTooltipUI.rectTransform);
    }


    // --------- FILL CONTENT ---------
    private void FillGeneralTooltip(string text) {
        generalTooltipUI.descriptionText.text = text;
    }
    private void FillItemTooltip(Item item) {
        itemTooltipUI.nameText.text        = item.Name;
        itemTooltipUI.nameText.color       = rarityColors.GetColor(item.Rarity);
        itemTooltipUI.typeText.text        = FormatType(item.Type);
        itemTooltipUI.iconImage.sprite     = item.Icon;
        itemTooltipUI.descriptionText.text = item.Description;
        itemTooltipUI.valueText.text       = $"{item.Value} CR";
    }

    private string FormatType(ItemType type) {
        return type switch {
            ItemType.None           => "",
            ItemType.Consumable     => "Consumable",
            ItemType.SystemResource => "System Resource",
            ItemType.QuestItem      => "Quest Item",
            ItemType.KeyItem        => "Key Item",
            ItemType.UpgradeMaterial=> "Upgrade Material",
            _ => type.ToString()
        };
    }


    // --------- POSITIONING ---------
    private void PositionAnchored(RectTransform tooltip, RectTransform anchor) {
        if (anchor == null) return;

        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners);

        // bottom-right corner of the rect  
        Vector3 pivot = corners[3];
        tooltip.position = pivot + (Vector3)offset;
    }
    
    private void PositionAtMouse(RectTransform tooltip) {
        tooltip.position = Input.mousePosition + (Vector3)offset;
    }
    private void PositionAtMouse(RectTransform tooltip) {
        if (Mouse.current == null) return; // safety check
        Vector3 mousePos = Mouse.current.position.ReadValue();
        tooltip.position = mousePos + (Vector3)offset;
    }

    */
}

