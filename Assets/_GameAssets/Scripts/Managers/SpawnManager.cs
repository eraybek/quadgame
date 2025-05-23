using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData
    {
        public SpawnZone zone;
        public GameObject prefab;
        public int countPerWave;
    }

    public SpawnData[] spawnSettings;
    public Transform[] targetPoints;
    public int waveCount = 3;
    public float spawnInterval = 10f; // Her dalga arası süre (saniye)

    private void Start()
    {
        Debug.Log("[SpawnManager] Spawner başlatıldı.");
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        for (int wave = 0; wave < waveCount; wave++)
        {
            Debug.Log($"[Wave {wave + 1}] Başlatılıyor @ Time: {Time.time:F2}");

            foreach (var data in spawnSettings)
            {
                SpawnWaveInstant(data, wave + 1);
            }

            Debug.Log($"[Wave {wave + 1}] Tüm objeler spawn edildi @ Time: {Time.time:F2}");

            if (wave < waveCount - 1)
            {
                Debug.Log($"[Wave {wave + 1}] {spawnInterval} saniye bekleniyor...");
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        Debug.Log("[SpawnManager] Tüm dalgalar tamamlandı.");
    }

    private void SpawnWaveInstant(SpawnData data, int waveNumber)
    {
        if (data.zone == null || data.prefab == null)
        {
            Debug.LogError("[SpawnManager] SpawnZone veya Prefab eksik!");
            return;
        }

        for (int i = 0; i < data.countPerWave; i++)
        {
            Vector3 spawnPos = data.zone.GetRandomPointInZone();
            GameObject spawned = Instantiate(data.prefab, spawnPos, Quaternion.identity);

            if (spawned != null)
            {
                var nav = spawned.GetComponent<NavTargetFollower>();
                if (nav != null)
                    nav.SetTargets(targetPoints); // hedefleri alıyor

                var ship = spawned.GetComponent<Ship>();
                if (ship != null)
                {
                    Transform closest = GetClosestTarget(spawnPos);
                    ship.SetTarget(closest); // ship hedefini öğreniyor
                }

                Debug.Log($"[Wave {waveNumber}] {data.prefab.name} spawnlandı @ {spawnPos} | Time: {Time.time:F2}");
            }
        }
    }

    private Transform GetClosestTarget(Vector3 fromPos)
    {
        float minDistance = float.MaxValue;
        Transform closest = null;

        foreach (var t in targetPoints)
        {
            float dist = Vector3.Distance(fromPos, t.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = t;
            }
        }

        return closest;
    }


}
