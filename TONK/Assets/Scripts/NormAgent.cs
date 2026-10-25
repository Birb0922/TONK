using UnityEngine;

public class NormAgent : AgentMovement
{
    private Vector2 lastKnownPosition;
    private bool searching = false;

    private Vector2 previousPosition;
    private float stuckTimer = 0f;
    private float checkTimer = 0f;
    private bool isStuck = false;

    private Vector2 attackPosition;
    private Vector2 attackTargetPosition;
    private bool hasAttackPosition = false;

    private float checkInterval = 1f;
    private float stuckThreshold = 0.15f;
    private float attackDistance = 10f;

    protected override void AgentBehaviour()
    {
        Transform target = vision.visibleTarget;

        CheckIfStuck();

        if (health < 30)
        {
            hasAttackPosition = false;

            if (target != null)
            {
                Vector2 away = (Vector2)transform.position
                    - (Vector2)target.position;

                Vector2 retreatPosition = (Vector2)transform.position
                    + away.normalized * vision.visionRange;

                MoveToward(retreatPosition);
            }
            else
            {
                MoveToward(lastKnownPosition);
            }

            return;
        }

        if (target != null)
        {
            lastKnownPosition = target.position;

            AimAt(target.position);
            ShootAt(target.position);

            if (!hasAttackPosition ||
            Vector2.Distance(attackTargetPosition, target.position) > 4f)
            {
                ChooseAttackPosition(target.position);
                attackTargetPosition = target.position;
            }

            if (isStuck)
            {
                ChooseAttackPosition(target.position);
                attackTargetPosition = target.position;
                isStuck = false;
            }

            MoveToward(attackPosition);
            return;
        }

        hasAttackPosition = false;

        if (isStuck)
        {
            hasWaypoint = false;
            FindWaypoint();
            isStuck = false;
        }

        if (searching)
        {
            MoveToward(lastKnownPosition);

            if (Vector2.Distance(transform.position,
                lastKnownPosition) < 0.5f)
            {
                searching = false;
            }

            return;
        }

        if (!hasWaypoint ||
            Vector2.Distance(transform.position, waypoint) < 0.5f)
        {
            FindWaypoint();
        }

        MoveToward(waypoint);
    }

    private void CheckIfStuck()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer >= checkInterval)
        {
            float distanceMoved = Vector2.Distance(
                transform.position, previousPosition);

            if (distanceMoved < stuckThreshold)
            {
                stuckTimer += checkTimer;
            }
            else
            {
                stuckTimer = 0f;
            }

            if (stuckTimer >= 2f)
            {
                isStuck = true;
                stuckTimer = 0f;
            }

            previousPosition = transform.position;
            checkTimer = 0f;
        }
    }

    private void ChooseAttackPosition(Vector2 targetPosition)
    {
        Vector2 awayFromTarget = (rb.position - targetPosition).normalized;

        float angleOffset = Random.Range(-35f, 35f);
        Vector2 direction = Quaternion.Euler(0f, 0f, angleOffset) * awayFromTarget;

        attackPosition = targetPosition + direction * attackDistance;
        hasAttackPosition = true;
    }

}