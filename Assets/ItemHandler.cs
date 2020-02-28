using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public GameObject throwingKnifePrefab;
    public GameObject rockPrefab;
    public GameObject decoyPrefab;
    public GameObject vanishPrefab;
    private Item currentItem;
    private bool choosingTarget;
    void Update()
    {
        if (choosingTarget)
        {
            if (Input.GetMouseButtonDown(0) && !Game.game.IsMouseOnInventory())
            {
                Vector3 targetPos = Game.game.GetMousePosInWorld();
                GameObject createdItem;
                if (currentItem is ThrowingKnife)
                {
                    createdItem = Instantiate(throwingKnifePrefab, Game.game.player.transform.position, Quaternion.identity);
                }
                else if (currentItem is Rock)
                {
                    createdItem = Instantiate(rockPrefab, Game.game.player.transform.position, Quaternion.identity);
                }
                else if (currentItem is Decoy)
                {
                    createdItem = Instantiate(decoyPrefab, Game.game.player.transform.position, Quaternion.identity);
                }
                else if (currentItem is Vanish)
                {
                    createdItem = Instantiate(vanishPrefab, Game.game.player.transform.position, Quaternion.identity);
                }
                else
                {
                    createdItem = null;
                }
                choosingTarget = false;
                Game.game.player.GetComponent<Inventory>().RemoveItem(currentItem);
                createdItem.GetComponent<Item>().UseItem(targetPos);

            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Game.game.usingItem = false;
        }
    }
    public void SelectItem(Item item)
    {
        Game.game.player.agent.ResetPath();
        if (item.requiresTarget)
        {
            choosingTarget = true;
            currentItem = item;
            Game.game.usingItem = true;
            
        }
        else
        {
            GameObject createdItem;
            if(item is Vanish)
            {
                createdItem = Instantiate(vanishPrefab, Game.game.player.transform.position, Quaternion.identity);
            }
            else
            {
                createdItem = null;
            }
            Game.game.player.GetComponent<Inventory>().RemoveItem(item);
            createdItem.GetComponent<Item>().UseItem(Vector3.zero);
        }
        
    }
    public void DeselectItem()
    {
        choosingTarget = false;
        currentItem = null;
    }
}
