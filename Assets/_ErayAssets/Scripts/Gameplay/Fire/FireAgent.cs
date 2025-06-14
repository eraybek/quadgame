using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class FireAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform baseTarget;
    private GridTileRuntime currentTargetTile;

    public void Initialize(Transform baseTargetRef)
    {
        baseTarget = baseTargetRef;
        agent = GetComponent<NavMeshAgent>();

        if (baseTarget == null)
        {
            Debug.LogError("[FireAgent] Base target null!");
            Destroy(gameObject);
            return;
        }

        // Uygun tile'ı bul
        var candidates = FindObjectsByType<GridTileRuntime>(FindObjectsSortMode.None)
            .Where(t =>
                t.FireStatus == TileFireStatus.Flammable &&
                Vector3.Distance(t.transform.position, baseTarget.position) <
                Vector3.Distance(transform.position, baseTarget.position))
            .OrderBy(t => Vector3.Distance(t.transform.position, transform.position))
            .ToArray();

        if (candidates.Length == 0)
        {
            Debug.Log("[FireAgent] Hiçbir uygun hedef tile bulunamadı.");
            Destroy(gameObject);
            return;
        }

        currentTargetTile = candidates[0];
        agent.SetDestination(currentTargetTile.transform.position);

        Debug.Log($"[FireAgent] Hedef tile: {currentTargetTile.name}");
    }

    private void Update()
    {
        if (agent == null || currentTargetTile == null)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (currentTargetTile.FireStatus == TileFireStatus.Flammable)
            {
                currentTargetTile.Ignite(baseTarget);
                Debug.Log($"[FireAgent] {currentTargetTile.name} yakıldı.");
            }
            else
            {
                Debug.Log($"[FireAgent] {currentTargetTile.name} artık Flammable değil, Ignite iptal.");
            }

            Destroy(gameObject);
        }
    }
}
