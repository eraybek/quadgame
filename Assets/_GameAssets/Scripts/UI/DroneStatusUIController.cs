using UnityEngine;
using UnityEngine.UI;
using TMPro; // eğer TextMeshPro kullanıyorsan

public class DroneStatusUIController : MonoBehaviour
{
    [Header("Battery")]
    [SerializeField] private Image chargeFill;
    [SerializeField] private Image chargeIcon;
    [SerializeField] private TextMeshProUGUI chargeText;

    [Header("Water Ammo")]
    [SerializeField] private Image waterFill;
    [SerializeField] private Image waterIcon;
    [SerializeField] private TextMeshProUGUI waterText;

    [Header("Bullet Ammo")]
    [SerializeField] private Image bulletFill;
    [SerializeField] private Image bulletIcon;
    [SerializeField] private TextMeshProUGUI bulletText;

    [Header("XP")]
    [SerializeField] private Image xpFill;
    [SerializeField] private Image xpIcon;
    [SerializeField] private TextMeshProUGUI xpText;

    [Header("References")]
    [SerializeField] private DroneStatus droneStatus;
    [SerializeField] private DroneStationDetector droneStationDetector;

    private Color chargeOriginalColor, waterOriginalColor, bulletOriginalColor, xpOriginalColor;

    private void Start()
    {
        if (chargeIcon != null) chargeOriginalColor = chargeIcon.color;
        if (waterIcon != null) waterOriginalColor = waterIcon.color;
        if (bulletIcon != null) bulletOriginalColor = bulletIcon.color;
        // if (xpIcon != null) xpOriginalColor = xpIcon.color;
    }

    private void Update()
    {
        if (droneStatus == null) return;

        UpdateFill(chargeFill, chargeText, droneStatus.currentBattery, droneStatus.maxBattery);
        PulseIcon(chargeIcon, droneStationDetector.isCharging, chargeOriginalColor);

        UpdateFill(waterFill, waterText, droneStatus.currentWaterAmmo, droneStatus.maxWaterAmmo);
        PulseIcon(waterIcon, droneStationDetector.isRefillingWater, waterOriginalColor);

        UpdateFill(bulletFill, bulletText, droneStatus.currentAmmo, droneStatus.maxAmmo);
        PulseIcon(bulletIcon, droneStationDetector.isRefillingAmmo, bulletOriginalColor);

        UpdateFill(xpFill, xpText, droneStatus.CurrentXP, droneStatus.MaxXP);
        // PulseIcon(xpIcon, droneStationDetector.GainedXPThisFrame, xpOriginalColor);
    }

    private void UpdateFill(Image fillImage, TextMeshProUGUI text, float current, float max)
    {
        if (fillImage != null && max > 0)
        {
            float percent = Mathf.Clamp01(current / max);
            fillImage.fillAmount = percent;

            if (text != null)
            {
                text.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(max)}";
                text.color = percent < 0.2f ? Color.red : Color.white;
            }
        }
    }

    private void PulseIcon(Image iconImage, bool isActive, Color originalColor)
    {
        if (iconImage == null) return;

        if (isActive)
        {
            float t = Mathf.PingPong(Time.time * 3f, 1f);
            iconImage.color = Color.Lerp(originalColor, Color.blue, t);
        }
        else
        {
            iconImage.color = originalColor;
        }
    }
}
