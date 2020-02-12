using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public GameObject throwingKnifePrefab;
    private Item currentItem;
    private bool choosingTarget;
    void Update()
    {
        if (choosingTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 targetPos = Game.game.GetMousePosInWorld();
                GameObject createdItem;
                if (currentItem is ThrowingKnife)
                {
                    createdItem = Instantiate(throwingKnifePrefab, Game.game.player.transform.position, Quaternion.identity);
                }
                else
                {
                    createdItem = null;
                }
                choosingTarget = false;

                createdItem.GetComponent<Item>().UseItem(targetPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Game.game.usingItem = false;
        }
    }
    public void UseItem(Item item)
    {
        choosingTarget = true;
        currentItem = item;
        Game.game.usingItem = true;
    }
}
