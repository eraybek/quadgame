using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool IsPaused => Instance != null && Instance.isPaused;

    [SerializeField] private GameObject pauseMenuUI;


    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("References")]
    [SerializeField] private HoverTextManager hoverTextManager;
    [SerializeField] private SettingsUIManager settingsUIManager;

    private bool isPaused = false;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(QuitGame);

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Oyun başlarken aktif olsun
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        hoverTextManager.ClearText();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private void OpenSettings()
    {
        //Debug.Log("Settings menu açılacak.");
        // Settings menüsünü açmak için gerekenleri buraya yazın
        settingsUIManager.OpenSettings();
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
