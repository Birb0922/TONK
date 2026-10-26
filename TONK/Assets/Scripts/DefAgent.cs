using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum Behaviors {
    FOLLOW,
    RETREAT,
    ATTACK,
    PATROL
}
public class DefAgent : AgentMovement
{
    [SerializeField]
    List<GameObject> teammates;

    float[] utilities = { 0, 0, 0 };
    GameObject teammateToDefend;

    GameObject enemyInMemory;
    float memoryTimer;
    float memoryDuration = 3f;

    
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
            case Behaviors.PATROL:
                
                Patrol();
                break;
        }
    }
    public void Follow()
    {
        if (teammateToDefend == null) return;

        Transform target = vision.visibleTarget;
        

        Vector2 positionToMove = teammateToDefend.transform.position + (teammateToDefend.transform.right * 2);

        if (target != null && !teammates.Contains(target.gameObject)) // Not a teammate
        {
            
            enemyInMemory = target.gameObject;
            memoryTimer = memoryDuration;
        }

        

        if (enemyInMemory != null)
        {
            
            Vector2 enemy2Teammate = teammateToDefend.transform.position - enemyInMemory.transform.position;
            
            positionToMove = (Vector2)teammateToDefend.transform.position + (enemy2Teammate).normalized * 4;
            AimAt(enemyInMemory.transform.position);
            ShootAt(enemyInMemory.transform.position);

            memoryTimer -= Time.deltaTime;

            if (memoryTimer <= 0)
            {
                enemyInMemory = null;

            }
        }
        if (Vector2.Distance(positionToMove, transform.position) < 1.5)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        MoveToward(positionToMove);
    }
    public void Patrol()
    {
        Transform target = vision.visibleTarget;
        if (target !=null && !teammates.Contains(target.gameObject))
        {
            AimAt(target.transform.position);
            ShootAt(target.transform.position);
            

            if (Vector2.Distance(transform.position, target.transform.position) < 3)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            MoveToward(target.transform.position);
        }
        else
        {
            

            if (waypoint == null || Vector2.Distance(transform.position, waypoint) < 1 )
            {
                FindWaypoint();
            }
            MoveToward(waypoint);
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

        float patrolUtil = follow == 0? 0.5f : 0;
        utilities[0] = follow * 2;
        utilities[1] = retreatUtil * 1;
        utilities[2] = patrolUtil * 1;

        float best = utilities.Max();
        
        
        if (best == utilities[0])
        {
            return Behaviors.FOLLOW;
        } else if (best == utilities[1])
        {
            return Behaviors.RETREAT;
        } else
        {
            return Behaviors.PATROL;
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

            PlayerMovement pl = teammate.GetComponent<PlayerMovement>();
            if (agent != null)
            {
                
                float util;
                if (agent.health == 100)
                {
                    util = 0.2f;
                } else {
                    float healthRatio = Mathf.Clamp01(agent.health / 100f);
                    float distRatio = Mathf.Clamp01(Vector2.Distance(transform.position, teammate.transform.position) / 10f);
                    util = (1 - healthRatio) * (1 - distRatio);
                    Debug.Log(util);
                }
                if (util > bestUtil)
                {
                    bestUtil = util;
                    bestMate = teammate;
                }
            }

            if (pl != null)
            {
                float util;
                if (pl.health == 100)
                {
                    util = 0.05f;
                } else
                {
                    float healthRatio = Mathf.Clamp01(pl.health / 100f);
                    float distRatio = Mathf.Clamp01(Vector2.Distance(transform.position, teammate.transform.position) / 10f);
                    util = (1 - healthRatio) * (1 - distRatio);
                }
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