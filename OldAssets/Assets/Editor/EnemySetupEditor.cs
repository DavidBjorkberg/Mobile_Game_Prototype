using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySetup))]
public class EnemySetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemySetup enemy = (EnemySetup)target;
        if (GUILayout.Button("Create enemy"))
        {
        }
        else
        {
        }
    }
}
