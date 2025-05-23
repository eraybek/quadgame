using UnityEngine;

[CreateAssetMenu(fileName = "GridObjectSO", menuName = "ScriptableObjects/GridObjectSO")]
public class GridObjectSO : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector3 _manualScale = Vector3.one;
    [SerializeField] private Vector3 _manualRotation = Vector3.zero;
    [SerializeField] private Vector3 _manualPositionOffset = Vector3.zero;
    [SerializeField] private GridCategory _category;
    [SerializeField] private TileFireStatus _fireStatus = TileFireStatus.None;
    [SerializeField] private FlammableStatus _flammableStatus;
    [SerializeField] private TileMovementStatus _movementStatus = TileMovementStatus.None;
    [SerializeField] private TileSpecialStatus _specialStatus = TileSpecialStatus.None;
    [SerializeField] private bool _isActive = true;


    [Header("Tool Ayarları")]
    public bool IsActive => _isActive;
    public GameObject Prefab => _prefab;
    public Vector3 ManualScale => _manualScale;
    public Vector3 ManualRotation => _manualRotation;
    public Vector3 ManualPositionOffset => _manualPositionOffset;
    public GridCategory Category => _category;
    public TileFireStatus FireStatus => _fireStatus;
    public TileMovementStatus MovementStatus => _movementStatus;
    public TileSpecialStatus SpecialStatus => _specialStatus;


    public FlammableStatus FlammableStatus
    {
        get => _flammableStatus;
        set => _flammableStatus = value;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // Tool penceresi varsa repaint et
        TilemapTool.RepaintWindow();
    }
#endif
}
