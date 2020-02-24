using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : Item
{
    public override void UseItem(Vector3 targetPos)
    {
        transform.position = targetPos;
        Game.game.activeDecoys.Add(this);
    }
    public void DestroyDecoy()
    {
        Game.game.activeDecoys.Remove(this);
        Destroy(gameObject);
    }
}
