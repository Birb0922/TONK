using UnityEngine;

public abstract class AgentMovement : MonoBehaviour, IDamageable
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public float bulletspeed = 10f;
    public float fireCooldown = 1f;
    float fireTimer;
    public float visionRange = 10f;

    public LayerMask obstacleMask;
    public Vector2 waypoint;
    public bool hasWaypoint = false;
    public Rigidbody2D rb;
    public Transform spinny;
    public GameObject bulletfab;
    public Transform firingpoint;
    public VisionSensor vision;
    protected Collider2D bodyCollider;
    private ContactFilter2D obstacleFilter;
    private RaycastHit2D[] obstacleHits = new RaycastHit2D[5];

    protected virtual void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        bodyCollider = GetComponent<Collider2D>();

        obstacleFilter = new ContactFilter2D();
        obstacleFilter.SetLayerMask(obstacleMask);
        obstacleFilter.useTriggers = false;
    }

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

    protected void MoveToward(Vector2 target)
    {
        Vector2 direction = target - rb.position;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg - 90f;

        float angle = Mathf.DeltaAngle(rb.rotation, targetAngle);

        float turn = Mathf.Clamp(
            angle,
            -rotationSpeed * Time.fixedDeltaTime,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(rb.rotation + turn);

        if (Mathf.Abs(angle) > 20f)
        {
            return;
        }

        Vector2 movement = (Vector2)transform.up
            * moveSpeed * Time.fixedDeltaTime;

        float distance = movement.magnitude;

        if (bodyCollider != null)
        {
            int hitCount = bodyCollider.Cast(
                movement.normalized,
                obstacleFilter,
                obstacleHits,
                distance + 0.05f
            );

            if (hitCount > 0)
            {
                return;
            }
        }

        rb.MovePosition(rb.position + movement);
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
    protected void FindWaypoint()
    {
        for (int i = 0; i < 20; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(visionRange * 0.7f, visionRange);

            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector2 candidate = rb.position + offset * distance;

            if (Physics2D.OverlapCircle(candidate, 0.5f, obstacleMask) != null)
            {
                continue;
            }

            if (Physics2D.Linecast(rb.position, candidate, obstacleMask))
            {
                continue;
            }

            waypoint = candidate;
            hasWaypoint = true;
            return;
        }

        hasWaypoint = false;
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