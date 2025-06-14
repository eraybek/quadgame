using UnityEngine;

public class FireDrone_Ahmet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private string bulletPoolTag = "Bullet";
    [SerializeField] private string waterPoolTag = "Water";
    [SerializeField] private Collider droneCollider;
    [SerializeField] private DroneStatus_Ahmet droneStatus;

    [Header("Shooting")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float maxRayDistance = 100f;
    [SerializeField] private LayerMask aimLayerMask;

    private float fireCooldown;
    private WeaponType currentWeapon = WeaponType.Bullet;

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        // Q tuşuyla silah değiştir
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleWeapon();
        }

        // Ateş et
        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = fireRate;
        }
    }

    private void ToggleWeapon()
    {
        currentWeapon = currentWeapon == WeaponType.Bullet ? WeaponType.Water : WeaponType.Bullet;
        Debug.Log("Switched to: " + currentWeapon);
    }

    private void Fire()
    {
        if (currentWeapon == WeaponType.Bullet && !droneStatus.HasAmmo()) return;
        if (currentWeapon == WeaponType.Water && !droneStatus.HasWaterAmmo()) return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, aimLayerMask)
            ? hit.point
            : ray.GetPoint(maxRayDistance);

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        string poolTag = currentWeapon == WeaponType.Bullet ? bulletPoolTag : waterPoolTag;
        GameObject projectile = ObjectPooler.Instance.SpawnFromPool(poolTag, firePoint.position, Quaternion.LookRotation(direction));

        PooledProjectile_Ahmet pooled = projectile.GetComponent<PooledProjectile_Ahmet>();
        if (pooled != null)
        {
            pooled.Launch(direction, droneCollider);
        }

        // Cephane azalt
        if (currentWeapon == WeaponType.Bullet)
            droneStatus.ConsumeAmmo(1);
        else
            droneStatus.ConsumeWater(1);
    }
}
