using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DroneStatus : MonoBehaviour
{
    [Header("Battery Settings")]
    [SerializeField] public float maxBattery = 100f;
    [SerializeField] private float batteryDrainRate = 5f; // saniyede ne kadar azalıyor
    public float currentBattery;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 10;
    private int currentAmmo;

    [Header("UI References")]
    [SerializeField] private Slider batterySlider;
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private GameObject chargingIcon;

    // İçsel durum kontrolü
    private bool isChargingOverTime = false;
    private bool isReloadingOverTime = false;

    private void Start()
    {
        currentBattery = maxBattery;
        currentAmmo = maxAmmo;
        UpdateUI();

        if (chargingIcon != null)
            chargingIcon.SetActive(false);
    }

    private void Update()
    {
        if (!isChargingOverTime)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (batterySlider != null)
            batterySlider.value = currentBattery / maxBattery;

        if (ammoSlider != null)
            ammoSlider.value = (float)currentAmmo / maxAmmo;
    }

    // 🔋 Coroutine ile zamanlanmış şarj
    public IEnumerator StartChargingOverTime(System.Action onComplete)
    {
        isChargingOverTime = true;

        float missing = maxBattery - currentBattery;
        float duration = 10f * (missing / maxBattery); // eksik oran kadar süre
        float timer = 0f;

        if (chargingIcon != null)
            chargingIcon.SetActive(true);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            currentBattery = Mathf.Lerp(currentBattery, maxBattery, timer / duration);
            UpdateUI();
            yield return null;
        }

        currentBattery = maxBattery;

        if (chargingIcon != null)
            chargingIcon.SetActive(false);

        isChargingOverTime = false;
        onComplete?.Invoke();
    }

    // 🔫 Coroutine ile zamanlanmış cephane doldurma
    public IEnumerator StartReloadingOverTime(System.Action onComplete)
    {
        isReloadingOverTime = true;

        int missing = maxAmmo - currentAmmo;
        float duration = 10f * ((float)missing / maxAmmo);
        float timer = 0f;

        if (chargingIcon != null)
            chargingIcon.SetActive(true);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            currentAmmo = Mathf.RoundToInt(Mathf.Lerp(currentAmmo, maxAmmo, timer / duration));
            UpdateUI();
            yield return null;
        }

        currentAmmo = maxAmmo;

        if (chargingIcon != null)
            chargingIcon.SetActive(false);

        isReloadingOverTime = false;
        onComplete?.Invoke();
    }

    // Dışarıdan erişim için kontroller
    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        currentAmmo = Mathf.Max(currentAmmo - 1, 0);
        UpdateUI();
    }

    public bool IsCharging => isChargingOverTime;
    public bool IsReloading => isReloadingOverTime;
}
