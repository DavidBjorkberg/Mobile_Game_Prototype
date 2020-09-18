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
    /// <summary>
    /// Checks FOV first for decoys, then for the player
    /// </summary>
    void CheckFoV()
    {
        GameObject hitGO;
        LayerMask layersToHit;
        float enemyToTargetLength;
        for (int i = 0; i < Game.game.activeDecoys.Count; i++)
        {
            layersToHit = 1 << 9;
            layersToHit += 1 << 10;
            layersToHit += 1 << 11;
            hitGO = CheckFOVForTarget(Game.game.activeDecoys[i].transform.position, layersToHit);
            enemyToTargetLength = (Game.game.activeDecoys[i].transform.position - transform.position).magnitude;
            if (hitGO != null && hitGO.TryGetComponent(out Decoy decoy))
            {
                DetectedDecoy(enemyToTargetLength, decoy.gameObject);
                return;
            }
        }

        layersToHit = 1 << 8;
        layersToHit += 1 << 10;
        layersToHit += 1 << 11;
        hitGO = CheckFOVForTarget(player.transform.position, layersToHit);
        enemyToTargetLength = (player.transform.position - transform.position).magnitude;
        if (hitGO != null && hitGO.TryGetComponent(out Player hitPlayer))
        {
            DetectedPlayer(enemyToTargetLength);
            return;
        }
    }
    GameObject CheckFOVForTarget(Vector3 target, LayerMask layersToHit)
    {
        Vector3 enemyToTarget = target - transform.position;
        float angle = Vector3.Angle(enemyToTarget, transform.forward) * 2;
        enemyToTarget.Normalize();

        if (angle <= fieldOfView.fov)
        {
            if (Physics.Raycast(transform.position, enemyToTarget, out RaycastHit hit, fieldOfView.viewDistance, layersToHit))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }
    void DetectedPlayer(float distanceToPlayer)
    {
        if (Game.game.player.isInvisible)
            return;

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
        }
    }
    public void SetChaseState(Vector3 targetPos)
    {
        state = MovementStates.Chasing;
        lastSeenPlayerTimer = 0;
        lastSeenPlayerPos = targetPos;
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
            float lengthOfPath = direction.magnitude;
            direction.Normalize();
            Vector3 waypointToEnemy = transform.position - waypoints[i].transform.position;
            float lengthToPath = Mathf.Clamp(Vector3.Dot(waypointToEnemy, direction), 0, lengthOfPath);
            Vector3 point = waypoints[i].transform.position + direction * lengthToPath;
            if (lengthToPath < closestDistance)
            {
                closestDistance = lengthToPath;
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
