using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : Item
{
    public float speed;
    internal Vector3 endPos;
    internal Vector3 direction;
    float destroyRange = 0.5f;
    private bool isActive;
    void Start()
    {

    }
    public override void UseItem(Vector3 targetPos)
    {
        Game.game.AddItem(this);
        isActive = true;
        direction = targetPos - transform.position;
        direction.y = 0;
        direction.Normalize();
        endPos = targetPos;
        endPos.y = Game.game.player.transform.position.y;
    }
    void Update()
    {
        if (isActive)
        {
            transform.position += speed * direction * Time.deltaTime;
            if ((transform.position - endPos).magnitude <= destroyRange)
            {
                Game.game.RemoveItem(this);
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            Game.game.enemyHandler.KillEnemy(enemy);
            DestroyRock();
        }
        else if (other.TryGetComponent(out Obstacle obstacle))
        {
            DestroyRock();
        }
    }
    void DestroyRock()
    {
        Game.game.RemoveItem(this);
        Destroy(gameObject);
    }
}
