using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileData", menuName = "Game Data/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public string bulletName;
    public float speed = 10f;
    public float impactRadius = 0.1f;
    public GameObject visualPrefab;
    public AudioClip fireSound;
    public AudioClip impactSound;
    public bool destroyOnImpact = true;
}