using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    public float visionRange = 12f;
    public float visionAngle = 90f;
    public int rayCount = 60;

    public LayerMask tankMask;
    public LayerMask obstacleMask;

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

            Vector2 direction =
                Quaternion.Euler(0, 0, angle) * transform.up;

            RaycastHit2D[] tankHits = Physics2D.RaycastAll(
                origin,
                direction,
                visionRange,
                tankMask
            );

            RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(
                origin,
                direction,
                visionRange,
                obstacleMask
            );

            RaycastHit2D nearestTank = new RaycastHit2D();
            bool foundTank = false;

            foreach (RaycastHit2D hit in tankHits)
            {
                if (hit.collider.transform == transform ||
                    hit.collider.transform.IsChildOf(transform.root))
                {
                    continue;
                }

                if (!foundTank || hit.distance < nearestTank.distance)
                {
                    nearestTank = hit;
                    foundTank = true;
                }
            }

            RaycastHit2D nearestObstacle = new RaycastHit2D();
            bool foundObstacle = false;

            foreach (RaycastHit2D hit in obstacleHits)
            {
                if (hit.collider.transform == transform ||
                    hit.collider.transform.IsChildOf(transform.root))
                {
                    continue;
                }

                if (!foundObstacle ||
                    hit.distance < nearestObstacle.distance)
                {
                    nearestObstacle = hit;
                    foundObstacle = true;
                }
            }

            Vector3 endpoint =
                origin + (Vector3)(direction * visionRange);

            bool tankVisible = foundTank &&
                (!foundObstacle ||
                 nearestTank.distance < nearestObstacle.distance);

            if (foundObstacle &&
                (!tankVisible ||
                 nearestObstacle.distance < nearestTank.distance))
            {
                endpoint = nearestObstacle.point;
            }
            else if (tankVisible)
            {
                endpoint = nearestTank.point;

                if (nearestTank.distance < closestDistance)
                {
                    closestDistance = nearestTank.distance;
                    visibleTarget = nearestTank.collider.transform;
                }
            }

            lineRenderer.SetPosition(i + 1, endpoint);
        }

        lineRenderer.SetPosition(rayCount + 1, origin);
    }
}