using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FireSpreadMapping
{
    public FlammableStatus flammableStatus;
    public FireSpreadData spreadData;
}

public class FireController : MonoBehaviour
{
    public static FireController Instance;

    [Header("Fire Spread Ayarları")]
    public List<FireSpreadMapping> spreadMappings = new();

    [Header("Global Fire Materials")]
    public Material flammableMaterial;
    public Material burningMaterial;
    public Material burnedOutMaterial;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public FireSpreadData GetSpreadDataFor(FlammableStatus status)
    {
        if (status == FlammableStatus.None)
            return null;

        var match = spreadMappings.FirstOrDefault(m => m.flammableStatus == status);
        if (match != null)
        {
            return match.spreadData;
        }

        Debug.LogWarning($"[FireController] '{status}' için spreadData bulunamadı!");
        return null;
    }


}
