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


    private int currentHealth;
    private Transform baseTarget;
    private float fireTimer = 0f;
    private bool isActivated = false;
    private NavMeshAgent agent;

    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask flammableLayer;

    private IEnemyAttackBehavior attackBehavior;

    private void Start()
    {
        Debug.Log($"[EnemyController] Başlatılıyor: {gameObject.name}");

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

        Debug.Log($"[EnemyController] BaseTarget bulundu: {baseTarget.name}");

        transform.localScale = enemyData.size;
        agent.speed = enemyData.moveSpeed;
        agent.stoppingDistance = enemyData.stopRange;

        Debug.Log($"[EnemyController] Hız: {agent.speed}, StopRange: {agent.stoppingDistance}");

        attackBehavior = GetAttackBehavior(enemyData.type);

        if (attackBehavior == null)
            Debug.LogWarning("[EnemyController] Saldırı davranışı atanamadı.");
        else
            Debug.Log($"[EnemyController] Saldırı davranışı atandı: {enemyData.type}");

        isActivated = true;
        Debug.Log("[EnemyController] Aktif edildi.");
    }

    private void Update()
    {
        if (!isActivated) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("[EnemyController] Düşman NavMesh üzerinde değil!");
            return;
        }

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
        currentHealth -= damage;
        Debug.Log($"[EnemyController] Hasar alındı: {damage}, Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("[EnemyController] Can bitti. Ölüyor...");
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[EnemyController] Die() çağrıldı, düşman yok ediliyor.");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetInitialTarget(Transform target)
    {
        baseTarget = target;
        isActivated = true;
        Debug.Log($"[EnemyController] Yeni hedef atandı: {target.name}");
    }
}
