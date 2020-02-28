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
    public Item throwingKnifePrefab;
    public Item decoyPrefab;
    public Item vanishPrefab;
    public List<GameObject> startPoints = new List<GameObject>();
    public List<Texture> playerTextures = new List<Texture>();
    public List<Camera> cameras = new List<Camera>();
    public GameObject endScreenGO;
    internal enum SlowMotionState { Slowmotion, paused, regular }
    internal SlowMotionState slowMotionState;
    internal List<Decoy> activeDecoys = new List<Decoy>();
    internal int nrOfAliveEnemies;
    internal EnemyHandler enemyHandler;
    internal bool usingItem;
    internal List<Item> activeItems = new List<Item>();
    private RectTransform inventoryUIRectTransform;
    private int currentRoom = 0;
    private float totalTime;
    internal bool setDetectedSlowMotionRunning;
    internal bool setItemSlowMotionRunning;
    private bool setPausedRunning;
    private bool setRegularSpeedRunning;
    private float slowmotionStateLerptime = 0.2f;
    private void Awake()
    {

        inventoryUIRectTransform = player.GetComponent<Inventory>().inventoryUI.transform.GetChild(0).GetComponent<RectTransform>();
        if (game == null)
        {
            game = this;
        }

        enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
        nrOfAliveEnemies = enemyHandler.enemies.Count;
        slowMotionState = SlowMotionState.regular;
    }
    void Update()
    {
        if (enemyHandler.IsAllEnemiesDeadInRoom(currentRoom) && currentRoom != 3)
        {
            NextRoom();
        }
    }
    public void NextRoom()
    {
        currentRoom++;
        cameras[currentRoom - 1].gameObject.SetActive(false);
        cameras[currentRoom].gameObject.SetActive(true);
        player.agent.Warp(startPoints[currentRoom].transform.position);
        player.agent.ResetPath();
        if (currentRoom == 1)
        {
            player.GetComponent<Inventory>().AddStartItem(decoyPrefab, 1);
        }
        else if (currentRoom == 2)
        {
            player.GetComponent<Inventory>().AddStartItem(vanishPrefab, 1);

        }
        player.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", playerTextures[currentRoom]);
        player.GetComponent<Inventory>().RefreshItems();
        if (currentRoom == 3)
        {
            player.GetComponent<Inventory>().inventoryUI.gameObject.SetActive(false);
            endScreenGO.SetActive(true);
            endScreenGO.transform.GetChild(0).GetComponent<Text>().text = "Your time was: " + totalTime;
        }

    }
    public IEnumerator SetDetectedSlowmotion()
    {
        StopCoroutine("SetRegularSpeed");
        setRegularSpeedRunning = false;
        if (!setDetectedSlowMotionRunning && !setItemSlowMotionRunning && !setPausedRunning)
        {
            setDetectedSlowMotionRunning = true;
            float elapsedTime = 0;
            float startTimeScale = Time.timeScale;
            while (elapsedTime < slowmotionStateLerptime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(startTimeScale, 0.2f, elapsedTime / slowmotionStateLerptime);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSecondsRealtime(0.5f);

            StartCoroutine(SetRegularSpeed(true));
            setDetectedSlowMotionRunning = false;
        }
        yield return null;
    }
    public IEnumerator SetItemSlowmotion()
    {
        StopCoroutine("SetRegularSpeed");
        StopCoroutine("SetDetectedSlowMotion");
        setDetectedSlowMotionRunning = false;
        setRegularSpeedRunning = false;
        if (!setItemSlowMotionRunning && !setPausedRunning)
        {
            setItemSlowMotionRunning = true;
            float elapsedTime = 0;
            float startTimeScale = Time.timeScale;
            while (elapsedTime < slowmotionStateLerptime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(startTimeScale, 0.2f, elapsedTime / slowmotionStateLerptime);
                yield return new WaitForEndOfFrame();
            }
            setItemSlowMotionRunning = false;
        }
        yield return null;
    }
    public IEnumerator SetPaused()
    {
        StopCoroutine("SetDetectedSlowMotion");
        StopCoroutine("SetRegularSpeed");
        StopCoroutine("SetItemSlowmotion");
        setItemSlowMotionRunning = false;
        setDetectedSlowMotionRunning = false;
        setRegularSpeedRunning = false;
        if (!setPausedRunning)
        {

            setPausedRunning = true;

            float elapsedTime = 0;
            float startTimeScale = Time.timeScale;
            while (elapsedTime < slowmotionStateLerptime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(startTimeScale, 0.00001f, elapsedTime / slowmotionStateLerptime);
                yield return new WaitForEndOfFrame();
            }
            setPausedRunning = false;
        }
        yield return null;
    }
    public IEnumerator SetRegularSpeed(bool forceRun = false)
    {
        if (!setRegularSpeedRunning && !setPausedRunning && !setDetectedSlowMotionRunning || forceRun)
        {
            setRegularSpeedRunning = true;
            float elapsedTime = 0;
            float startTimeScale = Time.timeScale;
            Time.timeScale = 1;
            //while (elapsedTime < slowmotionStateLerptime)
            //{
            //    elapsedTime += Time.unscaledDeltaTime * 5;
            //    Time.timeScale = Mathf.Lerp(startTimeScale, 1f, elapsedTime / slowmotionStateLerptime);
            //    yield return new WaitForEndOfFrame();
            //}
            setRegularSpeedRunning = false;

        }
        yield return null;
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
