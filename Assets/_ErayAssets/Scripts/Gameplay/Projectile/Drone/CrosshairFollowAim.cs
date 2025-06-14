using UnityEngine;

public class CrosshairFollowAim : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairUI;

    void Update()
    {
        // Fare pozisyonunu al
        Vector3 mousePosition = Input.mousePosition;

        // Crosshair'i fare konumuna yerleştir
        crosshairUI.position = mousePosition;
    }
}
