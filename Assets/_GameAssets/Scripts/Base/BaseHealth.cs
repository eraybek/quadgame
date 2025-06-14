using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance { get; private set; }

    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] private DamageVignetteController damageVignetteController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // veya log ver, sahnede yalnızca 1 tane olmalı
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Base hit! Current Health: {currentHealth}");

        if (damageVignetteController != null)
            damageVignetteController.ShowVignette();
        else
            Debug.LogWarning("[BaseHealth] damageVignetteController atanmamış!");

        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Base Destroyed!");
            GameOverManager.Instance.TriggerLose();
        }
    }
}
