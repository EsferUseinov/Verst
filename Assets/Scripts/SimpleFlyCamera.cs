using UnityEngine;

public class SimpleFlyCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostMultiplier = 2.5f;

    [Header("Look (hold RMB)")]
    [SerializeField] private float lookSensitivity = 2f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= boostMultiplier;
        }

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        if (Input.GetKey(KeyCode.E))
        {
            move += Vector3.up;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            move += Vector3.down;
        }

        transform.position += move * speed * Time.deltaTime;
    }

    private void HandleLook()
    {
        if (!Input.GetMouseButton(1))
        {
            return;
        }

        yaw += Input.GetAxis("Mouse X") * lookSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }
}