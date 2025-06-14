using TMPro;
using UnityEngine;

public class EnemyWaveUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI waveCounterText;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private TextMeshProUGUI activeEnemyCountText;

    [Header("References")]
    [SerializeField] private SpawnManager spawner;

    private void Update()
    {
        if (spawner == null) return;

        int wave = spawner.GetCurrentWave();
        float time = spawner.GetTimeUntilNextWave();
        int enemies = spawner.GetActiveEnemyCount();

        UpdateWaveCounter(wave);
        UpdateWaveTimer(time > 0f ? time : -1f); // wave in progress için -1 gönder
        UpdateEnemyCount(enemies);
    }


    public void UpdateWaveCounter(int waveNumber)
    {
        if (waveCounterText != null)
            waveCounterText.text = $"{waveNumber}/{spawner.GetWaveCount()}";
    }

    private void UpdateWaveTimer(float timeLeft)
    {
        if (waveTimerText != null)
        {
            if (timeLeft >= 0f)
                waveTimerText.text = $"{timeLeft:F1}s";
            else
                waveTimerText.text = "In Progress";
        }
    }

    private void UpdateEnemyCount(int count)
    {
        if (activeEnemyCountText != null)
            activeEnemyCountText.text = $"{count}";
    }
}
