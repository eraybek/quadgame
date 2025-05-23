using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBehavior : IEnemyAttackBehavior
{
    private float bulletMaxLifetime = 5f;

    public void Attack(EnemyController controller)
    {
        controller.FireTimer += Time.deltaTime;

        if (controller.FireTimer < controller.EnemyData.attackInterval)
        {
            Debug.Log("[Attack] Saldırı aralığı dolmadı, çıkılıyor.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            controller.transform.position,
            controller.EnemyData.attackRange,
            controller.FlammableLayer
        );

        Debug.Log("[Attack] Bulunan collider sayısı: " + hits.Length);

        List<Transform> flammableTiles = new();

        foreach (var hit in hits)
        {
            GridTileRuntime tile = hit.GetComponentInParent<GridTileRuntime>();
            if (tile != null)
            {
                if (tile.FireStatus == TileFireStatus.Flammable)
                {
                    flammableTiles.Add(tile.transform);
                }
            }
        }

        if (flammableTiles.Count == 0)
        {
            Debug.Log("[Attack] Hiç uygun hedef yok, çıkılıyor.");
            return;
        }

        Transform target = flammableTiles[Random.Range(0, flammableTiles.Count)];
        Debug.Log("[Attack] Hedef seçildi: " + target.name);

        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(
            "FireBullet",
            controller.FirePoint.position,
            controller.FirePoint.rotation
        );

        if (bullet == null)
        {
            Debug.LogError("[Attack] Mermi pool'dan alınamadı.");
            return;
        }

        Debug.Log("[Attack] Mermi spawn edildi. Coroutine başlatılıyor...");

        controller.StartCoroutine(MoveBullet(bullet, target.position, controller.EnemyData.projectileData));

        controller.FireTimer = 0f;
    }


    private System.Collections.IEnumerator MoveBullet(GameObject bullet, Vector3 targetPosition, ProjectileData data)
    {
        float timer = 0f;

        while (timer < bulletMaxLifetime)
        {
            if (bullet == null || !bullet.activeInHierarchy)
                yield break;

            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                targetPosition,
                data.speed * Time.deltaTime
            );

            bullet.transform.forward = (targetPosition - bullet.transform.position).normalized;

            if (Vector3.Distance(bullet.transform.position, targetPosition) < data.impactRadius)
            {
                Debug.Log("[MoveBullet] Hedefe ulaştı, pool'a dönülüyor.");

                Collider[] hits = Physics.OverlapSphere(bullet.transform.position, data.impactRadius);
                foreach (var hit in hits)
                {
                    GridTileRuntime tile = hit.GetComponentInParent<GridTileRuntime>();
                    if (tile != null && tile.FireStatus == TileFireStatus.Flammable)
                        tile.Ignite(GameObject.FindWithTag("BaseTarget")?.transform);
                }

                ObjectPooler.Instance.ReturnToPool("FireBullet", bullet);
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        ObjectPooler.Instance.ReturnToPool("FireBullet", bullet);
    }
}
