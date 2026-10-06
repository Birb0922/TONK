using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    public Rigidbody2D rb;
    public GameObject spinny;
    private Camera mainCamera;

    private Vector2 moveInput;
    private Vector2 turnInput;
    private Vector2 rotateInput;
    private Vector2 direction;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();
        turnInput = InputSystem.actions["Turn"].ReadValue<Vector2>();
        rotateInput = mainCamera.ScreenToWorldPoint(InputSystem.actions["Look"].ReadValue<Vector2>());

        direction = new Vector2(
            rotateInput.x - transform.position.x,
            rotateInput.y - transform.position.y
        );
    }

    void FixedUpdate()
    {
        float rot = -turnInput.x * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rot);

        Vector2 moveVector = transform.up * moveInput.y * moveSpeed;
        rb.MovePosition(rb.position + moveVector * Time.fixedDeltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spinny.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}