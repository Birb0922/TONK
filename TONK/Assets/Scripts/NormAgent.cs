using UnityEngine;

public class NormAgent : AgentMovement
{
    public float attackRange = 8f;

    protected override void Think()
    {
        if (vision == null || vision.visibleTarget == null)
        {
            return;
        }

        Transform target = vision.visibleTarget;

        Vector2 direction =
            (Vector2)target.position - rb.position;

        float angle = Vector2.SignedAngle(
            transform.up, direction
        );

        AimAt(target.position);

        if (direction.magnitude > attackRange)
        {
            Turn(angle);
        }
        else
        {
            Turn(angle);
            ShootAt(target.position);
        }
    }
}