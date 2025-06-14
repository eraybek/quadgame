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

    [Header("Wave Settings")]
    public SpawnData[] spawnSettings;
    public Transform[] targetPoints;
    public int waveCount = 3;
    public float spawnInterval = 10f;

    [Header("UI Reference")]
    [SerializeField] private EnemyWaveUI enemyWaveUI;

    private int currentWave = 0;
    private float countdown = 0f;
    private int totalEnemiesThisWave = 0;
    private int deadEnemiesThisWave = 0;
    // private bool isSpawningWave = false;


    private void Start()
    {
        MusicManager.Instance?.ResumeMusic();

        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        for (currentWave = 0; currentWave < waveCount; currentWave++)
        {
            int shownWave = currentWave + 1;

            UpdateWaveUI();

            foreach (var data in spawnSettings)
            {
                SpawnWaveInstant(data, shownWave);
            }


            // Son dalga değilse bekleme süresi başlat
            if (currentWave < waveCount - 1)
            {
                countdown = spawnInterval;
                while (countdown > 0f)
                {
                    countdown -= Time.deltaTime;
                    yield return null;
                }
            }
        }

        Debug.Log("[SpawnManager] Tüm dalgalar tamamlandı.");
        StartCoroutine(CheckForWinCondition());
    }

    private void UpdateWaveUI()
    {
        if (enemyWaveUI != null)
            enemyWaveUI.UpdateWaveCounter(currentWave + 1);
    }

    private IEnumerator CheckForWinCondition()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (currentWave >= waveCount && enemies.Length == 0)
            {
                Debug.Log("[SpawnManager] Tüm düşmanlar öldü, oyun kazanıldı.");
                GameOverManager.Instance.TriggerWin();
                yield break;
            }
        }
    }

    private void SpawnWaveInstant(SpawnData data, int waveNumber)
    {
        if (data.zone == null || data.prefab == null)
        {
            Debug.LogError("[SpawnManager] SpawnZone veya Prefab eksik!");
            return;
        }

        int randomSpawnCount = Random.Range(1, data.countPerWave + 1); // 🔹 1 dahil, max dahil değil → +1

        for (int i = 0; i < randomSpawnCount; i++)
        {
            Vector3 spawnPos = data.zone.GetRandomPointInZone();
            GameObject spawned = Instantiate(data.prefab, spawnPos, Quaternion.identity);
            totalEnemiesThisWave++;

            if (spawned != null)
            {
                var nav = spawned.GetComponent<NavTargetFollower>();
                if (nav != null)
                    nav.SetTargets(targetPoints);

                var ship = spawned.GetComponent<Ship>();
                if (ship != null)
                {
                    Transform closest = GetClosestTarget(spawnPos);
                    ship.SetTarget(closest);
                }

                var enemyDeath = spawned.GetComponent<EnemyDeathReporter>();
                if (enemyDeath != null)
                    enemyDeath.OnEnemyDeath += HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath()
    {
        deadEnemiesThisWave++;

        // Son dalgadaysak ve herkes öldüyse
        if (currentWave >= waveCount - 1 && deadEnemiesThisWave >= totalEnemiesThisWave)
        {
            if (BaseHealth.Instance.currentHealth > 0)
            {
                Debug.Log("[SpawnManager] Tüm düşmanlar öldü, oyun kazanıldı.");
                GameOverManager.Instance.TriggerWin();
            }
        }
    }

    public class EnemyDeathReporter : MonoBehaviour
    {
        public System.Action OnEnemyDeath;

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                OnEnemyDeath?.Invoke();
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

    // --- UI'da kullanılmak üzere public getterlar ---
    public int GetCurrentWave() => Mathf.Min(currentWave + 1, waveCount);
    public int GetWaveCount() => waveCount;
    public float GetTimeUntilNextWave() => countdown;
    public int GetActiveEnemyCount() => GameObject.FindGameObjectsWithTag("Enemy").Length;
}
