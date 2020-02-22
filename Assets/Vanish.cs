using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanish : Item
{
    bool isActive;
    public GameObject timeBar;
    public float duration;
    private float curDuration;
    public override void UseItem(Vector3 targetPos)
    {
        Game.game.AddItem(this);
        isActive = true;
        curDuration = duration;
    }
    private void Update()
    {
        if(isActive)
        {
           transform.position = Game.game.player.transform.position + new Vector3(0,1,1);
            timeBar.transform.localScale = new Vector3(curDuration / duration, 1f,1);
            curDuration -= Time.deltaTime;
        }
        if(curDuration <= 0)
        {
            DestroyItem();
        }
    }
    void DestroyItem()
    {
        Game.game.RemoveItem(this);
        Destroy(gameObject);
    }
}
