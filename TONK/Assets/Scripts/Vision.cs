using UnityEngine;

public class Vision : MonoBehaviour
{
    public float visionRange = 12f;
    public float visionAngle = 90f;
    public LayerMask obstacleMask;

    public Transform visibleTarget;

    private float checkTimer;

    public LineRenderer lineRenderer;
    public int rayCount = 60;

    void Update()
    {
        checkTimer -= Time.deltaTime;

        if (checkTimer <= 0)
        {
            checkTimer = 0.2f;
            FindTarget();
        }
    }

    void FindTarget()
    {
        visibleTarget = null;
        float closestDistance = visionRange;

        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tanks");

        foreach (GameObject tank in tanks)
        {
            if (tank == gameObject)
            {
                continue;
            }

            Vector2 direction = (Vector2)tank.transform.position - (Vector2)transform.position;

            float distance = direction.magnitude;

            // Check vision range
            if (distance > visionRange)
            {
                continue;
            }

            // Check vision angle
            float angle = Vector2.Angle(transform.up, direction);

            if (angle > visionAngle / 2f)
            {
                continue;
            }

            // Check for walls blocking vision
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, obstacleMask);

            if (hit.collider != null)
            {
                continue;
            }

            // Keep the closest visible target
            if (distance < closestDistance)
            {
                closestDistance = distance;
                visibleTarget = tank.transform;
            }
        }
    }

    void DrawVision()
    {
        Vector3 origin = transform.position;

        lineRenderer.positionCount = rayCount + 2;

        lineRenderer.SetPosition(0, origin);

        float startAngle = -visionAngle / 2f;
        float angleStep = visionAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 direction =
                Quaternion.Euler(0, 0, angle) * transform.up;

            RaycastHit2D hit = Physics2D.Raycast(origin,direction,visionRangeobstacleMask);

            Vector3 endpoint;

            if (hit.collider != null)
            {
                endpoint = hit.point;
            }
            else
            {
                endpoint = origin + (Vector3)(direction * visionRange);
            }

            lineRenderer.SetPosition(i + 1, endpoint);
        }
        lineRenderer.SetPosition(rayCount + 1, origin);
    }

    void LateUpdate()
    {
        DrawVision();
    }
}