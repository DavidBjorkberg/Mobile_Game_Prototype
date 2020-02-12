using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    public GameObject inventory;
    public ThrowingKnife throwingKnifePrefab;
    public float directionMarkerLength;
    bool choosingDirection;
    bool choosingTarget;
    Item selectedItem;
    Vector3 biljardDirection = Vector3.zero;
    public enum Items { Rock }
    public Items item;
    void Start()
    {

    }
    void Update()
    {
        inventory.SetActive(Game.game.isPaused());
        if (choosingTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 targetPos = Game.game.GetMousePosInWorld();
                selectedItem.UseItem(targetPos);
                
                choosingTarget = false;
            }
        }
        else if (choosingDirection)
        {
            
            if(Input.GetMouseButton(0))
            {
                biljardDirection = Game.game.player.transform.position - Game.game.GetMousePosInWorld();
                DrawDirection();
            }
            else if(Input.GetMouseButtonUp(0) && biljardDirection != Vector3.zero)
            {
                selectedItem.UseItem(biljardDirection);
                choosingDirection = false;
                Game.game.player.itemLr.positionCount = 0;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            Game.game.usingItem = false;
        }
    }
    public void SelectRock()
    {
        item = Items.Rock;
        selectedItem = Instantiate(throwingKnifePrefab, Game.game.player.transform.position, Quaternion.identity);

        choosingTarget = true;
        Game.game.usingItem = true;
    }
    public void SelectBiljardRock()
    {
        item = Items.Rock;
       // selectedItem = Instantiate(BiljardRockPrefab, Game.game.player.transform.position, Quaternion.identity);
        biljardDirection = Vector3.zero;
        choosingDirection = true;
        Game.game.usingItem = true;
    }
    void DrawDirection()
    {
        LineRenderer playerLr = Game.game.player.itemLr;

        playerLr.positionCount = 2;
        playerLr.SetPosition(0, Game.game.player.transform.position);
        playerLr.SetPosition(1, Game.game.player.transform.position + biljardDirection.normalized * directionMarkerLength);
    }
}
