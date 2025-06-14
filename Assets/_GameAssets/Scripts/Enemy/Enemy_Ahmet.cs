using UnityEngine;

public class Enemy_Ahmet : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody rb;

    [Header("Visual")]
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("FX")]
    [SerializeField] private GameObject hitParticlePrefab;

    private Color originalColor;

    private void OnEnable()
    {
        currentHealth = maxHealth;

        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 moveDir = -transform.forward; // İleri yönde git
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashDamageEffect());

            if (hitParticlePrefab != null)
                Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
        }
    }

    private void Die()
    {
        if (DroneStatus_Ahmet.Instance != null)
        {
            DroneStatus_Ahmet.Instance.GainXP(10);
        }
        gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FlashDamageEffect()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            enemyRenderer.material.color = originalColor;
        }
    }
}
