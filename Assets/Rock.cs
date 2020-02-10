using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Item
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
        isActive = true;
        direction = targetPos - transform.position;
        direction.Normalize();
        endPos = targetPos;
    }
    void Update()
    {
        if (isActive)
        {
            transform.position += speed * direction * Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.6f, 1 << 9);

            if (hits.Length > 0)
            {
                Game.game.enemyHandler.KillEnemy(hits[0].GetComponent<Enemy>());
            }
            if ((transform.position - endPos).magnitude <= destroyRange)
            {
                Destroy(gameObject);
            }
        }
    }
}
