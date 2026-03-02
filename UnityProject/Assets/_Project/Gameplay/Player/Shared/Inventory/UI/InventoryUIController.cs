using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InventoryUIController : MonoBehaviour {
    [Header("Inventory Panel")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Tab Buttons")]
    [SerializeField] private Button tabButton1;
    [SerializeField] private Button tabButton2;
    [SerializeField] private Button tabButton3;
    [SerializeField] private Button tabButton4;
    [SerializeField] private Button closeButton;

    [Header("Tab Contents")]
    [SerializeField] private GameObject tabContent1;
    [SerializeField] private GameObject tabContent2;
    [SerializeField] private GameObject tabContent3;
    [SerializeField] private GameObject tabContent4;

    private int currentTabIndex = 2;        //used
    private Button[] tabButtons;            //used
    private GameObject[] tabContents;       //used
    private bool delayInput = false;        //used
    private Action leftTabAction;           //used
    private Action rightTabAction;          //used
    private IInventoryTab[] tabUIs;         //used


    private void Awake() {
        inventoryPanel.SetActive(false);
        tabButtons = new[] { tabButton1, tabButton2, tabButton3, tabButton4 };
        tabContents = new[] { tabContent1, tabContent2, tabContent3, tabContent4 };

        tabUIs = new IInventoryTab[tabContents.Length];
        for (int i = 0; i < tabContents.Length; i++) {
            tabUIs[i] = tabContents[i].GetComponent<IInventoryTab>();
        }
    }
    private void Start() {
        leftTabAction = () => SwitchTab(-1);       //to unsubscribe later
        rightTabAction = () => SwitchTab(1);        //to unsubscribe later

        // Subscribe Listeners
        if (CoreRoot.Instance.Input != null) {
            CoreRoot.Instance.Input.OnInventoryToggle += InventoryToggle;    
            CoreRoot.Instance.Input.OnInventoryNavigate += Navigate;
            CoreRoot.Instance.Input.OnInventoryMouseMove += MouseMove;
            CoreRoot.Instance.Input.OnInventoryConfirm += Confirm;
            CoreRoot.Instance.Input.OnInventoryCancel += Cancel;
            CoreRoot.Instance.Input.OnInventoryLeftTab += leftTabAction;
            CoreRoot.Instance.Input.OnInventoryRightTab += rightTabAction;
        }
    }
    private void OnDestroy() {
        // Unsubscribe Listeners
        if (CoreRoot.Instance.Input != null) {
            CoreRoot.Instance.Input.OnInventoryToggle -= InventoryToggle;
            CoreRoot.Instance.Input.OnInventoryNavigate -= Navigate;
            CoreRoot.Instance.Input.OnInventoryMouseMove -= MouseMove;
            CoreRoot.Instance.Input.OnInventoryConfirm -= Confirm;
            CoreRoot.Instance.Input.OnInventoryCancel -= Cancel;
            CoreRoot.Instance.Input.OnInventoryLeftTab -= leftTabAction;
            CoreRoot.Instance.Input.OnInventoryRightTab -= rightTabAction;
        }
    }
    



    // ----------  Toggle ----------
    private void InventoryToggle() {
        if (delayInput) return;   //prevent rapid open/close
        if (inventoryPanel.activeSelf)
            CloseInventory();
        else
            OpenInventory();
    }
    public void OpenInventory() {
        if (inventoryPanel.activeSelf || delayInput) return;
        inventoryPanel.SetActive(true);
        StartCoroutine(InputCooldown());

        //Input Map
        CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Inventory);       //INPUT MAP (INVENTORY)
        
        //Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Open Tab
        OpenTabByIndex(currentTabIndex);

    }
    public void CloseInventory() {
        if (!inventoryPanel.activeSelf || delayInput) return;
        inventoryPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(InputCooldown());

        //Input Map
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene) {
            case "TutorialScene":
                CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Ship);
                Cursor.visible = false;                
                break;
            case "HangarScene":
                CoreRoot.Instance.Input.SwitchActionMap(ActionMapType.Character);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;             
        } 
        //close the tab (reset the tabIndex to 2 as inventory as default)
        currentTabIndex = 2;
    }
    private IEnumerator InputCooldown() {
        delayInput = true;
        yield return null; // ignore for 1 frame
        delayInput = false;
    }


    // ----------  Events to Delegate ----------
    private void Navigate(Vector2 direction) {
        tabUIs[currentTabIndex]?.OnNavigate(direction);     // Delegate to current tab UI
    }    
    private void MouseMove(Vector2 pos) {
        tabUIs[currentTabIndex]?.OnMouseMove(pos);             // Delegate to current tab UI
    }
    private void Confirm() {
        tabUIs[currentTabIndex]?.OnConfirm();               // Delegate to current tab UI
    }
    private void Cancel() {
        tabUIs[currentTabIndex]?.OnCancel();                 // Delegate to current tab UI
    }



    // -------- Switch Tabs -----------
    public void OpenTabByIndex(int index) {
        if (index == currentTabIndex) return;   //already open

        //close old tab
        tabUIs[currentTabIndex]?.OnTabClosed();
        currentTabIndex = index;

        //open new tab
        tabUIs[currentTabIndex]?.OnTabOpened();
        
        // Change the selected tabbutton visuals visual (make this better later)
        for (int i = 0; i < tabButtons.Length; i++) {
            var colors = tabButtons[i].colors;
            if (i == index) {
                colors.normalColor = Color.yellow; // Selected color
            } else {
                colors.normalColor = Color.white; // Default color
            }
            tabButtons[i].colors = colors;
        }
    }
    private void SwitchTab(int direction) {
        int newIndex = currentTabIndex + direction;
        if (newIndex < 0) newIndex = tabContents.Length - 1;
        if (newIndex >= tabContents.Length) newIndex = 0;
        OpenTabByIndex(newIndex);
    }
}
