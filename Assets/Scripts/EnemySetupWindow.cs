using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class EnemySetupWindow : EditorWindow
{
    [MenuItem("Window/Enemy setup")]
    public static void ShowWindow()
    {
        GetWindow<EnemySetupWindow>("Enemy setup");
    }
    void OnGUI()
    {
        GUILayout.ExpandWidth(true);
        if (GUILayout.Button("Create Enemy", GUILayout.Height(position.height / 5.3f)))
        {
            //Create new enemy
            EnemyHandler enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
            GameObject createdEnemy = enemyHandler.CreateEnemy();
            createdEnemy.transform.SetParent(GameObject.Find("Enemies").transform);
            EditorUtility.SetDirty(createdEnemy);

            createdEnemy.GetComponent<Enemy>().waypoints = new List<GameObject>();
            
            //Create new waypoint
            GameObject createdWaypoint = createdEnemy.GetComponent<Enemy>().AddWaypoint();
            EditorUtility.SetDirty(createdWaypoint);

            Selection.activeObject = createdEnemy;
        }
        GUILayout.ExpandWidth(false);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Delete latest Enemy", GUILayout.Width((position.width - position.width / 4)), GUILayout.Height(position.height / 5.3f)))
        {
            EnemyHandler enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
            enemyHandler.DestroyLastEnemy();
        }
        if (GUILayout.Button("Delete all enemies", GUILayout.Width(position.width / 4), GUILayout.Height(position.height / 5.3f)))
        {
            EnemyHandler enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
            enemyHandler.DestroyAllEnemies();
        }
        GUILayout.EndHorizontal();
        GUILayout.ExpandWidth(true);

        if (GUILayout.Button("Add waypoint", GUILayout.Height(position.height / 5.3f)))
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.TryGetComponent(out Enemy enemy))
                {
                    GameObject createdWaypoint = enemy.AddWaypoint();
                    Selection.activeObject = createdWaypoint;
                    EditorUtility.SetDirty(createdWaypoint);
                }
                else if (Selection.activeGameObject.TryGetComponent(out TargetEnemy waypoint))
                {
                    GameObject createdWaypoint = waypoint.owner.GetComponent<Enemy>().AddWaypoint();
                    Selection.activeObject = createdWaypoint;
                    EditorUtility.SetDirty(createdWaypoint);
                }

            }
        }
        if (GUILayout.Button("Remove latest waypoint", GUILayout.Height(position.height / 5.3f)))
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.RemoveLatestWaypoint();
                }
                else if (Selection.activeGameObject.TryGetComponent(out TargetEnemy waypoint))
                {
                    waypoint.owner.GetComponent<Enemy>().RemoveLatestWaypoint();
                    if (waypoint.owner.GetComponent<Enemy>().waypoints.Count > 0)
                    {
                        Selection.activeObject = waypoint.owner.GetComponent<Enemy>().waypoints[waypoint.owner.GetComponent<Enemy>().waypoints.Count - 1];
                    }
                    else
                    {
                        Selection.activeObject = waypoint.owner;

                    }

                }
            }
        }
        if (GUILayout.Button("Remove all waypoints", GUILayout.Height(position.height / 5.3f)))
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.RemoveAllWaypoints(false);
                }
                else if (Selection.activeGameObject.TryGetComponent(out TargetEnemy waypoint))
                {
                    waypoint.owner.GetComponent<Enemy>().RemoveAllWaypoints(false);
                }
            }
        }
    }
}
#endif