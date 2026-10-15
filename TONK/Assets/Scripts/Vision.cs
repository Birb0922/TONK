using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    public float visionRange = 12f;
    public float visionAngle = 90f;
    public int rayCount = 60;

    public LayerMask visionMask;
    public LineRenderer lineRenderer;

    public Transform visibleTarget;

    void LateUpdate()
    {
        ScanVision();
    }

    void ScanVision()
    {
        visibleTarget = null;

        Vector3 origin = transform.position;
        float closestDistance = visionRange;

        lineRenderer.positionCount = rayCount + 2;
        lineRenderer.SetPosition(0, origin);

        float startAngle = -visionAngle / 2f;
        float angleStep = visionAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.up;

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin,direction,visionRange,visionMask);

            RaycastHit2D nearestHit = new RaycastHit2D();
            bool hitSomething = false;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                {
                    continue;
                }

                if (!hitSomething || hit.distance < nearestHit.distance)
                {
                    nearestHit = hit;
                    hitSomething = true;
                }
            }

            Vector3 endpoint;

            if (hitSomething)
            {
                endpoint = nearestHit.point;

                if (nearestHit.collider.CompareTag("Tanks"))
                {
                    if (nearestHit.distance < closestDistance)
                    {
                        closestDistance = nearestHit.distance;
                        visibleTarget = nearestHit.collider.transform;
                    }
                }
            }
            else
            {
                endpoint = origin + (Vector3)(direction * visionRange);
            }

            lineRenderer.SetPosition(i + 1, endpoint);
        }

        lineRenderer.SetPosition(rayCount + 1, origin);
    }
}