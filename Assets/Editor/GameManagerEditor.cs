using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Game))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Game game = (Game)target;
        EditorUtility.SetDirty(game);
        if (game.startPos == null)
        {
            if (GUILayout.Button("Add Start Position", GUILayout.Height(40)))
            {
                game.CreateStartPos();
                Selection.activeObject = game.startPos;
            }
        }
        else
        {
            if (GUILayout.Button("Target Start Position", GUILayout.Height(40)))
            {
                Selection.activeObject = game.startPos;
            }
        }
        if (game.objective == Game.Objective.Sneak)
        {
            if (game.goal == null)
            {
                if (GUILayout.Button("Add Goal", GUILayout.Height(40)))
                {
                    game.CreateGoal();
                    Selection.activeObject = game.goal;
                }
            }
            else
            {
                if (GUILayout.Button("Target goal", GUILayout.Height(40)))
                {
                    Selection.activeObject = game.goal;
                }
            }
        }
        else
        {
            if (game.goal != null)
            {
                DestroyImmediate(game.goal);
            }
        }
    }
}
