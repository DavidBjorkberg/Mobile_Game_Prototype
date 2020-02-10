using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    public Button itemButton;
    public bool biljard;
    public Rock rockItemPrefab;
    bool choosingDirection;
    bool choosingTarget;
    Item selectedItem;
    public enum Items { Rock }
    public Items item;
    void Start()
    {

    }
    void LateUpdate()
    {
        itemButton.gameObject.SetActive(Game.game.isPaused());
        if (choosingTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 targetPos = Game.game.GetMousePosInWorld();
                selectedItem.UseItem(targetPos);
                Game.game.usingItem = false;
                choosingTarget = false;
            }
        }
        else if (choosingDirection)
        {

        }
    }
    public void SelectItem()
    {
        item = Items.Rock;
        selectedItem = Instantiate(rockItemPrefab, Game.game.player.transform.position, Quaternion.identity);
        choosingTarget = true;
        Game.game.usingItem = true;
    }
}
