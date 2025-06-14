using UnityEngine;

public class RefillIndicator : MonoBehaviour
{
    [SerializeField] private Transform drone; // Drone objesini buraya at
    [SerializeField] private float showRadius = 10f;
    [SerializeField] private GameObject visualIndicator; // Transparan alan

    private void Update()
    {
        if (drone == null || visualIndicator == null)
            return;

        float distance = Vector3.Distance(transform.position, drone.position);

        if (distance <= showRadius)
        {
            if (!visualIndicator.activeSelf)
                visualIndicator.SetActive(true);
        }
        else
        {
            if (visualIndicator.activeSelf)
                visualIndicator.SetActive(false);
        }
    }
}
