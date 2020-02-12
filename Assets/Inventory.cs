using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    static int nrOfItemSlots = 8;
    public ItemSlot[] itemSlots = new ItemSlot[nrOfItemSlots];
    public GameObject inventoryUI;
    private int nrOfUsedSlots = 0;
    void Start()
    {
        for (int i = 0; i < nrOfItemSlots; i++)
        {
            itemSlots[i] = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
        }
    }

    void Update()
    {
        inventoryUI.SetActive(Game.game.isPaused());
    }
    public void AddItem(Item newItem)
    {
        if (newItem.isStackable)
        {
            //Check if an item of this type already is in inventory
            for (int i = 0; i < nrOfUsedSlots; i++)
            {
                if (itemSlots[i].heldItem == newItem)
                {
                    itemSlots[i].nrOfCharges++;
                    return;
                }
            }

        }
        if (nrOfUsedSlots < nrOfItemSlots)
        {
            itemSlots[nrOfUsedSlots].heldItem = newItem;
            itemSlots[nrOfUsedSlots].nrOfCharges++;
            itemSlots[nrOfUsedSlots].holdsItem = true;
            //Activate inventory button, first child is the UI
            inventoryUI.transform.GetChild(nrOfUsedSlots + 1).gameObject.SetActive(true);
            inventoryUI.transform.GetChild(nrOfUsedSlots + 1).GetChild(0).GetComponent<Text>().text = itemSlots[nrOfUsedSlots].heldItem.itemName;
        }

    }
    public void RemoveItem(Item itemToRemove)
    {
        for (int i = 0; i < nrOfUsedSlots; i++)
        {
            if (itemSlots[i].heldItem == itemToRemove)
            {
                if (itemSlots[i].heldItem.isStackable)
                {
                    itemSlots[i].nrOfCharges--;
                    if (itemSlots[i].nrOfCharges <= 0)
                    {
                        itemSlots[i].holdsItem = false;
                        itemSlots[i].heldItem = null;
                        inventoryUI.transform.GetChild(nrOfUsedSlots + 1).gameObject.SetActive(false);
                    }
                }
            }
        }
        print("Tried to remove item that wasn't in inventory");
    }
    public void UseItem(int slotNumber)
    {
        Game.game.GetComponent<ItemHandler>().UseItem(itemSlots[slotNumber].heldItem);
    }
}
