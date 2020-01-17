using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Game : MonoBehaviour
{
    public static Game game;
    public GameObject player;
    public int totalAmountOfEnemies;
    internal int nrOfAliveEnemies;
    public List<Enemy> enemies;
    public int enemyWalkRange;
    public int enemyFOV;
    public Text gameStateText;
    internal bool isPlayerMoving = false;
    internal enum gameStates { ChooseSpawn, ChoosePath, ConfirmPath, Play }
    internal gameStates gameState;
    float roundTimer = 0;
    float roundTime = 1;
    private void Awake()
    {
        GameObject playerGO = Instantiate(player, new Vector3(-100, 0, -100), Quaternion.identity);
        playerGO.name = "Player";
        if (game == null)
        {
            game = this;
        }
        nrOfAliveEnemies = totalAmountOfEnemies;
    }
    void Start()
    {
        enemies = new List<Enemy>();
    }

    void Update()
    {
        gameStateText.text = "GameState: " + gameState.ToString();
        if (gameState == gameStates.Play)
        {
            roundTimer += Time.deltaTime;
            if (roundTimer >= roundTime)
            {
            }
            int counter = 0;
            foreach (Enemy enemy in enemies)
            {
                if (enemy.agent.hasPath)
                {
                    counter++;
                }
            }
            if (counter == 0)
            {
                EndRound();
            }
        }
        if (enemies.Count <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
    public void StartRound()
    {
        gameState = gameStates.Play;
        roundTimer = 0;
    }
    void EndRound()
    {
        gameState = gameStates.ChoosePath;
    }
    public Vector3 getMousePosInWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 0;
        return worldPos;
    }
    public void KillEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.fieldOfView.gameObject);
        Destroy(enemy.drawPath.gameObject);
        Destroy(enemy.gameObject);

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
