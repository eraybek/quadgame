using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button quitButton;

    [Header("References")]
    [SerializeField] private SettingsMainMenuUI settingsMainMenuUI;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;
    // [SerializeField] private float loadingDuration = 2f;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        howToPlayButton.onClick.AddListener(OnHowToPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        MusicManager.Instance?.PauseMusic();

        StartCoroutine(LoadSceneAsync(1));
    }

    private void OnHowToPlayClicked()
    {
        howToPlayPanel.SetActive(true);
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;

        loadingPanel.SetActive(true);

        float progressValue = 0f;

        while (operation.progress < 0.9f)
        {
            progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        // %90'a ulaştı → %100 göster
        yield return new WaitForSeconds(1f); // oyuncuya %100 gösterim hissi ver

        operation.allowSceneActivation = true; // sahne geçişine izin ver
    }



    private void OnSettingsClicked()
    {
        //Debug.Log("Settings clicked - gelecekte ayar menüsüne geçiş için kullanılacak.");
        settingsMainMenuUI.OpenSettings();
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Quit clicked - oyun kapatılıyor.");
    }
}