using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    GameObject player;
    public FieldofView fieldOfView;
    public int movementSpeed;
    public DrawPath drawPath;
    internal NavMeshAgent agent;
    List<Vector3> waypoints = new List<Vector3>();
    int wayPointIndex = 0;
    void Start()
    {
        fieldOfView.enemy = gameObject;
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
    }
    void Update()
    {
        agent.isStopped = Game.game.gameState != Game.gameStates.Play;

        switch (Game.game.gameState)
        {
            case Game.gameStates.ChooseSpawn:
                break;
            case Game.gameStates.ChoosePath:
                break;
            case Game.gameStates.ConfirmPath:
                break;
            case Game.gameStates.Play:
                Move();
                CheckFoV();
                break;
            default:
                break;
        }



        Ray backRay = new Ray(transform.position, -transform.forward);
        if (Physics.OverlapSphere(transform.position - transform.forward, 1.5f, 1 << 8).Length > 0)
        {
            Died();
        }
    }
    float switchWaypointDistance = 1;
    void Move()
    {
        if (!agent.hasPath || (transform.position - agent.destination).magnitude <= switchWaypointDistance)
        {
            agent.destination = waypoints[wayPointIndex];
            wayPointIndex++;
        }
    }
    void Died()
    {
        Game.game.enemyHandler.KillEnemy(this);

    }
    void CheckFoV()
    {
        Vector3 enemyToPlayer = player.transform.position - transform.position;
        float angle = Vector3.Angle(enemyToPlayer.normalized, transform.forward) * 2;
        if (angle <= fieldOfView.fov)
        {
            if (Physics.Raycast(transform.position, enemyToPlayer.normalized, out RaycastHit hit, fieldOfView.viewDistance, 1 << 8 | 1 << 10))
            {
                if (hit.transform.TryGetComponent(out Player player))
                {
                    DetectedPlayer();
                }
            }
        }
    }
    void DetectedPlayer()
    {
        player.GetComponent<Player>().Died();

    }
    IEnumerator StopAndWait()
    {
        yield return new WaitForSeconds(1f);
        SwitchDirection();
    }
    void SwitchDirection()
    {
        NavMeshPath tempPath = new NavMeshPath();
        if (wayPointIndex == 0)
        {
           // agent.CalculatePath(patrolGoal, tempPath);
            if (tempPath.corners.Length < 2)
                return;
            wayPointIndex++;
        }
        else if (wayPointIndex == 1)
        {
          //  agent.CalculatePath(patrolStart, tempPath);
            if (tempPath.corners.Length < 2)
                return;
            wayPointIndex = 0;
        }
        float pathLength = Game.game.GetPathLength(tempPath);
        tempPath = Game.game.RoundDownPath(pathLength, tempPath, agent);

        agent.path = tempPath;
        drawPath.navPath = agent.path;
    }
    private void OnDestroy()
    {
        
    }
}
