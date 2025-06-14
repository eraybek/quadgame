using UnityEngine;
using UnityEngine.UI;

public class DroneStatus_Ahmet : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float batteryDrainRate = 1f;
    public float batteryChargeRate = 10f;

    [Header("Ammo Settings")]
    public int maxAmmo = 50;
    public int currentAmmo = 50;
    public float ammoRefillRate = 100f;

    [Header("Water Ammo Settings")]
    public int maxWaterAmmo = 100;
    public int currentWaterAmmo = 100;
    public float waterRefillRate = 10f;

    [Header("XP Settings")]
    public int CurrentXP = 0;
    public int MaxXP = 100;

    // [Header("UI")]
    // [SerializeField] private Image batteryFillImage;
    // [SerializeField] private Image ammoFillImage;
    // [SerializeField] private Image waterAmmoFillImage;

    [Header("Platform Detection")]
    [SerializeField] private DroneStationDetector stationDetector;

    public static DroneStatus_Ahmet Instance { get; private set; }

    private void Update()
    {
        HandleBattery();
        HandleAmmo();
        HandleWaterAmmo();
        // UpdateUI();
    }

    private void HandleBattery()
    {
        if (stationDetector.isCharging)
        {
            currentBattery += batteryChargeRate * Time.deltaTime;
        }
        else
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
        }

        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);
    }

    private void HandleAmmo()
    {
        if (stationDetector.isRefillingAmmo)
        {
            currentAmmo += Mathf.RoundToInt(ammoRefillRate * Time.deltaTime);
            currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
        }
    }

    private void HandleWaterAmmo()
    {
        if (stationDetector.isRefillingWater)
        {
            currentWaterAmmo += Mathf.RoundToInt(waterRefillRate * Time.deltaTime);
            currentWaterAmmo = Mathf.Clamp(currentWaterAmmo, 0, maxWaterAmmo);
        }
    }

    public void GainXP(int amount)
    {
        CurrentXP += amount;

        if (CurrentXP >= MaxXP)
        {
            CurrentXP = 0; // reset bar
            // opsiyonel: seviye atlama veya başka ödül
        }
    }

    // private void UpdateUI()
    // {
    //     if (batteryFillImage != null)
    //         batteryFillImage.fillAmount = currentBattery / maxBattery;

    //     if (ammoFillImage != null)
    //         ammoFillImage.fillAmount = (float)currentAmmo / maxAmmo;

    //     if (waterAmmoFillImage != null)
    //         waterAmmoFillImage.fillAmount = (float)currentWaterAmmo / maxWaterAmmo;
    // }

    public bool HasAmmo() => currentAmmo > 0;
    public bool HasWaterAmmo() => currentWaterAmmo > 0;

    public void ConsumeAmmo(int amount) => currentAmmo = Mathf.Max(0, currentAmmo - amount);
    public void ConsumeWater(int amount) => currentWaterAmmo = Mathf.Max(0, currentWaterAmmo - amount);

    public bool HasBattery() => currentBattery > 0f;
}
