using UnityEngine;

public class NormAgent : AgentMovement
{
    private Vector2 lastKnownPosition;
    private bool searching = false;

    protected override void AgentBehaviour()
    {
        Transform target = vision.visibleTarget;

        if (health < 30)
        {
            if (target != null)
            {
                Vector2 away = transform.position - target.transform.position;
                MoveToward((Vector2)transform.position + away.normalized * vision.visionRange);
            }
            else
            {
                MoveToward(lastKnownPosition);
            }

            return;
        }

        if (target != null)
        {
            lastKnownPosition = target.transform.position;

            AimAt(target.transform.position);
            ShootAt(target.transform.position);

            MoveToward(target.transform.position);
            return;
        }

        if (searching)
        {
            MoveToward(lastKnownPosition);

            if (Vector2.Distance(transform.position, lastKnownPosition) < 0.5f)
            {
                searching = false;
            }

            return;
        }

        if (!hasWaypoint || Vector2.Distance(transform.position, waypoint) < 0.5f)
        {
            FindWaypoint();
        }

        MoveToward(waypoint);
    }
}