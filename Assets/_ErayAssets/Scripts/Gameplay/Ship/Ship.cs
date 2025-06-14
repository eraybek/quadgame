using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Ship : MonoBehaviour
{

    [Header("Enemy Spawn Settings")]
    [SerializeField] private int enemyCapacity = 5;
    [SerializeField] private List<GameObject> enemyPrefabs; // tüm enemy türleri burada
    [SerializeField] private float searchRadius = 5f;
    [SerializeField] private LayerMask spawnGridLayer;

    private List<GameObject> carriedEnemies = new();
    private Transform targetPoint;
    private bool reachedTarget = false;

    public void SetTarget(Transform target)
    {
        targetPoint = target;
    }

    private void Start()
    {
        PrepareCarriedEnemies();
    }

    private void Update()
    {
        if (reachedTarget) return;

        if (targetPoint != null && Vector3.Distance(transform.position, targetPoint.position) < searchRadius)
        {
            reachedTarget = true;
            SpawnEnemiesAtNearbyGrids();
            Destroy(gameObject);
        }
    }

    private void PrepareCarriedEnemies()
    {
        carriedEnemies.Clear();

        for (int i = 0; i < enemyCapacity; i++)
        {
            if (enemyPrefabs.Count == 0)
            {
                Debug.LogWarning("[Ship] Enemy prefab listesi boş!");
                return;
            }

            int randomIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject selectedPrefab = enemyPrefabs[randomIndex];
            carriedEnemies.Add(selectedPrefab);
        }
    }


    private void SpawnEnemiesAtNearbyGrids()
    {
        Collider[] colliders = Physics.OverlapSphere(targetPoint.position, 10f, spawnGridLayer);
        List<GridTileRuntime> spawnTiles = new();

        Debug.Log($"[Ship] Spawn alanı aranıyor @ {targetPoint.position}, radius: {searchRadius}");

        foreach (var col in colliders)
        {
            GridTileRuntime tile = col.GetComponentInParent<GridTileRuntime>();
            if (tile != null && tile.SpecialStatus == TileSpecialStatus.SpawnZone)
            {
                spawnTiles.Add(tile); // ❗️Eksik olan satır bu!
                Debug.Log($"[Ship] Grid bulundu: {tile.name}, Status: {tile.SpecialStatus}");
            }
        }

        int gridCount = spawnTiles.Count;
        int enemyCount = carriedEnemies.Count;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = spawnTiles[i % gridCount].transform.position;
            Instantiate(carriedEnemies[i], spawnPos, Quaternion.identity);
        }
    }
}