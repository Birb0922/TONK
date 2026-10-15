using UnityEngine;

public abstract class AgentMovement : MonoBehaviour, IDamageable
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public float bulletspeed = 10f;
    public float fireCooldown = 1f;
    float fireTimer;

    public Rigidbody2D rb;
    public Transform spinny;
    public GameObject bulletfab;
    public Transform firingpoint;
    public VisionSensor vision;

    protected virtual void Update()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }

        AgentBehaviour();
    }

    protected abstract void AgentBehaviour();

    protected void MoveForward(float amount)
    {
        Vector2 movement = (Vector2)transform.up * amount * moveSpeed;

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    protected void Turn(float angle)
    {
        float turn = Mathf.Clamp(angle, -rotationSpeed * Time.fixedDeltaTime, rotationSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation + turn);
    }

    protected void AimAt(Vector2 position)
    {
        Vector2 direction = position - rb.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        spinny.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    protected void ShootAt(Vector2 position)
    {
        if (fireTimer > 0)
        {
            return;
        }

        Vector2 direction = position - (Vector2) firingpoint.position;

        GameObject bullet = Instantiate(bulletfab, firingpoint.position, spinny.rotation);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        bulletRb.linearVelocity = direction.normalized * bulletspeed;

        fireTimer = fireCooldown;
    }

    public virtual void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}