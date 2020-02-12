using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    public LineRenderer itemLr;
    bool moving;
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
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
    }
    void StrategyStart()
    {
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
        Game.game.gameState = Game.GameStates.ChoosePath;
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
        RectTransform inventoryUIRectTransform = GetComponent<Inventory>().inventoryUI.transform.GetChild(0).GetComponent<RectTransform>();
        Vector2 mouseVector2 = inventoryUIRectTransform.InverseTransformPoint(Input.mousePosition);
        Rect inventoryRect = inventoryUIRectTransform.rect;
        InitializeRound();
        if (Input.GetMouseButton(0))
        {
            if (!Game.game.isPaused() || Game.game.isPaused() && !inventoryRect.Contains(mouseVector2))
            {
                if (!Game.game.usingItem)
                {
                    agent.destination = Game.game.GetMousePosInWorld();
                }
            }
        }

    }
    void StrategyUpdate()
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
        GetComponent<LineRenderer>().positionCount = agent.path.corners.Length;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            lr.SetPosition(i, agent.path.corners[i]);
        }
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
