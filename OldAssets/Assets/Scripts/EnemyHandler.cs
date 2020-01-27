using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public GameObject enemyPrefab;
    public List<Enemy> enemies = new List<Enemy>();




    public Enemy GetEnemy(int index)
    {
        return enemies[index];
    }

    public GameObject CreateEnemy()
    {
        GameObject enemyGO = Instantiate(enemyPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        enemies.Add(enemyGO.GetComponent<Enemy>());
        return enemyGO;
    }
    public void ClearEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            DestroyEnemyImmediate(enemy);
        }
        enemies.Clear();
    }
    void DestroyEnemyImmediate(Enemy enemy)
    {
        DestroyImmediate(enemy.fieldOfView.gameObject);
        DestroyImmediate(enemy.drawPath.gameObject);
        DestroyImmediate(enemy.gameObject);
    }
    public void KillEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.fieldOfView.gameObject);
        Destroy(enemy.drawPath.gameObject);
        Destroy(enemy.gameObject);
    }
}
