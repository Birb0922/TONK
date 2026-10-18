using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RangerAgent : AgentMovement
{
    private Vector2 lastKnownPosition;
    private bool searching = false;
    private bool patrol = false;
    private bool firstWaypoint = false;
    private float minY=-4;
    private float maxY=4;
    private float minX=-8;
    private float maxX=8;
    private float minAttackRange=3;
    private float maxAttackRange=5;
    



    protected override void AgentBehaviour()
    {
        Transform target = vision.visibleTarget;


        if (target == null) { 
            patrol = true;
        }

        if (patrol) {

            if (!firstWaypoint) { 
                GetWaypoint();
                firstWaypoint = true;
            }

            Vector2 position = (Vector2)transform.position;
            if (position != waypoint) {
                MoveToward(waypoint);
            }
            else {
                GetWaypoint();
            }
                
        }

      





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
            patrol = false;

            float toTarget= Vector2.Distance(transform.position, target.position);


            if (toTarget < minAttackRange)
            {
                Vector2 away=(Vector2)transform.position+(Vector2)target.transform.position.normalized;



                /*float tempX = -target.position.x;
                float tempY = -target.position.y;
                Vector2 tempPosition = new Vector2(tempX, tempY);
                MoveToward(tempPosition);*/

                MoveToward(away);
            }
            else if (toTarget > maxAttackRange) { 
                MoveToward(target.position);
            }
            else
            {
                lastKnownPosition = target.transform.position;
                AimAt(target.transform.position);
                ShootAt(target.transform.position);
            }
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

/*        if (!hasWaypoint || Vector2.Distance(transform.position, waypoint) < 0.5f)
        {
            FindWaypoint();
        }*/

        //MoveToward(waypoint);
    }

    protected  void GetWaypoint() {

        while (Vector2.Distance(waypoint, (Vector2)transform.position)<2){
            float tempY = Random.Range(minY, maxY);
            float tempX = Random.Range(minX, maxX);

            waypoint = new Vector2(tempX, tempY);
        }

        
    }

}