using UnityEngine;

public class FireDrone : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private ProjectileData droneProjectileData;
    [SerializeField] private ProjectileData waterProjectileData;
    [SerializeField] private GameObject _cameraRig;
    [SerializeField] private Transform _playerVisualTransform;
    [SerializeField] private LayerMask aimMask;
    [SerializeField] private ParticleSystem waterVFX;


    private enum WeaponType { Bullet, Water }
    private WeaponType currentWeapon = WeaponType.Bullet;

    private DroneStatus droneStatus;

    private void Start()
    {
        droneStatus = GetComponent<DroneStatus>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

    }

    private void Update()
    {
        // Silah değiştirme
        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentWeapon = WeaponType.Bullet;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            currentWeapon = WeaponType.Water;

        if (Input.GetMouseButtonDown(0))
        {
            TryFire();
        }
    }

    private void TryFire()
    {
        if (droneStatus == null || !droneStatus.HasAmmo())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
            return;

        Vector3 targetPoint = hit.point;
        Vector3 fireDirection = (targetPoint - firePoint.position).normalized;

        ProjectileData projectileData = currentWeapon == WeaponType.Bullet ? droneProjectileData : waterProjectileData;

        if (projectileData == null || projectileData.visualPrefab == null)
            return;

        string poolTag = projectileData.visualPrefab.name; // veya başka bir tag alanı eklenebilir
        GameObject projectile = ObjectPooler.Instance.SpawnFromPool(poolTag, firePoint.position, Quaternion.LookRotation(fireDirection));

        if (projectile == null)
            return;

        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = Quaternion.LookRotation(fireDirection);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = fireDirection * projectileData.speed;
        }

        // Debug
        Debug.DrawRay(ray.origin, ray.direction * 300f, Color.red, 2f);
        Debug.DrawLine(firePoint.position, targetPoint, Color.green, 2f);

        droneStatus.ConsumeAmmo();
    }

}