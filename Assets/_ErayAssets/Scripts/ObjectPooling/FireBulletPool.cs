using System.Collections.Generic;
using UnityEngine;

public class FireBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    public static FireBulletPool Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true); // → sahneye geri dönüyor
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(true);
            return bullet;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); // → tekrar havuza alınıyor
        bulletPool.Enqueue(bullet);
    }

}
