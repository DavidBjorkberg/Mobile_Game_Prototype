using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiljardRock : Item
{
    public float speed;
    internal Vector3 direction;
    private bool isActive;
    void Start()
    {

    }
    public override void UseItem(Vector3 direction)
    {
        Game.game.AddItem(this);
        isActive = true;
        this.direction = direction;
        this.direction.y = 0;
        this.direction.Normalize();
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
