using System.Collections.Generic;
using UnityEngine;

public class BaseFireManager : MonoBehaviour
{
    public static BaseFireManager Instance { get; private set; }

    [SerializeField] private GameObject baseTargetFireVFX;
    private readonly HashSet<GridTileRuntime> burningBaseGrids = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterBurningBaseGrid(GridTileRuntime tile)
    {
        if (!burningBaseGrids.Contains(tile))
        {
            burningBaseGrids.Add(tile);
            UpdateBaseVFX();
        }
    }

    public void UnregisterBurningBaseGrid(GridTileRuntime tile)
    {
        if (burningBaseGrids.Contains(tile))
        {
            burningBaseGrids.Remove(tile);
            UpdateBaseVFX();
        }
    }

    private void UpdateBaseVFX()
    {
        if (baseTargetFireVFX != null)
        {
            baseTargetFireVFX.SetActive(burningBaseGrids.Count > 0);
        }
    }

    public bool IsBaseBurning() => burningBaseGrids.Count > 0;
}
