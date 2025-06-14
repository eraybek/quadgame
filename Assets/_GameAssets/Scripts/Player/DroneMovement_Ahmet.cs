using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 45f;

    private float yaw = 0f;   // Y ekseni dönüş
    private float pitch = 0f; // X ekseni dönüş

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f); // Roll sabit 0
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 moveDirection = transform.TransformDirection(inputDirection);
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // Yukarı (Space)
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }

        // Aşağı (Ctrl)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
        }
    }
}
