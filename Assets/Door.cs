using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Door : MonoBehaviour
{
    public Door exitDoor;
    public Camera roomCamera;
    public bool locked;
    public Key requiredKey;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (locked)
            {
                if (!player.GetComponent<Inventory>().SearchInventory(requiredKey))
                {
                    return;
                }
                else
                {
                    Game.game.player.GetComponent<Inventory>().RemoveItem(requiredKey);
                    Instantiate(requiredKey).UseItem(Vector3.zero);
                    locked = false;
                }
            }
            player.agent.ResetPath();
            player.agent.Warp(exitDoor.transform.position + exitDoor.transform.forward * 2);

            Camera.main.gameObject.SetActive(false);
            exitDoor.roomCamera.gameObject.SetActive(true);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + transform.forward * 2, 0.7f);
    }
}
