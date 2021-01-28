using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathFindingTest))]
public class PathFindingTestEditor : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("FindPath"))
        {
            (target as PathFindingTest).FindPath();
        }

        if (GUILayout.Button("FindPathAsync"))
        {
            (target as PathFindingTest).FindPathAsync();
        }

        if (GUILayout.Button("FindPathInCoroutine"))
        {
            (target as PathFindingTest).FindPathInCoroutine();
        }
    }
}
