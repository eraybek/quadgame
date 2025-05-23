using UnityEngine;

public class GridVisualObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BaseZone"))
        {
            var tile = GetComponentInParent<GridTileRuntime>();
            if (tile != null)
            {
                Debug.Log("Tile base'e ulaştı!");
                //tile.OnBaseZoneHit(); // Parent’ta yazacağın fonksiyonu tetikle
            }
        }
    }
}
