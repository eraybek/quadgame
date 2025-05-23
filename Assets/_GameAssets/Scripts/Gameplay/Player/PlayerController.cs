using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform playerChildTransform;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject _cameraRig;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float cameraFollowSmoothTime = 0.1f;

    private float _horizontal;
    private float _vertical;
    private Vector3 _cameraVelocity;
    private Camera _mainCam;


    private void Awake()
    {
        _mainCam = _cameraRig.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        GetMovementInputs();
    }

    private void FixedUpdate()
    {
        SetMovement();
        SetRotation();
    }

    private void LateUpdate()
    {
        SetRotation();     // ROTASYON artık burada
        FollowCamera();    // Zaten burada
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

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayerMask))
        {
            Vector3 lookTarget = hitInfo.point;
            Vector3 direction = lookTarget - playerChildTransform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // 1. Yalnızca YAW (y ekseni) alınır, pitch ve roll sıfırlanır
                Vector3 yawOnly = new Vector3(0, targetRotation.eulerAngles.y, 0);

                // 2. Pitch (x ekseni) sabit 15 derece olacak şekilde ayarlanır
                float fixedPitch = 15f;

                // 3. Roll (z ekseni) efekti istersen ekle, istemezsen 0 yap
                float dynamicRoll = -_horizontal * 15f;

                // 4. Nihai Euler açısını oluştur
                Vector3 finalEuler = new Vector3(fixedPitch, yawOnly.y, dynamicRoll);

                // 5. Uygula
                Quaternion finalRotation = Quaternion.Euler(finalEuler);
                playerChildTransform.rotation = Quaternion.Slerp(
                    playerChildTransform.rotation,
                    finalRotation,
                    Time.fixedDeltaTime * rotationSpeed
                );
            }
        }
    }


    private Vector3 GetNewVelocity()
    {
        Vector3 inputDirection = new Vector3(_horizontal, 0f, _vertical).normalized;
        Vector3 worldDirection = playerChildTransform.TransformDirection(inputDirection);

        return worldDirection * moveSpeed * Time.fixedDeltaTime;
    }

}