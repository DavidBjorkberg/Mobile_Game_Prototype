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
    internal NavMeshAgent agent;
    LineRenderer lr;
    public LineRenderer itemLr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
    }
    void ActionStart()
    {
        lr = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = movementSpeed * Game.game.movementSpeedFactor;
    }
    void Update()
    {
        RectTransform inventoryUIRectTransform = GetComponent<Inventory>().inventoryUI.transform.GetChild(0).GetComponent<RectTransform>();
        Vector2 mouseVector2 = inventoryUIRectTransform.InverseTransformPoint(Input.mousePosition);
        Rect inventoryRect = inventoryUIRectTransform.rect;
        InitializeRound();
        if (Input.GetMouseButton(0))
        {
            if (!Game.game.IsPaused() || Game.game.IsPaused() && !inventoryRect.Contains(mouseVector2))
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
