using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine;
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public FieldofView fieldOfView;
    public int movementSpeed;
    public List<GameObject> waypoints;
    public int nrOfSteps;
    public float killDistance;
    public float chaseTime;
    internal NavMeshAgent agent;
    private int wayPointIndex = 0;
    private GameObject player;
    private Vector3 endPos;
    private float lastSeenPlayerTimer;
    private Vector3 lastSeenPlayerPos;
    private float chaseSpeed;
    private List<Vector3> pathCorners = new List<Vector3>();
    private enum MovementStates
    {
        Standard, Chasing, Returning
    }
    private MovementStates state;
    void Start()
    {
        switch (Game.game.gameMode)
        {
            case Game.GameMode.Action:
                ActionStart();
                break;
            case Game.GameMode.Strategy:
                StrategyStart();
                break;
            default:
                break;
        }
    }
    void ActionStart()
    {
        chaseSpeed = movementSpeed * 2f;
        if (waypoints == null)
        {
            waypoints = new List<GameObject>();
        }
        fieldOfView.enemy = gameObject;
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
    }
    void StrategyStart()
    {
        chaseSpeed = movementSpeed * 2f;
        if (waypoints == null)
        {
            waypoints = new List<GameObject>();
        }
        fieldOfView.enemy = gameObject;
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
        InitializeRound();
        state = MovementStates.Standard;
    }
    void Update()
    {
        switch (Game.game.gameMode)
        {
            case Game.GameMode.Action:
                ActionUpdate();
                break;
            case Game.GameMode.Strategy:
                StrategyUpdate();
                break;
            default:
                break;
        }
    }
    void ActionUpdate()
    {
        ActionMove();
        CheckFoV();

        if (Physics.OverlapSphere(transform.position - transform.forward, 1f, 1 << 8).Length > 0)
        {
            Died();
        }
    }
    void StrategyUpdate()
    {
        switch (Game.game.gameState)
        {
            case Game.GameStates.ChooseSpawn:
                break;
            case Game.GameStates.ChoosePath:
                break;
            case Game.GameStates.ConfirmPath:
                break;
            case Game.GameStates.Play:
                StrategyMove();
                CheckFoV();
                break;
            default:
                break;
        }

        if (Physics.OverlapSphere(transform.position - transform.forward, 1f, 1 << 8).Length > 0)
        {
            Died();
        }
    }
    readonly float switchWaypointDistance = 0.7f;
    void StrategyMove()
    {
        switch (state)
        {
            case MovementStates.Standard:
                if (!agent.hasPath || (transform.position - agent.destination).magnitude <= switchWaypointDistance)
                {
                    wayPointIndex = ++wayPointIndex % waypoints.Count;
                    agent.destination = waypoints[wayPointIndex].transform.position;
                }

                agent.isStopped = Vector3.Distance(transform.position, endPos) <= 0.5f;
                break;
            case MovementStates.Chasing:
                agent.speed = chaseSpeed * Game.game.movementSpeedFactor;
                if (agent.destination.x != lastSeenPlayerPos.x && agent.destination.z != lastSeenPlayerPos.z)
                {
                    agent.destination = lastSeenPlayerPos;
                }   
                if ((transform.position - lastSeenPlayerPos).magnitude <= 1f)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }

                lastSeenPlayerTimer += Time.deltaTime;
                if (lastSeenPlayerTimer >= chaseTime)
                {
                    agent.isStopped = false;
                    state = MovementStates.Returning;
                    agent.destination = GetClosestPointInPath();
                }
                break;
            case MovementStates.Returning:
                agent.speed = movementSpeed * Game.game.movementSpeedFactor;
                if ((transform.position - agent.destination).magnitude <= switchWaypointDistance)
                {
                    state = MovementStates.Standard;
                    agent.destination = waypoints[wayPointIndex].transform.position;
                }
                break;
            default:
                break;
        }
    }
    void ActionMove()
    {
        switch (state)
        {
            case MovementStates.Standard:
                if (!agent.hasPath || (transform.position - agent.destination).magnitude <= switchWaypointDistance)
                {
                    wayPointIndex = ++wayPointIndex % waypoints.Count;
                    agent.destination = waypoints[wayPointIndex].transform.position;
                }

                agent.isStopped = Vector3.Distance(transform.position, endPos) <= 0.5f;
                break;
            case MovementStates.Chasing:
                agent.speed = chaseSpeed * Game.game.movementSpeedFactor;
                if (agent.destination.x != lastSeenPlayerPos.x && agent.destination.z != lastSeenPlayerPos.z)
                {
                    agent.destination = lastSeenPlayerPos;
                }
                if((transform.position - lastSeenPlayerPos).magnitude <= 1f)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }

                lastSeenPlayerTimer += Time.deltaTime;
                if (lastSeenPlayerTimer >= chaseTime)
                {
                    agent.isStopped = false;
                    state = MovementStates.Returning;
                    agent.destination = GetClosestPointInPath();
                }

                break;
            case MovementStates.Returning:
                agent.speed = movementSpeed * Game.game.movementSpeedFactor;
                if ((transform.position - agent.destination).magnitude <= switchWaypointDistance)
                {
                    state = MovementStates.Standard;
                    agent.destination = waypoints[wayPointIndex].transform.position;
                }
                break;
            default:
                break;
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
                    DetectedPlayer(enemyToPlayer.magnitude);
                }
            }
        }
    }
    void DetectedPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > killDistance)
        {
            if (state != MovementStates.Chasing)
            {
                state = MovementStates.Chasing;
            }

            lastSeenPlayerTimer = 0;
            lastSeenPlayerPos = player.transform.position;
        }
        else
        {
            state = MovementStates.Standard;
            player.GetComponent<Player>().Died();
        }

    }
    Vector3 GetClosestPointInPath()
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < pathCorners.Count - 1; i++)
        {
            Vector3 direction = pathCorners[i + 1] - pathCorners[i];
            float length = direction.magnitude;
            direction.Normalize();
            Vector3 v = transform.position - pathCorners[i];
            float d = Mathf.Clamp(Vector3.Dot(v, direction), 0, length);
            Vector3 point = pathCorners[i] + direction * d;
            if (d < closestDistance)
            {
                closestDistance = d;
                closestPoint = point;
            }
        }
        return closestPoint;
    }
    //Called when Gamestate switches to "Choose path"
    public void InitializeRound()
    {
        pathCorners.Clear();
        pathCorners.Add(transform.position);
        GetComponent<LineRenderer>().positionCount = waypoints.Count;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        float distanceToWalk = (nrOfSteps * 2) * Game.game.stepSize;
        int tempIndex = wayPointIndex;
        //First waypoint is on player, skip to next
        if (wayPointIndex == 0)
            tempIndex = 1;
        int i = 1;
        while (distanceToWalk > 0)
        {
            GetComponent<LineRenderer>().positionCount = i + 1;
            Vector3 nextWaypoint = waypoints[(tempIndex + i - 1) % waypoints.Count].transform.position;
            Vector3 curWaypoint = waypoints[(tempIndex + i - 2) % waypoints.Count].transform.position;
            float distanceToNextWaypoint;
            //Use players position in the first iteration
            if (i == 1)
            {
                distanceToNextWaypoint = Vector3.Distance(transform.position, nextWaypoint);
            }
            else
            {
                distanceToNextWaypoint = Vector3.Distance(curWaypoint, nextWaypoint);
            }
            if (distanceToNextWaypoint < distanceToWalk)
            {
                GetComponent<LineRenderer>().SetPosition(i, nextWaypoint);
                pathCorners.Add(nextWaypoint);
                distanceToWalk -= distanceToNextWaypoint;
            }
            else
            {
                endPos = nextWaypoint;

                Vector3 cutoffDir = curWaypoint - nextWaypoint;
                cutoffDir.Normalize();
                float cutoffLength = distanceToNextWaypoint - distanceToWalk;
                endPos += cutoffDir * cutoffLength;

                GetComponent<LineRenderer>().SetPosition(i, endPos);
                pathCorners.Add(endPos);
                distanceToWalk = 0;
            }
            i++;
        }

        //Draw the enemies path
        //for (int i = 1; i < waypoints.Count; i++)
        //{
        //    if (distanceToWalk <= 0)
        //    {
        //        GetComponent<LineRenderer>().positionCount = i;
        //        break;
        //    }

        //    Vector3 nextWaypoint = waypoints[(tempIndex + i - 1) % waypoints.Count].transform.position;
        //    Vector3 curWaypoint = waypoints[(tempIndex + i - 2) % waypoints.Count].transform.position;
        //    float distanceToNextWaypoint;
        //    //Use players position in the first iteration
        //    if (i == 1)
        //    {
        //        distanceToNextWaypoint = Vector3.Distance(transform.position, nextWaypoint);
        //    }
        //    else
        //    {
        //        distanceToNextWaypoint = Vector3.Distance(curWaypoint, nextWaypoint);
        //    }
        //    if (distanceToNextWaypoint < distanceToWalk)
        //    {
        //        GetComponent<LineRenderer>().SetPosition(i, nextWaypoint);
        //        pathCorners.Add(nextWaypoint);
        //        distanceToWalk -= distanceToNextWaypoint;
        //    }
        //    else
        //    {
        //        endPos = nextWaypoint;

        //        Vector3 cutoffDir = curWaypoint - nextWaypoint;
        //        cutoffDir.Normalize();
        //        float cutoffLength = distanceToNextWaypoint - distanceToWalk;
        //        endPos += cutoffDir * cutoffLength;

        //        GetComponent<LineRenderer>().SetPosition(i, endPos);
        //        pathCorners.Add(endPos);
        //        distanceToWalk = 0;
        //    }
        //}
    }











    ////Editor functions
    public GameObject AddWaypoint()
    {
        GameObject newWaypoint = new GameObject("Waypoint");
        newWaypoint.transform.position = transform.position;
        newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);

        TargetEnemy targetEnemy = newWaypoint.AddComponent<TargetEnemy>();
        targetEnemy.owner = gameObject;
        if (waypoints.Count > 0)
        {
            targetEnemy.previous = waypoints[waypoints.Count - 1];
            EditorUtility.SetDirty(targetEnemy.previous);
        }
        EditorUtility.SetDirty(targetEnemy);
        EditorUtility.SetDirty(targetEnemy.owner);
        waypoints.Add(newWaypoint);
        return newWaypoint;
    }
    public void RemoveLatestWaypoint()
    {
        if (waypoints.Count > 0)
        {
            DestroyImmediate(waypoints[waypoints.Count - 1]);
            waypoints.RemoveAt(waypoints.Count - 1);
        }
    }
    public void RemoveAllWaypoints(bool removeLast)
    {
        for (int i = waypoints.Count - 1; i > 0; i--)
        {
            DestroyImmediate(waypoints[i]);
            waypoints.RemoveAt(i);
        }
        if (removeLast)
        {
            DestroyImmediate(waypoints[0]);
            waypoints.RemoveAt(0);
        }
    }
}