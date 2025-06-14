using UnityEngine;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private GameObject panelToToggle; // Açılıp kapanacak panel
    [SerializeField] private Button backButton;


    void Start()
    {
        backButton.onClick.AddListener(CloseSettings);

        panelToToggle.SetActive(false);
    }

    public void OpenSettings()
    {
        panelToToggle.SetActive(true);
    }

    public void CloseSettings()
    {
        panelToToggle.SetActive(false);
    }


}
