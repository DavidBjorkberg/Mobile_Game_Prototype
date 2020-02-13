using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Item heldItem;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Game.game.player.GetComponent<Inventory>().AddItem(heldItem);
            Destroy(gameObject);
        }
    }
}
