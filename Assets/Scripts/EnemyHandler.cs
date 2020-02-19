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
#if UNITY_EDITOR
    public GameObject CreateEnemy()
    {
        GameObject enemyGO = Instantiate(enemyPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        enemies.Add(enemyGO.GetComponent<Enemy>());
        return enemyGO;
    }
    public void DestroyAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            DestroyEnemyImmediate(enemies[i]);
        }

        enemies.Clear();
    }
    public void DestroyLastEnemy()
    {
        if (enemies.Count > 0)
        {
            DestroyEnemyImmediate(enemies[enemies.Count - 1]);
            enemies.RemoveAt(enemies.Count - 1);
        }
    }
    void DestroyEnemyImmediate(Enemy enemy)
    {
        DestroyImmediate(enemy.fieldOfView.gameObject);
        DestroyImmediate(enemy.gameObject);
        enemy.RemoveAllWaypoints(true);
    }
#endif
    public void KillEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.fieldOfView.gameObject);
        Destroy(enemy.gameObject);
    }
}
