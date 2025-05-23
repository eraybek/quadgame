using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnZone : MonoBehaviour
{
    private Collider spawnArea;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    public Vector3 GetRandomPointInZone()
    {
        Bounds bounds = spawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, bounds.center.y, z);
    }
}
