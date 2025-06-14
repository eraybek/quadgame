using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform playerChildTransform;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject _cameraRig;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float cameraFollowSmoothTime = 0.1f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    private float mouseSensitivity = 1f; // default

    private float _horizontal;
    private float _vertical;
    private Vector3 _cameraVelocity;
    private Camera _mainCam;
    private Vector3 lastLookDirection = Vector3.forward;


    private void Awake()
    {
        _mainCam = _cameraRig.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (GameOverManager.IsGameOver) return;

        if (PauseManager.IsPaused) return;

        GetMovementInputs();
    }

    private void FixedUpdate()
    {
        if (GameOverManager.IsGameOver) return;

        if (PauseManager.IsPaused) return;

        SetMovement();
        SetRotation();
    }

    private void LateUpdate()
    {
        if (GameOverManager.IsGameOver) return;

        if (PauseManager.IsPaused) return;

        SetRotation();
        FollowCamera();
    }


    private void FollowCamera()
    {
        if (_cameraRig.transform == null) return;

        Vector3 targetPosition = transform.position;

        _cameraRig.transform.position = Vector3.SmoothDamp(
            _cameraRig.transform.position,
            targetPosition,
            ref _cameraVelocity,
            cameraFollowSmoothTime
        );

        // Kamera da karakterin baktığı yöne baksın
        _cameraRig.transform.rotation = Quaternion.Slerp(
            _cameraRig.transform.rotation,
            Quaternion.LookRotation(playerChildTransform.forward, Vector3.up),
            Time.deltaTime * 10f // takip hızı
        );

    }


    private void GetMovementInputs()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
    }

    private void SetMovement()
    {
        playerRigidbody.linearVelocity = GetNewVelocity();
    }

    private void SetRotation()
    {
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (!PauseManager.IsPaused)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000000f, groundLayerMask))
            {
                Vector3 lookTarget = hitInfo.point;
                Vector3 direction = lookTarget - playerChildTransform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    lastLookDirection = direction;
                }
            }
        }

        if (lastLookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastLookDirection);
            Vector3 yawOnly = new Vector3(0, targetRotation.eulerAngles.y, 0);
            float fixedPitch = 15f;
            float dynamicRoll = -_horizontal * 15f;

            Vector3 finalEuler = new Vector3(fixedPitch, yawOnly.y, dynamicRoll);
            Quaternion finalRotation = Quaternion.Euler(finalEuler);
            playerChildTransform.rotation = Quaternion.Slerp(
                playerChildTransform.rotation,
                finalRotation,
                Time.fixedDeltaTime * rotationSpeed
            );
        }
    }



    private Vector3 GetNewVelocity()
    {
        Vector3 inputDirection = new Vector3(_horizontal, 0f, _vertical).normalized;
        Vector3 worldDirection = playerChildTransform.TransformDirection(inputDirection);

        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier;

        return worldDirection * currentSpeed * Time.fixedDeltaTime;
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = Mathf.Clamp(value, 0.1f, 5f);
    }


}