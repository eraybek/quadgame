using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 5;

    private void OnEnable()
    {
        CancelInvoke(); // olası önceki çağrıları sıfırla
        Invoke(nameof(Disable), lifeTime);
    }

    private void Disable()
    {
        ObjectPooler.Instance.ReturnToPool(gameObject.tag, gameObject);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("DroneBullet"))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
            Disable();

        }
    }

}
