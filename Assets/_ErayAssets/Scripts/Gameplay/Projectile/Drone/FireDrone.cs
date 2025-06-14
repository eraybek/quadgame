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
    [SerializeField] private float fireRate = 0.2f; // saniyede 5 mermi
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private AudioClip bulletFireClip;
    [SerializeField] private AudioClip waterFireClip;
    [SerializeField] private ParticleSystem muzzleFlash;

    private float fireCooldown = 0f;


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
        if (GameOverManager.IsGameOver) return;

        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(1) && fireCooldown <= 0f)
        {
            currentWeapon = WeaponType.Water;
            TryFire(currentWeapon);
            fireCooldown = fireRate;        
        }

        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            currentWeapon = WeaponType.Bullet;
            TryFire(currentWeapon);
            fireCooldown = fireRate;
        }
    }

    private void TryFire(WeaponType currentWeapon)
    {
        if (currentWeapon == WeaponType.Bullet && !droneStatus.HasAmmo()) return;
        if (currentWeapon == WeaponType.Water && !droneStatus.HasWaterAmmo()) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 fireDirection;
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
            return;

        fireDirection = (hit.point - firePoint.position).normalized;

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

        if (fireAudioSource != null)
        {
            AudioClip selectedClip = currentWeapon == WeaponType.Bullet ? bulletFireClip : waterFireClip;
            fireAudioSource.PlayOneShot(selectedClip);
        }


        if (currentWeapon == WeaponType.Bullet)
        {
            if (muzzleFlash != null && currentWeapon == WeaponType.Bullet)
                muzzleFlash.Play();

            droneStatus.ConsumeAmmo(1);
        }
        else
            droneStatus.ConsumeWater(1);


    }

}