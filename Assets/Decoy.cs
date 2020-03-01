using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : Item
{
    public float duration;
    public GameObject timeBar;
    private bool isActive;
    private float curDuration;

    public override void UseItem(Vector3 targetPos)
    {
        transform.position = targetPos + new Vector3(0, 1, 0);
        Game.game.activeDecoys.Add(this);
        curDuration = duration;
        isActive = true;    
    
    }
    private void Update()
    {
        if(isActive)
        {
            timeBar.transform.localScale = new Vector3(curDuration / duration, 1f, 1);
            curDuration -= Time.deltaTime;
            if(curDuration <= 0)
            {
                DestroyDecoy();
            }
        }
    }
    public void DestroyDecoy()
    {
        Game.game.activeDecoys.Remove(this);
        Destroy(gameObject);
    }
}
