using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class GridTileRuntime : MonoBehaviour
{
    [Header("Tile Status")]
    [SerializeField] private TileFireStatus _fireStatus;
    [SerializeField] private FlammableStatus _flammableStatus;
    [SerializeField] private TileMovementStatus _movementStatus;
    [SerializeField] private TileSpecialStatus _specialStatus;

    [Header("Visual Settings")]
    [SerializeField] private Renderer _tileRenderer;

    [Header("Fire Settings")]
    [SerializeField] private GameObject fireVFX;
    private Material _flammableMaterial;
    private Material _burningMaterial;
    private Material _burnedOutMaterial;

    [Header("Water Settings")]
    [SerializeField] private float extinguishProgress;

    private FireSpreadData fireSpreadData;

    public Vector2Int GridCoords { get; private set; }

    private float burnTimer;
    private float burnOutTimer;

    // private bool isBeingExtinguished = false;
    private bool hasSpread = false;

    private float burnTime;
    private Transform baseTarget;
    private float baseDamageTimer = 0f;
    private float baseDamageInterval = 3f; // her 3 saniyede bir

    public TileFireStatus FireStatus
    {
        get => _fireStatus;
        set => _fireStatus = value;
    }
    public FlammableStatus FlammableStatus => _flammableStatus;
    public TileMovementStatus MovementStatus => _movementStatus;
    public TileSpecialStatus SpecialStatus => _specialStatus;
    public bool IsBurning => _fireStatus == TileFireStatus.Burning;


    private void Awake()
    {
        if (_tileRenderer == null)
            _tileRenderer = GetComponentInChildren<Renderer>();

        if (fireVFX == null)
        {
            fireVFX = GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(t => t.CompareTag("FireVFX") && t != transform)?.gameObject;

            fireVFX?.SetActive(false);
        }
    }

    private void Start()
    {
        baseTarget ??= GameObject.FindWithTag("BaseTarget")?.transform;

        if (baseTarget == null)
            return;
        // Debug.LogError("BaseTarget bulunamadı!");

        if (FireController.Instance == null)
        {
            Debug.LogWarning("FireController.Instance not ready, delaying initialization.");
            return;
        }

        _flammableMaterial = FireController.Instance?.flammableMaterial;
        _burningMaterial = FireController.Instance?.burningMaterial;
        _burnedOutMaterial = FireController.Instance?.burnedOutMaterial;

        fireSpreadData = FireController.Instance?.GetSpreadDataFor(_flammableStatus);
    }

    private void Update()
    {
        if (_fireStatus == TileFireStatus.Burning)
        {
            burnTimer += Time.deltaTime;

            if (CompareTag("BaseGrid"))
            {
                baseDamageTimer += Time.deltaTime;

                if (baseDamageTimer >= baseDamageInterval)
                {
                    baseDamageTimer = 0f;
                    Debug.Log($"🔥 {name} → Base'e 1 hasar verildi!");
                    BaseHealth.Instance.TakeDamage(1);
                }

                BaseFireManager.Instance.RegisterBurningBaseGrid(this);
                return;
            }

            // 🔥 Yayılma kontrolü (BaseGrid olmayanlar için)
            if (!hasSpread && burnTimer >= burnTime)
            {
                hasSpread = true;
                SpreadFire();
            }

            // 🔥 Sönme (BaseGrid olmayanlar için)
            if (burnTimer >= burnTime)
            {
                _fireStatus = TileFireStatus.BurnedOut;
                burnOutTimer = 0f;
                UpdateVisual();
            }
        }
        else if (_fireStatus == TileFireStatus.BurnedOut)
        {
            bool isBaseGrid = CompareTag("BaseGrid");
            bool isGrass = CompareTag("Grass");

            Debug.Log("BurnOut: " + gameObject.name + " " + isBaseGrid + " " + isGrass);
            if (!isBaseGrid && !isGrass)
            {
                Debug.Log("YOK EDİLDİ !!!!!");
                Destroy(gameObject);
                return;
            }

            burnOutTimer += Time.deltaTime;

            if (burnOutTimer >= fireSpreadData.burnOutTime)
            {
                _fireStatus = TileFireStatus.Flammable;
                burnOutTimer = 0f;
                UpdateVisual();
            }
        }

    }

    public void Ignite(Transform target)
    {
        if (_fireStatus != TileFireStatus.Flammable)
        {
            return;
        }

        fireSpreadData = FireController.Instance?.GetSpreadDataFor(_flammableStatus);
        if (fireSpreadData == null)
        {
            return;
        }

        _fireStatus = TileFireStatus.Burning;
        baseTarget = target;
        burnTimer = 0f;
        hasSpread = false;
        burnTime = fireSpreadData.burnTime;

        UpdateVisual();
    }

    private void SpreadFire()
    {
        float currentDistance = Vector3.Distance(transform.position, baseTarget.position);

        Collider[] hits = Physics.OverlapBox(transform.position, Vector3.one * 1f);

        var next = hits
            .Select(h => h.GetComponentInParent<GridTileRuntime>())
            .Where(t => t != null && t.FireStatus == TileFireStatus.Flammable)
            .OrderBy(t => Vector3.Distance(t.transform.position, baseTarget.position))
            .FirstOrDefault();

        if (next != null)
        {
            float nextDistance = Vector3.Distance(next.transform.position, baseTarget.position);

            // ✅ Sadece daha yakınsa yayıl
            if (nextDistance < currentDistance)
            {
                next.Ignite(baseTarget);
            }
            else
            {
                // Debug.Log($"🔥 {name} yayılmayı durdurdu. Daha yakına gidemiyor.");
            }
        }
    }


    public void UpdateVisual()
    {
        Debug.Log($"[GridTile] Visual update: {_fireStatus}");

        if (_tileRenderer == null) return;

        switch (_fireStatus)
        {
            case TileFireStatus.Flammable:
                fireVFX?.SetActive(false);
                break;

            case TileFireStatus.Burning:
                fireVFX?.SetActive(true);
                break;

            case TileFireStatus.BurnedOut:
                fireVFX?.SetActive(false);
                break;

            case TileFireStatus.Wet:
            default:
                fireVFX?.SetActive(false);
                break;
        }

        var visuals = GetComponentsInChildren<Transform>(true);

        foreach (var t in visuals)
        {
            if (t.CompareTag("Flammable_Visual"))
            {
                t.gameObject.SetActive(_fireStatus == TileFireStatus.Flammable || _fireStatus == TileFireStatus.None);
            }
            else if (t.CompareTag("Burned_Visual"))
            {
                t.gameObject.SetActive(_fireStatus == TileFireStatus.Burning || _fireStatus == TileFireStatus.BurnedOut);
            }
        }
    }


    public void Extinguish()
    {
        if (!IsBurning)
        {
            Debug.LogWarning("[GridTileRuntime] Extinguish ama tile zaten yanmıyor.");
            return;
        }

        _fireStatus = TileFireStatus.BurnedOut;
        burnTimer = 0f;
        baseDamageTimer = 0f;

        if (CompareTag("BaseGrid"))
        {
            BaseFireManager.Instance.UnregisterBurningBaseGrid(this);
        }

        UpdateVisual();
    }




    public void Initialize(TileFireStatus fire, FlammableStatus flammable, TileMovementStatus move, TileSpecialStatus special)
    {
        _fireStatus = fire;
        _flammableStatus = flammable;
        _movementStatus = move;
        _specialStatus = special;
        UpdateVisual();
    }
}
