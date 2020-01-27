using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Game : MonoBehaviour
{
    public static Game game;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    [Range(0, 15)]
    public int totalAmountOfEnemies;
    public int enemyFOV;
    public float stepSize;
    public Text gameStateText;
    internal int nrOfAliveEnemies;
    internal EnemyHandler enemyHandler;
    internal enum GameStates { ChooseSpawn, ChoosePath, ConfirmPath, Play }
    internal GameStates gameState;
    GameObject player;
    private void Awake()
    {
        player = Instantiate(playerPrefab, new Vector3(-100, 0, -100), Quaternion.identity);
        player.name = "Player";

        if (game == null)
        {
            game = this;
        }
        enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
        foreach (Enemy enemy in enemyHandler.enemies)
        {
            enemy.GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(1 / (stepSize * enemy.movementSpeed), 0);
        }
        player.GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(1 / (stepSize * player.GetComponent<Player>().movementSpeed), 0);
        nrOfAliveEnemies = totalAmountOfEnemies;
    }
    void Start()
    {
    }

    void Update()
    {
        if (gameState == GameStates.Play)
        {
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
        gameStateText.text = "";
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
}
