
using UnityEngine;

public class RoutePatrolAgent : AgentMovement
{
    public enum PatrolState
    {
        Patrol,
        Pausing,
        Engage,
        Investigate
    }

    [Header("Patrol Route")]
    public Transform[] patrolPoints;
    public bool loopRoute = true;
    public float arrivalDistance = 0.5f;
    private int routeDirection = 1;

    [Header("Waypoint Pause")]
    public float pauseDuration = 2f;

    [Header("Investigation")]
    public float investigationPause = 4f;
    public float investigationArrivalDistance = 0.75f;

    [Header("Combat")]
    public bool shootAtVisibleEnemies = true;

    public PatrolState currentState = PatrolState.Patrol;

    private int currentPointIndex = 0;
    private float pauseTimer = 0f;

    private Vector2 lastKnownPosition;
    private bool hasLastKnownPosition = false;
    private bool wasEngaging = false;

    protected override void Awake()
    {
        base.Awake();

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning(
                name + ": No patrol points assigned!",
                this
            );
        }
    }

    protected override void AgentBehaviour()
    {
        Transform target = vision != null
            ? vision.visibleTarget
            : null;

        // ENEMY VISIBLE: record its position and engage.
        if (target != null)
        {
            lastKnownPosition = target.position;
            hasLastKnownPosition = true;
            wasEngaging = true;

            currentState = PatrolState.Engage;

            AimAt(target.position);

            if (shootAtVisibleEnemies)
            {
                ShootAt(target.position);
            }

            // Hold position while engaging.
            return;
        }

        // ENEMY LOST: investigate its last known position.
        if (wasEngaging)
        {
            wasEngaging = false;

            if (hasLastKnownPosition)
            {
                currentState = PatrolState.Investigate;
                pauseTimer = 0f;
            }
            else
            {
                currentState = PatrolState.Patrol;
            }
        }

        switch (currentState)
        {
            case PatrolState.Patrol:
                PatrolRoute();
                break;

            case PatrolState.Pausing:
                PauseAtWaypoint();
                break;

            case PatrolState.Investigate:
                InvestigatePosition();
                break;
        }
    }

    private void PatrolRoute()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        Transform point = patrolPoints[currentPointIndex];

        if (point == null)
        {
            AdvanceToNextPoint();
            return;
        }

        float distance = Vector2.Distance(
            rb.position,
            point.position
        );

        if (distance <= arrivalDistance)
        {
            currentState = PatrolState.Pausing;
            pauseTimer = 0f;
            return;
        }

        MoveToward(point.position);
    }

    private void PauseAtWaypoint()
    {
        pauseTimer += Time.deltaTime;

        if (pauseTimer >= pauseDuration)
        {
            AdvanceToNextPoint();
            currentState = PatrolState.Patrol;
        }
    }

    private void AdvanceToNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            return;
        }

        if (loopRoute)
        {
            currentPointIndex += routeDirection;

            // Reverse direction at either end.
            if (currentPointIndex >= patrolPoints.Length - 1)
            {
                currentPointIndex = patrolPoints.Length - 1;
                routeDirection = -1;
            }
            else if (currentPointIndex <= 0)
            {
                currentPointIndex = 0;
                routeDirection = 1;
            }
        }
        else
        {
            if (currentPointIndex < patrolPoints.Length - 1)
            {
                currentPointIndex++;
            }
        }
    }

    private void InvestigatePosition()
    {
        float distance = Vector2.Distance(
            rb.position,
            lastKnownPosition
        );

        if (distance > investigationArrivalDistance)
        {
            MoveToward(lastKnownPosition);
            return;
        }

        // Reached the last known enemy position.
        pauseTimer += Time.deltaTime;

        if (pauseTimer >= investigationPause)
        {
            hasLastKnownPosition = false;
            pauseTimer = 0f;
            currentState = PatrolState.Patrol;
        }
    }

    // Call this from another script when something suspicious
    // is detected at a location.
    public void InvestigatePosition(Vector2 position)
    {
        lastKnownPosition = position;
        hasLastKnownPosition = true;
        wasEngaging = false;
        pauseTimer = 0f;
        currentState = PatrolState.Investigate;
    }

    // Useful for testing patrol behavior from another script.
    public void ResumePatrol()
    {
        wasEngaging = false;
        pauseTimer = 0f;
        currentState = PatrolState.Patrol;
    }
}
