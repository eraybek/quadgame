using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BaseHealthBarUI : MonoBehaviour
{
    [SerializeField] private BaseHealth baseHealth;

    [Header("Dolacak olan barlar")]
    [SerializeField] private List<Image> fillImages; // birden fazla dolan görsel

    private void Update()
    {
        if (baseHealth == null || fillImages == null || fillImages.Count == 0)
            return;

        float healthPercent = (float)baseHealth.currentHealth / baseHealth.maxHealth;
        healthPercent = Mathf.Clamp01(healthPercent);

        foreach (Image img in fillImages)
        {
            if (img != null)
                img.fillAmount = healthPercent;
        }
    }
}
