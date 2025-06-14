using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public ProjectileData fireBullet;
    public Vector3 targetPosition;

    private float lifeTimer;
    private const float MaxLifetime = 5f;

    private void OnEnable()
    {
        lifeTimer = 0f;

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.forward = direction;
    }


    private void Update()
    {
        // Güvenlik: fireBullet atanmadıysa hata ver
        if (fireBullet == null)
        {
            Debug.LogError("FireBullet: fireBullet (ProjectileData) null!");
            FireBulletPool.Instance.ReturnBullet(gameObject);
            return;
        }

        // Hareket
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, fireBullet.speed * Time.deltaTime);

        // Hedefe ulaştı mı?
        if (Vector3.Distance(transform.position, targetPosition) < fireBullet.impactRadius)
        {
            // Debug.Log("Hedefe ulaştı.");
            Collider[] hits = Physics.OverlapSphere(transform.position, fireBullet.impactRadius);

            foreach (var hit in hits)
            {
                GridTileRuntime tile = hit.GetComponentInParent<GridTileRuntime>();
                if (tile != null && tile.FireStatus == TileFireStatus.Flammable)
                {
                    // Debug.Log("Ignite etti.");

                    tile.Ignite(GameObject.FindWithTag("BaseTarget")?.transform);

                    // EŞ ZAMANLI: Çarpışma varsa hemen pool'a dön
                    if (fireBullet.destroyOnImpact)
                    {
                        FireBulletPool.Instance.ReturnBullet(gameObject);
                        return; // Güncellemeyi durdur
                    }
                }
            }

            // Hiç flammable yoksa da yok et (isteğe bağlı)
            FireBulletPool.Instance.ReturnBullet(gameObject);
            return;
        }

        // Timeout (güvenlik)
        lifeTimer += Time.deltaTime;
        if (lifeTimer > MaxLifetime)
        {
            FireBulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     HandleHit(collision.gameObject);
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     HandleHit(other.gameObject);
    // }

    // private void HandleHit(GameObject hitObject)
    // {
    //     if (hitObject.layer == LayerMask.NameToLayer("FlammableGrid"))
    //     {
    //         GridTileRuntime tile = hitObject.GetComponentInParent<GridTileRuntime>();
    //         if (tile != null && tile.FireStatus == TileFireStatus.Flammable)
    //         {
    //             Debug.Log($"🔥 {tile.name} -> FlammableGrid'e çarpıldı. Statüler güncelleniyor.");

    //             tile.Initialize(TileFireStatus.None, FlammableStatus.NormalIgnition, tile.MovementStatus, tile.SpecialStatus);
    //             Destroy(tile.gameObject); // ya da sadece görseli sil
    //         }
    //     }
    // }

}
