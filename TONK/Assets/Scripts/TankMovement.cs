using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour, IDamageable
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public float firerate = 5f;
    public float bulletspeed = 20f;

    public Rigidbody2D rb;
    public GameObject spinny;
    public GameObject bulletfab;
    public Transform firingpoint;
    public BoxCollider2D hitbox;
    private Camera mainCamera;

    private Vector2 moveInput;
    private Vector2 turnInput;
    private Vector2 rotateInput;
    private Vector2 direction;


    void shoot()
    {
        GameObject bullet = Instantiate(bulletfab, firingpoint.position, firingpoint.rotation);
        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
        brb.linearVelocity = firingpoint.right * bulletspeed;
    }

    public void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (InputSystem.actions["Shoot"].WasPressedThisFrame())
        {
            shoot();
        }
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