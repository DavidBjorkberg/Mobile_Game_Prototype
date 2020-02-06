using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public int movementSpeed;
    public int acceleration;
    public int walkRange;
    Vector3 destination;
    internal NavMeshAgent agent;
    Vector3 confirmPathClickPos;
    float confirmPathRadius = 1;
    LineRenderer lr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
        Game.game.gameState = Game.GameStates.ChoosePath;
    }
    void Update()
    {
        if (agent != null)
        {
            agent.isStopped = Game.game.gameState != Game.GameStates.Play;
        }
        switch (Game.game.gameState)
        {
            case Game.GameStates.ChoosePath:
                ChoosePath();
                break;
            case Game.GameStates.ConfirmPath:
                ConfirmPath();
                ChoosePath();
                break;
            case Game.GameStates.Play:
                Move();
                break;
            default:
                break;
        }
    }
    public void Spawn(Vector3 spawnPos)
    {
        transform.position = spawnPos;

    }
    void ChoosePath()
    {
        if (Input.GetMouseButtonDown(0))
        {
            destination = Game.game.GetMousePosInWorld();
            Vector3 playerToDestination = new Vector3(destination.x, transform.position.y, destination.z) - transform.position;

            if (playerToDestination.magnitude >= walkRange)
            {
                destination = transform.position + playerToDestination.normalized * walkRange;
            }

            NavMeshPath tempPath = new NavMeshPath();
            agent.CalculatePath(destination, tempPath);

            if (tempPath.corners.Length >= 2)
            {
                agent.path = Game.game.RoundPath(Game.game.GetPathLength(tempPath), movementSpeed, tempPath, agent);
                DrawPath();

                confirmPathClickPos = Game.game.GetMousePosInWorld();
                if (Game.game.gameState == Game.GameStates.ChoosePath)
                {
                    Game.game.gameState = Game.GameStates.ConfirmPath;
                }
            }
        }
    }
    void ConfirmPath()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Game.game.GetMousePosInWorld();

            if ((mousePos - confirmPathClickPos).magnitude <= confirmPathRadius)
            {
                Game.game.StartRound();
            }
        }
    }
    void Move()
    {
    }

    public void Died()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void DrawPath()
    {
        lr.positionCount = agent.path.corners.Length;
        lr.SetPositions(agent.path.corners);
    }
    public void InitializeRound()
    {
        lr.positionCount = 0;
    }

    void DrawWalkRange()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        int segments = 50;
        float x;
        float z;

        float angle = 20f;
        lineRenderer.positionCount = segments + 1;
        for (int i = 0; i < (segments + 1); i++)
        {
            x = transform.position.x + Mathf.Sin(Mathf.Deg2Rad * angle) * walkRange;
            z = transform.position.z + Mathf.Cos(Mathf.Deg2Rad * angle) * walkRange;

            lineRenderer.SetPosition(i, new Vector3(x, 1, z));

            angle += (360f / segments);
        }
    }
}
