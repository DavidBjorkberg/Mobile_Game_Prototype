using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemSlot : ScriptableObject
{
    public Item heldItem;
    public bool holdsItem = false;
    public float nrOfCharges = 0;
}
