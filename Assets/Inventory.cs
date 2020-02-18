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
        inventoryUI.SetActive(Game.game.IsPaused());
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
                    GetInventorySlotGO(i).transform.GetChild(0).GetComponent<Text>().text = "x" + itemSlots[i].nrOfCharges.ToString();
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
            GetInventorySlotGO(nrOfUsedSlots).SetActive(true);
            GetInventorySlotGO(nrOfUsedSlots).transform.GetChild(0).GetComponent<Text>().text = "x" + itemSlots[nrOfUsedSlots].nrOfCharges.ToString();
            GetInventorySlotGO(nrOfUsedSlots).GetComponent<Button>().GetComponent<Image>().sprite = itemSlots[nrOfUsedSlots].heldItem.inventorySprite;
            GetInventorySlotGO(nrOfUsedSlots).GetComponent<Button>().interactable = itemSlots[nrOfUsedSlots].heldItem.isInteractable;
            nrOfUsedSlots++;
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
                    GetInventorySlotGO(i).transform.GetChild(0).GetComponent<Text>().text = "x" + itemSlots[i].nrOfCharges.ToString();
                    if (itemSlots[i].nrOfCharges <= 0)
                    {
                        RemoveItemFromInventory(i);
                    }
                }
                else
                {
                    RemoveItemFromInventory(i);
                }
            }
        }
    }
    private GameObject GetInventorySlotGO(int index)
    {
        return inventoryUI.transform.GetChild(index + 1).gameObject;
    }
    void RemoveItemFromInventory(int index)
    {
        itemSlots[index].holdsItem = false;
        itemSlots[index].heldItem = null;
        inventoryUI.transform.GetChild(nrOfUsedSlots).gameObject.SetActive(false);
        nrOfUsedSlots--;

    }
    public void UseItem(int slotNumber)
    {
        Game.game.GetComponent<ItemHandler>().UseItem(itemSlots[slotNumber].heldItem);
    }
    public bool SearchInventory(Item item)
    {
        for (int i = 0; i < nrOfUsedSlots; i++)
        {
            if (itemSlots[i].heldItem == item)
            {
                return true;
            }
        }
        return false;
    }
}
