using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public int movementSpeed;
    public int acceleration;
    public int walkRange;
    internal NavMeshAgent agent;
    internal bool isInvisible;
    LineRenderer lr;
    public LineRenderer itemLr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
    }
    void Update()
    {
        InitializeRound();
        if (Input.GetMouseButton(0))
        {
            if (!Game.game.IsMouseOnInventory())
            {
                if (!Game.game.usingItem)
                {
                    agent.destination = Game.game.GetMousePosInWorld();
                }
            }
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
