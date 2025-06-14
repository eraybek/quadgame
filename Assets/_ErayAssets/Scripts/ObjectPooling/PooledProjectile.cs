using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 5;
    [SerializeField] private ParticleSystem waterSplashVFX;
    [SerializeField] private ParticleSystem bloodSplashVFX;


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
                var vfx = Instantiate(bloodSplashVFX, collision.contacts[0].point, Quaternion.identity);

                float totalTime = vfx.main.duration + vfx.main.startLifetime.constant;
                Destroy(vfx.gameObject, totalTime);

                damageable.TakeDamage(damage);
            }


            Disable();
        }

        else if (gameObject.CompareTag("WaterBullet"))
        {
            Debug.Log("[WaterBullet] Çarpışma algılandı: " + collision.gameObject.name);

            if (collision.gameObject.TryGetComponent(out GridTileRuntime gridTileRuntime))
            {
                Debug.Log("[WaterBullet] GridTileRuntime bulundu.");

                if (gridTileRuntime.FireStatus == TileFireStatus.Burning)
                {
                    Debug.Log("[WaterBullet] Tile yanıyor, söndürülmeye çalışılıyor.");
                    gridTileRuntime.Extinguish();
                }
                else
                {
                    Debug.Log("[WaterBullet] Tile yanmıyor, hiçbir işlem yapılmadı.");
                }
            }
            else
            {
                Debug.Log("[WaterBullet] GridTileRuntime bulunamadı.");
            }

            if (waterSplashVFX != null)
            {
                var vfx = Instantiate(waterSplashVFX, collision.contacts[0].point, Quaternion.identity);
                float totalTime = vfx.main.duration + vfx.main.startLifetime.constant;
                Destroy(vfx.gameObject, totalTime);
            }

            Disable();
        }

    }

}
