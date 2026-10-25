using UnityEngine;

public class AttackAgent : AgentMovement
{
    public Transform enemyBase;
    public float shootRange = 5f;
    public float centerLaneX = 0f;

    protected override void AgentBehaviour()
    {
        Transform target = vision.visibleTarget;

        if (target != null)
        {
            Attack(target);
            return;
        }

        Advance();
    }

    void Advance()
    {
        Vector2 destination;

        if (enemyBase != null)
        {
            destination = new Vector2(centerLaneX, enemyBase.position.y);
        }
        else
        {
            destination = new Vector2(centerLaneX, rb.position.y + 10f);
        }

        MoveToward(destination);
    }

    void Attack(Transform target)
    {
        Vector2 targetPosition = target.position;

        AimAt(targetPosition);

        float distance = Vector2.Distance(rb.position, targetPosition);

        if (distance <= shootRange)
        {
            ShootAt(targetPosition);
            return;
        }

        MoveToward(targetPosition);
    }
}
