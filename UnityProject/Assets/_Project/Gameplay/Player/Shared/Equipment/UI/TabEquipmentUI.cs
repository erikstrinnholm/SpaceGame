using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//BUGS LIST
// THE SWAP DOES NOT WORK, it dublicates and overrides
// The Drop into dropslot click does not work!


public class TabEquipmentUI : MonoBehaviour, IInventoryTab {
    [Header("References")]
    [SerializeField] private Canvas canvas; //test
    [SerializeField] private Image dragImage;
    [SerializeField] private GameObject contentPanel;

    [Header("Weapon Slots")]
    [SerializeField] private List<EquipmentSlotUI> weaponSlots;

    [Header("ShipSystem Slots")]
    [SerializeField] private List<EquipmentSlotUI> shipSystemSlots;

    [Header("Storage Slots")]
    [SerializeField] private List<EquipmentSlotUI> storageSlots;

    [Header("Special Slots")]
    [SerializeField] private EquipmentSlotUI slotDropL;
    [SerializeField] private EquipmentSlotUI slotDropR;
    [SerializeField] private EquipmentSlotUI slotTrash;
    [SerializeField] private GameObject trashText;
    [SerializeField] private GameObject dropLeftText;
    [SerializeField] private GameObject dropRightText;    

    [Header("UI Preferences")]
    [SerializeField] private Color lockedSlotColor = new Color(1f, 0.7f, 0.7f, 1f);

    //runtime
    private List<EquipmentSlotUI> allSlots = new List<EquipmentSlotUI>();
    private int selectedIndex = 0;
    private int clickedIndex = -1;
    private GameObject firstSelectable;    
    private EquipmentNavigator lockedNavigator;   //WILL IMPLEMENT LATER
    private enum NavigationState { Normal, Locked }
    private enum InputMode { Select, Mouse }
    private NavigationState navState = NavigationState.Normal;
    private InputMode inputMode = InputMode.Select;


    // -------- DATA MODEL ----------
    private PlayerEquipment playerEquipment =>     GameRoot.Instance.PlayerData.PlayerEquipment;
    private Equipment draggedEquipment = null;



