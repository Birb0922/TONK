using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum Behaviors {
    FOLLOW,
    RETREAT,
    ATTACK,
    IDLE
}
public class DefAgent : AgentMovement
{
    [SerializeField]
    List<GameObject> teammates;

    float[] utilities = { 0, 0, 0 };
    GameObject teammateToDefend;

    Behaviors currentBehavior = Behaviors.FOLLOW;
    protected override void AgentBehaviour()
    {
        currentBehavior = CalculateUtility();

        RunBehaviors();
    }

    public void RunBehaviors()
    {
        switch (currentBehavior)
        {
            case Behaviors.FOLLOW:
                Follow();
                break;
            case Behaviors.RETREAT:
                Retreat();
                break;
            case Behaviors.ATTACK:
                Attack();
                break;
            case Behaviors.IDLE:
                break;
        }
    }
    public void Follow()
    {
        if (teammateToDefend == null) return;

        Transform target = vision.visibleTarget;

        Vector3 positionToMove = teammateToDefend.transform.position + (teammateToDefend.transform.right * 2);

        if (Vector2.Distance(positionToMove, transform.position) < 3)
        {
            positionToMove = transform.position;
        }

        if (vision.visibleTarget != null && !teammates.Contains(target.gameObject)) // Not a teammate
        {
            positionToMove += target.position - teammateToDefend.transform.position;
            AimAt(target.transform.position);
            ShootAt(target.transform.position);
        }
        MoveToward(positionToMove);
    }
    public void Attack()
    {
        Transform target = vision.visibleTarget;
        if (!teammates.Contains(target.gameObject))
        {
            AimAt(target.transform.position);
            ShootAt(target.transform.position);
            MoveToward(target.transform.position);
        }
    }

    public void Retreat()
    {
        Transform target = vision.visibleTarget;
        Vector2 away = transform.position - target.transform.position;
        MoveToward((Vector2)transform.position + away.normalized * vision.visionRange);
    }
    public Behaviors CalculateUtility()
    {
        float follow = CalculateUtilityOfTeammates();

        float retreatUtil = health == 100? 0 : 1 / (100 - health);

        float attackUtil = 0;
        utilities[0] = follow * 2;
        utilities[1] = retreatUtil * 1;
        utilities[2] = attackUtil * 1;

        float best = utilities.Max();

        if (best == utilities[0])
        {
            return Behaviors.FOLLOW;
        } else if (best == utilities[1])
        {
            return Behaviors.RETREAT;
        } else
        {
            return Behaviors.ATTACK;
        }
    }

    public float CalculateUtilityOfTeammates()
    {
        if (teammates.Count == 0)
        {
            return 0;
        }
        float bestUtil = 0.0f;
        GameObject bestMate = teammates[0];
        foreach (GameObject teammate in teammates)
        {
            if (teammate == null) continue;

            AgentMovement agent = teammate.GetComponent<AgentMovement>();
            if (agent != null)
            {
                float util = (1 / agent.health) * (1 / Vector2.Distance(transform.position, teammate.transform.position));

                if (util > bestUtil)
                {
                    bestUtil = util;
                    bestMate = teammate;
                }
            }
            
        }

        teammateToDefend = bestMate;
        return bestUtil;
    }
}