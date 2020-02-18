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
    public int enemyFOV;
    public float stepSize;
    public float movementSpeedFactor;
    internal int nrOfAliveEnemies;
    internal EnemyHandler enemyHandler;
    internal bool usingItem;
    public List<Item> activeItems = new List<Item>();
    public Player player;
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

        enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
        nrOfAliveEnemies = enemyHandler.enemies.Count;
    }
    void Update()
    {
        if (IsPaused())
        {
            Time.timeScale = 0.00001f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void AddItem(Item item)
    {
        activeItems.Add(item);
    }
    public void RemoveItem(Item item)
    {
        activeItems.Remove(item);
    }
    public bool IsPaused()
    {
        return !player.GetComponent<Player>().agent.hasPath && activeItems.Count <= 0;
    }
    public Vector3 GetMousePosInWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 0;
        return worldPos;
    }
    public void CreateSoundSource(Vector3 origin, float strength)
    {

    }
}
