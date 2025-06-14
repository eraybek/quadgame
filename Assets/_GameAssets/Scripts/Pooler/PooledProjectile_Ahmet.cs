using UnityEngine;

public class PooledProjectile_Ahmet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;
    private Collider projectileCollider;
    private Collider shooterCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        // Mermi her yeniden aktifleştiğinde eski hareket verilerini temizle
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Mermiyi başlatır.
    /// </summary>
    /// <param name="direction">Gideceği yön</param>
    /// <param name="shooter">Fırlatan nesnenin collider'ı</param>
    public void Launch(Vector3 direction, Collider shooter)
    {
        shooterCollider = shooter;

        // Fırlatıcı ile çarpışmayı engelle
        if (projectileCollider != null && shooterCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider);
        }

        // Hız ver
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        // Belirli sürede yok et
        Invoke(nameof(Disable), lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Disable();
    }

    private void Disable()
    {
        CancelInvoke(); // güvenlik amaçlı
        gameObject.SetActive(false);
    }
}
