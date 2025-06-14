using UnityEngine;

public class BulletDamage_Ahmet : MonoBehaviour
{
    [SerializeField] private int damageAmount = 25;

    private void OnCollisionEnter(Collision collision)
    {
        Enemy_Ahmet enemy = collision.collider.GetComponent<Enemy_Ahmet>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
        }

        // Çarpınca mermiyi havuza geri gönder
        gameObject.SetActive(false);
    }
}
