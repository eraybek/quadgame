using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private Image batteryBarImage; // UI → Image
    [SerializeField] private DroneStatus droneStatus; // Batarya değerini tutan script

    private void Update()
    {
        float fill = droneStatus.currentBattery / droneStatus.maxBattery;
        batteryBarImage.fillAmount = fill;
    }
}
