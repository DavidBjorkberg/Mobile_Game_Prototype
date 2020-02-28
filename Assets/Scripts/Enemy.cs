using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public FieldofView fieldOfView;
    public int movementSpeed;
    public List<GameObject> waypoints;
    public float killDistance;
    public float chaseTime;
    public bool circulate = true;
    public int roomNumber;
    internal NavMeshAgent agent;
    private bool walkingBack;
    private int wayPointIndex = 0;
    private GameObject player;
    private float lastSeenPlayerTimer;
    private Vector3 lastSeenPlayerPos;
    private float chaseSpeed;
    private bool isStunned;
    private float stunTimer;
    private float detectedDecoyRangeDivider = 2;
    private bool isTurning;
    private enum MovementStates
    {
        Standard, Chasing, Returning
    }
    private MovementStates state;
    void Start()
    {
        chaseSpeed = movementSpeed * 2f;
        if (waypoints == null)
        {
            waypoints = new List<GameObject>();
        }
        fieldOfView.enemy = gameObject;
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
    }

    void Update()
    {
        if (!isStunned)
        {
            CheckFoV();
            Move();
        }
        else if (stunTimer <= 0)
        {
            fieldOfView.gameObject.SetActive(true);
            agent.isStopped = false;
            isStunned = false;

        }
        else
        {
            stunTimer -= Time.deltaTime;
        }
        if (Physics.OverlapSphere(transform.position - transform.forward, 1f, 1 << 8).Length > 0)
        {
            Died();
        }
    }
    readonly float switchWaypointDistance = 0.7f;
    void Move()
    {
        switch (state)
        {
            case MovementStates.Standard:
                if (!agent.hasPath || (transform.position - agent.destination).magnitude <= switchWaypointDistance)
                {
                    if (circulate)
                    {
                        wayPointIndex = ++wayPointIndex % waypoints.Count;
                    }
                    else
                    {
                        if (walkingBack)
                        {
                            wayPointIndex--;
                        }
                        else
                        {
                            wayPointIndex++;
                        }
                        if (wayPointIndex == 0)
                        {
                            walkingBack = false;
                        }
                        else if (wayPointIndex >= waypoints.Count - 1)
                        {
                            walkingBack = true;
                        }
                    }
                    agent.destination = waypoints[wayPointIndex].transform.position;
                }
                break;
            case MovementStates.Chasing:
                agent.speed = chaseSpeed;
                if (agent.destination.x != lastSeenPlayerPos.x && agent.destination.z != lastSeenPlayerPos.z)
                {
                    agent.destination = lastSeenPlayerPos;
                }
                if ((transform.position - lastSeenPlayerPos).magnitude <= 1.5f)
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
                    SetReturnState();
                }

                break;
            case MovementStates.Returning:
                agent.speed = movementSpeed;
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
        //Player
        Vector3 enemyToTarget = player.transform.position - transform.position;
        float angle = Vector3.Angle(enemyToTarget.normalized, transform.forward) * 2;
        if (angle <= fieldOfView.fov)
        {
            if (Physics.Raycast(transform.position, enemyToTarget.normalized, out RaycastHit hit, fieldOfView.viewDistance, 1 << 8 | 1 << 10))
            {
                if (hit.transform.TryGetComponent(out Player player))
                {
                    DetectedPlayer(enemyToTarget.magnitude);
                }
                else
                {
                    Game.game.player.RemoveDetectedEnemy(this);
                }
            }
        }
        for (int i = 0; i < Game.game.activeDecoys.Count; i++)
        {
            enemyToTarget = Game.game.activeDecoys[i].transform.position - transform.position;
            angle = Vector3.Angle(enemyToTarget.normalized, transform.forward) * 2;
            if (angle <= fieldOfView.fov)
            {
                if (Physics.Raycast(transform.position, enemyToTarget.normalized, out RaycastHit hit, fieldOfView.viewDistance, 1 << 11 | 1 << 10))
                {
                    if (hit.transform.TryGetComponent(out Decoy decoy))
                    {
                        DetectedDecoy(enemyToTarget.magnitude, decoy.gameObject);
                    }
                }
            }
        }
    }
    void DetectedPlayer(float distanceToPlayer)
    {
        if (Game.game.player.isInvisible)
            return;

       // Game.game.player.AddDetectedEnemy(this);
        if (distanceToPlayer > killDistance)
        {
            SetChaseState(player.transform.position);
        }
        else
        {
            SetReturnState();
            player.GetComponent<Player>().Died();
        }
    }
    void DetectedDecoy(float distanceToDecoy, GameObject decoy)
    {
        if (distanceToDecoy > killDistance)
        {
            SetChaseState(decoy.transform.position);
        }
        else
        {
            fieldOfView.viewDistance /= detectedDecoyRangeDivider;
            Invoke("SetReturnState", 1.5f);
            Invoke("ResetFOVDistance", 1.5f);
            agent.speed = movementSpeed;
            decoy.GetComponent<Decoy>().DestroyDecoy();
        }
    }
    public void SetChaseState(Vector3 targetPos)
    {
        state = MovementStates.Chasing;
        lastSeenPlayerTimer = 0;
        lastSeenPlayerPos = targetPos;
    }
    void ResetFOVDistance()
    {
        fieldOfView.viewDistance *= detectedDecoyRangeDivider;
    }
    void SetReturnState()
    {

        agent.isStopped = false;
        state = MovementStates.Returning;
        agent.destination = GetClosestPointInPath();
    }
    public void Stun(float stunDuration = 1)
    {
        agent.isStopped = true;
        isStunned = true;
        stunTimer = stunDuration;
        fieldOfView.gameObject.SetActive(false);
    }
    Vector3 GetClosestPointInPath()
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 direction = waypoints[i + 1].transform.position - waypoints[i].transform.position;
            float length = direction.magnitude;
            direction.Normalize();
            Vector3 v = transform.position - waypoints[i].transform.position;
            float d = Mathf.Clamp(Vector3.Dot(v, direction), 0, length);
            Vector3 point = waypoints[i].transform.position + direction * d;
            if (d < closestDistance)
            {
                closestDistance = d;
                closestPoint = point;
            }
        }
        return closestPoint;
    }










#if UNITY_EDITOR
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
#endif
}