    // -------- INTIALIZATION ----------
    private void Awake() {
        BuildSlotList();
        RegisterButtonListeners();

        lockedNavigator = new EquipmentNavigator(); //WILL IMPLEMENT LATER
        firstSelectable = allSlots[0].gameObject;

        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);
        ShowSpecialSlots(false);
        ClearAllHoverImages();
    }
    private void Start() {
        RefreshUI();
    }
    private void OnDestroy() {
        foreach (var slot in allSlots) {
            slot.button.onClick.RemoveAllListeners();
        }
    }
    private void BuildSlotList() {
        allSlots.Clear();

        for (int i = 0; i < weaponSlots.Count; i++) {
            EquipmentSlotUI slot = weaponSlots[i];
            slot.isWeaponSlot = true;
            slot.weaponIndex = i;
            allSlots.Add(slot);
        }
        for (int i = 0; i < shipSystemSlots.Count; i++) {
            EquipmentSlotUI slot = shipSystemSlots[i];
            slot.isShipSystemSlot = true;
            slot.shipSystemIndex = i;
            allSlots.Add(slot);
        }
        for (int i = 0; i < storageSlots.Count; i++) {
            EquipmentSlotUI slot = storageSlots[i];
            slot.isStorageSlot = true;
            slot.storageIndex = i;
            allSlots.Add(slot);
        }

        slotDropL.isDropSlot = true;
        slotDropR.isDropSlot = true;
        allSlots.Add(slotDropL);
        allSlots.Add(slotDropR);

        slotTrash.isTrashSlot = true;
        allSlots.Add(slotTrash);
    }
    private void RegisterButtonListeners() {
        for (int i = 0; i < allSlots.Count; i++) {
            int indexCopy = i;
            var slot = allSlots[i];
            slot.button.onClick.RemoveAllListeners();   //clear nay existing listeners
            slot.button.onClick.AddListener(() => OnSlotClicked(indexCopy)); 
        }
    }      



    // --------- OPEN / CLOSE ----------
    public void OnTabOpened() {
        contentPanel.SetActive(true);

        navState = NavigationState.Normal;
        inputMode = InputMode.Select;

        clickedIndex = -1;
        draggedEquipment = null;
        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);

        ShowSpecialSlots(false);
        SetSelected(firstSelectable);        
    }
    public void OnTabClosed() {
        contentPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        navState = NavigationState.Normal;
        inputMode = InputMode.Select;

        draggedEquipment = null;
        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);        
    }
    

    
    // ---------- Navigation ----------------    
    public void OnNavigate(Vector2 direction) {
        if (inputMode == InputMode.Mouse) {
            inputMode = InputMode.Select;
            dragImage.enabled = draggedEquipment != null;    
            SetSelected(firstSelectable);
            return;
        }
        if (navState == NavigationState.Normal) {
            HandleNormalNavigation();
        }
        else {
            HandleLockedNavigation(direction);
        }
    }
    private void HandleNormalNavigation() {
        // Let Unity handle explicit navigation
        if (EventSystem.current.currentSelectedGameObject == null) SetSelected(firstSelectable);
    }
    private void HandleLockedNavigation(Vector2 dir) {
        selectedIndex = lockedNavigator.GetNextIndex(selectedIndex, dir);
        SetSelected(allSlots[selectedIndex].button.gameObject);

        if (draggedEquipment == null) return;
        var slot = allSlots[selectedIndex];

        if (slot.isLocked ||
         (slot.isWeaponSlot && draggedEquipment is not Weapon) ||
         (slot.isShipSystemSlot && draggedEquipment is not ShipSystem)) {
            ShowLockedHoverAt(selectedIndex, draggedEquipment.Icon);
        }
        else {
            ShowHoverAt(selectedIndex, draggedEquipment.Icon);
        }
    }  



    // ---------- MOUSE INPUT ----------------
    public void OnMouseMove(Vector2 mousePos) {
        inputMode = InputMode.Mouse;
        EventSystem.current.SetSelectedGameObject(null);
        if (draggedEquipment != null) UpdateDragImagePosition(mousePos);
        if (navState == NavigationState.Locked) ShowSpecialSlots(true);
    }
    private void UpdateDragImagePosition(Vector2 screenPos) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint);
        dragImage.rectTransform.anchoredPosition = localPoint;
    }

    

    // --------- CONFIRM / CANCEL ----------    
    public void OnConfirm() => OnSlotClicked(selectedIndex);
    public void OnCancel() {
        Debug.Log("CANCEL");
        ClearAllHoverImages();

        if (draggedEquipment != null && clickedIndex >= 0 && clickedIndex < allSlots.Count) {
            PutBack(clickedIndex);
        } 
    
        ClearDragImage();

        navState = NavigationState.Normal;
        SetUnityNavigationEnabled(true);
        ShowSpecialSlots(false);

        RefreshUI();
    }



    // --------- CLICK / DRAG / DROP LOGIC  ----------
    public void OnSlotClicked(int index) {
        if (allSlots[index].isLocked) return;

        if (draggedEquipment != null) {
            TryDrop(index);
            return;
        }
        TryPickUp(index);
    }
    private void TryPickUp(int index) {
        EquipmentSlotUI target = allSlots[index];
        Equipment picked = null;

        if (target.isWeaponSlot)
            picked = playerEquipment.TakeFromWeaponSlot(target.weaponIndex);
        else if (target.isShipSystemSlot)
            picked = playerEquipment.TakeFromShipSystemSlot(target.shipSystemIndex);
        else if (target.isStorageSlot)
            picked = playerEquipment.TakeFromStorageSlot(target.storageIndex);
        if (picked == null) return;
        
        draggedEquipment = picked;
        clickedIndex = index;
        selectedIndex = index;

        if (draggedEquipment != null) ShowHoverAt(selectedIndex, draggedEquipment.Icon);
        UpdateDragImage();

        if(inputMode == InputMode.Mouse) {
            dragImage.enabled = true;
            dragImage.gameObject.SetActive(true);
        }
        
        // Lock Navigation
        navState = NavigationState.Locked;
        SetUnityNavigationEnabled(false);   // disable Unity navigation
        ShowSpecialSlots(true);
        RefreshUI();    
    }
    private void TryDrop(int index) {
        EquipmentSlotUI target = allSlots[index];
        bool dropCompleted = false;

        if (target.isTrashSlot) {
            Debug.Log("EQUIPMENT DELETED");
            SetSelected(firstSelectable);
            draggedEquipment = null;         //permanently delete
            dropCompleted = true;
                        
        }        
        else if (target.isDropSlot) {
            Debug.Log("EQUIPMENT DROPPED");
            SetSelected(firstSelectable);
            draggedEquipment = null;         //permanently delete (later eject into world)
            dropCompleted = true;            
        }
        else if(target.isWeaponSlot) {
            if (draggedEquipment is not Weapon weapon) return;

            int weaponIndex = target.weaponIndex;
            Weapon existing = playerEquipment.TakeFromWeaponSlot(weaponIndex);
            if (existing == null) {
                //INSERT
                playerEquipment.InsertIntoWeaponSlot(weaponIndex, weapon);
                draggedEquipment = null;
                dropCompleted = true;
            } else {
                //SWAP
                playerEquipment.InsertIntoWeaponSlot(weaponIndex, weapon);
                draggedEquipment = existing;
                UpdateDragImage();
            }
        }
        else if (target.isShipSystemSlot) {
            if (draggedEquipment is not ShipSystem system) return;

            int sysIndex = target.shipSystemIndex;
            ShipSystem existing = playerEquipment.TakeFromShipSystemSlot(sysIndex);
            if (existing == null) {
                // INSERT
                playerEquipment.InsertIntoShipSystemSlot(sysIndex, system);
                draggedEquipment = null;
                dropCompleted = true;
            } else {
                // SWAP
                playerEquipment.InsertIntoShipSystemSlot(sysIndex, system);
                draggedEquipment = existing;
                UpdateDragImage();
            }
        }
        else if (target.isStorageSlot) {
            if (draggedEquipment == null) return;

            int storageIndex = target.storageIndex;
            Equipment existing = playerEquipment.TakeFromStorageSlot(storageIndex);
            if (existing == null) {
                // INSERT
                playerEquipment.InsertIntoStorageSlot(storageIndex, draggedEquipment);
                draggedEquipment = null;
                dropCompleted = true;
            } else {
                // SWAP
                playerEquipment.InsertIntoStorageSlot(storageIndex, draggedEquipment);
                draggedEquipment = existing;
                UpdateDragImage();
            }
        }

        // -------- FINAL CLEANUP ------------
        if (dropCompleted) {
            ClearDragImage();
            navState = NavigationState.Normal;
            SetUnityNavigationEnabled(true);
            ShowSpecialSlots(false);            
        }
        ClearAllHoverImages();
        clickedIndex = index;
        selectedIndex = index;
        RefreshUI();
    }



    //HELPERS
    private void UpdateDragImage() {
        dragImage.sprite = draggedEquipment.Icon;
        dragImage.preserveAspect = true;
    }
    private void ClearDragImage() {
        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);
        dragImage.sprite = null;
    }
    private void PutBack(int originalIndex) {
        EquipmentSlotUI target = allSlots[originalIndex];
        if (draggedEquipment == null) return;

        if(target.isWeaponSlot) {
            int i = target.weaponIndex;
            if (playerEquipment.IsWeaponSlotEmpty(i))
                playerEquipment.InsertIntoWeaponSlot(i, draggedEquipment as Weapon);
            else
                playerEquipment.TryAddIntoStorageSlots(draggedEquipment);
        }
        else if(target.isShipSystemSlot) {
            int i = target.shipSystemIndex;
            if (playerEquipment.IsShipSystemSlotEmpty(i))
                playerEquipment.InsertIntoShipSystemSlot(i, draggedEquipment as ShipSystem);
            else
                playerEquipment.TryAddIntoStorageSlots(draggedEquipment);
        }
        else if(target.isStorageSlot) {
            int i = target.storageIndex;
            if (playerEquipment.IsStorageSlotEmpty(i))
                playerEquipment.InsertIntoStorageSlot(i, draggedEquipment);
            else
                playerEquipment.TryAddIntoStorageSlots(draggedEquipment);
        }        
        draggedEquipment = null;
    }





    public void RefreshUI() {
        if (playerEquipment == null) return;

        for (int i = 0; i < weaponSlots.Count; i++) {
            EquipmentSlotUI uiSlot = weaponSlots[i];
            EquipmentSlot dataslot = playerEquipment.weaponSlots[i];
            uiSlot.isLocked = dataslot.locked;
            ApplySlotLockVisual(uiSlot);
            uiSlot.BindEquipment(playerEquipment.GetFromWeaponSlot(i));

            //old version
            //weaponSlots[i].BindEquipment(equipmentInventory.GetFromWeaponSlot(i));
        }
        for (int i = 0; i < shipSystemSlots.Count; i++) {
            EquipmentSlotUI uiSlot = shipSystemSlots[i];
            EquipmentSlot dataslot = playerEquipment.shipSystemSlots[i];
            uiSlot.isLocked = dataslot.locked;
            ApplySlotLockVisual(uiSlot);
            uiSlot.BindEquipment(playerEquipment.GetFromShipSystemSlot(i));

            //old version
            //shipSystemSlots[i].BindEquipment(equipmentInventory.GetFromShipSystemSlot(i));
        }
        for (int i = 0; i < storageSlots.Count; i++) {
            EquipmentSlotUI uiSlot = storageSlots[i];
            EquipmentSlot dataslot = playerEquipment.storageSlots[i];
            uiSlot.isLocked = dataslot.locked;
            ApplySlotLockVisual(uiSlot);
            uiSlot.BindEquipment(playerEquipment.GetFromStorageSlot(i));

            //old version
            //storageSlots[i].BindEquipment(equipmentInventory.GetFromStorageSlot(i));
        }
    }




    // ---------------- UI Helpers ----------------
    private void SetUnityNavigationEnabled(bool enabled) {
        foreach (var slot in allSlots) {
            var nav = slot.button.navigation;
            nav.mode = enabled ? Navigation.Mode.Automatic : Navigation.Mode.None;
            slot.button.navigation = nav;
        }
    }    
    private void SetSelected(GameObject go) {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }    
    private void ClearAllHoverImages() {
        foreach (var s in allSlots) {
            s.equipmentHoverImage.sprite = null;
            s.equipmentHoverImage.enabled = false;
        }
    }
    private void ShowHoverAt(int index, Sprite sprite) {
        ClearAllHoverImages();
        var img = allSlots[index].equipmentHoverImage;
        img.sprite = sprite;
        img.enabled = true;
    }   
    private void ShowSpecialSlots(bool visible) {
        trashText.SetActive(visible);
        dropLeftText.SetActive(visible);
        dropRightText.SetActive(visible);        
        slotDropL.gameObject.SetActive(visible);
        slotDropR.gameObject.SetActive(visible);
        slotTrash.gameObject.SetActive(visible);
    }


    //New Helper
    private void ShowLockedHoverAt(int index, Sprite sprite) {
        ClearAllHoverImages();
        var img = allSlots[index].equipmentHoverImage;
        img.sprite = sprite;    //can change to null as well
        img.color = new Color(1f, 0.3f, 0.3f, 0.8f);  // red tinted
        img.enabled = true;
    }
    private void ApplySlotLockVisual(EquipmentSlotUI uiSlot) {
        if (uiSlot.backgroundImage == null) return;
        if (uiSlot.isLocked) uiSlot.backgroundImage.color = lockedSlotColor;
        else uiSlot.backgroundImage.color = Color.white;    //default
    }    
}
