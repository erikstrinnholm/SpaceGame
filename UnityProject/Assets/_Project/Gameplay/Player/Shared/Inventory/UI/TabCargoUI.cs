using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class TabCargoUI : MonoBehaviour, IInventoryTab {

    [Header("UI References")]
    [SerializeField] private Canvas canvas; //test
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private Image dragImage;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private GameObject capacityText;       //TODO use later

    [Header("Special Slots")]
    [SerializeField] private InventorySlotUI slotDropL;
    [SerializeField] private InventorySlotUI slotDropR;
    [SerializeField] private InventorySlotUI slotQuick0;
    [SerializeField] private InventorySlotUI slotQuick1;
    [SerializeField] private InventorySlotUI slotQuick2;
    [SerializeField] private InventorySlotUI slotQuick3;
    [SerializeField] private InventorySlotUI slotTrash;
    [SerializeField] private GameObject trashText;
    [SerializeField] private GameObject dropLeftText;
    [SerializeField] private GameObject dropRightText;

    [Header("Slot Prefab")]
    [SerializeField] private GameObject gridSlotPrefab;

    [Header("Cargo Grid Settings")]
    [SerializeField] private int gridColumns = 8;
    [SerializeField] private int gridRows = 3;

    [Header("UI Preferences")]
    [SerializeField] private Color lockedSlotColor = new Color(1f, 0.7f, 0.7f, 1f);

    // Runtime
    private List<InventorySlotUI> allSlots = new List<InventorySlotUI>();
    private int selectedIndex = 0;
    private int clickedIndex = -1;
    private GameObject firstSelectable;
    private CargoNavigator lockedNavigator;
    private enum NavigationState { Normal, Locked }
    private enum InputMode { Select, Mouse }
    private NavigationState navState = NavigationState.Normal;
    private InputMode inputMode = InputMode.Select;
    private bool sortNameAsc = true;
    private bool sortValueAsc = true;
    private bool sortRarityAsc = true;

    // -------- DATA MODEL ----------
    private PlayerInventory inventory =>    GameRoot.Instance.PlayerData.PlayerInventory;
    private Item draggedItem = null;



    // -------- INTIALIZATION ----------
    private void Awake() {
        BuildSlotList();
        RegisterButtonListeners();

        lockedNavigator = new CargoNavigator(gridColumns, gridRows);
        firstSelectable = allSlots[0].gameObject;

        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);
        ShowSpecialSlots(false);
        ClearAllHoverImages();
    }
    public void Start() {
        inventory.OnCargoSlotsUpdated += RefreshCargoUI;
        inventory.OnQuickSlotsUpdated += RefreshQuickUI;
        RefreshAllUI();

    }
    private void OnDestroy() {
        foreach (var slot in allSlots) {
            slot.button.onClick.RemoveAllListeners();
        }
        if (inventory == null) return;
        inventory.OnCargoSlotsUpdated -= RefreshCargoUI;
        inventory.OnQuickSlotsUpdated -= RefreshQuickUI;
    }
    private void BuildSlotList() {
        allSlots.Clear();
        //IMPORTANT SEE CARGO NAVIGATOR INDEX ORDER

        // STEP 1: CARGO SLOTS
        int cargoCount = gridColumns * gridRows;
        for(int i = 0; i < cargoCount; i++) {
            GameObject obj = Instantiate(gridSlotPrefab, gridParent);
            InventorySlotUI slot = obj.GetComponent<InventorySlotUI>();
            slot.isCargoSlot = true;
            slot.cargoIndex = i;
            allSlots.Add(slot);
        }

        // STEP 2: DROP SLOTS
        slotDropL.isDropSlot = true;
        slotDropR.isDropSlot = true;
        allSlots.Add(slotDropL);
        allSlots.Add(slotDropR);
        
        // STEP 3: QUICK SLOTS
        InventorySlotUI[] quick = { slotQuick0, slotQuick1, slotQuick2, slotQuick3 };
        for (int i = 0; i < quick.Length; i++) {
            quick[i].isQuickSlot = true;
            quick[i].quickIndex = i;
            allSlots.Add(quick[i]);
        }

        // STEP 4: TRASH SLOT
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
        draggedItem = null;
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

        draggedItem = null;
        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);
    }



    // ---------- Navigation ----------------
    public void OnNavigate(Vector2 direction) {
        if (inputMode == InputMode.Mouse) {
            inputMode = InputMode.Select;
            dragImage.enabled = draggedItem != null;    
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

        if (draggedItem == null) return;
        if (allSlots[selectedIndex].isLocked) ShowLockedHoverAt(selectedIndex, draggedItem.data.icon);
        else if (allSlots[selectedIndex].isQuickSlot && !draggedItem.data.isConsumable) ShowLockedHoverAt(selectedIndex, draggedItem.data.icon); //only consumables in quick slots
        else ShowHoverAt(selectedIndex, draggedItem.data.icon);
    }



    // ---------- MOUSE INPUT ----------------
    public void OnMouseMove(Vector2 mousePos) {
        inputMode = InputMode.Mouse;
        EventSystem.current.SetSelectedGameObject(null);
        if (draggedItem != null) UpdateDragImagePosition(mousePos);
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
        ClearAllHoverImages();

        if (draggedItem != null && clickedIndex >= 0 && clickedIndex < allSlots.Count) {
            PutBack(clickedIndex);
        } 
    
        ClearDragImage();

        navState = NavigationState.Normal;
        SetUnityNavigationEnabled(true);
        ShowSpecialSlots(false);

        //RefreshAllUI();
    }



    // --------- CLICK / DRAG / DROP LOGIC  ----------
    public void OnSlotClicked(int index) {
        // Ignore clicks on the same slot while dragging (prevents double triggering)
        //if (draggedItem != null && index == clickedIndex) return;

        if (allSlots[index].isLocked) return;

        // If holding a sprite → drop
        if (draggedItem != null) {
            TryDrop(index);
            return;
        }
        // Not holding → try pick up
        TryPickUp(index);
    }
    private void TryPickUp(int index) {
        InventorySlotUI target = allSlots[index];
        Item pickedItem = null;

        if (target.isCargoSlot) {
            pickedItem = inventory.TakeCargoItem(target.cargoIndex);
        }
        else if (target.isQuickSlot) {
            pickedItem = inventory.TakeQuickItem(target.quickIndex);
        }
        if (pickedItem == null) {
            return; //nothing to pick up
        }

        // Update indices
        draggedItem = pickedItem;
        clickedIndex = index;
        selectedIndex = index;

        //Show Hover
        if (draggedItem != null) ShowHoverAt(selectedIndex, draggedItem.data.icon);

        // Update Drag Visual
        UpdateDragImage();
        if(inputMode == InputMode.Mouse) {
            dragImage.enabled = true;
            dragImage.gameObject.SetActive(true);
        }
        // Lock Navigation
        navState = NavigationState.Locked;
        SetUnityNavigationEnabled(false);   // disable Unity navigation
        ShowSpecialSlots(true);

        //RefreshAllUI();    
    }
    private void TryDrop(int index) {
        InventorySlotUI target = allSlots[index];
        bool dropCompleted = false;

        if (target.isTrashSlot) {
            Debug.Log("ITEM DELETED");
            draggedItem = null;         //permanently delete
            dropCompleted = true;
        }
        else if (target.isDropSlot) {
            Debug.Log("ITEM DROPPED");
            draggedItem = null;         //permanently delete (later eject into world)
            dropCompleted = true;
        }
        else if (target.isQuickSlot) {
            int qIndex = target.quickIndex;
            Item existing = inventory.GetQuickItem(qIndex);
            
            if (!draggedItem.data.isConsumable) return;                    //only consumables in quick slots

            else if (existing == null) {
                inventory.InsertQuickItem(qIndex, draggedItem);         //INSERT
                draggedItem = null;
                dropCompleted = true;
            }
            else if (existing.data.id == draggedItem.data.id && existing.data.isStackable && draggedItem.data.isStackable) {
                bool fullyAdded = inventory.TryStackQuickItem(qIndex, draggedItem);  //STACKING

                if (fullyAdded) {
                    draggedItem = null;
                    dropCompleted = true;
                } else {
                    UpdateDragImage();
                }
            }
            else {
                inventory.InsertQuickItem(qIndex, draggedItem);         //SWAP
                draggedItem = existing;                
                UpdateDragImage();
            }
            // 🔥 ENSURE UI UPDATES
            inventory.OnQuickSlotsUpdated?.Invoke();           
        }
        else if (target.isCargoSlot) {
            int cIndex = target.cargoIndex;
            Item existing = inventory.GetCargoItem(cIndex);
            if (existing == null) {
                inventory.InsertCargoItem(cIndex, draggedItem);         //INSERT
                draggedItem = null;
                dropCompleted = true;
            }
            else if (existing.data.id == draggedItem.data.id && existing.data.isStackable && draggedItem.data.isStackable) {
                bool fullyAdded = inventory.TryStackCargoItem(cIndex, draggedItem);  //STACKING

                if (fullyAdded) {
                    draggedItem = null;
                    dropCompleted = true;
                } else {
                    UpdateDragImage();
                }
            }                       
            else {
                inventory.InsertCargoItem(cIndex, draggedItem);         //SWAP
                draggedItem = existing;                
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
        selectedIndex = index;  //test?
        //RefreshAllUI();
    }
    //HELPERS
    private void UpdateDragImage() {
        dragImage.sprite = draggedItem.data.icon;
        dragImage.preserveAspect = true;
    }
    private void ClearDragImage() {
        dragImage.enabled = false;
        dragImage.gameObject.SetActive(false);
        dragImage.sprite = null;
    }

    private void PutBack(int originalIndex) {
        InventorySlotUI target = allSlots[originalIndex];

        if(target.isCargoSlot) {
            int cIndex = target.cargoIndex;
            if (!inventory.HasCargoItem(cIndex)) {
                inventory.InsertCargoItem(cIndex, draggedItem);
            } 
            else {
                if (!inventory.TryAddCargoItem(draggedItem)) {
                    Debug.LogWarning("No space to return item to cargo!");
                }
            }
        }
        else if (target.isQuickSlot) {
            int qIndex = target.quickIndex;
            if (!inventory.HasQuickItem(qIndex)) {
                inventory.InsertQuickItem(qIndex, draggedItem);
            }
            else {
                if (!inventory.TryAddQuickItem(draggedItem)) {
                    Debug.LogWarning("No space to return item to quick slots!");
                }
            }
        }
        draggedItem = null;
    }


    // -------------- SORTING ------------
    public void OnSortByName() {
        inventory.SortCargoByName(sortNameAsc);
        sortNameAsc = !sortNameAsc;
    }
    public void OnSortByValue() {
        inventory.SortCargoByValue(sortValueAsc);
        sortValueAsc = !sortValueAsc;
    }
    public void OnSortByRarity() {
        inventory.SortCargoByRarity(sortRarityAsc);
        sortRarityAsc = !sortRarityAsc;
    }


    public void RefreshAllUI() {
        RefreshCargoUI();
        RefreshQuickUI();    
    }
    public void RefreshCargoUI() {
        int cargoCount = inventory.cargoSlots.Count;
        for (int i = 0; i < cargoCount; i++) {
            InventorySlotUI uiSlot = allSlots[i];
            InventorySlot dataSlot = inventory.cargoSlots[i];

            uiSlot.isLocked = dataSlot.locked;
            ApplySlotLockVisual(uiSlot);

            uiSlot.BindItem(dataSlot.item);
        }        
    }
    public void RefreshQuickUI() {
        InventorySlotUI[] quickSlotUIs = { slotQuick0, slotQuick1, slotQuick2, slotQuick3 };
        int quickCount = Mathf.Min(quickSlotUIs.Length, inventory.quickSlots.Length);
        for (int i = 0; i < quickCount; i++) {
            InventorySlotUI uiSlot = quickSlotUIs[i];
            InventorySlot dataSlot = inventory.quickSlots[i];

            uiSlot.isLocked = dataSlot.locked;
            ApplySlotLockVisual(uiSlot);

            uiSlot.BindItem(dataSlot.item);
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
            s.itemHoverImage.sprite = null;
            s.itemHoverImage.color = Color.white; //reset color here
            s.itemHoverImage.enabled = false;
        }
    }
    private void ShowHoverAt(int index, Sprite sprite) {
        ClearAllHoverImages();
        var img = allSlots[index].itemHoverImage;
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
        var img = allSlots[index].itemHoverImage;
        img.sprite = sprite;    //can change to null as well
        img.color = new Color(1f, 0.3f, 0.3f, 0.8f);  // red tinted
        img.enabled = true;
    }
    private void ApplySlotLockVisual(InventorySlotUI uiSlot) {
        if (uiSlot.backgroundImage == null) return;
        if (uiSlot.isLocked) uiSlot.backgroundImage.color = lockedSlotColor;
        else uiSlot.backgroundImage.color = Color.grey;    //default
    }
}
