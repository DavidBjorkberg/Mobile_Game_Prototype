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
    internal int nrOfAliveEnemies;
    internal EnemyHandler enemyHandler;
    [Range(10, 30)]
    public int enemyWalkRange;
    public int enemyFOV;
    public Text gameStateText;
    internal bool isPlayerMoving = false;
    internal enum gameStates { ChooseSpawn, ChoosePath, ConfirmPath, Play }
    internal gameStates gameState;
    private void Awake()
    {
        GameObject playerGO = Instantiate(playerPrefab, new Vector3(-100, 0, -100), Quaternion.identity);
        playerGO.name = "Player";
        if (game == null)
        {
            game = this;
        }
        enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
        nrOfAliveEnemies = totalAmountOfEnemies;
    }
    void Start()
    {
    }

    void Update()
    {
        gameStateText.text = "GameState: " + gameState.ToString();
    }
    public void StartRound()
    {
        gameState = gameStates.Play;
    }
    void EndRound()
    {
        gameState = gameStates.ChoosePath;
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
    //Cuts off end of path so its length is divisible by .5f and returns the new length
    public NavMeshPath RoundDownPath(float lengthOfPath, NavMeshPath path, NavMeshAgent agent)
    {
        float roundedLength = Mathf.Floor(lengthOfPath);
        if (lengthOfPath - roundedLength > 0.5f)
        {
            roundedLength += 0.5f;
        }
        float cutoffLength = lengthOfPath - roundedLength;
        Vector3 cutoffDir = path.corners[path.corners.Length - 2] - path.corners[path.corners.Length - 1];
        Vector3 endPos = path.corners[path.corners.Length - 1];
        endPos += cutoffDir.normalized * cutoffLength;
        NavMeshPath shortenedPath = new NavMeshPath();
        agent.CalculatePath(endPos, shortenedPath);
        return shortenedPath;
    }
}
