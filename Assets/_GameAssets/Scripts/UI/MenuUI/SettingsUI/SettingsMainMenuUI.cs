using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private TextMeshProUGUI mouseSensitivityText;

    [Header("Buttons")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

        backButton.onClick.AddListener(CloseSettings);

        settingsPanel.SetActive(false);

        LoadSettings();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    private void OnMusicVolumeChanged(float value)
    {
        musicVolumeText.text = Mathf.RoundToInt(value * 100f) + "%";
        PlayerPrefs.SetFloat("MusicVolume", value);
        // AudioManager.Instance.SetMusicVolume(value); // opsiyonel
    }

    private void OnSFXVolumeChanged(float value)
    {
        sfxVolumeText.text = Mathf.RoundToInt(value * 100f) + "%";
        PlayerPrefs.SetFloat("SFXVolume", value);
        // AudioManager.Instance.SetSFXVolume(value); // opsiyonel
    }

    private void OnMouseSensitivityChanged(float value)
    {
        mouseSensitivityText.text = value.ToString("F1");
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        // DroneMovement.Instance.SetMouseSensitivity(value); // opsiyonel
    }

    private void LoadSettings()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);

        musicVolumeSlider.value = music;
        sfxVolumeSlider.value = sfx;
        mouseSensitivitySlider.value = sensitivity;
    }
}
