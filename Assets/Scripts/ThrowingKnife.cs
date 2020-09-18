using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : Item
{
    public float speed;
    internal Vector3 endPos;
    internal Vector3 direction;
    private bool isActive;
    public override void UseItem(Vector3 targetPos)
    {
        Game.game.AddItem(this);
        isActive = true;
        direction = targetPos - transform.position;
        direction.y = 0;
        direction.Normalize();
        endPos = targetPos;
        endPos.y = Game.game.player.transform.position.y;
        transform.LookAt(transform.position + direction);

    }
    void Update()
    {
        if (isActive)
        {
            transform.position += speed * direction * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            Game.game.enemyHandler.KillEnemy(enemy);
            DestroyItem();
        }
        else if (other.TryGetComponent(out Obstacle obstacle))
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
