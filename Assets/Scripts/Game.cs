using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Game : MonoBehaviour
{
    public static Game game;
    public GameObject playerPrefab;
    public int enemyFOV;
    public float stepSize;
    public float movementSpeedFactor;
    public enum Objective { Sneak, Elimination }
    public Objective objective;
    internal int nrOfAliveEnemies;
    internal EnemyHandler enemyHandler;
    internal enum GameStates { ChooseSpawn, ChoosePath, ConfirmPath, Play }
    internal GameStates gameState;
    public enum GameMode { Action, Strategy }
    public GameMode gameMode;
    GameObject player;
    [HideInInspector]
    public GameObject goal;
    [HideInInspector]
    public GameObject startPos;
    private void Awake()
    {
        if (game == null)
        {
            game = this;
        }
        player = Instantiate(playerPrefab);
        player.GetComponent<Player>().Spawn(startPos.transform.position);
        player.name = "Player";

        enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
        foreach (Enemy enemy in enemyHandler.enemies)
        {
            enemy.GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(1 / (stepSize * enemy.movementSpeed), 0);
        }
        player.GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(1 / (stepSize * player.GetComponent<Player>().movementSpeed), 0);
        nrOfAliveEnemies = enemyHandler.enemies.Count;
    }
    void Start()
    {

    }

    void Update()
    {
        switch (gameMode)
        {
            case GameMode.Action:
                ActionUpdate();
                break;
            case GameMode.Strategy:
                StrategyUpdate();
                break;
            default:
                break;
        }
    }
    void StrategyUpdate()
    {
        if (gameState == GameStates.Play)
        {
            if (objective == Objective.Elimination)
            {
                if (enemyHandler.enemies.Count <= 0)
                {
                    CompletedLevel();
                }
            }
            else if (objective == Objective.Sneak)
            {
                if ((player.transform.position - goal.transform.position).magnitude <= 2)
                {
                    CompletedLevel();
                }
            }
            bool noEnemyHasPath = true;
            foreach (Enemy enemy in enemyHandler.enemies)
            {
                if (!enemy.agent.isStopped)
                {
                    noEnemyHasPath = false;
                }
            }
            //No enemy or player has a path
            if (noEnemyHasPath && !player.GetComponent<Player>().agent.hasPath)
            {
                EndRound();
            }
        }
    }
    void ActionUpdate()
    {
        if (objective == Objective.Elimination)
        {
            if (enemyHandler.enemies.Count <= 0)
            {
                CompletedLevel();
            }
        }
        else if (objective == Objective.Sneak)
        {
            if ((player.transform.position - goal.transform.position).magnitude <= 2)
            {
                CompletedLevel();
            }
        }
    }
    public void StartRound()
    {
        gameState = GameStates.Play;

    }
    void EndRound()
    {
        gameState = GameStates.ChoosePath;
        foreach (Enemy enemy in enemyHandler.enemies)
        {
            enemy.InitializeRound();
        }
        player.GetComponent<Player>().InitializeRound();
    }
    void CompletedLevel()
    {
        LoadNextScene();
    }
    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void CreateGoal()
    {
        goal = new GameObject();
        goal.name = "Goal";
        goal.transform.position += new Vector3(0, 1, 0);
        EditorUtility.SetDirty(goal);
    }
    public void CreateStartPos()
    {
        startPos = new GameObject();
        startPos.name = "Start Position";
        startPos.transform.position += new Vector3(0, 1, 0);
        EditorUtility.SetDirty(startPos);
    }
    public Vector3 GetMousePosInWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 0;
        return worldPos;
    }

    public float GetPathLength(NavMeshPath path)
    {
        float lengthOfPath = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            lengthOfPath += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return lengthOfPath;
    }
    public NavMeshPath RoundPath(float lengthOfPath, int movementSpeed, NavMeshPath path, NavMeshAgent agent)
    {
        float roundedLength;
        if (lengthOfPath < movementSpeed * stepSize)
        {
            roundedLength = movementSpeed * stepSize;
        }
        else
        {
            int nrOfStepsRoundDown = Mathf.FloorToInt(lengthOfPath / (movementSpeed * stepSize));
            roundedLength = (movementSpeed * stepSize) * nrOfStepsRoundDown;
        }

        float lengthChange = lengthOfPath - roundedLength;

        Vector3 cutoffDir = path.corners[path.corners.Length - 2] - path.corners[path.corners.Length - 1];
        Vector3 endPos = path.corners[path.corners.Length - 1];
        endPos += cutoffDir.normalized * lengthChange;
        NavMeshPath shortenedPath = new NavMeshPath();
        agent.CalculatePath(endPos, shortenedPath);
        return shortenedPath;
    }
    private void OnDrawGizmos()
    {
        if (goal != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(goal.transform.position, 1);
        }
        if (startPos != null && !Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPos.transform.position, 1);
        }
    }
}
