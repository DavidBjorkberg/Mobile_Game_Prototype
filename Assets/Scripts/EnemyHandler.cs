using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoints;

    private void Start()
    {
        for (int i = 0; i < Game.game.totalAmountOfEnemies; i++)
        {
            Vector3 spawnPos;
            do
            {
                spawnPos = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-10f, 10f));

            } while (Physics.CheckSphere(spawnPos + new Vector3(0, 1, 0), 1f));
            Vector3 patrolGoal;
            do
            {
                patrolGoal = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-10f, 10f));
            } while (Physics.CheckSphere(patrolGoal + new Vector3(0, 1, 0), 1f) || (spawnPos - patrolGoal).magnitude <= 5f || (spawnPos - patrolGoal).magnitude >= Game.game.enemyWalkRange);
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.patrolGoal = patrolGoal;
            enemy.patrolStart = spawnPos;
            enemy.enemyNR = i;
            enemy.fieldOfView.fov = Game.game.enemyFOV;
            Game.game.enemies.Add(enemy);
        }
    }
}
