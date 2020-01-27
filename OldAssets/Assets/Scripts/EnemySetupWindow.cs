using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EnemySetupWindow : EditorWindow
{
    [MenuItem("Window/Enemy setup")]
    public static void ShowWindow()
    {
        GetWindow<EnemySetupWindow>("Enemy setup");
    }
    void OnGUI()
    {
        if (GUILayout.Button("Create Enemy"))
        {
            EnemyHandler enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
            GameObject createdEnemy = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>().CreateEnemy();
            Selection.activeObject = createdEnemy;
        }
        if (GUILayout.Button("Delete all enemies"))
        {
            EnemyHandler enemyHandler = GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>();
            GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>().ClearEnemies();
        }

        foreach (Enemy enemy in GameObject.Find("EnemyHandler").GetComponent<EnemyHandler>().enemies)
        {

        }
        List<string> options = new List<string>();
        EditorGUILayout.Popup
        if (GUILayout.Button("Add wayPoint"))
        {

        }
    }
}
