using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine;
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public FieldofView fieldOfView;
    public int movementSpeed;
    public DrawPath drawPath;
    public List<GameObject> waypoints;
    public int nrOfSteps;
    internal NavMeshAgent agent;
    int wayPointIndex = 0;
    GameObject player;
    Vector3 previousPos;
    float distanceWalked = 0;
    void Start()
    {
        if (waypoints == null)
        {
            waypoints = new List<GameObject>();
        }
        fieldOfView.enemy = gameObject;
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        InitializeRound();
    }
    void Update()
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

    readonly float switchWaypointDistance = 0.6f;
    void Move()
    {
        if (!agent.hasPath || (transform.position - agent.destination).magnitude <= switchWaypointDistance)
        {
            agent.destination = waypoints[wayPointIndex].transform.position;
            wayPointIndex = ++wayPointIndex % waypoints.Count;
        }

        UpdateDistanceWalked();
        agent.isStopped = distanceWalked / Game.game.stepSize >= nrOfSteps * 2;

    }
    void UpdateDistanceWalked()
    {
        if (previousPos == Vector3.zero)
        {
            previousPos = transform.position;
        }
        else
        {
            float distanceSinceLastFrame = (previousPos - transform.position).magnitude;
            distanceWalked += distanceSinceLastFrame;
            previousPos = transform.position;
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
    //Called when Gamestate switches to "Choose path"
    public void InitializeRound()
    {

        GetComponent<LineRenderer>().positionCount = waypoints.Count;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        float distanceToWalk = nrOfSteps * 2;
        for (int i = 1; i <= waypoints.Count; i++)
        {
            if (distanceToWalk <= 0)
            {
                GetComponent<LineRenderer>().positionCount = i;
                return;
            }

            float distanceToNextWaypoint = Vector3.Distance(transform.position, waypoints[wayPointIndex + i].transform.position);
            if (distanceToNextWaypoint <= distanceToWalk)
            {
                GetComponent<LineRenderer>().SetPosition(i, waypoints[wayPointIndex + i].transform.position);
                distanceToWalk -= distanceToNextWaypoint;
            }
            else
            {
                Vector3 endPos = waypoints[wayPointIndex + i].transform.position;

                Vector3 cutoffDir = waypoints[wayPointIndex + i].transform.position - waypoints[wayPointIndex + i - 1].transform.position;
                cutoffDir.Normalize();
                float cutoffLength = distanceToNextWaypoint - nrOfSteps * 2;
                endPos += cutoffDir * cutoffLength;

                GetComponent<LineRenderer>().SetPosition(i, endPos);
                distanceToWalk = 0;
            }

        }
        distanceWalked = 0;
    }











    //Editor functions
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