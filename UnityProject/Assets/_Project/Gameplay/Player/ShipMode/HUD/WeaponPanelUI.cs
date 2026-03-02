using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponPanelUI : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private ShipWeaponController weaponController;

    [Header("General")]
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private Image weaponImage;

    [Header("Panels")]
    [SerializeField] private GameObject heatPanel;
    //[SerializeField] private GameObject energyUIPanel;
    [SerializeField] private GameObject ammoUIPanel;

    /*
    [Header("Energy")]
    [SerializeField] private EnergySystem shipEnergySystem;
    [SerializeField] private Image energyBarFill;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField, Range(0.01f, 1f)] private float energySmoothSpeed = 0.05f;
    */

    [Header("Heat")]
    [SerializeField] private TextMeshProUGUI heatWarningText;
    [SerializeField] private Image heatBarFill;
    [SerializeField] private Image overheatBarFill;
    [SerializeField] private Color normalHeatColor = Color.red;
    [SerializeField] private Color overheatedColor = new Color(0.5f, 0f, 0f);
    [SerializeField, Range(0.5f, 5f)] private float heatPulseSpeed = 2f;
    [SerializeField, Range(0.01f, 1f)] private float heatSmoothSpeed = 0.1f;
    private const float OVERHEAT_WARNING_RATIO = 0.8f; 

    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image ammoBarFill;
    [SerializeField] private Color ammoNormalColor = Color.green;
    [SerializeField] private Color ammoReloadColor = Color.yellow;
    [SerializeField, Range(0.01f, 1f)] private float ammoSmoothSpeed = 0.1f;
    [Header("Reload FX")]
    [SerializeField] private float reloadCompleteFlashTime = 0.15f;
    [SerializeField] private Color reloadCompleteFlashColor = Color.white;


    // ---------------- Runtime ----------------
    private WeaponBase currentWeapon;
    private IAmmoUser ammoUser;
    private IHeatUser heatUser;
    //private IEnergyUser energyUser;

    //private float displayedEnergy;
    private float displayedHeat;
    private float displayedAmmoRatio;

    private float reloadFlashTimer;
    //private float overheatFlashTimer;


    // ---------------- Unity ----------------
    private void Awake() => HideAll();
    private void Start() {
        if (!weaponController) {
            Debug.LogError("WeaponUI: WeaponController reference missing!");
            enabled = false;
            return;
        }

        weaponController.OnWeaponSwitched += OnWeaponSwitched;
        OnWeaponSwitched(weaponController.CurrentWeapon);

        /*
        // Subscribe to energy system events
        if (shipEnergySystem != null) {
            displayedEnergy = shipEnergySystem.CurrentEnergy;
            shipEnergySystem.OnEnergyChanged += OnEnergyChanged;
        }
        */
    }
    private void OnDestroy() {
        if (weaponController != null)
            weaponController.OnWeaponSwitched -= OnWeaponSwitched;

        /*
        if (shipEnergySystem != null)
            shipEnergySystem.OnEnergyChanged -= OnEnergyChanged;
        */

        UnbindWeapon();
    }
    private void Update() {
        //UpdateEnergyUI();
        UpdateHeatUI();
        UpdateAmmoUI();
    }



    // ---------------- Weapon Binding ----------------
    private void OnWeaponSwitched(WeaponBase newWeapon) {
        InterruptReload();
        UnbindWeapon();
        BindWeapon(newWeapon);
    }
    private void BindWeapon(WeaponBase weapon) {
        currentWeapon = weapon;

        if (currentWeapon == null) {
            HideAll();
            return;
        }

        weaponName.text = $"{weapon.weapon.Name}";     
        weaponImage.sprite = weapon.weapon.Icon;
        ammoUser   = weapon as IAmmoUser;
        heatUser   = weapon as IHeatUser;
        //energyUser = weapon as IEnergyUser;
        
        currentWeapon.OnWeaponStateChanged += RefreshStaticUI;
        SetupPanels();
        RefreshStaticUI();
        reloadFlashTimer = 0f;    
        //overheatFlashTimer = 0f;  
    }
    private void UnbindWeapon() {
        if (currentWeapon != null)
            currentWeapon.OnWeaponStateChanged -= RefreshStaticUI;

        currentWeapon = null;
        ammoUser = null;
        heatUser = null;
        //energyUser = null;
    }
    private void InterruptReload() {
        if (ammoUser is IReloadInterruptable interruptable)
            interruptable.InterruptReload();
    }



    // ---------------- Panels ----------------
    private void SetupPanels() {
        //energyUIPanel.SetActive(energyUser != null);
        ammoUIPanel.SetActive(ammoUser != null);
        heatPanel.SetActive(heatUser != null);
    }
    private void HideAll() {
        //energyUIPanel.SetActive(false);
        ammoUIPanel.SetActive(false);
        heatPanel.SetActive(false);
    }


    // ---------------- Energy ----------------    
    /*
    private void OnEnergyChanged() {
        displayedEnergy = shipEnergySystem.CurrentEnergy;
    }
    private void UpdateEnergyUI() {
        if (energyUser == null || shipEnergySystem == null) return;

        displayedEnergy = Mathf.Lerp(displayedEnergy, shipEnergySystem.CurrentEnergy, Time.deltaTime * energySmoothSpeed);
        float max = shipEnergySystem.MaxEnergy;
        energyBarFill.fillAmount = max > 0f ? displayedEnergy / max : 0f;
        energyText.text = $"{Mathf.CeilToInt(displayedEnergy)} / {Mathf.CeilToInt(max)}";
    }
    */


    // ---------------- Heat ----------------    
    private void UpdateHeatUI() {
        if (heatUser == null || heatBarFill == null || overheatBarFill == null || heatWarningText == null) return;

        // Smooth normal heat bar
        displayedHeat = Mathf.Lerp(displayedHeat, heatUser.CurrentHeat, Time.deltaTime / heatSmoothSpeed);
        float heatRatio = heatUser.MaxHeat > 0f ? Mathf.Clamp01(displayedHeat / heatUser.MaxHeat) : 0f;
        heatBarFill.fillAmount = heatRatio;
        heatBarFill.color = normalHeatColor;

        // Overheat logic
        if (heatUser.IsOverheated) {
            heatWarningText.text = "OVERHEATED";
            overheatBarFill.gameObject.SetActive(true);
            float overheatRatio = Mathf.Clamp01(heatUser.OverheatProgress);
            overheatBarFill.fillAmount = overheatRatio;
            overheatBarFill.color = overheatedColor;
        } else {
            heatWarningText.text = "";
            overheatBarFill.gameObject.SetActive(false);
        
            // Near-overheat pulse
            if (heatRatio >= OVERHEAT_WARNING_RATIO) {
                float pulse = Mathf.PingPong(Time.time * heatPulseSpeed, 1f);
                heatBarFill.color = Color.Lerp(normalHeatColor, overheatedColor, pulse * 0.5f);
            }
        }
    }


    // ---------------- Ammo ----------------
    private void UpdateAmmoUI() {
        if (ammoUser == null || ammoBarFill == null) return;

        bool isReloading = ammoUser.IsReloading;

        float targetFill;

        if (isReloading) {
            // Reload: fill left → right
            ammoBarFill.color = ammoReloadColor;
            ammoBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;

            targetFill = ammoUser.ReloadProgress;
        }
        else {
            // Shooting: drain right → left
            ammoBarFill.color = ammoNormalColor;
            ammoBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;

            targetFill = ammoUser.MagazineSize > 0
                ? (float)ammoUser.CurrentAmmo / ammoUser.MagazineSize
                : 0f;
        }

        displayedAmmoRatio = Mathf.Lerp(
            displayedAmmoRatio,
            targetFill,
            Time.deltaTime / ammoSmoothSpeed
        );

        ammoBarFill.fillAmount = displayedAmmoRatio;


        // Reload completion flash
        if (reloadFlashTimer > 0f) {
            reloadFlashTimer -= Time.deltaTime;
            ammoBarFill.color = Color.Lerp(
                reloadCompleteFlashColor,
                ammoNormalColor,
                1f - (reloadFlashTimer / reloadCompleteFlashTime)
            );
        }

        ammoText.text = $"{ammoUser.CurrentAmmo} / {ammoUser.MagazineSize}";
    }


    // ---------------- Events ----------------
    private void RefreshStaticUI() {
        if (ammoUser != null)
            ammoText.text = $"{ammoUser.CurrentAmmo} / {ammoUser.MagazineSize}";
    }
}
