using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne değişimini dinle
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Event'den çık
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestartMusic(); // Her sahne değişiminde müziği başlat
    }

    public void RestartMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();    // Durdur
            audioSource.time = 0f; // Başa sar
            audioSource.Play();    // Baştan çal
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play(); // UnPause yerine baştan çalmak istiyorsan bu
    }
}
