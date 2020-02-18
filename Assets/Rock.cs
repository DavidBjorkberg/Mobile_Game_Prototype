using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Item
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
            enemy.Stun();
            DestroyRock();
        }
        else if (other.TryGetComponent(out Obstacle obstacle))
        {
            Game.game.CreateSoundSource(transform.position);
            DestroyRock();
        }
    }
    void DestroyRock()
    {
        Game.game.RemoveItem(this);
        Destroy(gameObject);
    }
}
