using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    public int movementSpeed;
    public int acceleration;
    public int walkRange;
    internal NavMeshAgent agent;
    internal bool isInvisible;
    LineRenderer lr;
    public LineRenderer itemLr;
    internal bool detected;
    internal List<Enemy> detectedEnemies = new List<Enemy>();
    private GameObject enemyToFollow;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed;
    }
    void Update()
    {
        InitializeRound();
        if (enemyToFollow != null)
        {
            agent.destination = enemyToFollow.transform.position;
        }
        if (Input.GetMouseButton(0))
        {
            if (!Game.game.IsMouseOnInventory())
            {
                if (!Game.game.usingItem)
                {
                    Collider[] hits = Physics.OverlapSphere(Game.game.GetMousePosInWorld(), 1, 1 << 9);
                    if (hits.Length > 0)
                    {
                        enemyToFollow = hits[0].gameObject;
                        agent.destination = enemyToFollow.transform.position;
                    }
                    else
                    {
                        agent.destination = Game.game.GetMousePosInWorld();
                        enemyToFollow = null;
                    }
                }
            }
        }
    }
    public void AddDetectedEnemy(Enemy enemy)
    {
        if (!detectedEnemies.Contains(enemy))
        {
            detectedEnemies.Add(enemy);
            StopCoroutine(Game.game.SetDetectedSlowmotion());
            Game.game.setDetectedSlowMotionRunning = false;
            StartCoroutine(Game.game.SetDetectedSlowmotion());
        }
    }
    public void RemoveDetectedEnemy(Enemy enemy)
    {
        if (detectedEnemies.Contains(enemy))
        {
            detectedEnemies.Remove(enemy);
        }
    }
    public void SelectPathWhileUsingItem()
    {
        if (!Game.game.IsMouseOnInventory())
        {
            agent.destination = Game.game.GetMousePosInWorld();
        }
    }
    public void Died()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

}
