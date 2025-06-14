using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TryAgainButton : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject[] uiObjectsToHide; // Diğer UI objelerini buraya ekle
    [SerializeField] private float delayBeforeReload = 1.5f; // loading süresi

    public void TryAgain()
    {
        Time.timeScale = 1f;

        // Diğer UI öğelerini kapat
        foreach (var uiObject in uiObjectsToHide)
        {
            if (uiObject != null)
                uiObject.SetActive(false);
        }

        // Loading paneli göster
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        MusicManager.Instance?.PauseMusic();

        // Delay sonrası sahneyi yeniden yükle
        StartCoroutine(ReloadSceneWithDelay());
    }

    private IEnumerator ReloadSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeReload);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
