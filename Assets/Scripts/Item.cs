﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract void UseItem(Vector3 pos);
    public bool isStackable = true;
    public bool isInteractable = true;
    public bool requiresTarget = true;
    public Sprite inventorySprite;
    public Sprite selectedSprite;
}
