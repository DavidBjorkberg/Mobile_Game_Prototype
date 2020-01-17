using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    internal Vector3 patrolStart;
    internal Vector3 patrolGoal;
    GameObject player;
    internal int enemyNR;
    public FieldofView fieldOfView;
    public int movementSpeed;
    bool stopAndWaitRunning = false;
    public DrawPath drawPath;
    internal NavMeshAgent agent;
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
                stopAndWaitRunning = false;
                if(!stopAndWaitRunning)
                {
                    SwitchDirection();
                    stopAndWaitRunning = true;
                }
                drawPath.navPath = agent.path;
                drawPath.DrawCurrentPath(Game.game.GetPathLength(agent.path));
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

    void Move()
    {
        if (!agent.hasPath)
        {
            if (!stopAndWaitRunning)
            {
                SwitchDirection();
                stopAndWaitRunning = true;
                //StartCoroutine(StopAndWait());
            }
        }

    }
    void Died()
    {
        Game.game.KillEnemy(this);
    
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
        stopAndWaitRunning = true;
        yield return new WaitForSeconds(1f);
        SwitchDirection();
        stopAndWaitRunning = false;
    }
    void SwitchDirection()
    {
        NavMeshPath tempPath = new NavMeshPath();
        if (wayPointIndex == 0)
        {
            agent.CalculatePath(patrolGoal, tempPath);
            wayPointIndex++;
        }
        else if (wayPointIndex == 1)
        {
            agent.CalculatePath(patrolStart, tempPath);
            wayPointIndex = 0;
        }
        float pathLength = Game.game.GetPathLength(tempPath);
        tempPath = Game.game.RoundDownPath(pathLength, tempPath, agent);

        agent.path = tempPath;
        drawPath.navPath = agent.path;
    }
    private void OnDrawGizmosSelected()
    {
        switch (enemyNR)
        {
            case 0:
                Gizmos.color = Color.yellow;
                break;
            case 1:
                Gizmos.color = Color.cyan;
                break;
            case 2:
                Gizmos.color = Color.green;
                break;
            case 3:
                Gizmos.color = Color.black;
                break;
            default:
                break;
        }
        Gizmos.DrawSphere(patrolStart + new Vector3(0, 1, 0), 1f);
        Gizmos.DrawSphere(patrolGoal + new Vector3(0, 1, 0), 1f);
    }
}
