using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner_Ahmet : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float timeBetweenWaves = 30f;
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private int maxWaves = 10;

    private int currentWave = 0;
    private float countdown = 0f;
    private bool isSpawning = false;

    [SerializeField] private EnemyWaveUI enemyWaveUI;

    private void Start()
    {
        countdown = timeBetweenWaves;
        UpdateWaveUI();
    }

    private void Update()
    {
        if (isSpawning || currentWave >= maxWaves)
            return;

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;

        if (currentWave >= maxWaves && ObjectPooler_Ahmet.Instance.GetActiveCount(enemyTag) == 0)
        {
            GameOverManager.Instance.TriggerWin();
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        currentWave++;
        UpdateWaveUI();

        for (int i = 0; i < enemiesPerWave && i < spawnPoints.Count; i++)
        {
            GameObject enemy = ObjectPooler.Instance.SpawnFromPool(enemyTag, spawnPoints[i].position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }

        isSpawning = false;
    }

    private void UpdateWaveUI()
    {
        if (enemyWaveUI != null)
        {
            enemyWaveUI.UpdateWaveCounter(currentWave);
        }
    }

    public int GetCurrentWave() => currentWave;
    public float GetTimeUntilNextWave() => countdown;
    public int GetActiveEnemyCount() => ObjectPooler_Ahmet.Instance.GetActiveCount(enemyTag);
}
