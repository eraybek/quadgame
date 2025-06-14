using UnityEngine;

public class CrosshairFollowAim_Ahmet : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private float maxDistance = 100f;

    void Update()
    {
        Vector3 aimPoint;

        // FirePoint'ten ileriye doğru ray gönder
        Ray ray = new Ray(firePoint.position, firePoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = ray.GetPoint(maxDistance);
        }

        // Hedef noktasını ekran konumuna çevir
        Vector3 screenPos = mainCamera.WorldToScreenPoint(aimPoint);

        // Ekran dışında değilse crosshair pozisyonunu ayarla
        if (screenPos.z > 0f)
        {
            crosshairUI.position = screenPos;
        }
    }
}
