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
    public Player player;
    public SoundSource soundSourcePrefab;
    public Item ThrowingKnifePrefab;
    public Item RockPrefab;
    public List<GameObject> startPoints = new List<GameObject>();
    public List<Texture> playerTextures = new List<Texture>();
    public List<Camera> cameras = new List<Camera>();
    public GameObject endScreenGO;
    RectTransform inventoryUIRectTransform;
    internal List<Decoy> activeDecoys = new List<Decoy>();
    internal int nrOfAliveEnemies;
    internal EnemyHandler enemyHandler;
    internal bool usingItem;
    internal List<Item> activeItems = new List<Item>();
    private int currentRoom = 0;
    private float totalTime;
    private void Awake()
    {

        inventoryUIRectTransform = player.GetComponent<Inventory>().inventoryUI.transform.GetChild(0).GetComponent<RectTransform>();
        if (game == null)
        {
            game = this;
        }

        enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
        nrOfAliveEnemies = enemyHandler.enemies.Count;
    }
    void Update()
    {
        if(enemyHandler.IsAllEnemiesDeadInRoom(currentRoom) && currentRoom != 3)
        {
            NextRoom();
        }
        if (IsPaused())
        {
            Time.timeScale = 0.00001f;
        }
        else
        {
            totalTime += Time.deltaTime;
            Time.timeScale = 1;
        }
    }
    public void NextRoom()
    {
        currentRoom++;
        cameras[currentRoom - 1].gameObject.SetActive(false);
        cameras[currentRoom].gameObject.SetActive(true);
        player.agent.Warp(startPoints[currentRoom].transform.position);
        player.agent.ResetPath();
        if(currentRoom == 1)
        {
           // player.GetComponent<Inventory>().AddStartItem(RockPrefab, 2);   
        }
        else if(currentRoom == 2)
        {
           // player.GetComponent<Inventory>().AddStartItem(ThrowingKnifePrefab, 2);

        }
        player.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", playerTextures[currentRoom]);   
        player.GetComponent<Inventory>().RefreshItems();
        if(currentRoom == 3)
        {
            player.GetComponent<Inventory>().inventoryUI.gameObject.SetActive(false);
            endScreenGO.SetActive(true);
            endScreenGO.transform.GetChild(0).GetComponent<Text>().text = "Your time was: " + totalTime;
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
    public bool IsMouseOnInventory()
    {
        Vector2 mouseVector2 = inventoryUIRectTransform.InverseTransformPoint(Input.mousePosition);
        Rect inventoryRect = inventoryUIRectTransform.rect;
        return inventoryRect.Contains(mouseVector2);
    }
    public bool IsPaused()
    {
        return !player.GetComponent<Player>().agent.hasPath && activeItems.Count <= 0 || usingItem;
    }
    public Vector3 GetMousePosInWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 0;
        return worldPos;
    }
    public void CreateSoundSource(Vector3 origin, float range = 10)
    {
        SoundSource soundSourceGO = Instantiate(soundSourcePrefab, origin, Quaternion.identity);
        soundSourceGO.range = range;
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
