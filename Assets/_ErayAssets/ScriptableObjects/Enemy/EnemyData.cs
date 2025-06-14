using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyData", fileName = "NewEnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Header("Core Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 3.5f;
    public float stopRange = 1.5f;
    public Vector3 size = Vector3.one;

    [Header("Attack Settings")]
    public EnemyType type; // enum: Melee / Ranger
    public float attackRange = 2f; // Melee için dar, Ranger için geniş olabilir
    public float attackInterval = 1.5f;

    [Header("Projectile (Ranged Only)")]
    public ProjectileData projectileData;

    [Header("FX")]
    public GameObject deathEffect;
    [Header("XP Drop")]
    public int xpDrop;
}
