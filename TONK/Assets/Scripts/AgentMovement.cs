using UnityEngine;

public class AgentMovement: MonoBehaviour, IDamageable
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public float firerate;
    public float tbs;
    public float bulletspeed = 1f;

    public Rigidbody2D rb;
    public GameObject spinny;
    public GameObject bulletfab;

    public Transform firingpoint;
    public BoxCollider2D hitbox;

    private Vector2 moveInput;
    private Vector2 turnInput;
    private Vector2 rotateInput;
    private Vector2 direction;


    void shoot()
    {
        GameObject bullet = Instantiate(bulletfab, firingpoint.position, spinny.transform.rotation);
        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
        brb.linearVelocity = direction.normalized * bulletspeed;
    }

    void move()
    {

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

    }

    void Update()
    {
        
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