using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/FireSpreadData")]
public class FireSpreadData : ScriptableObject
{
    public float burnTime;
    public float burnOutTime;
    public GameObject burnedTilePrefab;
}
