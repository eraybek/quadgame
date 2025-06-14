using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    public EnemyData enemyData;
    public Transform FirePoint => firePoint;
    public LayerMask FlammableLayer => flammableLayer;
    public float FireTimer { get => fireTimer; set => fireTimer = value; }
    public EnemyData EnemyData => enemyData;
    public Transform BaseTarget => baseTarget;
    private Collider baseTargetCollider;
    public Animator animator;


    private int currentHealth;
    private Transform baseTarget;
    private float fireTimer = 0f;
    private bool isActivated = false;
    private NavMeshAgent agent;

    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask flammableLayer;

    private IEnemyAttackBehavior attackBehavior;

    [SerializeField] private Renderer enemyRenderer; // düşmanın Mesh Renderer’ı
    [SerializeField] private float flashDuration = 0.1f; // ne kadar parlayacak

    private Material originalMaterial;
    private Material flashMaterial;
    private bool isDead;
    public float xpAmount;


    private void Start()
    {
        animator = GetComponent<Animator>();

        // Orijinal materyali kaydet
        originalMaterial = enemyRenderer.material;

        // Flash için kırmızı materyal oluştur
        flashMaterial = new Material(originalMaterial);
        flashMaterial.color = Color.red;

        currentHealth = enemyData.maxHealth;

        baseTarget = GameObject.FindWithTag("BaseTarget")?.transform;

        if (baseTarget == null)
        {
            Debug.LogError("[EnemyController] BaseTarget bulunamadı!");
            enabled = false;
            return;
        }

        baseTargetCollider = baseTarget.GetComponent<Collider>();

        if (baseTargetCollider == null)
        {
            Debug.LogError("[EnemyController] BaseTarget üzerinde Collider yok!");
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();


        transform.localScale = enemyData.size;
        agent.speed = enemyData.moveSpeed;
        agent.stoppingDistance = enemyData.stopRange;


        attackBehavior = GetAttackBehavior(enemyData.type);

        if (attackBehavior == null)
            Debug.LogWarning("[EnemyController] Saldırı davranışı atanamadı.");

        isActivated = true;
    }

    private void Update()
    {
        if (!isActivated) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("[EnemyController] Düşman NavMesh üzerinde değil!");
            return;
        }

        animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f);

        if (IsDistanceLargerThanFireRange())
        {
            Vector3 closestPoint = baseTargetCollider.ClosestPoint(transform.position);
            Vector3 destination = closestPoint;

            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogWarning("[EnemyController] Hedef NavMesh üzerinde değil!");
            }

            // Opsiyonel: Biraz rastgelelik eklemek istersen
            Vector3 randomOffset = Random.insideUnitSphere * 0.5f;
            randomOffset.y = 0;

            Vector3 targetWithOffset = closestPoint + randomOffset;

            agent.SetDestination(targetWithOffset);
        }
        else
        {
            agent.ResetPath();
            attackBehavior?.Attack(this);
        }

    }

    private bool IsDistanceLargerThanFireRange()
    {
        Vector3 closestPoint = baseTargetCollider.ClosestPoint(transform.position);

        float distance = Vector3.Distance(transform.position, closestPoint);
        float threshold = enemyData.stopRange + 0.8f;

        return distance > threshold;
    }


    private IEnemyAttackBehavior GetAttackBehavior(EnemyType type)
    {
        return type switch
        {
            EnemyType.Ranger => new RangedAttackBehavior(),
            EnemyType.Melee => new MeleeAttackBehavior(),
            _ => null
        };
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"[EnemyController] Hasar alındı: {damage}, Kalan can: {currentHealth}");

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Debug.Log("[EnemyController] Can bitti. Ölüyor...");
            Die();
        }
    }

    private void Die()
    {
        // 🔒 Hareketi durdur
        isActivated = false;
        agent.ResetPath();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (animator != null)
        {
            animator.ResetTrigger("attackTrigger");
            animator.SetBool("isMoving", false);
            animator.SetTrigger("dieTrigger");
        }

        isDead = true;

        xpAmount = enemyData.xpDrop;

        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(1.5f); // Animasyon süresi kadar bekle
        Destroy(gameObject);
    }

    private IEnumerator FlashRed()
    {
        if (enemyRenderer != null && !isDead)
        {
            enemyRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashDuration);
            enemyRenderer.material = originalMaterial;
        }
    }


}
