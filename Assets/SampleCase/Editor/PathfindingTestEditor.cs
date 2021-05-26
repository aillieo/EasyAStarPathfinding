using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingTest))]
public class PathfindingTestEditor : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("FindPath"))
        {
            (target as PathfindingTest).FindPath();
        }

        if (GUILayout.Button("FindPathAsync"))
        {
            (target as PathfindingTest).FindPathAsync();
        }

        if (GUILayout.Button("FindPathInCoroutine"))
        {
            (target as PathfindingTest).FindPathInCoroutine();
        }
    }
}
