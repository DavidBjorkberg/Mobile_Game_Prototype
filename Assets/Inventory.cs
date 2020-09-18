using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    static int nrOfItemSlots = 3;
    public List<Item> startItems = new List<Item>();
    public List<int> startCharges = new List<int>();
    public ItemSlot[] itemSlots = new ItemSlot[nrOfItemSlots];
    public GameObject inventoryUI;
    private int nrOfUsedSlots = 0;
    void Start()
    {
        for (int i = 0; i < nrOfItemSlots; i++)
        {
            itemSlots[i] = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
        }
        ResetItems();
    }

    void Update()
    {
        SelectedItemSlowdownCheck();
    }
    void SelectedItemSlowdownCheck()
    {
        bool isAnySlotSelected = false;
        for (int i = 0; i < nrOfItemSlots; i++)
        {
            if (itemSlots[i].isSelected)
            {
                isAnySlotSelected = true;
                StartCoroutine(Game.game.SetItemSlowmotion());
                break;
            }
        }
        if (!isAnySlotSelected)
        {
            Time.timeScale = 1;
        }
    }
    public void ResetItems()
    {
        for (int i = nrOfUsedSlots - 1; i >= 0; i--)
        {
            RemoveItemFromInventory(i);
        }
        for (int i = 0; i < startItems.Count; i++)
        {
            for (int j = 0; j < startCharges[i]; j++)
            {
                AddItem(startItems[i]);
            }
        }
    }
    public void AddStartItem(Item item, int nrOfCharges)
    {
        startItems.Add(item);
        startCharges.Add(nrOfCharges);
    }
    public void AddItem(Item newItem)
    {
        if (newItem.isStackable)
        {
            //Check if an item of this type already is in inventory
            for (int i = 0; i < nrOfItemSlots; i++)
            {
                if (itemSlots[i].heldItem == newItem)
                {
                    itemSlots[i].nrOfCharges++;
                    GetInventorySlotGO(i).transform.GetChild(0).GetComponent<Text>().text 
                        = "x" + itemSlots[i].nrOfCharges.ToString();
                    return;
                }
            }

        }
        if (nrOfUsedSlots < nrOfItemSlots)
        {
            itemSlots[nrOfUsedSlots].heldItem = newItem;
            itemSlots[nrOfUsedSlots].nrOfCharges++;
            //Activate inventory button, first child is the UI
            GetInventorySlotGO(nrOfUsedSlots).SetActive(true);
            GetInventorySlotGO(nrOfUsedSlots).transform.GetChild(0).GetComponent<Text>().text 
                = "x" + itemSlots[nrOfUsedSlots].nrOfCharges.ToString();
            GetInventorySlotGO(nrOfUsedSlots).GetComponent<Button>().GetComponent<Image>().sprite 
                = itemSlots[nrOfUsedSlots].heldItem.inventorySprite;
            GetInventorySlotGO(nrOfUsedSlots).GetComponent<Button>().interactable 
                = itemSlots[nrOfUsedSlots].heldItem.isInteractable;
            nrOfUsedSlots++;
        }

    }
    public void RemoveItem(Item itemToRemove)
    {
        for (int i = 0; i < nrOfItemSlots; i++)
        {
            if (itemSlots[i].heldItem == itemToRemove)
            {
                DeselectItem(i);
                if (itemSlots[i].heldItem.isStackable)
                {
                    itemSlots[i].nrOfCharges--;
                    GetInventorySlotGO(i).transform.GetChild(0).GetComponent<Text>().text 
                        = "x" + itemSlots[i].nrOfCharges.ToString();
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
    void RemoveItemFromInventory(int index)
    {
        itemSlots[index].heldItem = null;
        itemSlots[index].nrOfCharges = 0;
        itemSlots[index].isSelected = false;
        GetInventorySlotGO(index).SetActive(false);
        nrOfUsedSlots--;
    }
    private GameObject GetInventorySlotGO(int index)
    {
        return inventoryUI.transform.GetChild(index + 1).gameObject;
    }
    public void OnInventoryButtonClick(int slotNumber)
    {
        //Deselect all other items
        for (int i = 0; i < nrOfItemSlots; i++)
        {
            if (i != slotNumber && itemSlots[i].heldItem != null)
            {
                DeselectItem(i);
            }
        }

        if (itemSlots[slotNumber].isSelected)
        {
            DeselectItem(slotNumber);
        }
        else
        {
            SelectItem(slotNumber);
        }
    }
    void SelectItem(int slotNumber)
    {
        GetInventorySlotGO(slotNumber).GetComponent<Button>().image.sprite 
            = itemSlots[slotNumber].heldItem.selectedSprite;

        itemSlots[slotNumber].isSelected = true;
        Game.game.GetComponent<ItemHandler>().SelectItem(itemSlots[slotNumber].heldItem);
    }
    void DeselectItem(int slotNumber)
    {
        Game.game.GetComponent<ItemHandler>().DeselectItem();
        GetInventorySlotGO(slotNumber).GetComponent<Button>().image.sprite 
            = itemSlots[slotNumber].heldItem.inventorySprite;

        itemSlots[slotNumber].isSelected = false;
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
