using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private int damageToBase = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Base"))
        {
            BaseHealth baseHealth = other.GetComponent<BaseHealth>();
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(damageToBase);
            }

            // Düşman yok olsun
            Destroy(gameObject); // veya pooling'e geri gönder
        }
    }
}
