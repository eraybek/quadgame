using UnityEngine;

public class DroneStationDetector : MonoBehaviour
{
    [HideInInspector] public bool isCharging = false;
    [HideInInspector] public bool isRefillingAmmo = false;
    [HideInInspector] public bool isRefillingWater = false;
    [HideInInspector] public bool isXpDroping = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[StationDetector] Trigger girdi: " + other.gameObject.name);

        if (other.CompareTag("ChargePlatform"))
        {
            isCharging = true;
        }

        if (other.CompareTag("AmmoPlatform"))
        {
            isRefillingAmmo = true;
        }

        if (other.CompareTag("WaterPlatform"))
        {
            isRefillingWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ChargePlatform"))
        {
            isCharging = false;
        }

        if (other.CompareTag("AmmoPlatform"))
        {
            isRefillingAmmo = false;
        }

        if (other.CompareTag("WaterPlatform"))
        {
            isRefillingWater = false;
        }
    }
}
