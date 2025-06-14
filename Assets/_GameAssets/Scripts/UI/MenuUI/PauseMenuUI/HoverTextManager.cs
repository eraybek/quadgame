using TMPro;
using UnityEngine;

public class HoverTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hoverText;

    public static HoverTextManager Instance;

    private void Awake()
    {
        Instance = this;
        if (hoverText != null)
            hoverText.text = "";
    }

    public void ShowText(string message)
    {
        if (hoverText != null)
            hoverText.text = message;
        hoverText.gameObject.SetActive(true);
    }

    public void ClearText()
    {
        if (hoverText != null)
            hoverText.text = "";
        hoverText.gameObject.SetActive(false);
    }
}
