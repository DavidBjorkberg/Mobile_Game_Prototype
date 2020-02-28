using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemSlot : ScriptableObject
{
    public bool isSelected;
    public Item heldItem;
    public float nrOfCharges = 0;
}
