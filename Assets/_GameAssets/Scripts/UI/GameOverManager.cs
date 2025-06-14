using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private bool isGameOver = false;

    public static GameOverManager Instance { get; private set; }
    public static bool IsGameOver => Instance != null && Instance.isGameOver;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameOverPanel.SetActive(false);
    }

    public void TriggerWin()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverText.text = "Win!";
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TriggerLose()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverText.text = "Lose!";
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
