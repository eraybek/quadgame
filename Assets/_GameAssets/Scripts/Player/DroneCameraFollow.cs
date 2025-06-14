using UnityEngine;

public class DroneCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Drone
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Kamera offseti
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Pozisyonu güncelle (Drone'un lokal eksenine göre offset uygula)
        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Rotasyonu güncelle (Drone'un rotasyonuna doğru döndür)
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}
