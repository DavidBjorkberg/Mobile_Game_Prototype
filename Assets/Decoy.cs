using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : Item
{
    public int placementRange;
    public override void UseItem(Vector3 targetPos)
    {
        transform.position = targetPos;
        transform.position += new Vector3(0, 1, 0);
        Game.game.activeDecoys.Add(this);
    }
    public void DestroyDecoy()
    {
        Game.game.activeDecoys.Remove(this);
        Destroy(gameObject);
    }
}
