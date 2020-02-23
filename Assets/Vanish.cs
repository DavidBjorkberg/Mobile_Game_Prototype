using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanish : Item
{
    bool isActive;
    public GameObject timeBar;
    public float duration;
    private float curDuration;
    Color standardColour;
    public Color invisColour;
    public override void UseItem(Vector3 targetPos)
    {
        standardColour = Game.game.player.GetComponent<MeshRenderer>().material.color;
        Game.game.player.GetComponent<MeshRenderer>().material.color = invisColour;
        transform.position = Game.game.player.transform.position + new Vector3(0, 1, 1);
        isActive = true;
        curDuration = duration;
        Game.game.player.isInvisible = true;
    }
    private void Update()
    {
        if (isActive && !Game.game.IsPaused())
        {
            transform.position = Game.game.player.transform.position + new Vector3(0, 1, 1);
            timeBar.transform.localScale = new Vector3(curDuration / duration, 1f, 1);
            curDuration -= Time.deltaTime;
        }
        if (curDuration <= 0)
        {
            DestroyItem();
        }
    }
    void DestroyItem()
    {
        Game.game.player.GetComponent<MeshRenderer>().material.color = standardColour;
        Game.game.player.isInvisible = false;
        Destroy(gameObject);
    }
}
