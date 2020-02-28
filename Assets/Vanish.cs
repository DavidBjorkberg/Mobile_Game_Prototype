using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanish : Item
{
    bool isActive;
    public GameObject timeBar;
    public float duration;
    public float speedMultiplier;
    private float curDuration;
    Material standardMaterial;
    public Material invisMaterial;
    public override void UseItem(Vector3 targetPos)
    {
        Game.game.player.SelectPathWhileUsingItem();
        standardMaterial = Game.game.player.GetComponent<MeshRenderer>().material;
        Game.game.player.GetComponent<MeshRenderer>().material = invisMaterial;
        transform.position = Game.game.player.transform.position + new Vector3(0, 1, 1);
        isActive = true;
        curDuration = duration;
        Game.game.player.isInvisible = true;
        Game.game.player.agent.speed *= speedMultiplier;
    }
    private void Update()
    {
        if (isActive)
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
        Game.game.player.agent.speed /= speedMultiplier;
        Game.game.player.GetComponent<MeshRenderer>().material = standardMaterial;
        Game.game.player.isInvisible = false;
        Destroy(gameObject);
    }
}
