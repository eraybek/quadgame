using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavTargetFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform[] allTargets;
    private Transform chosenTarget;

    private float spawnStartTime;
    private Vector3 spawnOrigin;
    private bool teleportedBack = false;

    [SerializeField] private float maxAliveTime = 20f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnStartTime = Time.time;
        spawnOrigin = transform.position;

        if (chosenTarget != null)
            agent.SetDestination(chosenTarget.position);
    }

    public void SetTargets(Transform[] targets)
    {
        allTargets = targets;
        chosenTarget = GetClosestTarget();

        if (agent != null && chosenTarget != null)
            agent.SetDestination(chosenTarget.position);
    }

    private Transform GetClosestTarget()
    {
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var t in allTargets)
        {
            float dist = Vector3.Distance(transform.position, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    private void Update()
    {
        if (chosenTarget == null) return;

        // Varsa hedefe ulaştıysa yok et
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log($"{gameObject.name} hedefe ulaştı ve yok ediliyor.");
            Destroy(gameObject);
        }

        // Çok uzun süredir yaşıyorsa ve hedefe ulaşamadıysa, ışınla
        if (!teleportedBack && Time.time - spawnStartTime > maxAliveTime)
        {
            Debug.LogWarning($"{gameObject.name} hedefe ulaşamadı, geri ışınlanıyor.");
            agent.ResetPath();
            transform.position = spawnOrigin;
            agent.SetDestination(chosenTarget.position);
            teleportedBack = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShipTargetPoint"))
        {
            Destroy(gameObject);
        }
    }


}
